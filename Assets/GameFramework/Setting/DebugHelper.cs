//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Hu Tao. All rights reserved.
// </copyright>
// <describe> #调试器帮助类# </describe>
// <email> 987947865@qq.com </email>
// <time> #2018年7月25日 17点05分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace GameFramework.Taurus
{
    public class DebugHelper : MonoBehaviour
    {
        #region Private Field
        private DebugType _debugType = DebugType.Console;
        //FPS
        private int _fps = 0;
        private Color _fpsColor = Color.white;
        private float _lastShowFPSTime = 0f;
        private bool _expansion = false;
        private Rect _windowRect = new Rect(0, 0, 100, 60);
        //Console
        private List<LogData> _logInformations = new List<LogData>();
        private int _currentLogIndex = -1;
        private int _infoLogCount = 0;
        private int _warningLogCount = 0;
        private int _errorLogCount = 0;
        private int _fatalLogCount = 0;
        private bool _showInfoLog = true;
        private bool _showWarningLog = true;
        private bool _showErrorLog = true;
        private bool _showFatalLog = true;
        private Vector2 _scrollLogView = Vector2.zero;
        private Vector2 _scrollCurrentLogView = Vector2.zero;
        //DrawCall
        private Vector2 _scrollDrawCallView = Vector2.zero;
        //System
        private Vector2 _scrollSystemView = Vector2.zero;
        #endregion

        private void Awake()
        {
            Application.logMessageReceived += LogCallback;
        }
        private void Update()
        {
            FPSUpdate();
        }
        private void OnGUI()
        {
            if (_expansion)
            {
                _windowRect = GUI.Window(0, _windowRect, ExpansionGUIWindow, "DEBUG");
            }
            else
            {
                _windowRect = GUI.Window(0, _windowRect, ShrinkGUIWindow, "DEBUG");
            }
        }
        private void OnDestory()
        {
            Application.logMessageReceived -= LogCallback;
        }

        /// <summary>
        /// 刷新FPS
        /// </summary>
        private void FPSUpdate()
        {
            float time = Time.realtimeSinceStartup - _lastShowFPSTime;
            if (time >= 1)
            {
                _fps = (int)(1.0f / Time.deltaTime);
                _lastShowFPSTime = Time.realtimeSinceStartup;
            }
        }
        /// <summary>
        /// 日志回调
        /// </summary>
        private void LogCallback(string condition, string stackTrace, LogType type)
        {
            LogData log = new LogData();
            log.time = DateTime.Now.ToString("HH:mm:ss");
            log.message = condition;
            log.stackTrace = stackTrace;
            if (type == LogType.Assert)
            {
                log.type = "Fatal";
                _fatalLogCount += 1;
            }
            else if (type == LogType.Exception || type == LogType.Error)
            {
                log.type = "Error";
                _errorLogCount += 1;
            }
            else if (type == LogType.Warning)
            {
                log.type = "Warning";
                _warningLogCount += 1;
            }
            else if (type == LogType.Log)
            {
                log.type = "Info";
                _infoLogCount += 1;
            }
            log.showName = "[" + log.type + "] [" + log.time + "] " + log.message;
            _logInformations.Add(log);

            if (_warningLogCount > 0)
            {
                _fpsColor = Color.yellow;
            }
            if (_errorLogCount > 0)
            {
                _fpsColor = Color.red;
            }
        }

        /// <summary>
        /// 展开的GUI窗口
        /// </summary>
        private void ExpansionGUIWindow(int windowId)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            ExpansionTitleGUI();

            switch (_debugType)
            {
                case DebugType.Console:
                    ExpansionConsoleGUI();
                    break;
                case DebugType.Memory:
                    ExpansionMemoryGUI();
                    break;
                case DebugType.DrawCall:
                    ExpansionDrawCallGUI();
                    break;
                case DebugType.System:
                    ExpansionSystemGUI();
                    break;
                case DebugType.Screen:
                    ExpansionScreenGUI();
                    break;
                case DebugType.Quality:
                    ExpansionQualityGUI();
                    break;
                case DebugType.Time:
                    ExpansionTimeGUI();
                    break;
                case DebugType.Environment:
                    ExpansionEnvironmentGUI();
                    break;
                default:
                    break;
            }
        }
        private void ExpansionTitleGUI()
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = _fpsColor;
            if (GUILayout.Button("FPS: " + _fps, GUILayout.Height(30)))
            {
                _expansion = false;
                _windowRect.width = 100;
                _windowRect.height = 60;
            }
            GUI.contentColor = (_debugType == DebugType.Console ? Color.white : Color.gray);
            if (GUILayout.Button("Console", GUILayout.Height(30)))
            {
                _debugType = DebugType.Console;
            }
            GUI.contentColor = (_debugType == DebugType.Memory ? Color.white : Color.gray);
            if (GUILayout.Button("Memory", GUILayout.Height(30)))
            {
                _debugType = DebugType.Memory;
            }
            GUI.contentColor = (_debugType == DebugType.DrawCall ? Color.white : Color.gray);
            if (GUILayout.Button("DrawCall", GUILayout.Height(30)))
            {
                _debugType = DebugType.DrawCall;
            }
            GUI.contentColor = (_debugType == DebugType.System ? Color.white : Color.gray);
            if (GUILayout.Button("System", GUILayout.Height(30)))
            {
                _debugType = DebugType.System;
            }
            GUI.contentColor = (_debugType == DebugType.Screen ? Color.white : Color.gray);
            if (GUILayout.Button("Screen", GUILayout.Height(30)))
            {
                _debugType = DebugType.Screen;
            }
            GUI.contentColor = (_debugType == DebugType.Quality ? Color.white : Color.gray);
            if (GUILayout.Button("Quality", GUILayout.Height(30)))
            {
                _debugType = DebugType.Quality;
            }
            GUI.contentColor = (_debugType == DebugType.Time ? Color.white : Color.gray);
            if (GUILayout.Button("Time", GUILayout.Height(30)))
            {
                _debugType = DebugType.Time;
            }
            GUI.contentColor = (_debugType == DebugType.Environment ? Color.white : Color.gray);
            if (GUILayout.Button("Environment", GUILayout.Height(30)))
            {
                _debugType = DebugType.Environment;
            }
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
        private void ExpansionConsoleGUI()
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = Color.white;
            if (GUILayout.Button("Clear", GUILayout.Width(80)))
            {
                _logInformations.Clear();
                _fatalLogCount = 0;
                _warningLogCount = 0;
                _errorLogCount = 0;
                _infoLogCount = 0;
                _currentLogIndex = -1;
                _fpsColor = Color.white;
            }
            GUI.contentColor = (_showInfoLog ? Color.white : Color.gray);
            _showInfoLog = GUILayout.Toggle(_showInfoLog, "Info [" + _infoLogCount + "]");
            GUI.contentColor = (_showWarningLog ? Color.white : Color.gray);
            _showWarningLog = GUILayout.Toggle(_showWarningLog, "Warning [" + _warningLogCount + "]");
            GUI.contentColor = (_showErrorLog ? Color.white : Color.gray);
            _showErrorLog = GUILayout.Toggle(_showErrorLog, "Error [" + _errorLogCount + "]");
            GUI.contentColor = (_showFatalLog ? Color.white : Color.gray);
            _showFatalLog = GUILayout.Toggle(_showFatalLog, "Fatal [" + _fatalLogCount + "]");
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();

            _scrollLogView = GUILayout.BeginScrollView(_scrollLogView, "Box", GUILayout.Height(165));
            for (int i = 0; i < _logInformations.Count; i++)
            {
                bool show = false;
                Color color = Color.white;
                switch (_logInformations[i].type)
                {
                    case "Fatal":
                        show = _showFatalLog;
                        color = Color.red;
                        break;
                    case "Error":
                        show = _showErrorLog;
                        color = Color.red;
                        break;
                    case "Info":
                        show = _showInfoLog;
                        color = Color.white;
                        break;
                    case "Warning":
                        show = _showWarningLog;
                        color = Color.yellow;
                        break;
                    default:
                        break;
                }

                if (show)
                {
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = color;
                    if (GUILayout.Toggle(_currentLogIndex == i, _logInformations[i].showName))
                    {
                        _currentLogIndex = i;
                    }
                    GUILayout.FlexibleSpace();
                    GUI.contentColor = Color.white;
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();

            _scrollCurrentLogView = GUILayout.BeginScrollView(_scrollCurrentLogView, "Box", GUILayout.Height(100));
            if (_currentLogIndex != -1)
            {
                GUILayout.Label(_logInformations[_currentLogIndex].message + "\r\n\r\n" + _logInformations[_currentLogIndex].stackTrace);
            }
            GUILayout.EndScrollView();
        }
        private void ExpansionMemoryGUI()
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = Color.white;
            GUILayout.Label("Memory Information");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box", GUILayout.Height(250));
            GUILayout.Label("Total Memory: " + Profiler.GetTotalReservedMemoryLong() / 1000000 + "MB");
            GUILayout.Label("Total Used Memory: " + Profiler.GetTotalAllocatedMemoryLong() / 1000000 + "MB");
            GUILayout.Label("Total Unused Memory: " + Profiler.GetTotalUnusedReservedMemoryLong() / 1000000 + "MB");
            GUILayout.Label("Mono Memory: " + Profiler.GetMonoHeapSizeLong() / 1000000 + "MB");
            GUILayout.Label("Mono Used Memory: " + Profiler.GetMonoUsedSizeLong() / 1000000 + "MB");
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Unload Unused Assets"))
            {
                Resources.UnloadUnusedAssets();
            }
            if (GUILayout.Button("GC Collect"))
            {
                GC.Collect();
            }
            GUILayout.EndHorizontal();
        }
        private void ExpansionDrawCallGUI()
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = Color.white;
            GUILayout.Label("DrawCall Information");
            GUILayout.EndHorizontal();

            _scrollDrawCallView = GUILayout.BeginScrollView(_scrollDrawCallView, "Box");
#if UNITY_EDITOR
            GUILayout.Label("DrawCalls: " + UnityEditor.UnityStats.drawCalls);
            GUILayout.Label("Batches: " + UnityEditor.UnityStats.batches);
            GUILayout.Label("Static Batched DrawCalls: " + UnityEditor.UnityStats.staticBatchedDrawCalls);
            GUILayout.Label("Static Batches: " + UnityEditor.UnityStats.staticBatches);
            GUILayout.Label("Dynamic Batched DrawCalls: " + UnityEditor.UnityStats.dynamicBatchedDrawCalls);
            GUILayout.Label("Dynamic Batches: " + UnityEditor.UnityStats.dynamicBatches);
            if (UnityEditor.UnityStats.triangles > 10000)
            {
                GUILayout.Label("Triangles: " + UnityEditor.UnityStats.triangles / 10000 + "W");
            }
            else
            {
                GUILayout.Label("Triangles: " + UnityEditor.UnityStats.triangles);
            }
            if (UnityEditor.UnityStats.vertices > 10000)
            {
                GUILayout.Label("Vertices: " + UnityEditor.UnityStats.vertices / 10000 + "W");
            }
            else
            {
                GUILayout.Label("Vertices: " + UnityEditor.UnityStats.vertices);
            }
#else
            GUILayout.Label("DrawCall Information can only be visible in editor mode!");
#endif
            GUILayout.EndScrollView();
        }
        private void ExpansionSystemGUI()
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = Color.white;
            GUILayout.Label("System Information");
            GUILayout.EndHorizontal();

            _scrollSystemView = GUILayout.BeginScrollView(_scrollSystemView, "Box");
            GUILayout.Label("Operating System: " + SystemInfo.operatingSystem);
            GUILayout.Label("System Memory: " + SystemInfo.systemMemorySize + "MB");
            GUILayout.Label("Processor Type: " + SystemInfo.processorType);
            GUILayout.Label("Processor Count: " + SystemInfo.processorCount);
            GUILayout.Label("Graphics Name: " + SystemInfo.graphicsDeviceName);
            GUILayout.Label("Graphics Type: " + SystemInfo.graphicsDeviceType);
            GUILayout.Label("Graphics Memory: " + SystemInfo.graphicsMemorySize + "MB");
            GUILayout.Label("Graphics ID: " + SystemInfo.graphicsDeviceID);
            GUILayout.Label("Device Model: " + SystemInfo.deviceModel);
            GUILayout.Label("Device Name: " + SystemInfo.deviceName);
            GUILayout.Label("Device Type: " + SystemInfo.deviceType);
            GUILayout.Label("Device Unique Identifier: " + SystemInfo.deviceUniqueIdentifier);
            GUILayout.EndScrollView();
        }
        private void ExpansionScreenGUI()
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = Color.white;
            GUILayout.Label("Screen Information");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box", GUILayout.Height(250));
            GUILayout.Label("DPI: " + Screen.dpi);
            GUILayout.Label("Program Resolution: " + Screen.width + " x " + Screen.height);
            GUILayout.Label("Device Resolution: " + Screen.currentResolution.ToString());
            GUILayout.Label("Device Sleep: " + (Screen.sleepTimeout == SleepTimeout.NeverSleep ? "Never Sleep" : "System Setting"));
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Sleep"))
            {
                if (Screen.sleepTimeout == SleepTimeout.NeverSleep)
                {
                    Screen.sleepTimeout = SleepTimeout.SystemSetting;
                }
                else
                {
                    Screen.sleepTimeout = SleepTimeout.NeverSleep;
                }
            }
            if (GUILayout.Button("Full Screen"))
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, !Screen.fullScreen);
            }
            GUILayout.EndHorizontal();
        }
        private void ExpansionQualityGUI()
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = Color.white;
            GUILayout.Label("Quality Information");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box", GUILayout.Height(250));
            string value = "";
            if (QualitySettings.GetQualityLevel() == 0)
            {
                value = " [Lowest]";
            }
            else if (QualitySettings.GetQualityLevel() == QualitySettings.names.Length - 1)
            {
                value = " [Highest]";
            }

            GUILayout.Label("Quality Level: " + QualitySettings.names[QualitySettings.GetQualityLevel()] + value);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Decrease Level"))
            {
                QualitySettings.DecreaseLevel();
            }
            if (GUILayout.Button("Increase Level"))
            {
                QualitySettings.IncreaseLevel();
            }
            GUILayout.EndHorizontal();
        }
        private void ExpansionTimeGUI()
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = Color.white;
            GUILayout.Label("Time Information");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box", GUILayout.Height(250));
            GUILayout.Label("System Time: " + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            GUILayout.Label("Realtime Since Startup: " + (int)Time.realtimeSinceStartup);
            GUILayout.Label("Time Scale: " + Time.timeScale);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("0.1 Scale"))
            {
                Time.timeScale = 0.1f;
            }
            if (GUILayout.Button("0.2 Scale"))
            {
                Time.timeScale = 0.2f;
            }
            if (GUILayout.Button("0.5 Scale"))
            {
                Time.timeScale = 0.5f;
            }
            if (GUILayout.Button("1 Scale"))
            {
                Time.timeScale = 1;
            }
            if (GUILayout.Button("2 Scale"))
            {
                Time.timeScale = 2;
            }
            if (GUILayout.Button("5 Scale"))
            {
                Time.timeScale = 5;
            }
            if (GUILayout.Button("10 Scale"))
            {
                Time.timeScale = 10;
            }
            GUILayout.EndHorizontal();
        }
        private void ExpansionEnvironmentGUI()
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = Color.white;
            GUILayout.Label("Environment Information");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box", GUILayout.Height(250));
            GUILayout.Label("Product Name: " + Application.productName);
            GUILayout.Label("Identifier: " + Application.identifier);
            GUILayout.Label("Version: " + Application.version);
            GUILayout.Label("Data Path: " + Application.dataPath);
            GUILayout.Label("Company Name: " + Application.companyName);
            GUILayout.Label("Unity Version: " + Application.unityVersion);
            GUILayout.Label("Has Pro License: " + Application.HasProLicense());
            string internetState = "No Connection";
            if (Application.internetReachability == NetworkReachability.NotReachable)
                internetState = "No Connection";
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                internetState = "WIFI";
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                internetState = "Data Network";
            GUILayout.Label("Network State: " + internetState);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Quit"))
            {
                Application.Quit();
            }
            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// 收缩的GUI窗口
        /// </summary>
        private void ShrinkGUIWindow(int windowId)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            GUI.contentColor = _fpsColor;
            if (GUILayout.Button("FPS: " + _fps, GUILayout.Width(80), GUILayout.Height(30)))
            {
                _expansion = true;
                _windowRect.width = 700;
                _windowRect.height = 360;
            }
            GUI.contentColor = Color.white;
        }
    }
    public struct LogData
    {
        public string time;
        public string type;
        public string message;
        public string stackTrace;
        public string showName;
    }
    public enum DebugType
    {
        Console,
        Memory,
        DrawCall,
        System,
        Screen,
        Quality,
        Time,
        Environment
    }
}
