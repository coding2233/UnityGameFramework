//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #Protobuf的序列化的实现，后期再提接口# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年8月6日 17点01分# </time>
//-----------------------------------------------------------------------

using System;
using Google.Protobuf;

namespace HotFix.Taurus
{
    public class ProtobufPacker
    {
        public byte[] ToBytes(object message)
        {
            IMessage msg = message as IMessage;
            return msg.ToByteArray();
        }

        public object ToMessage(Type type, byte[] datas)
        {
            IMessage result = (IMessage)Activator.CreateInstance(type);
            result.MergeFrom(datas, 0, datas.Length);
            return result;
        }
    }
}