//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #lua的运行脚本# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2019年2月7日 00点03分# </time>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace GameFramework.Taurus
{
	[LuaCallCSharp]
	public class LuaBehaviour : MonoBehaviour
	{
		/// <summary>
		/// 需要赋值的物体
		/// </summary>
		public Injection[] injections;
		//lua脚本
		private string _luaScript;

		private Action _start;
		private Action _close;
		private Action _update;
		private Action _enable;
		private Action _disable;

		/// <summary>
		/// 运行lua的脚本
		/// </summary>
		public void Run(string luaScriptName)
		{
			if (string.IsNullOrEmpty(_luaScript) && !string.IsNullOrEmpty(luaScriptName))
			{
				LuaTable scriptEnv = GameMode.HotFix.LuaEnv.NewTable();

				// 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
				LuaTable meta = GameMode.HotFix.LuaEnv.NewTable();
				meta.Set("__index", GameMode.HotFix.LuaEnv.Global);
				scriptEnv.SetMetaTable(meta);
				meta.Dispose();

				scriptEnv.Set("self", this);
				foreach (var injection in injections)
				{
					scriptEnv.Set(injection.name, injection.value);
				}

				string luaScript = GameMode.HotFix.LuaScriptLoader(luaScriptName);

				GameMode.HotFix.LuaEnv.DoString(luaScript, luaScriptName, scriptEnv);
				scriptEnv.Get("Start", out _start);
				scriptEnv.Get("Update", out _update);
				scriptEnv.Get("Close", out _close);
				scriptEnv.Get("Enable", out _enable);
				scriptEnv.Get("Disable", out _disable);

				_start?.Invoke();

				_luaScript = luaScriptName;
			}

		}

		private void OnEnable()
		{
			_enable?.Invoke();
		}

		private void OnDisable()
		{
			_disable?.Invoke();
		}
		private void Update()
		{
			_update?.Invoke();
		}
		private void OnDestroy()
		{
			_close?.Invoke();
		}
	}
}
