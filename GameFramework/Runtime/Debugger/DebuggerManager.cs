using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Wanderer.GameFramework
{
    public class DebuggerManager : GameFrameworkModule, IImGui, IUpdate
    {
        /// <summary>
        /// 窗口缩放
        /// </summary>
        public float WindowScale = 1.5f;
        private bool _showFullWindow = false;
        //皮肤
        private GUISkin _consoleSkin;

        private Rect _defaultSmallRect = new Rect(10, 10, 60, 60);
        private Rect _defaultFullRect = new Rect(10, 10, 700, 500);
        public Rect FullRect;

        private Rect _dragRect = new Rect(0f, 0f, float.MaxValue, 25f);
        private int _selectIndex = -1;

        private List<IDebuggerWindow> _allDebuggerWindows;
        private IDebuggerWindow _currentDebuggerWindow;
        private string[] _debuggerWindowTitle;
        //ugui的EventSystem
        private EventSystem _currentEventSystem;
        //帧率计算
        private FPSCounter _fpsCounter;
        //控制台窗口
        private LogWindow _logWindow;

        private bool _enable = false;

        private bool _instance = false;
        public DebuggerManager()
        {
        }

        public void SetDebuggerEnable(bool enable)
        {
            Debug.unityLogger.logEnabled = enable;
            _enable = enable;
            if (!enable)
                return;

            //实例化
            if (_instance)
                return;
            _instance = true;

            //其他参数
            FullRect = _defaultFullRect;
            // Debug.unityLogger.logEnabled = false;
            _currentEventSystem = EventSystem.current;
            _fpsCounter = new FPSCounter();
            _consoleSkin = Resources.Load<GUISkin>("Console/ConsoleSkin");
            //_debuggerWindowTitle = new string[] { "<b>Close</b>" };
            //实例化 窗口标题以及窗口
            List<string> _windowTitles = new List<string>();
            _allDebuggerWindows = new List<IDebuggerWindow>();
            //在当前类型
            List<DebuggerWindowAttribute> listDebuggerAttribute = new List<DebuggerWindowAttribute>();
            foreach (var item in TypeUtility.AssemblyTypes)
            {
                if (item.IsAbstract)
                    continue;
                object[] objs = item.GetCustomAttributes(typeof(DebuggerWindowAttribute), true);
                if (objs != null && objs.Length > 0)
                {
                    DebuggerWindowAttribute attr = objs[0] as DebuggerWindowAttribute;
                    if (attr != null)
                    {
                        IDebuggerWindow instance = (IDebuggerWindow)System.Activator.CreateInstance(item);
                        if (attr.Priority == 0)
                        {
                            listDebuggerAttribute.Add(attr);
                            _windowTitles.Add(attr.Title);
                            _allDebuggerWindows.Add(instance);
                        }
                        else
                        {
                            bool insert = false;
                            for (int i = 0; i < listDebuggerAttribute.Count; i++)
                            {
                                if (attr.Priority < listDebuggerAttribute[i].Priority)
                                {
                                    listDebuggerAttribute.Insert(i, attr);
                                    _windowTitles.Insert(i, attr.Title);
                                    _allDebuggerWindows.Insert(i, instance);
                                    insert = true;
                                    break;
                                }
                            }
                            if (!insert)
                            {
                                listDebuggerAttribute.Add(attr);
                                _windowTitles.Add(attr.Title);
                                _allDebuggerWindows.Add(instance);
                            }
                        }

                        instance.OnInit(_consoleSkin);
                        //日志窗口特殊处理
                        if (instance is LogWindow)
                        {
                            _logWindow = instance as LogWindow;
                        }
                    }
                }
            }
            listDebuggerAttribute.Clear();
            //添加默认的Close窗口
            _windowTitles.Add("<b>Close</b>");
            _debuggerWindowTitle = _windowTitles.ToArray();
        }

        public void OnImGui()
        {
            if (!_enable)
                return;

            GUISkin lastGuiSkin = GUI.skin;
            Matrix4x4 lastMatrix = GUI.matrix;

            GUI.skin = _consoleSkin;
            GUI.matrix = Matrix4x4.Scale(new Vector3(WindowScale, WindowScale, 1f));
            if (_showFullWindow)
            {
                FullRect = GUILayout.Window(0, FullRect, DrawDebuggerFullWindow, "<b>GAME FRAMEWORK DEBUGGER</b>");
            }
            else
            {
                _defaultSmallRect = GUILayout.Window(0, _defaultSmallRect, DrawDebuggerSmallWindow, "<b>DEBUGGER</b>");
            }
            GUI.matrix = lastMatrix;
            GUI.skin = lastGuiSkin;
        }

        public override void OnClose()
        {
            if (_allDebuggerWindows != null)
            {
                for (int i = 0; i < _allDebuggerWindows.Count; i++)
                {
                    _allDebuggerWindows[i].OnClose();
                }
                _allDebuggerWindows.Clear();
            }
        }

        /// <summary>
        /// 获取窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T GetWindow<T>() where T : class, IDebuggerWindow
        {
            if (_allDebuggerWindows != null)
            {
                for (int i = 0; i < _allDebuggerWindows.Count; i++)
                {
                    if (_allDebuggerWindows[i].GetType() == typeof(T))
                    {
                        return (T)_allDebuggerWindows[i];
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// 重置布局
        /// </summary>
        public void ResetLayout()
        {
            WindowScale = 1.0f;
            FullRect = _defaultFullRect;
        }

        #region  内部函数
        //绘制大窗口
        private void DrawDebuggerFullWindow(int windowId)
        {
            GUI.DragWindow(_dragRect);

            if (_debuggerWindowTitle == null || _debuggerWindowTitle.Length <= 0)
                return;

            int selectIndex = GUILayout.Toolbar(_selectIndex, _debuggerWindowTitle, GUILayout.Height(30f), GUILayout.MaxWidth(Screen.width));
            if (_currentDebuggerWindow == null && _allDebuggerWindows.Count > 0)
            {
                selectIndex = 0;
            }
            if (selectIndex != _selectIndex)
            {
                if (selectIndex == _debuggerWindowTitle.Length - 1)
                {
                    _showFullWindow = false;
                    SetUGuiEventSystem(true);
                    return;
                }
                else
                {
                    _currentDebuggerWindow?.OnExit();
                    _selectIndex = selectIndex;
                    _currentDebuggerWindow = _allDebuggerWindows[_selectIndex];
                    _currentDebuggerWindow.OnEnter();
                }
            }

            //调用窗口
            if (_currentDebuggerWindow != null)
            {
                // GUILayout.BeginVertical("window");
                _currentDebuggerWindow?.OnDraw();
                //  GUILayout.EndVertical();
            }
        }
        //绘制小窗口
        private void DrawDebuggerSmallWindow(int windowId)
        {
            GUI.DragWindow(_dragRect);
            Color defaultColor = GUI.contentColor;
            if (_logWindow != null)
            {
                GUI.contentColor = _logWindow.GetLogColor();
            }
            if (GUILayout.Button(_fpsCounter.FPS.ToString("f2"), GUILayout.Width(100f), GUILayout.Height(40f)))
            {
                _showFullWindow = true;
                SetUGuiEventSystem(false);
            }
            GUI.contentColor = defaultColor;
        }
        //设置ugui EventSystem是否激活
        private void SetUGuiEventSystem(bool active)
        {
            if (_currentEventSystem != null)
            {
                _currentEventSystem.enabled = active;
            }
        }

        public void OnUpdate()
        {
            if (!_enable)
                return;
            _fpsCounter?.OnUpdate();
        }
        #endregion
    }

    //帧率计数
    internal class FPSCounter
    {
        private float _lastTime;
        private float _fpsCount = 0;
        private float _fps;

        public float FPS { get { return _fps; } }

        public FPSCounter()
        {
            _lastTime = Time.realtimeSinceStartup;
            _fpsCount = 0;
            _fps = 0;
        }

        public void OnUpdate()
        {
            float intervalTime = Time.realtimeSinceStartup - _lastTime;
            _fpsCount++;
            if (intervalTime > 1)
            {
                _fps = _fpsCount / intervalTime;
                _fpsCount = 0;
                _lastTime = Time.realtimeSinceStartup;
            }
        }
    }
}
