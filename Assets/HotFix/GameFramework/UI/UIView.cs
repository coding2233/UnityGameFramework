//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #view的实现虚类 热更新 不继承Monobehaviour# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 16点59分# </time>
//-----------------------------------------------------------------------

using UnityEngine;
namespace HotFix.Taurus
{
    public abstract class UIView : IUIView
    {
        public GameObject gameObject { get; }

        protected UIView(GameObject uiView)
        {
            gameObject = uiView;
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="parameters">不确定参数</param>
        public abstract void OnEnter(params object[] parameters);
        /// <summary>
        /// 退出界面
        /// </summary>
        public abstract void OnExit();
        /// <summary>
        /// 暂停界面
        /// </summary>
        public abstract void OnPause();
        /// <summary>
        /// 恢复界面
        /// </summary>
        public abstract void OnResume();
   
    }
}