//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #加载热更新状态# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月15日 16点47分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Taurus
{
	[GameState]
	public class LoadHotfixState : GameState
	{
		//热更新的资源
		private string _dllPath = "Assets/Game/HotFix/HotFix.dll.bytes";
		private string _pdbPath = "Assets/Game/HotFix/HotFix.pdb.bytes";

		#region 重写函数
		public override void OnEnter(params object[] parameters)
		{
			base.OnEnter(parameters);
			//加载热更新
			LoadHotFix();
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
		}

		public override void OnInit()
		{
			base.OnInit();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
		}
		#endregion

		#region 内部函数

		private void LoadHotFix()
		{
			byte[] dllDatas = GameMode.Resource.LoadAsset<TextAsset>(_dllPath).bytes;
			byte[] pdbDatas = null;
#if UNITY_EDITOR
			pdbDatas = GameMode.Resource.LoadAsset<TextAsset>(_pdbPath).bytes;
			GameMode.HotFix.Appdomain.DebugService.StartDebugService(56000);
#endif
			GameMode.HotFix.LoadHotfixAssembly(dllDatas, pdbDatas);
		}

		#endregion

	}
}
