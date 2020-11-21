using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;


namespace Wanderer.GameFramework
{
    [CustomModuleEditor("FSM Module", 0.141f, 0.408f, 0.635f)]
    public class FSModuleEditor : ModuleEditorBase
    {
        private Vector2 _scrollPos = Vector2.zero;
        Dictionary<Type, List<Type>> _fsmTypes = new Dictionary<Type, List<Type>>();
        Dictionary<Type, FSMStateType> _fsmStateType = new Dictionary<Type, FSMStateType>();
        //图标
        private Texture2D[] _fsmIcons;
        //当前正在运行的状态的
        List<string> _currentStateFullNames = new List<string>();
        public FSModuleEditor(string name, Color mainColor, GameMode gameMode) : base(name, mainColor, gameMode)
        {
            List<Type> types = new List<Type>();
            _fsmTypes.Clear();
            _fsmStateType.Clear();
            // //获取所有程序的类型
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                types.AddRange(assemblies[i].GetTypes());
            }

            //整理类型是否满足状态
            for (int i = 0; i < types.Count; i++)
            {
                Type t = types[i];
                if (t.IsAbstract)
                    continue;

                object[] objs = t.GetCustomAttributes(typeof(FSMAttribute), true);
                if (objs != null && objs.Length > 0)
                {
                    FSMAttribute attr = objs[0] as FSMAttribute;
                    if (attr != null)
                    {
                        _fsmStateType.Add(t, attr.StateType);
                        if (_fsmTypes.TryGetValue(t.BaseType, out List<Type> fsmStates))
                        {
                            fsmStates.Add(t);
                        }
                        else
                        {
                            fsmStates = new List<Type>();
                            fsmStates.Add(t);
                            _fsmTypes.Add(t.BaseType, fsmStates);
                        }
                    }
                }
            }

            types.Clear();

            //图标
            _fsmIcons = new Texture2D[5];
            _fsmIcons[0] = EditorResourceLibrary.GetTexture2D("icons/fsm_start");
            _fsmIcons[1] = EditorResourceLibrary.GetTexture2D("icons/fsm_normal");
            _fsmIcons[2] = EditorResourceLibrary.GetTexture2D("icons/fsm_ignore");
            _fsmIcons[3] = EditorResourceLibrary.GetTexture2D("icons/fsm_over_start");
            _fsmIcons[4] = EditorResourceLibrary.GetTexture2D("icons/fsm_run");
        }

        public override void OnDrawGUI()
        {
            GUILayout.BeginVertical("HelpBox", GUILayout.Height(150));
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            //正在运行
            _currentStateFullNames.Clear();
            if (EditorApplication.isPlaying)
            {
                BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                FieldInfo prfieldInfo = typeof(FSManager).GetField("_fsms", flag);
                Dictionary<Type, FSMBase> fsms = (Dictionary<Type, FSMBase>)prfieldInfo.GetValue(GameMode.FSM);
                if (fsms != null)
                {
                    foreach (var item in fsms.Values)
                    {
                        if (item == null)
                            continue;
                        prfieldInfo = item.GetType().GetField("_curState", flag);
                        if (prfieldInfo != null)
                        {
                            string fullName = prfieldInfo.GetValue(item).GetType().FullName;
                            _currentStateFullNames.Add(fullName);
                        }
                    }
                }
            }

            foreach (var item in _fsmTypes)
            {
                GUILayout.BeginVertical("Box");
                GUILayout.Label(item.Key.FullName, EditorStyles.boldLabel);
                for (int i = 0; i < item.Value.Count; i++)
                {
                    Type stateType = item.Value[i];
                    GUILayout.BeginHorizontal();
                    Texture2D ico = _fsmIcons[(int)_fsmStateType[stateType]];
                    if (_currentStateFullNames.Count > 0)
                    {
                        for (int m = 0; m < _currentStateFullNames.Count; m++)
                        {
                            if (_currentStateFullNames[m].Equals(stateType.FullName))
                            {
                                ico = _fsmIcons[4];
                                break;
                            }
                        }
                    }
                    GUILayout.Label(ico, GUILayout.Width(20), GUILayout.Height(20));
                    GUILayout.Label(stateType.FullName);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public override void OnClose()
        {
            _fsmTypes.Clear();
            _fsmStateType.Clear();
            _fsmIcons = null;
        }
    }
}