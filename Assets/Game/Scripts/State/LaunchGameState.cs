//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #启动游戏状态 默认为初始状态# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月8日 14点37分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
	[GameState(GameStateType.Start)]
	public class LaunchGameState : GameState
	{
		#region 重写函数
		public override void OnEnter(params object[] parameters)
		{
			base.OnEnter(parameters);

			//加载更新或加载界面
			//...

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

			//选择更新 | 读取本地 | 编辑器
			switch (GameMode.Resource.ResUpdateType)
			{
				case ResourceUpdateType.Update:
					ChangeState<CheckResourceState>();
					break;
				case ResourceUpdateType.Local:
					ChangeState<LoadResourceState>();
					break;
				case ResourceUpdateType.Editor:
#if UNITY_EDITOR
					GameMode.Resource.SetResourceHelper(new EditorResourceHelper());
					ChangeState<PreloadState>();
#else
					//如果在非编辑器模式下选择了Editor，则默认使用本地文件
					GameMode.Resource.ResUpdateType = ResourceUpdateType.Local;
					ChangeState<LoadResourceState>();
#endif
					break;
			}
		}
		#endregion

	}
}
