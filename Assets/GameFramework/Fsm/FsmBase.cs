//fsm context base 方便FsmManager管理
//wanderer
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public abstract class FsmBase
    {
        /// <summary>
        /// 开始
        /// </summary>
        public abstract void OnBegin();

        public abstract void OnUpdate();
        /// <summary>
        /// 结束
        /// </summary>
        public abstract void OnStop();
    }
}
