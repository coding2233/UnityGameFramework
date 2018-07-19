//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #预加载状态# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月8日 15点55分# </time>
//-----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
	[GameState]
	public class PreloadState : GameState
	{
		#region 重写函数
		public override void OnEnter(params object[] parameters)
		{
			base.OnEnter(parameters);

            //测试 直接切换到热更新状态里面去
		    ChangeState<LoadHotfixState>();
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
	}
}
