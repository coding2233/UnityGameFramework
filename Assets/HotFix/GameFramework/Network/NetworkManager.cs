//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #网络管理器# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年8月6日 16点08分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using GT = GameFramework.Taurus;

namespace HotFix.Taurus
{
    public sealed class NetworkManager:GameFrameworkModule
    {
        #region 属性
        private readonly Dictionary<Type, List<MessageHandlerBase>> _messageHandler = new Dictionary<Type, List<MessageHandlerBase>>();
        private readonly Dictionary<ushort, Type> _messageCodeType = new Dictionary<ushort, Type>();
        private readonly ProtobufPacker _protobufPacker;
        private int _rpcId = 0;
        private Dictionary<int, Action<object>> _responseCallback=new Dictionary<int, Action<object>>();
        #endregion


        public NetworkManager()
        {
            _protobufPacker = new ProtobufPacker();
             GT.GameMode.Network.ReceiveMsgCallback += ReceiveMsgData;

            //加载所有的类型
            LoadMessageAttribute();
        }
		

	    public void SetPort(int port)
        {
            GT.GameMode.Network.SetPort(port);
        }

        public void SendMessage(object message, IPEndPoint endPoint)
        {
            byte[] messageData = _protobufPacker.ToBytes(message);

            object[] attribute = message.GetType().GetCustomAttributes(typeof(MessageAttribute), false);
            if (attribute.Length <= 0)
                throw new GT.GamekException("class not found MessageAttribute");
			MessageAttribute mgAttribute = attribute[0] as MessageAttribute;
			if (mgAttribute!=null)
				GT.GameMode.Network.SendMessage(mgAttribute.TypeCode, messageData, endPoint);
        }

        public Task<T> Call<T>(IRequest message, IPEndPoint endPoint) where T : class, IResponse
        {
            var task = new TaskCompletionSource<T>();
            message.RpcId = ++_rpcId;
            _responseCallback[message.RpcId] = (msg) =>
            {
                T response = msg as T;
                task.SetResult(response);
            };
            SendMessage(message, endPoint);
            return task.Task;
        }

        private void LoadMessageAttribute()
        {
            Type[] types = GT.GameMode.HotFix.GetHotFixTypes.ToArray();
            foreach (var item in types)
            {
                //get  messagehandler
                object[] attribute = item.GetCustomAttributes(typeof(MessageHandlerAttribute), false);
                if (attribute.Length > 0 && !item.IsAbstract)
                {
					MessageHandlerAttribute msHanderAttibute = attribute[0] as MessageHandlerAttribute;
					if (msHanderAttibute!=null)
	                {
		                Type handType = Type.GetType(msHanderAttibute.TypeMessage);
	                    if (handType != null)
	                    {
	                        if (!_messageHandler.ContainsKey(handType))
	                            _messageHandler[handType] = new List<MessageHandlerBase>();
	                        _messageHandler[handType]
	                            .Add((MessageHandlerBase) Activator.CreateInstance(item));
	                    }
	                }
                }

                //get message
                attribute = item.GetCustomAttributes(typeof(MessageAttribute), false);
                if (attribute.Length > 0 && !item.IsAbstract)
                {
					MessageAttribute msAttibute = attribute[0] as MessageAttribute;
					if (msAttibute!=null)
						_messageCodeType[msAttibute.TypeCode] = item;
                }

            }
        }
        
        public override void OnClose()
        {
        }

        private void ReceiveMsgData(ushort typeCode, byte[] msgData)
        {
			Type type;
            if (_messageCodeType.TryGetValue(typeCode, out type))
            {
                object message = _protobufPacker.ToMessage(type, msgData);
				IResponse response = message as IResponse;
				if (response!=null)
                {
                    if (_responseCallback.ContainsKey(response.RpcId))
                    {
                        _responseCallback[response.RpcId](response);
                        _responseCallback.Remove(response.RpcId);
                    }
                }
                //消息处理类
                else if (_messageHandler.ContainsKey(type))
                {
                    foreach (var item in _messageHandler[type])
                        item.Handle(message);
                }
            }
            
        }
        
    }
}