//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #游戏事件参数的基类# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年6月22日 15点28分# </time>
//-----------------------------------------------------------------------

using System;

namespace Wanderer.GameFramework
{
    public interface IEventArgs
    {
        int Id { get; }
    }

    public abstract class GameEventArgs<T> : IEventArgs where T : IEventArgs
    {
        public int Id
        {
            get
            {
                return typeof(T).GetHashCode();
            }
        }
    }
}
