using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    [DebuggerWindow("Log", -1000)]
    public class LogWindow : IDebuggerWindow
    {
        //当前的日志
        private readonly Queue<LogNode> _logNodes = new Queue<LogNode>();
        //Text编辑器
        private readonly TextEditor _textEditor = new TextEditor();

        //最大的日志数量
        private int _logMaxLine = 100;
        //日志的滚动数据
        private Vector2 _logScrollPosition = Vector2.zero;
        //锁住滚动
        private bool _lockLogScroll = false;
        //各类日志计数        
        private int _infoCount = 0;
        private int _warningCount = 0;
        private int _errorCount = 0;
        private int _fatalCount = 0;
        //过滤日志
        private bool _infoFilter = true;
        private bool _warningFilter = true;
        private bool _errorFilter = true;
        private bool _fatalFilter = true;
        //选中的当前的日志
        private LogNode _selectLogNode = null;

        //堆栈滚动的位置
        private Vector2 _stackScrollPosition = Vector2.zero;

        public void OnInit(params object[] args)
        {
            _logMaxLine = (int)GameFrameworkMode.GetModule<ConfigManager>()["DebugLogMaxLine"];
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
            //刷新计数
            RefreshLogCount();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Clear All", GUILayout.Width(100f)))
                {
                    Clear();
                }
                _lockLogScroll = GUILayout.Toggle(_lockLogScroll, "Lock Scroll", GUILayout.Width(90f));
                GUILayout.FlexibleSpace();
                _infoFilter = GUILayout.Toggle(_infoFilter, string.Format("Info ({0})", _infoCount.ToString()), GUILayout.Width(90f));
                _warningFilter = GUILayout.Toggle(_warningFilter, string.Format("Warning ({0})", _warningCount.ToString()), GUILayout.Width(90f));
                _errorFilter = GUILayout.Toggle(_errorFilter, string.Format("Error ({0})", _errorCount.ToString()), GUILayout.Width(90f));
                _fatalFilter = GUILayout.Toggle(_fatalFilter, string.Format("Fatal ({0})", _fatalCount.ToString()), GUILayout.Width(90f));
            }
            GUILayout.EndHorizontal();

            //日志列表
            GUILayout.BeginVertical("box");
            //锁住滚动
            if (_lockLogScroll)
            {
                _logScrollPosition.y = float.MaxValue;
            }

            _logScrollPosition = GUILayout.BeginScrollView(_logScrollPosition);
            bool hasLogNode = false;
            foreach (LogNode logNode in _logNodes)
            {

                switch (logNode.LogType)
                {
                    case LogType.Log:
                        if (!_infoFilter)
                        {
                            continue;
                        }
                        break;

                    case LogType.Warning:
                        if (!_warningFilter)
                        {
                            continue;
                        }
                        break;

                    case LogType.Error:
                        if (!_errorFilter)
                        {
                            continue;
                        }
                        break;

                    case LogType.Exception:
                        if (!_fatalFilter)
                        {
                            continue;
                        }
                        break;
                }
                if (GUILayout.Toggle(_selectLogNode == logNode, logNode.ToString()))
                {
                    hasLogNode = true;
                    if (_selectLogNode != logNode)
                    {
                        _selectLogNode = logNode;
                        _stackScrollPosition = Vector2.zero;
                    }
                }
            }
            if (!hasLogNode)
            {
                _selectLogNode = null;
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            //日志堆栈信息
            GUILayout.BeginVertical("box");
            _stackScrollPosition = GUILayout.BeginScrollView(_stackScrollPosition, GUILayout.Height(100f));
            {
                if (_selectLogNode != null)
                {
                    GUILayout.BeginHorizontal();
                    Color32 color = _selectLogNode.GetStringColor();
                    GUILayout.Label(string.Format("<color=#{0}{1}{2}{3}><b>{4}</b></color>", color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"), _selectLogNode.LogMessage));
                    if (GUILayout.Button("COPY", GUILayout.Width(60f), GUILayout.Height(30f)))
                    {
                        _textEditor.text = string.Format("{0}{2}{2}{1}", _selectLogNode.LogMessage, _selectLogNode.StackTrack, Environment.NewLine);
                        _textEditor.OnFocus();
                        _textEditor.Copy();
                        _textEditor.text = null;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Label(_selectLogNode.StackTrack);
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();


        }

        public void OnClose()
        {
            _selectLogNode = null;
            Application.logMessageReceived -= OnLogMessageReceived;
        }


        //获取日志的颜色
        public Color GetLogColor()
        {
            RefreshLogCount();
            if (_fatalCount > 0 || _errorCount > 0)
            {
                return Color.red;
            }
            else if (_warningCount > 0)
            {
                return Color.yellow;
            }
            return Color.white;
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

        #region  内部函数
        //刷新日志计数
        private void RefreshLogCount()
        {
            _infoCount = 0;
            _warningCount = 0;
            _errorCount = 0;
            _fatalCount = 0;
            foreach (var item in _logNodes)
            {
                switch (item.LogType)
                {
                    case LogType.Log:
                        _infoCount++;
                        break;
                    case LogType.Warning:
                        _warningCount++;
                        break;
                    case LogType.Error:
                        _errorCount++;
                        break;
                    case LogType.Exception:
                        _fatalCount++;
                        break;
                    default:
                        break;
                }
            }
        }

        //清理所有的日志
        private void Clear()
        {
            _selectLogNode = null;
            while (_logNodes.Count > 0)
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

