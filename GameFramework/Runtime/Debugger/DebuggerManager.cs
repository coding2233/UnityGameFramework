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
        }

        #region  内部函数
        int _selectIndex=-1;
        private void DrawDebuggerFullWindow(int windowId)
        {
            GUI.DragWindow(_dragRect);
            List<string> names = new List<string>();
            names.Add("<b>Open</b>");
            names.Add("<b>Close</b>");
            int selectIndex = GUILayout.Toolbar(_selectIndex,names.ToArray(),GUILayout.Height(30f), GUILayout.MaxWidth(Screen.width));
            if(selectIndex!=_selectIndex)
            {
                _showFullWindow=false;
            }
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
