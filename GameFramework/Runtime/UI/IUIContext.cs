using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public interface IUIContext
	{
		/// <summary>
		/// 名称
		/// </summary>
		string Name { get; }
		/// <summary>
		/// 资源路径
		/// </summary>
		string AssetPath { get; }
		/// <summary>
		/// 支持打开多个界面
		/// </summary>
		bool Multiple { get; }
		/// <summary>
		/// 复制
		/// </summary>
		/// <returns></returns>
		IUIContext Clone();
	}

}