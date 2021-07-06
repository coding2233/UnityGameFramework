//fsm context base 方便FsmManager管理
//wanderer
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public abstract class FSMBase:IUpdate
    {
        public abstract void OnBegin();

        public abstract void OnUpdate();

        public abstract void OnStop();

        public abstract void OnClose();
    }
}
