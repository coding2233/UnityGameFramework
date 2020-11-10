using Cysharp.Threading.Tasks;
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
        //动画开始回调 
        IUITween OnAnimationStart(Action OnAnimStart);
        //动画结束
        IUITween OnAnimationComplete(Action OnAnimComplete);
        //动画切换
        IUITween OnAnimationChanged(Action<IUIAnimation, IUIAnimation> OnAnimChanged);
        //设置动画数组
        IUITween SetAnimations(IUIAnimation[] anims);
        //设置动画队列
        IUITween SetAnimations(Queue<IUIAnimation> anims);
        //播放动画
        IUITween RunAnimation();
    }

    internal class UITween : IUITween
    {
        private Action _onAnimStart;
        private Action _onAnimComplete;
        private Action<IUIAnimation, IUIAnimation> _onAnimChanged;
        private IUIAnimation[] _arrayAnims;
        private Queue<IUIAnimation> _queueAnims;

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
            _onAnimStart = null;
            _onAnimComplete = null;
            _onAnimChanged = null;
            LastUIView = null;
            NextUIView = null;
            _arrayAnims = null;
            _queueAnims = null;

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

        private void SetAnimationStart()
        {
            _onAnimStart?.Invoke();
        }

        private void SetAnimationComplete()
        {
            _onAnimComplete?.Invoke();
            Flush();
        }

        private void SetAnimationChanged(IUIAnimation lastAnim, IUIAnimation nextAnim)
        {
            _onAnimChanged?.Invoke(lastAnim, nextAnim);
        }

        public IUITween OnAnimationChanged(Action<IUIAnimation, IUIAnimation> OnAnimChanged)
        {
            _onAnimChanged += OnAnimChanged;
            return this;
        }

        public IUITween OnAnimationComplete(Action OnAnimComplete)
        {
            _onAnimComplete += OnAnimComplete;
            return this;
        }

        public IUITween OnAnimationStart(Action OnAnimStart)
        {
            _onAnimStart += OnAnimStart;
            return this;
        }

        public IUITween SetAnimations(IUIAnimation[] anims)
        {
            _arrayAnims = anims;
            return this;
        }

        public IUITween SetAnimations(Queue<IUIAnimation> anims)
        {
            _queueAnims = anims;
            return this;
        }

        public IUITween RunAnimation()
        {
            RunAnim();
            return this;
        }

        private async void RunAnim()
        {
            if (_arrayAnims != null && _arrayAnims.Length > 0)
            {
                await UniTask.NextFrame();
                UniTask[] animTask = new UniTask[_arrayAnims.Length];
                for (int i = 0; i < _arrayAnims.Length; i++)
                {
                    //call ui view
                    AnimStartCallUIView(_arrayAnims[i]);
                    animTask[i] = _arrayAnims[i].Run();
                }
                //call tween 
                this.SetAnimationStart();
                await UniTask.WhenAll(animTask);
                await UniTask.NextFrame();
                for (int i = 0; i < _arrayAnims.Length; i++)
                {
                    //call ui view
                    AnimEndCallUIView(_arrayAnims[i]);
                }
                //call tween
                this.SetAnimationComplete();
            }
            else if (_queueAnims != null && _queueAnims.Count > 0)
            {
                await UniTask.NextFrame();
                this.SetAnimationStart();
                IUIAnimation lastAnim = null;
                while (_queueAnims.Count > 0)
                {
                    IUIAnimation nextAnim = _queueAnims.Dequeue();
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