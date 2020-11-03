using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    [DebuggerWindow("Console")]
    public class ConsoleWindow : IDebuggerWindow
    {
        //当前的日志
        private Queue<LogNode> _logNodes = new Queue<LogNode>();
        private int _logMaxLine = 100;
        private Vector2 _logScrollPosition = Vector2.zero;

        public void OnInit(params object[] args)
        {
            _logMaxLine = (int)GameFrameworkMode.GetModule<ConfigManager>()["LogMaxLine"];
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
            _logScrollPosition = GUILayout.BeginScrollView(_logScrollPosition, GUILayout.Height(100f));

            foreach (LogNode logNode in _logNodes)
            {
                // switch (logNode.LogType)
                // {
                //     case LogType.Log:
                //         if (!m_InfoFilter)
                //         {
                //             continue;
                //         }
                //         break;

                //     case LogType.Warning:
                //         if (!m_WarningFilter)
                //         {
                //             continue;
                //         }
                //         break;

                //     case LogType.Error:
                //         if (!m_ErrorFilter)
                //         {
                //             continue;
                //         }
                //         break;

                //     case LogType.Exception:
                //         if (!m_FatalFilter)
                //         {
                //             continue;
                //         }
                //         break;
                // }
                if (GUILayout.Toggle(null == logNode, logNode.ToString()))
                {
                    // if (m_SelectedNode != logNode)
                    // {
                    //     m_SelectedNode = logNode;
                    //     m_StackScrollPosition = Vector2.zero;
                    // }
                }
            }
            GUILayout.EndScrollView();
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
            _logNodes.Enqueue(LogNodePool.Get(condition, stackTrace, type));
            if (_logNodes.Count > _logMaxLine)
            {
                LogNodePool.Release(_logNodes.Dequeue());
            }
        }
        #endregion

    }

    internal class LogNode
    {
        public DateTime LogTime { get; private set; }
        public int LogFrameCount { get; private set; }
        public LogType LogType { get; private set; }
        public string LogMessage { get; private set; }
        public string StackTrack { get; private set; }

        public LogNode Set(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Assert)
            {
                type = LogType.Error;
            }
            LogTime = DateTime.Now;
            LogFrameCount = Time.frameCount;
            LogType = type;
            LogMessage = condition;
            StackTrack = stackTrace;
            return this;
        }

        public override string ToString()
        {
            Color32 color = GetStringColor();
            return string.Format("<color=#{0}{1}{2}{3}>[{4}][{5}] {6}</color>",
                color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"),
                LogTime.ToString("HH:mm:ss.fff"), LogFrameCount.ToString(), LogMessage);
        }

        public Color GetStringColor()
        {
            Color32 color = Color.white;
            switch (LogType)
            {
                case LogType.Log:
                    color = Color.white;
                    break;

                case LogType.Warning:
                    color = Color.yellow;
                    break;

                case LogType.Error:
                    color = Color.red;
                    break;

                case LogType.Exception:
                    color = Color.red;
                    break;
            }

            return color;
        }

    }

    internal static class LogNodePool
    {
        private static readonly ObjectPool<LogNode> _objectPool = new ObjectPool<LogNode>(null, null);

        public static LogNode Get(string condition, string stackTrace, LogType type)
        {
            return _objectPool.Get().Set(condition, stackTrace, type);
        }

        public static void Release(LogNode logNode)
        {
            _objectPool.Release(logNode);
        }
    }

}

