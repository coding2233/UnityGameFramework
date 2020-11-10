//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #启动游戏状态 默认为初始状态# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年7月8日 14点37分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace Wanderer.GameFramework
{
    [FSM(FSMStateType.Start)]
    public class LaunchGameState : FSMState<GameStateContext>
    {
        #region 重写函数
        public override void OnEnter(FSM<GameStateContext> fsm)
        {
            base.OnEnter(fsm);
        }

        public override void OnExit(FSM<GameStateContext> fsm)
        {
            base.OnExit(fsm);
        }

        public override void OnInit(FSM<GameStateContext> fsm)
        {
            base.OnInit(fsm);
        }

        public override void OnUpdate(FSM<GameStateContext> fsm)
        {
            base.OnUpdate(fsm);

            //选择更新 | 读取本地 | 编辑器
            switch (GameMode.Resource.ResUpdateType)
            {
                case ResourceUpdateType.None:
                    ChangeState<PreloadState>(fsm);
                    break;
                case ResourceUpdateType.Update:
                    ChangeState<CheckResourceState>(fsm);
                    break;
                case ResourceUpdateType.Local:
                    ChangeState<LoadResourceState>(fsm);
                    break;
                case ResourceUpdateType.Editor:
#if UNITY_EDITOR
                    GameMode.Resource.SetResourceHelper(new EditorResourceHelper());
                    ChangeState<PreloadState>(fsm);
#else
					//如果在非编辑器模式下选择了Editor，则默认使用本地文件
					GameMode.Resource.ResUpdateType = ResourceUpdateType.Local;
					ChangeState<LoadResourceState>(fsm);
#endif
                    break;
            }
        }



        #endregion

    }
}
