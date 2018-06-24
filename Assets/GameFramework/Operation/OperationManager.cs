//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #可操作物体的管理类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点46分# </time>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace GameFramework.Taurus
{
	public sealed class OperationManager : GameFrameworkModule
	{
		/// <summary>
		/// 所有的可以操作物体
		/// </summary>
		public readonly Dictionary<long, OperationAssetConfig> AllOperationAssets;

		public OperationManager()
		{
			AllOperationAssets = new Dictionary<long, OperationAssetConfig>();
		}

		/// <summary>
		/// 设置所有的可操作物体
		/// </summary>
		/// <param name="opAssets"></param>
		public void SetAllOperationAssets(OperationAssetConfig[] opAssets)
		{
			foreach (var item in opAssets)
			{
				AllOperationAssets[item.OperationId] = item;
			}
		}
		

		public override void OnClose()
		{
			AllOperationAssets.Clear();
		}

	}
}
