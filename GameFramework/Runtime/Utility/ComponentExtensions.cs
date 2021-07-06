using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Wanderer.GameFramework
{
	public static class ComponentExtensions
	{
		/// <summary>
		/// 复制
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <returns></returns>
		public static T Clone<T>(this T t, string[] ignoreNames = null) where T : Component
		{
			return t.Clone<T>(t.gameObject, ignoreNames);
		}


		/// <summary>
		/// 复制
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <returns></returns>
		public static T Clone<T>(this T t,GameObject target,string[] ignoreNames = null) where T : Component
		{
			Type type = typeof(T);
			T clone = target.AddComponent<T>();
			var fields = type.GetFields();
			foreach (var field in fields)
			{
				if (field.IsStatic) continue;
				//检查是否有需要忽略的属性
				if (ignoreNames != null && ignoreNames.Length > 0)
				{
					bool ignore = false;
					for (int i = 0; i < ignoreNames.Length; i++)
					{
						if (field.Name.Equals(ignoreNames[i]))
						{
							ignore = true;
							break;
						}
					}
					if (ignore)
						continue;
				}
				field.SetValue(clone, field.GetValue(t));
			}
			var props = type.GetProperties();
			foreach (var prop in props)
			{
				if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;

				//检查是否有需要忽略属性
				if (ignoreNames != null && ignoreNames.Length > 0)
				{
					bool ignore = false;
					for (int i = 0; i < ignoreNames.Length; i++)
					{
						if (prop.Name.Equals(ignoreNames[i]))
						{
							ignore = true;
							break;
						}
					}
					if (ignore)
						continue;
				}

				prop.SetValue(clone, prop.GetValue(t, null), null);
			}
			return clone;
		}
	}

	

}