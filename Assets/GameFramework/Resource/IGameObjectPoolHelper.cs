//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #对象池接口# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月24日 17点03分# </time>
//-----------------------------------------------------------------------


using UnityEngine;

namespace GameFramework.Taurus
{

	public interface IGameObjectPoolHelper
	{
		/// <summary>
		/// 对象池名称
		/// </summary>
		string PoolName { set; get; }
		/// <summary>
		/// 加载预设信息
		/// </summary>
		/// <param name="assetName"></param>
		/// <param name="prefabInfo"></param>
		void AddPrefab(string assetBundleName, string assetName, PoolPrefabInfo prefabInfo);
		/// <summary>
		/// 是否包含预设
		/// </summary>
		/// <param name="assetName"></param>
		/// <returns></returns>
		bool HasPrefab(string assetName);
		/// <summary>
		/// 生成物体
		/// </summary>
		/// <param name="assetName"></param>
		/// <returns></returns>
		GameObject Spawn(string assetName);
		/// <summary>
		/// 销毁物体
		/// </summary>
		/// <param name="go"></param>
		/// <param name="isDestroy"></param>
		void Despawn(GameObject go, bool isDestroy = false);
		/// <summary>
		/// 关闭所有物体
		/// </summary>
		void DespawnAll();
		/// <summary>
		/// 销毁所有物体
		/// </summary>
		void DestroyAll();
		/// <summary>
		/// 关闭预设的所有物体
		/// </summary>
		/// <param name="assetName"></param>
		void DespawnPrefab(string assetName);
	}

	//对象池预设信息
	public struct PoolPrefabInfo
	{
		public GameObject Prefab;
		public int PreloadAmount;
		public int MaxAmount;
	}
}
