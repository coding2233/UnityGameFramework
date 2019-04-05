﻿//-----------------------------------------------------------------------
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
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace GameFramework.Taurus
{
	[LuaCallCSharp]
	public sealed class HotFixManager:GameFrameworkModule,IUpdate
	{
		//lua的环境变量
		public LuaEnv LuaEnv = new LuaEnv();
		private LuaTable _scriptEnv;

		//资源管理器
		private ResourceManager _resource;
		//lua assetbundle 名称
		private string _luaAssetBundle;
		//lua脚本前缀
		private string _luaPathPrefix;
		//lua脚本扩展名
		private string _luaPathExtension;

		Action _start;
		Action _update;
		Action _close;

		//lua 计时
		private static float _lastGCTime = 0.0f;
		//lua tick的间隔时间
		private const float _luaTickInterval = 1.0f;


		/// <summary>
		/// 加载热更新脚本
		/// </summary>
		/// <param name="assetBundle"></param>
		/// <param name="luaScript"></param>
		/// <param name="luaPathPrefix"></param>
		/// <param name="luaPathExtension"></param>
		public async void LoadHotFix(string assetBundle="hotfix",string luaScript="main",
			string luaPathPrefix="Assets/Game/HotFix",string luaPathExtension=".lua.txt")
		{
			_resource = GameFrameworkMode.GetModule<ResourceManager>();
			_luaAssetBundle = assetBundle;
			_luaPathPrefix = luaPathPrefix;
			_luaPathExtension = luaPathExtension;

			await _resource.LoadAssetBundle(_luaAssetBundle);

			_scriptEnv = LuaEnv.NewTable();
			
			// 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
			LuaTable meta = LuaEnv.NewTable();
			meta.Set("__index", LuaEnv.Global);
			_scriptEnv.SetMetaTable(meta);
			meta.Dispose();

			_scriptEnv.Set("self", this);
			LuaEnv.AddLoader(CustomLoader);
			LuaEnv.DoString($"require '{luaScript}'", luaScript, _scriptEnv);
			_scriptEnv.Get("Start", out _start);
			_scriptEnv.Get("Update", out _update);
			_scriptEnv.Get("Close", out _close);

			_start?.Invoke();
		}

		//自定义加载
		private byte[] CustomLoader(ref string filePath)
		{
			string path = System.IO.Path.Combine(_luaPathPrefix, $"{filePath}{_luaPathExtension}");
			TextAsset textAsset = _resource.LoadAssetSync<TextAsset>(_luaAssetBundle, path);
			return textAsset.bytes;
		}

		/// <summary>
		/// 加载lua的文本
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string LuaScriptLoader(string name)
		{
			string path = System.IO.Path.Combine(_luaPathPrefix, $"{name}{_luaPathExtension}");
			return _resource.LoadAssetSync<TextAsset>(_luaAssetBundle, path).text;
		}

		public void OnUpdate()
		{
			_update?.Invoke();

			//每隔一段时间对lua进行一次GC回收
			if (Time.time - _lastGCTime > _luaTickInterval)
			{
				LuaEnv.Tick();
				_lastGCTime = Time.time;
			}
		}
		
		public override void OnClose()
		{
			_close?.Invoke();
			//_luaEnv?.Dispose();
		}
	}
}
