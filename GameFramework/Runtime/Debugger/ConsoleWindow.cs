using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    [DebuggerWindow("Console")]
    public class ConsoleWindow : IDebuggerWindow
    {
        public void OnInit(params object[] args)
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
        
        public void OnDraw()
        {
            GUILayout.BeginVertical("box");
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical("box");
            // m_StackScrollPosition = GUILayout.BeginScrollView(m_StackScrollPosition, GUILayout.Height(100f));
            // GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public void OnClose()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }


        #region  事件回调
        //log 信息回调
        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
        }
        #endregion

    }
}

