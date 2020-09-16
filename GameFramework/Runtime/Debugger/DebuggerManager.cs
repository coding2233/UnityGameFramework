using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class DebuggerManager : GameFrameworkModule, IImGui
    {
        /// <summary>
        /// 窗口缩放
        /// </summary>
        public float WindowScale=1.0f;
        private bool _showFullWindow=false;

        private Rect _defaultSmallRect = new Rect(10,10,60,60);
        private Rect _defaultFullRect =new Rect(10,10,640,480);
        private Rect _dragRect = new Rect(0f, 0f, float.MaxValue, 25f);
        private int _selectIndex=-1;

        private List<IDebuggerWindow> _allDebuggerWindows;
        private IDebuggerWindow _currentDebuggerWindow;
        private string[] _debuggerWindowTitle;

        public DebuggerManager()
        {
            _debuggerWindowTitle=new string[]{"<b>Close</b>"};
        }

        public void OnImGui()
        {
            GUISkin lastGuiSkin = GUI.skin;
            Matrix4x4 lastMatrix = GUI.matrix;

            GUI.matrix = Matrix4x4.Scale(new Vector3(WindowScale, WindowScale, 1f));
            if(_showFullWindow)
            {
                _defaultFullRect=GUILayout.Window(0,_defaultFullRect,DrawDebuggerFullWindow,"<b>GAME FRAMEWORK DEBUGGER</b>");
            }
            else
            {
                _defaultSmallRect=GUILayout.Window(0,_defaultSmallRect,DrawDebuggerSmallWindow,"<b>DEBUGGER</b>");
            }
            GUI.matrix = lastMatrix;
            GUI.skin = lastGuiSkin;
        }

        public override void OnClose()
        {
            if(_allDebuggerWindows!=null)
            {
                for (int i = 0; i < _allDebuggerWindows.Count; i++)
                {
                    _allDebuggerWindows[i].OnClose();
                }
                _allDebuggerWindows.Clear();
            }
        }

        #region  内部函数
        
        private void DrawDebuggerFullWindow(int windowId)
        {
            GUI.DragWindow(_dragRect);

            if(_debuggerWindowTitle==null||_debuggerWindowTitle.Length<=0)
                return;

            int selectIndex = GUILayout.Toolbar(_selectIndex,_debuggerWindowTitle,GUILayout.Height(30f), GUILayout.MaxWidth(Screen.width));
            if(selectIndex!=_selectIndex)
            {
                if(selectIndex==_debuggerWindowTitle.Length-1)
                {
                    _showFullWindow=false;
                  //  _currentDebuggerWindow=null;
                    return;
                }
                else
                {
                    _currentDebuggerWindow?.OnExit();
                    _selectIndex=selectIndex;
                    _currentDebuggerWindow=_allDebuggerWindows[_selectIndex];
                    _currentDebuggerWindow.OnEnter();
                }
            }
            //调用窗口
            _currentDebuggerWindow?.OnDraw();
        }

        private void DrawDebuggerSmallWindow(int windowId)
        {
            GUI.DragWindow(_dragRect);
            if (GUILayout.Button("FPS", GUILayout.Width(100f), GUILayout.Height(40f)))
            {
                _showFullWindow = true;
            }
        }
        #endregion
    }

}
