//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #数据节点管理类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点16分# </time>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace GameFramework.Taurus
{
	public sealed class NodeManager:GameFrameworkModule
	{
		#region 属性
		private readonly Dictionary<int, NodeDataBase> _allNodeDatas = new Dictionary<int, NodeDataBase>();
		#endregion

		#region set
		/// <summary>
		/// 设置数据节点
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Set<T>(string key, T value)
		{
			NodeDataBase nodeDataBase;
			int hashCode = typeof(T).GetHashCode();
			if (!_allNodeDatas.TryGetValue(hashCode, out nodeDataBase))
				nodeDataBase = new NodeData<T>();
			NodeData<T> nodeData = nodeDataBase as NodeData<T>;
			nodeData.Set(key, value);
		}
		#endregion

		#region get
		/// <summary>
		/// 获取数据节点
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public T Get<T>(string key,T defaultValue=default(T))
		{
			int hashCode = typeof(T).GetHashCode();
			NodeDataBase nodeDataBase;
			if (_allNodeDatas.TryGetValue(hashCode, out nodeDataBase))
			{
				NodeData<T> nodeData = nodeDataBase as NodeData<T>;
				return nodeData.Get(key, defaultValue);
			}

			return defaultValue;
		}
		#endregion

		#region has
		/// <summary>
		/// 是否包含数据节点
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Has<T>(string key)
		{
			int hashCode = typeof(T).GetHashCode();
			NodeDataBase nodeDataBase;
			if (_allNodeDatas.TryGetValue(hashCode, out nodeDataBase))
			{
				NodeData<T> nodeData = nodeDataBase as NodeData<T>;
				return nodeData.Has(key);
			}
			return false;
		}
		#endregion

		#region remove
		/// <summary>
		/// 移除数据节点
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		public void Remove<T>(string key)
		{
			int hashCode = typeof(T).GetHashCode();
			NodeDataBase nodeDataBase;
			if (_allNodeDatas.TryGetValue(hashCode, out nodeDataBase))
			{
				NodeData<T> nodeData = nodeDataBase as NodeData<T>;
				nodeData.Remove(key);
			}
		}
		#endregion

		#region clear
		/// <summary>
		/// 清除数据节点
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void Clear<T>()
		{
			int hashCode = typeof(T).GetHashCode();
			NodeDataBase nodeDataBase;
			if (_allNodeDatas.TryGetValue(hashCode, out nodeDataBase))
				_allNodeDatas.Remove(hashCode);
		}
		#endregion

		#region 重写函数
		public override void OnClose()
		{
			foreach (var item in _allNodeDatas.Values)
			{
				item.Clear();
			}

			_allNodeDatas.Clear();
		}
		#endregion

	}
}
