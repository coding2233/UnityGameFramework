//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #事件管理器# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月25日 10点50分# </time>
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;

namespace HotFix.Taurus
{
    public sealed class EventManager : GameFrameworkModule
    {
        #region 属性
        //所有的事件
        private readonly Dictionary<int, Action<object, IEventArgs>> _allActions = new Dictionary<int, Action<object, IEventArgs>>();
        #endregion

        #region 外部接口
        /// <summary>
        /// 添加事件监听函数
        /// </summary>
        /// <typeparam name="T">事件的类型</typeparam>
        /// <param name="handler">监听事件的回调函数</param>
        public void AddListener<T>(Action<object, IEventArgs> handler) where T : IEventArgs
        {
            int eventId = typeof(T).GetHashCode();
            Action<object, IEventArgs> eventHandler = null;
            if (!_allActions.TryGetValue(eventId, out eventHandler))
                _allActions[eventId] = handler;
            else
            {
                eventHandler += handler;
                _allActions[eventId] = eventHandler;
            }
        }

        /// <summary>
        /// 移除监听函数
        /// </summary>
        /// <typeparam name="T">BaseEventArgs</typeparam>
        /// <param name="handler"></param>
        public void RemoveListener<T>(Action<object, IEventArgs> handler) where T : IEventArgs
        {
            int eventId = typeof(T).GetHashCode();
            Action<object, IEventArgs> eventHandler = null;
            if (!_allActions.TryGetValue(eventId, out eventHandler))
                return;
            else
            {
                if (eventHandler != null)
                {
                    eventHandler -= handler;
                    if (eventHandler == null)
                        _allActions.Remove(eventId);
                    else
                        _allActions[eventId] = eventHandler;
                }
            }
        }

        /// <summary>
        /// 触发事件 带事件类型 事件默认为null
        /// </summary>
        /// <typeparam name="T">事件类</typeparam>
        /// <param name="sender">触发事件的对象</param>
        public void Trigger<T>(object sender) where T : IEventArgs
        {
            int eventId = typeof(T).GetHashCode();
            HanleEvent(sender, eventId);
        }

        /// <summary>
        /// 触发事件 
        /// </summary>
        /// <param name="sender">触发事件的对象</param>
        /// <param name="value">事件参数</param>
        public void Trigger(object sender, IEventArgs value)
        {
            HanleEvent(sender, value);
        }

        #endregion


        #region 内部函数
        //处理事件
        private void HanleEvent(object sender, IEventArgs args)
        {
            if (args == null)
                return;
            Action<object, IEventArgs> eventHandler = null;
            int Id = args.Id;
            if (_allActions.TryGetValue(Id, out eventHandler))
            {
                if (eventHandler != null)
                    eventHandler(sender, args);
            }
        }
        //处理事件  不带参数
        private void HanleEvent(object sender, int eventId)
        {
            Action<object, IEventArgs> eventHandler = null;
            if (_allActions.TryGetValue(eventId, out eventHandler))
            {
                if (eventHandler != null)
                    eventHandler(sender, null);
            }
        }
        #endregion


        public override void OnClose()
        {
            _allActions.Clear();
        }
    }
}