//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #加载热更新状态# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2019年2月4日 16点10分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Taurus
{
	[GameState]
	public class LoadHotfixState:GameState
	{
		
		public override void OnInit()
		{
		}

		public override void OnEnter(params object[] parameters)
		{
			GameMode.HotFix.LoadHotFix();
		}

		public override void OnExit()
		{
		}

		public override void OnUpdate()
		{
		}

		public override void OnFixedUpdate()
		{
		}
	}
}
