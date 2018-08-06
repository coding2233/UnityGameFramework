//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #Kcp 通信接口,简单封包+序列化# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月29日 21点05分# </time>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Google.Protobuf;

namespace GameFramework.Taurus
{
	public class KcpService
	{
		#region 属性
		private KcpChannel _kcpChannel;

		private Queue<byte[]> _receiveDatas;

		private List<byte> _lastReceiveDatas;

		private Action<ushort, byte[]> _receiveDataCallback;

		#endregion

		public KcpService(int port,Action<ushort, byte[]> receiveDataCallback)
		{
			_lastReceiveDatas = new List<byte>();
			_receiveDatas = new Queue<byte[]>();
			_kcpChannel = new KcpChannel(port, ReceiveData);
		    _receiveDataCallback = receiveDataCallback;
		}
        

		public void Update()
		{
			while (_receiveDatas.Count > 0)
			{
				_lastReceiveDatas.AddRange(_receiveDatas.Dequeue());
				if (_lastReceiveDatas.Count > 4)
				{
					byte[] datas = _lastReceiveDatas.ToArray();
					int length = System.BitConverter.ToInt32(datas,0);
					if (_lastReceiveDatas.Count >=length+4)
					{
						ushort type =System.BitConverter.ToUInt16(datas, 4);
						byte[] messageData = _lastReceiveDatas.GetRange(6, length-2).ToArray();
						_receiveDataCallback?.Invoke(type, messageData);
						_lastReceiveDatas.RemoveRange(0, length + 4);
					}
				}
			}
		}

		public void SendMessage(ushort typeCode, byte[] messageData,IPEndPoint endPoint)
		{
			byte[] typeData = BitConverter.GetBytes(typeCode);
			byte[] length = BitConverter.GetBytes(messageData.Length+2);
			byte[] result = new byte[messageData.Length+ 2 + 4];
			length.CopyTo(result, 0);
			typeData.CopyTo(result, 4);
			messageData.CopyTo(result, 6);
			_kcpChannel?.Send(result, endPoint);
		}
        

		private void ReceiveData(byte[] data)
		{
			_receiveDatas.Enqueue(data);
		}

		

	}
}