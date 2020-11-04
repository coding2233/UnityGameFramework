using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public interface IDebuggerWindow
    {
        void OnInit(params object[] args);

        void OnEnter();

        void OnDraw();

        void OnExit();

        void OnClose();
    }

    public abstract class DebuggerWindowBase : IDebuggerWindow
    {
        public virtual void OnInit(params object[] args)
        {
        }

        public virtual void OnClose()
        {
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }
        public virtual void OnDraw()
        {
        }

    }

    public abstract class ToolbarDebuggerWindow : IDebuggerWindow
    {
        protected IDebuggerWindow[] _childWindows;
        protected string[] _windowsTitle;
        protected int _selectIndex = -1;
        protected IDebuggerWindow _currentWindow;

        public virtual void OnInit(params object[] args)
        {
        }

        public virtual void OnClose()
        {
            _currentWindow = null;
            _windowsTitle = null;
            if (_childWindows != null)
            {
                foreach (var item in _childWindows)
                {
                    item.OnClose();
                }
                _childWindows = null;
            }
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnDraw()
        {
            int selectIndex = GUILayout.Toolbar(_selectIndex, _windowsTitle, GUILayout.Height(30f), GUILayout.MaxWidth(Screen.width));
            if (_currentWindow == null && _childWindows.Length > 0)
            {
                selectIndex = 0;
            }
            if (selectIndex != _selectIndex)
            {
                _currentWindow?.OnExit();
                _selectIndex = selectIndex;
                _currentWindow = _childWindows[_selectIndex];
                _currentWindow.OnEnter();
            }
            if (_currentWindow != null)
            {
                _currentWindow.OnDraw();
            }
        }

        #region  内部函数
        protected void SetChildWindows(IDebuggerWindow[] childWindows, string[] windowsTitle, params object[] args)
        {
            _childWindows = childWindows;
            _windowsTitle = windowsTitle;
            foreach (var item in childWindows)
            {
                item.OnInit(args);
            }
        }
        #endregion

    }
}
