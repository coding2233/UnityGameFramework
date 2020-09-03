//fsm 标记
//wanderer
//

using System;

namespace Wanderer.GameFramework
{
    [System.AttributeUsage(AttributeTargets.Class,Inherited=true)]
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
        /// <summary>
        //开始状态
        /// <summary>
        Start,
        /// <summary>
        //普通状态
        /// <summary>
        Normal,
        /// <summary>
        //忽略状态
        /// <summary>
        Ignore,
        /// <summary>
        /// 覆盖开始
        /// </summary>
        OverStart,
    }
}