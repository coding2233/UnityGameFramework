//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #可操作物体的状态# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点43分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Taurus
{
	public sealed class OperationAssetStatus : ScriptableObject
	{
		private static OperationAssetStatus _instance;
		public List<string> _allStates;

		public static OperationAssetStatus Instance
		{
			get
			{
				if (_instance == null)
					_instance = Resources.Load<OperationAssetStatus>("OperationAssetStatus");
				else
					return _instance;
				if (_instance == null)
					_instance = new OperationAssetStatus();
				return _instance;
			}
		}

		public OperationAssetStatus()
		{
			_allStates = new List<string>() { "Normal", "Broken" };
		}

		public void Add(string str)
		{
			if (_allStates.Contains(str))
				return;

			_allStates.Add(str);
		}


		public void Remove()
		{
			if (_allStates.Count > 1)
				_allStates.RemoveAt(_allStates.Count - 1);
		}

		public void Replace(int key, string value)
		{
			if (key >= _allStates.Count)
				return;
			if (_allStates.Contains(value))
				return;

			_allStates[key] = value;
		}

		public int GetStatus(string name)
		{
			int index = _allStates.IndexOf(name);

			return 1 << index;
		}

		public string GetName(int index)
		{
			index = 1 >> index;

			if (index < _allStates.Count)
			{
				return _allStates[index];
			}
			return "";
		}

	}
}
