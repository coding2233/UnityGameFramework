using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public class UGUIView : UIView
	{
		protected CanvasGroup _canvasGroup;

		protected virtual void Awake()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
			if (_canvasGroup == null)
			{
				_canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}
		}

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="parameters">不确定参数</param>
        public override void OnEnter(IUIContext uiConext, Action<string> callBack = null, params object[] parameters)
        {
            base.OnEnter(uiConext, callBack, parameters);
            _canvasGroup.blocksRaycasts = true;
        }
        /// <summary>
        /// 退出界面
        /// </summary>
        public override void OnExit(IUIContext uiConext)
        {
            base.OnExit(uiConext);
            _canvasGroup.blocksRaycasts = false;
        }
        /// <summary>
        /// 暂停界面
        /// </summary>
        public override void OnPause(IUIContext uiConext)
        {
            base.OnPause(uiConext);
            _canvasGroup.blocksRaycasts = false;
        }
        /// <summary>
        /// 恢复界面
        /// </summary>
        public override void OnResume(IUIContext uiConext)
        {
            base.OnResume(uiConext);
            _canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// 动画开始
        /// </summary>
        /// <param name="uiAnim"></param>
        public override void OnAnimationStart(IUIAnimation uiAnim)
        {
            base.OnAnimationStart(uiAnim);
            _canvasGroup.blocksRaycasts = false;
        }
        /// <summary>
        /// 动画结束
        /// </summary>
        /// <param name="uiAnim"></param>
        public override void OnAnimationComplete(IUIAnimation uiAnim)
        {
            base.OnAnimationComplete(uiAnim);
            _canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// 设置深度
        /// </summary>
        /// <param name="depth"></param>
		public override void SetDepth(int depth)
		{
            base.SetDepth(depth);
            Canvas canvas = GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = depth;
            }
		}
	}
}