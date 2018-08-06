//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #Kcp通道# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月26日 23点25分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GameFramework.Taurus
{
	public class KcpChannel 
	{
		private static readonly DateTime utc_time = new DateTime(1970, 1, 1);

		public static UInt32 iclock()
		{
			return (UInt32)(Convert.ToInt64(DateTime.UtcNow.Subtract(utc_time).TotalMilliseconds) & 0xffffffff);
		}

		#region 属性

		private KCP _kcp;

		private UdpClient _udpClient;

		private IPEndPoint _recEndPoint;

        private IPEndPoint _currentEndPoint;

		private Queue<byte[]> _receiveMeesages;

		private Action<byte[]> _reveiveHandler;
		#endregion
        
		public KcpChannel(int port,Action<byte[]> reveiveHandler)
		{
			_recEndPoint = new IPEndPoint(IPAddress.Any, 0);

			_reveiveHandler = reveiveHandler;

			_udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, port));
			_kcp = new KCP((UInt32) new Random((int) DateTime.Now.Ticks).Next(1, Int32.MaxValue),
				UdpSendData);

			//kcp fast mode.
			_kcp.NoDelay(1, 10, 2, 1);
			_kcp.WndSize(128, 128);

			_receiveMeesages = new Queue<byte[]>();

		    _udpClient.BeginReceive(UdpReceiveMessage, this);

		    new Task(Update).Start();
        }

	    private void UdpReceiveMessage(IAsyncResult asyncCallback)
	    {
	        byte[] datas = _udpClient.EndReceive(asyncCallback,ref _recEndPoint);
	        _receiveMeesages.Enqueue(datas);
	        _udpClient?.BeginReceive(UdpReceiveMessage, this);
	    }
        

		private void UdpSendData(byte[] datas, int length)
		{
			if(_currentEndPoint!=null)
				_udpClient?.Send(datas, length, _currentEndPoint);
		}

		public void Send(byte[] datas,IPEndPoint endPoint)
		{
		    if (endPoint == null)
		        return;
			_currentEndPoint = endPoint;

            _kcp?.Send(datas);
		}
		
		private void Update()
		{
		    while (true)
		    {
		        if (_receiveMeesages.Count > 0)
		        {
		            var buf = _receiveMeesages.Dequeue();

		            _kcp.Input(buf);
		            // mNeedUpdateFlag = true;

		            for (var size = _kcp.PeekSize(); size > 0; size = _kcp.PeekSize())
		            {
		                var buffer = new byte[size];
						if (_kcp.Recv(buffer)>0)
						{
							_reveiveHandler(buffer);
		                }
		            }
		        }

		        _kcp.Update(iclock());
		        Thread.Sleep(10);
		    }
		}
        

	}

}
