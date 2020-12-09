using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Wanderer.GameFramework
{
    [DebuggerWindow("Console")]
    public class ConsoleWindow : IDebuggerWindow
    {
        //命令
        private List<string> _commands = new List<string>();
        //静态函数 反射调用
        private Dictionary<string, MethodInfo> _commandMethod = new Dictionary<string, MethodInfo>();
        //action执行函数
        private Dictionary<string, Action> _commandAction = new Dictionary<string, Action>();
        //支持反射
        private bool _reflectionSupported = true;
        //支持消息发送
        private bool _sendMessageSupported = true;
        //显示文本
        private List<string> _showTexts = new List<string>();
        //滚动文本的位置坐标
        private Vector2 _showScrollPos = Vector2.zero;
        //输入命令
        private string _inputCommand = "";

        public void OnInit(params object[] args)
        {
            _commandAction.Add("d1", () =>
            {
                Debug.unityLogger.logEnabled = true;
            });
            _commandAction.Add("d0", () =>
            {
                Debug.unityLogger.logEnabled = false;
            });
            _commandAction.Add("f1", () =>
            {
                GameFrameworkMode.GetModule<DebuggerManager>().SetLogFileEnable(true);
            });
            _commandAction.Add("f0", () =>
            {
                GameFrameworkMode.GetModule<DebuggerManager>().SetLogFileEnable(false);
            });
        }

        public void OnEnter()
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }

        public void OnExit()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }

        public void OnClose()
        {
        }

        public void OnDraw()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("clear", GUILayout.Width(60)))
            {
                ClearLines();
            }
            _reflectionSupported = GUILayout.Toggle(_reflectionSupported, "Reflection supported");
            _sendMessageSupported = GUILayout.Toggle(_sendMessageSupported, "SendMessage supported");
            GUILayout.EndHorizontal();

            _showScrollPos = GUILayout.BeginScrollView(_showScrollPos, "box");
            foreach (var item in _showTexts)
            {
                if (item.StartsWith("$ "))
                {
                    GUILayout.Label(item);
                }
                else
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Label(item);
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndScrollView();

            GUILayout.BeginVertical("box", GUILayout.Height(40));
            GUILayout.BeginHorizontal();
            _inputCommand = GUILayout.TextField(_inputCommand, GUILayout.Width(500));
            GUILayout.Space(10);
            if (GUILayout.Button("Exec", GUILayout.Width(60), GUILayout.Height(25)))
            {
                ExecuteCommand(_inputCommand.Trim());
                _inputCommand = "";
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        #region  外部接口
        /// <summary>
        /// 添加命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callAction"></param>
        public void AddCommand(string command, Action callAction)
        {
            if (!_commandAction.ContainsKey(command))
            {
                _commandAction.Add(command, callAction);
            }
        }
        /// <summary>
        /// 添加命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callMethod"></param>
        public void AddCommand(string command, MethodInfo callMethod)
        {
            if (!_commandMethod.ContainsKey(command))
            {
                _commandMethod.Add(command, callMethod);
            }
        }
        /// <summary>
        /// 移除命令
        /// </summary>
        /// <param name="command"></param>
        public void RemoveCommand(string command)
        {
            if (_commandAction.ContainsKey(command))
            {
                _commandAction.Remove(command);
            }
            if (_commandMethod.ContainsKey(command))
            {
                _commandMethod.Remove(command);
            }
        }
        #endregion


        #region 事件回调
        //事件回调
        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            string log = $"<color={GetLogTypeColor(type)}>[{type.ToString()}]</color> {condition}";
            AddLine(log);
        }
        #endregion

        #region 内部函数

        private void AddLine(string line)
        {
            _showTexts.Add(line);
            _showScrollPos.y = float.MaxValue;
        }

        //执行命令
        private void ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            //0 返回
            int defaultResult = ExecuteDefaultCommand(command);
            switch (defaultResult)
            {
                case 0:
                    return;
            }

            //普通命令
            AddLine($"$ <color=green>{command}</color>");
            if (_commandAction.TryGetValue(command, out Action callAction))
            {
                callAction.Invoke();
                return;
            }

            //静态函数命令
            string[] args = command.Split(' ');
            string[] parameters = null;
            if (args != null && args.Length > 1)
            {
                command = args[0];
                //动态添加参数
                parameters = new string[args.Length - 1];
                Array.Copy(args, 1, parameters, 0, parameters.Length);
            }
            MethodInfo callMethod;
            if (!_commandMethod.TryGetValue(command, out callMethod))
            {
               
                int index = command.LastIndexOf('.');
                if (index > 0 && index < command.Length - 1)
                {
                    string fullName = command.Substring(0, index);
                    index++;
                    string methodName = command.Substring(index, command.Length - index);
                    //优先查找反射
                    if (_reflectionSupported)
                    {
                        Type callType = TypeUtility.AllAssemblyTypes.Find(x => x.FullName.Equals(fullName));
                        if (callType != null)
                        {
                            callMethod = callType.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                            if (callMethod != null)
                            {
                                bool call = false;
                                var gp = callMethod.GetParameters();
                                if (gp.Length == 0)
                                {
                                    parameters = null;
                                    call = true;
                                }
                                else if (gp.Length == parameters.Length)
                                {
                                    call = true;
                                }
                                if (call)
                                {
                                    callMethod.Invoke(null, parameters);
                                    return;
                                }
                            }
                        }
                    }

                    //支持查找GameObject SendMessage
                    if (_sendMessageSupported)
                    {
                        GameObject findGameObject = GameObject.Find(fullName);
                        if (findGameObject != null)
                        {
                            if (args == null)
                            {
                                findGameObject.SendMessage(methodName);
                            }
                            else if (args.Length == 1)
                            {
                                findGameObject.SendMessage(methodName, args[0]);
                            }
                            else
                            {
                                findGameObject.SendMessage(methodName, args);
                            }
                            return;
                        }
                    }
                }
            }

            
            //添加反馈
            AddLine($"<color=yellow>$ [{command}]</color> Can't find command or parameters error!");
        }

        //执行默认的命令
        private int ExecuteDefaultCommand(string command)
        {
            switch (command)
            {
                case "clear":
                    ClearLines();
                    return 0;
            }
            return -1;
        }

        //清理数据
        private void ClearLines()
        {
            _showTexts.Clear();
        }

        private string GetLogTypeColor(LogType type)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    return "red";
                case LogType.Warning:
                    return "yellow";
                case LogType.Log:
                    return "white";
            }
            return "white";
        }
        #endregion

    }
}

