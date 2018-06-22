//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #游戏模块的基类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 14点59分# </time>
//-----------------------------------------------------------------------

namespace GameFramework.Taurus
{
    public abstract class GameFrameworkModule
    {
        /// <summary>
        /// 关闭当前模块
        /// </summary>
        public abstract void OnClose();

    }
}
