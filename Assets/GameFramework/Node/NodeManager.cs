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
		public readonly Dictionary<string, int> IntNodes = new Dictionary<string, int>();
		public readonly Dictionary<string, float> FloatNodes = new Dictionary<string, float>();
		public readonly Dictionary<string, bool> BoolNodes = new Dictionary<string, bool>();
		public readonly Dictionary<string, string> StringNodes = new Dictionary<string, string>();
		public readonly Dictionary<string, object> ObjectNodes = new Dictionary<string, object>();
		#endregion

		#region set
		public void SetInt(string key, int value)
		{
			IntNodes[key] = value;
		}

		public void SetFloat(string key, float value)
		{
			FloatNodes[key] = value;
		}

		public void SetBool(string key, bool value)
		{
			BoolNodes[key] = value;
		}

		public void SetString(string key, string info)
		{
			StringNodes[key] = info;
		}

		public void SetObject(string key, object obj)
		{
			ObjectNodes[key] = obj;
		}
		#endregion

		#region get
		public int GetInt(string key, int defaultValue = -1)
		{
			IntNodes.TryGetValue(key, out defaultValue);
			return defaultValue;
		}

		public float GetFloat(string key, float defaultValue = -1.0f)
		{
			FloatNodes.TryGetValue(key, out defaultValue);
			return defaultValue;
		}

		public bool GetBool(string key, bool defaultValue = false)
		{
			BoolNodes.TryGetValue(key, out defaultValue);
			return defaultValue;
		}

		public string GetString(string key, string defaultValue = "")
		{
			StringNodes.TryGetValue(key, out defaultValue);
			return defaultValue;
		}

		public object GetObject(string key, object defaultValue = null)
		{
			ObjectNodes.TryGetValue(key, out defaultValue);
			return defaultValue;
		}

		public T GetObject<T>(string key) where T : new()
		{
			object obj;
			ObjectNodes.TryGetValue(key, out obj);
			return (T)obj;
		}

		public T GetOrSetObject<T>(string key) where T : new()
		{
			object obj;
			if (!ObjectNodes.TryGetValue(key, out obj))
			{
				T t = new T();
				ObjectNodes.Add(key, t);
				return t;
			}
			return (T)obj;
		}

		#endregion

		#region has
		public bool HasInt(string key)
		{
			return IntNodes.ContainsKey(key);
		}

		public bool HasFloat(string key)
		{
			return FloatNodes.ContainsKey(key);
		}

		public bool HasBool(string key)
		{
			return BoolNodes.ContainsKey(key);
		}

		public bool HasString(string key)
		{
			return StringNodes.ContainsKey(key);
		}

		public bool HasObject(string key)
		{
			return ObjectNodes.ContainsKey(key);
		}
		#endregion

		#region remove
		public void RemoveInt(string key)
		{
			if (IntNodes.ContainsKey(key))
				IntNodes.Remove(key);
		}

		public void RemoveFloat(string key)
		{
			if (FloatNodes.ContainsKey(key))
				FloatNodes.Remove(key);
		}

		public void RemoveBool(string key)
		{
			if (BoolNodes.ContainsKey(key))
				BoolNodes.Remove(key);
		}

		public void RemoveString(string key)
		{
			if (StringNodes.ContainsKey(key))
				StringNodes.Remove(key);
		}

		public void RemoveObject(string key)
		{
			if (ObjectNodes.ContainsKey(key))
				ObjectNodes.Remove(key);
		}
		#endregion

		#region 重写函数
		public override void OnClose()
		{
			IntNodes.Clear();
			FloatNodes.Clear();
			StringNodes.Clear();
			ObjectNodes.Clear();
		}
		#endregion

	}
}
