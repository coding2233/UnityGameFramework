//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #Id生成类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点36分# </time>
//-----------------------------------------------------------------------

namespace GameFramework.Taurus
{
	public static class IdGenerater
	{
		private static ushort value;
		/// <summary>
		/// 计算Id
		/// </summary>
		/// <returns>Id</returns>
		public static long GenerateId()
		{
			string timeStr = System.DateTime.Now.ToString("yyyyMMddHHmmss");
			long time = long.Parse(timeStr);
			return (time << 16) + ++value;
		}
	}
}
