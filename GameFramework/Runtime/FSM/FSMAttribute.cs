//fsm 标记
//wanderer
//

using System;

namespace Wanderer.GameFramework
{
    [System.AttributeUsage(AttributeTargets.Class)]
    public class FSMAttribute : Attribute
    {
        public FSMStateType StateType { get; protected set; }

        public FSMAttribute(FSMStateType stateType = FSMStateType.Normal)
        {
            StateType = stateType;
        }
    }

    public enum FSMStateType
    {
        //开始状态
        Start,
        //普通状态
        Normal,
        //忽略状态
        Ignore
    }
}