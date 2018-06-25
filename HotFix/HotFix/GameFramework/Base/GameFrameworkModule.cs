//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #游戏模块的基类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月25日 10点46分# </time>
//-----------------------------------------------------------------------

namespace HotFix.Taurus
{
    public abstract class GameFrameworkModule
    {
        /// <summary>
        /// 关闭当前模块
        /// </summary>
        public abstract void OnClose();

    }
}