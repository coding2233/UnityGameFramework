﻿//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
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
    public abstract class UIView : MonoBehaviour, IUIView
    {
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
