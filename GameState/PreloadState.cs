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
using UnityEngine;

namespace Wanderer.GameFramework
{
    public partial class PreloadState : FSMState<GameStateContext>
    {
        public override void OnEnter(FSM<GameStateContext> fsm)
        {
            base.OnEnter(fsm);

            Debug.Log("PreloadState");
        }

        // public override void OnExit(FSM<GameStateContext> fsm)
        // {
        //     base.OnExit(fsm);
        // }

        // public override void OnInit(FSM<GameStateContext> fsm)
        // {
        //     base.OnInit(fsm);
        // }

        // public override void OnUpdate(FSM<GameStateContext> fsm)
        // {
        //     base.OnUpdate(fsm);
        // }

    }
}
