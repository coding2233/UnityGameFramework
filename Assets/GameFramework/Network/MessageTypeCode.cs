//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #网络通信类型枚举# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月29日 21点29分# </time>
//-----------------------------------------------------------------------

using System;

namespace GameFramework.Taurus
{
	/// <summary>
	/// 列举通信类型的类型
	/// 因为枚举不支持带.的字符串，所以建议继承Google.Protobuf.IMagess的类型不要加命名空间
	/// 暂时简化操作，后面可以再在通信类型上添加标记
	///  </summary>
	public enum MessageTypeCode: ushort
	{
		/// <summary>
		/// proto测试协议
		/// </summary>
		//ProtoTest=0,

	}

}