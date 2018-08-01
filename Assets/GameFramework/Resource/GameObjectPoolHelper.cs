//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #对象池管理帮助实现类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 17点05分# </time>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
	internal class GameObjectPoolHelper : MonoBehaviour, IGameObjectPoolHelper
	{
		/// <summary>
		///		对象池名称
		/// </summary>
		public string PoolName { get; set; }

		/// <summary>
		///     对象池所有的预设
		/// </summary>
		private readonly Dictionary<string, PoolPrefabInfo> _prefabs = new Dictionary<string, PoolPrefabInfo>();

		/// <summary>
		///     已经生成并显示物体
		/// </summary>
		private readonly Dictionary<string, List<GameObject>> _spawneds = new Dictionary<string, List<GameObject>>();

		/// <summary>
		///     已经生成未显示物体
		/// </summary>
		private readonly Dictionary<string, Queue<GameObject>> _despawneds = new Dictionary<string, Queue<GameObject>>();

		public void AddPrefab(string assetBundleName,string assetName, PoolPrefabInfo prefabInfo)
		{
			if (_prefabs.ContainsKey(assetName))
			{
				Debug.Log("已经存在资源:" + assetName);
				return;
			}
			if (prefabInfo.Prefab == null)
			{
				//根据assetName,直接从ResourceManager里面加载
				prefabInfo.Prefab = GameFrameworkMode.GetModule<ResourceManager>().LoadAsset<GameObject>(assetBundleName,assetName);
				if (prefabInfo.Prefab == null)
				{
					Debug.Log("预设资源为null:" + assetName);
					return;
				}
			}
			_prefabs[assetName] = prefabInfo;
			_spawneds[assetName] = new List<GameObject>();
			
			Initialization(assetName, prefabInfo);
		}

		public bool HasPrefab(string assetName)
		{
			return _prefabs.ContainsKey(assetName);
		}

		private void Initialization(string assetName, PoolPrefabInfo prefabInfo)
		{
			var objects = new Queue<GameObject>();
			for (var i = 0; i < prefabInfo.PreloadAmount; i++)
			{
				var obj = GameObject.Instantiate(prefabInfo.Prefab);
				obj.transform.SetParent(transform);
				obj.SetActive(false);
				objects.Enqueue(obj);
			}
			_despawneds[assetName] = objects;
		}

		public GameObject Spawn(string assetName)
		{
			//if (!_despawneds.ContainsKey(assetName))
			//{
			//	//在没有添加预设的时候  默认添加一个预设
			//	AddPrefab(assetName, new PoolPrefabInfo());
			//}

			GameObject gameObject;
			Queue<GameObject> queueGos = _despawneds[assetName];
			if (queueGos.Count > 0)
			{
				gameObject = queueGos.Dequeue();
				gameObject.SetActive(true);
			}
			else
			{

				gameObject = GameObject.Instantiate((GameObject)_prefabs[assetName].Prefab);
				gameObject.transform.SetParent(transform);
			}
			_spawneds[assetName].Add(gameObject);

			return gameObject;
		}

		public void Despawn(GameObject go, bool isDestroy = false)
		{
			foreach (var item in _spawneds)
			{
				if (item.Value.Contains(go))
				{
					if (isDestroy)
					{
						item.Value.Remove(go);
						MonoBehaviour.Destroy(go);
					}
					else
					{
						Queue<GameObject> _queueObjs = _despawneds[item.Key];
						if ((_prefabs[item.Key].MaxAmount >= 0)
							&& (item.Value.Count + _queueObjs.Count) > _prefabs[item.Key].MaxAmount)
						{
							item.Value.Remove(go);
							MonoBehaviour.Destroy(go);
						}
						else
						{
							item.Value.Remove(go);
							go.SetActive(false);
							_despawneds[item.Key].Enqueue(go);
						}
					}
					return;
				}
			}
		}

		public void DespawnAll()
		{
			foreach (var item in _spawneds)
				foreach (var go in item.Value)
				{
					item.Value.Remove(go);
					go.SetActive(false);
					_despawneds[item.Key].Enqueue(go);
				}
		}

		public void DestroyAll()
		{
			foreach (var item in _spawneds)
				foreach (var go in item.Value)
				{
					item.Value.Remove(go);
					MonoBehaviour.Destroy(go);
				}

			foreach (var item in _despawneds.Values)
				MonoBehaviour.Destroy(item.Dequeue());
		}

		public void DespawnPrefab(string assetName)
		{
			if (_spawneds.ContainsKey(assetName))
			{
				var objs = _spawneds[assetName];
				foreach (var go in objs)
				{
					objs.Remove(go);
					go.SetActive(false);
					_despawneds[assetName].Enqueue(go);
				}
			}
		}
	}
}
