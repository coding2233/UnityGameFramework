//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #游戏模块的基类# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年6月22日 14点59分# </time>
//-----------------------------------------------------------------------

namespace Wanderer.GameFramework
{
    public abstract class GameFrameworkModule
    {
        /// <summary>
        /// 优先级,默认100
        /// </summary>
        public virtual int Priority => 100;

        //初始化
        public virtual void OnInit()
        { }

        /// <summary>
        /// 关闭当前模块
        /// </summary>
        public abstract void OnClose();

        /// <summary>
        /// 缓存大小 字节
        /// </summary>
        /// <returns></returns>
        public virtual long CacheSize()
        {
            return 0;
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        public virtual void ClearCache()
        { }
    }
}
