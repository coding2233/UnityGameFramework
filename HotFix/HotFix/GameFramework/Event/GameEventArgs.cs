//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #游戏事件参数的基类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 15点28分# </time>
//-----------------------------------------------------------------------


namespace HotFix.Taurus
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