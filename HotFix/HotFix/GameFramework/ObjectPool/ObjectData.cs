//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>$
// <describe> #对象池数据类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年8月26日 17点22# </time>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix.Taurus
{
	public class ObjectData<T> :ObjectDataBase where T:class,new()
	{
		private Stack<T> _objects = new Stack<T>();

		public T Spawn()
		{
			if (_objects.Count > 0)
				return _objects.Pop();
			else
				return new T();
		}

		public void Despawn(T obj)
		{
			_objects.Push(obj);
		}

		public override void Clear()
		{
			_objects.Clear();
		}

	}
}
