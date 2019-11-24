//fsm 标记
//wanderer
//

using System;

namespace Wanderer.GameFramework
{
    public class FsmAttribute : Attribute
    {
        public FsmStateType StateType { get; protected set; }

        public FsmAttribute(FsmStateType stateType = FsmStateType.Normal)
        {
            StateType = stateType;
        }
    }

    public enum FsmStateType
    {
        //开始状态
        Start,
        //普通状态
        Normal,
        //忽略状态
        Ignore
    }
}