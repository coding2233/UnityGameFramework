//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>$
// <describe> #对象池管理器# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年8月26日 17点23# </time>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
namespace HotFix.Taurus
{
	public class PoolManager:GameFrameworkModule
	{
		private readonly Dictionary<int, ObjectDataBase> _allObjectPool;

		public PoolManager()
		{
			_allObjectPool=new Dictionary<int, ObjectDataBase>();
		}

		public T Spawn<T>() where T:class,new()
		{
			int hashCode = typeof(T).GetHashCode();
			ObjectDataBase objectData;
			if (!_allObjectPool.TryGetValue(hashCode, out objectData))
			{
				objectData = new ObjectData<T>();
				_allObjectPool[hashCode] = objectData;
			}
			ObjectData<T> data= objectData as ObjectData<T>;
			return data.Spawn();
		}

		public void Despawn<T>(T obj) where T : class, new()
		{
			int hashCode = typeof(T).GetHashCode();
			ObjectDataBase objectData;
			if (_allObjectPool.TryGetValue(hashCode, out objectData))
			{
				ObjectData<T> data = objectData as ObjectData<T>;
				data.Despawn(obj);
			}
		}

		public void Clear<T>() where T : class, new()
		{
			int hashCode = typeof(T).GetHashCode();
			ObjectDataBase objectData;
			if (_allObjectPool.TryGetValue(hashCode, out objectData))
			{
				objectData.Clear();
				_allObjectPool.Remove(hashCode);
			}
		}


		public override void OnClose()
		{
			foreach (var item in _allObjectPool.Values)
			{
				item.Clear();
			}
			_allObjectPool.Clear();
		}

	}
}
