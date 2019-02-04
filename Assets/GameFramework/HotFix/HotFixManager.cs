//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #热更新管理器# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2019年2月4日 15点11分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace GameFramework.Taurus
{
	public sealed class HotFixManager:GameFrameworkModule,IUpdate
	{
		//lua的环境变量
		private LuaEnv _luaEnv = new LuaEnv();
		private LuaTable _scriptEnv;

		Action _start;
		Action _update;
		Action _close;

		public void LoadHotFix(string luaScript)
		{
			_scriptEnv = _luaEnv.NewTable();

			// 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
			LuaTable meta = _luaEnv.NewTable();
			meta.Set("__index", _luaEnv.Global);
			_scriptEnv.SetMetaTable(meta);
			meta.Dispose();

			_scriptEnv.Set("self", this);
			_luaEnv.DoString(luaScript, "HotFixManager", _scriptEnv);
			_scriptEnv.Get("Start", out _start);
			_scriptEnv.Get("Update", out _update);
			_scriptEnv.Get("Close", out _close);

			_start?.Invoke();
		}

		public void OnUpdate()
		{
			_update?.Invoke();
		}
		
		public override void OnClose()
		{
			_close?.Invoke();
		}
	}
}
