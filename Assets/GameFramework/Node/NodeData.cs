//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>$
// <describe> #数据节点# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年8月26日 17点35# </time>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
	public class NodeData<T>:NodeDataBase
	{
		private readonly Dictionary<string, T> _nodeDatas = new Dictionary<string, T>();

		public T Get(string key,T defaultValue = default(T))
		{
			T t;
			if (!_nodeDatas.TryGetValue(key, out t))
				t = defaultValue;
			return t;
		}

		public void Set(string key,T value)
		{
			_nodeDatas[key] = value;
		}

		public bool Has(string key)
		{
			return _nodeDatas.ContainsKey(key);
		}

		public void Remove(string key)
		{
			if (_nodeDatas.ContainsKey(key))
				_nodeDatas.Remove(key);
		}

		public override void Clear()
		{
			_nodeDatas.Clear();
		}
	}
}
