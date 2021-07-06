//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #view的实现虚类 继承Monobehaviour# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年6月22日 16点59分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public abstract class UIView : MonoBehaviour
    {
        //外部的回调
        protected Action<string> _callBack;

        /// <summary>
        /// 当前UI的UIContext
        /// </summary>
        public IUIContext UIContext { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="uiContext"></param>
        public virtual void OnInit(IUIContext uiContext)
        {
            UIContext = uiContext;
        }
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="uiContext"></param>
        public virtual void OnFree(IUIContext uiContext)
        { }
        /// <summary>
        /// 更新界面
        /// </summary>
        /// <param name="uiContext"></param>
        public virtual void OnUpdate(IUIContext uiContext,float deltaTime)
        { }
        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="parameters">不确定参数</param>
        public virtual void OnEnter(IUIContext uiConext, Action<string> callBack=null,params object[] parameters)
        {
            _callBack = callBack;
        }
        /// <summary>
        /// 退出界面
        /// </summary>
        public virtual void OnExit(IUIContext uiConext)
        {
            _callBack = null;
        }
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
        { }
        /// <summary>
        /// 动画结束
        /// </summary>
        /// <param name="uiAnim"></param>
        public virtual void OnAnimationComplete(IUIAnimation uiAnim)
        { }
        /// <summary>
        /// 设置深度
        /// </summary>
        /// <param name="depth"></param>
        public virtual void SetDepth(int depth)
        {}
        /// <summary>
        /// 调用回调
        /// </summary>
        /// <param name="data"></param>
        protected virtual void Call(string data)
        {
            _callBack?.Invoke(data);
        }

        /// <summary>
        /// UIModel变量更新
        /// </summary>
        /// <param name="key"></param>
        internal virtual void OnUIModelVariableChanged(string key)
        {
            
        }

        /// <summary>
        /// 本身物体的销毁
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (UIContext != null)
            {
                OnFree(UIContext);
            }
        }
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
