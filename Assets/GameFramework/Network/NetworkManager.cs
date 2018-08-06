//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #网络管理类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年8月4日 13点51分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Google.Protobuf;

namespace GameFramework.Taurus
{
    public sealed class NetworkManager:GameFrameworkModule,IUpdate
    {
        private SystemManager _systemManager;

        private KcpService _kcpService;
        private int _port = 8359;
        private ProtobufPacker _protobufPacker;
        private readonly Dictionary<Type, List<MessageHandlerBase>> _messageHandler = new Dictionary<Type, List<MessageHandlerBase>>();

        private readonly Dictionary<ushort, Type> _messageCodeType = new Dictionary<ushort, Type>();
        //留给hotfix的接口
        public Action<ushort,byte[]> ReceiveMsgCallback;

        private int _rpcId = 0;
        private Dictionary<int, Action<object>> _responseCallback;

        public NetworkManager()
        {
            _systemManager = GameFrameworkMode.GetModule<SystemManager>();
            _responseCallback = new Dictionary<int, Action<object>>();
            // _kcpService = new KcpService(_port, ReceiveMessage);
            _protobufPacker = new ProtobufPacker();
            //加载message标记类
            LoadMessageAttribute();
        }
        
        public void SetPort(int port)
        {
            _port=port;
            _kcpService = new KcpService(_port, ReceiveMessage);
        }

        public Task<T> Call<T>(IRequest message, IPEndPoint endPoint) where T : class,IResponse
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

        public void SendMessage(object message,IPEndPoint endPoint)
        {
            byte[] messageData = _protobufPacker.ToBytes(message);

            object[] attribute = message.GetType().GetCustomAttributes(typeof(MessageAttribute), false);
            if (attribute.Length <= 0)
                throw new GamekException("class not found MessageAttribute");
            MessageAttribute mgAttribute = (MessageAttribute)attribute[0];
            
            SendMessage(mgAttribute.TypeCode, messageData, endPoint);
        }

        public void SendMessage(ushort typeCode, byte[] messageData, IPEndPoint endPoint)
        {
            _kcpService?.SendMessage(typeCode, messageData, endPoint);
        }

        public void OnUpdate()
        {
            _kcpService?.Update();
        }
        
        public override void OnClose()
        {

        }

        private void LoadMessageAttribute()
        {
            Type[] types = _systemManager.GetTypes;
            foreach (var item in types)
            {
                //get  messagehandler
                object[] attribute = item.GetCustomAttributes(typeof(MessageHandlerAttribute), false);
                if (attribute.Length > 0 && !item.IsAbstract && item.BaseType == typeof(MessageHandlerBase))
                {
                    MessageHandlerAttribute msHanderAttibute = (MessageHandlerAttribute) attribute[0];
                    if (!_messageHandler.ContainsKey(msHanderAttibute.TypeMessage))
                        _messageHandler[msHanderAttibute.TypeMessage] = new List<MessageHandlerBase>();
                    _messageHandler[msHanderAttibute.TypeMessage].Add((MessageHandlerBase)Activator.CreateInstance(item));
                }

                //get message
                attribute = item.GetCustomAttributes(typeof(MessageAttribute), false);
                if (attribute.Length > 0 && !item.IsAbstract)
                {
                    MessageAttribute msAttibute = (MessageAttribute)attribute[0];
                    _messageCodeType[msAttibute.TypeCode] = item;
                }

            }
        }

        private void ReceiveMessage(ushort typeCode, byte[] messageData)
        {
            Type type;
            if (_messageCodeType.TryGetValue(typeCode, out type))
            {
                object message = _protobufPacker.ToMessage(type, messageData);

                IResponse response = message as IResponse;
                if (response != null)
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

            ReceiveMsgCallback?.Invoke(typeCode, messageData);
        }

    }

}