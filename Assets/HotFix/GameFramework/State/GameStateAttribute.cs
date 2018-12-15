//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #游戏状态的类标记# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月25日 10点52分# </time>
//-----------------------------------------------------------------------


using System;

namespace HotFix.Taurus
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GameStateAttribute : Attribute
    {
        public GameStateType StateType
        {
            get;
            private set;
        }

        public GameStateAttribute()
        {
            StateType = GameStateType.Normal;

        }
        public GameStateAttribute(GameStateType value)
        {
            StateType = value;
        }
    }

    //游戏状态的类型
    public enum GameStateType
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 忽略
        /// </summary>
        Ignore,
        /// <summary>
        /// 开始
        /// </summary>
        Start,
    }
}