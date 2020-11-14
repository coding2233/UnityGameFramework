﻿using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public interface IUITween
    {
        UIView LastUIView { get; }

        UIView NextUIView { get; }
        //UITween准备好
        IUITween OnUITweenReady(Action<UIView,UIView> onUITweenReady);
        //动画开始回调 
        IUITween OnAnimationStart(Action<UIView, UIView> onAnimStart);
        //动画结束
        IUITween OnAnimationComplete(Action<UIView, UIView> onAnimComplete);
        //动画切换
        IUITween OnAnimationChanged(Action<IUIAnimation, IUIAnimation> onAnimChanged);
        //设置动画数组
        IUITween SetAnimations(IUIAnimation[] anims);
        //播放动画
        IUITween RunAnimation(bool isQueue = false);
    }

    internal class UITween : IUITween
    {
        private Action<UIView,UIView> _onUITweenReady;
        private Action<UIView, UIView> _onAnimStart;
        private Action<UIView, UIView> _onAnimComplete;
        private Action<IUIAnimation, IUIAnimation> _onAnimChanged;
        private List<IUIAnimation>  _anims=new List<IUIAnimation>();

        /// <summary>
        /// 上一个UIView
        /// </summary>
        public UIView LastUIView { get; private set; }
        /// <summary>
        /// 下一个UIView
        /// </summary>
        public UIView NextUIView { get; private set; }

        public UITween Flush()
        {
            _onUITweenReady = null;
            _onAnimStart = null;
            _onAnimComplete = null;
            _onAnimChanged = null;
            LastUIView = null;
            NextUIView = null;
            _anims.Clear();
            return this;
        }

        public void SetLastUIView(UIView uiView)
        {
            LastUIView = uiView;
        }

        public void SetNextUIView(UIView uiView)
        {
            NextUIView = uiView;
        }

        private void SetUITweenReady()
        {
            _onUITweenReady?.Invoke(LastUIView,NextUIView);
        }

        private void SetAnimationStart()
        {
            _onAnimStart?.Invoke(LastUIView, NextUIView);
        }

        private void SetAnimationComplete()
        {
            _onAnimComplete?.Invoke(LastUIView, NextUIView);
            Flush();
        }

        private void SetAnimationChanged(IUIAnimation lastAnim, IUIAnimation nextAnim)
        {
            _onAnimChanged?.Invoke(lastAnim, nextAnim);
        }

        public IUITween OnUITweenReady(Action<UIView, UIView> onUITweenReady)
        {
            _onUITweenReady = onUITweenReady;
            return this;
        }

        public IUITween OnAnimationChanged(Action<IUIAnimation, IUIAnimation> onAnimChanged)
        {
            _onAnimChanged += onAnimChanged;
            return this;
        }

        public IUITween OnAnimationComplete(Action<UIView, UIView> onAnimComplete)
        {
            _onAnimComplete += onAnimComplete;
            return this;
        }

        public IUITween OnAnimationStart(Action<UIView, UIView> onAnimStart)
        {
            _onAnimStart += onAnimStart;
            return this;
        }

        public IUITween SetAnimations(IUIAnimation[] anims)
        {
            _anims.AddRange(anims);
            return this;
        }


        public IUITween RunAnimation(bool isQueue = false)
        {
            RunAnim(isQueue);
            return this;
        }

        private async void RunAnim(bool isQueue = false)
        {
            //回调准备
            SetUITweenReady();
            //播放动画
            if (_anims != null && _anims.Count > 0)
            {
                await UniTask.NextFrame();
                if (isQueue)
                {
                    this.SetAnimationStart();
                    IUIAnimation lastAnim = null;
					for (int i = 0; i < _anims.Count; i++)
					{
                        IUIAnimation nextAnim = _anims[i];
                        //call ui view
                        if (lastAnim != null)
                        {
                            AnimEndCallUIView(lastAnim);
                        }
                        AnimStartCallUIView(nextAnim);
                        //call tween 
                        this.SetAnimationChanged(lastAnim, nextAnim);
                        await nextAnim.Run();
                        lastAnim = nextAnim;

                    }
                    await UniTask.NextFrame();
                }
				else
				{
                    UniTask[] animTask = new UniTask[_anims.Count];
                    for (int i = 0; i < _anims.Count; i++)
                    {
                        //call ui view
                        AnimStartCallUIView(_anims[i]);
                        animTask[i] = _anims[i].Run();
                    }
                    //call tween 
                    this.SetAnimationStart();
                    await UniTask.WhenAll(animTask);
                    await UniTask.NextFrame();
                    for (int i = 0; i < _anims.Count; i++)
                    {
                        //call ui view
                        AnimEndCallUIView(_anims[i]);
                    }
                }
             
                //call tween
                this.SetAnimationComplete();
            }
        }

        private void AnimStartCallUIView(IUIAnimation uiAnim)
        {
            if (LastUIView != null && LastUIView == uiAnim.TargetUIView)
            {
                LastUIView.OnAnimationStart(uiAnim);
            }
            else if (NextUIView != null && NextUIView == uiAnim.TargetUIView)
            {
                NextUIView.OnAnimationStart(uiAnim);
            }
        }

        private void AnimEndCallUIView(IUIAnimation uiAnim)
        {
            if (LastUIView != null && LastUIView == uiAnim.TargetUIView)
            {
                LastUIView.OnAnimationComplete(uiAnim);
            }
            else if (NextUIView != null && NextUIView == uiAnim.TargetUIView)
            {
                NextUIView.OnAnimationComplete(uiAnim);
            }
        }

    }
}