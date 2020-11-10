//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #view的实现虚类 继承Monobehaviour# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年6月22日 16点59分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public abstract class UIView : MonoBehaviour
    {
        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="parameters">不确定参数</param>
        public virtual void OnEnter(IUIContext uiConext, params object[] parameters)
        { }
        /// <summary>
        /// 退出界面
        /// </summary>
        public virtual void OnExit(IUIContext uiConext)
        { }
        /// <summary>
        /// 暂停界面
        /// </summary>
        public virtual void OnPause(IUIContext uiConext)
        { }
        /// <summary>
        /// 恢复界面
        /// </summary>
        public virtual void OnResume(IUIContext uiConext)
        { }

        /// <summary>
        /// 动画开始
        /// </summary>
        /// <param name="uiAnim"></param>
        public virtual void OnAnimationStart(IUIAnimation uiAnim)
        { 
        }
        /// <summary>
        /// 动画结束
        /// </summary>
        /// <param name="uiAnim"></param>
        public virtual void OnAnimationComplete(IUIAnimation uiAnim)
        { 
        }

        /// <summary>
        /// 设置深度
        /// </summary>
        /// <param name="depth"></param>
        public virtual void SetDepth(int depth)
        { }
    }

    //    //[AttributeUsage(AttributeTargets.Class)]
    //    //public sealed class UIViewAttribute : Attribute
    //    //{
    //    //    public string AssetBundleName { get; private set; }
    //    //    public string ViewPath { get; private set; }

    //    //    public UIViewAttribute(string assetBundleName, string viewPath)
    //    //    {
    //    //        AssetBundleName = assetBundleName;
    //    //        ViewPath = viewPath;
    //    //    }
    //    //}
}
