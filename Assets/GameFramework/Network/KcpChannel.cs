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

		private IPEndPoint _targetEndPoinnt;

		private IPEndPoint _allEndPoint;

		private IPEndPoint _currentEndPoint;

		private IPEndPoint _recEndPoint;

		private Queue<byte[]> _receiveMeesages;

		private Action<byte[]> _reveiveHandler;
		#endregion


		public KcpChannel(string ip,int port,Action<byte[]> reveiveHandler)
		{
			_allEndPoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), port);
			_recEndPoint = new IPEndPoint(IPAddress.Any, 0);

			_reveiveHandler = reveiveHandler;

			_udpClient = new UdpClient(new IPEndPoint(IPAddress.Parse(ip), port));
			_kcp = new KCP((UInt32) new Random((int) DateTime.Now.Ticks).Next(1, Int32.MaxValue),
				UdpSendData);

			//kcp fast mode.
			_kcp.NoDelay(1, 10, 2, 1);
			_kcp.WndSize(128, 128);

			_receiveMeesages = new Queue<byte[]>();

			Thread revThread = new Thread(ReceiveMessage);
			revThread.Start();
			Thread updataThread = new Thread(Update);
			updataThread.Start();
		}

		public void Connect(string ip, int port)
		{
			_targetEndPoinnt = new IPEndPoint(IPAddress.Parse(ip), port);
			_currentEndPoint = _targetEndPoinnt;
		}

		private void UdpSendData(byte[] datas, int length)
		{
			if(_currentEndPoint!=null)
				_udpClient?.Send(datas, length, _currentEndPoint);
		}

		public void Send(byte[] datas,bool toAll=false)
		{
			if (toAll)
				_currentEndPoint = _allEndPoint;
			else
				_currentEndPoint = _targetEndPoinnt;
			_kcp?.Send(datas);
		}

		private void ReceiveMessage()
		{
			while (true)
			{
				byte[] datas = _udpClient.Receive(ref _recEndPoint);
				if (datas != null)
				{
					_receiveMeesages.Enqueue(datas);
				}
			}
		}

		private void Update()
		{
			while (_receiveMeesages.Count > 0)
			{
				var buf = _receiveMeesages.Dequeue();

				_kcp.Input(buf);
				// mNeedUpdateFlag = true;

				for (var size = _kcp.PeekSize(); size > 0; size = _kcp.PeekSize())
				{
					var buffer = new byte[size];
					if (_kcp.Recv(buffer) > 0)
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
