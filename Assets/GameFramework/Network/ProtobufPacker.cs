//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #Protobuf的序列化的实现，后期再提接口# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月29日 21点28分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;

namespace GameFramework.Taurus
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
			IMessage result = (IMessage) Activator.CreateInstance(type);
			result.MergeFrom(datas, 0, datas.Length);
			return result;
		}

	}
}
