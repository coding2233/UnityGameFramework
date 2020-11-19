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
		public static T Clone<T>(this T t) where T : Component
		{
			return t.Clone<T>(t.gameObject);
		}


		/// <summary>
		/// 复制
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <returns></returns>
		public static T Clone<T>(this T t,GameObject target) where T : Component
		{
			Type type = typeof(T);
			T clone = target.AddComponent<T>();
			var fields = type.GetFields();
			foreach (var field in fields)
			{
				if (field.IsStatic) continue;
				field.SetValue(clone, field.GetValue(t));
			}
			var props = type.GetProperties();
			foreach (var prop in props)
			{
				if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
				prop.SetValue(clone, prop.GetValue(t, null), null);
			}
			return clone;
		}
	}
}