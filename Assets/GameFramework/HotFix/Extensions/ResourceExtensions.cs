//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #Resource模块的热更新接口扩展# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2019年2月6日 19点09分# </time>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameFramework.Taurus
{
	[XLua.LuaCallCSharp]
	public static class ResourceExtensions
	{
		/// <summary>
		/// 加载资源
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resManager"></param>
		/// <param name="result"></param>
		/// <param name="assetBundleName"></param>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public static async Task<T> LoadAsset<T>(this ResourceManager resManager,T result, string assetBundleName, string assetName) where T : UnityEngine.Object
		{
			result = await resManager.LoadAsset<T>(assetBundleName, assetName);
			return result;
		}

	}
}
