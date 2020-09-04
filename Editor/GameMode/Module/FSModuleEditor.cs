using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using System.Reflection;

namespace Wanderer.GameFramework
{
     [CustomModuleEditor("FSM Module", 0.141f, 0.408f, 0.635f)]
    public class FSModuleEditor : ModuleEditorBase
    {
        private Vector2 _scrollPos=Vector2.zero;
        Dictionary<Type,List<Type>> _fsmTypes=new Dictionary<Type, List<Type>>();
        Dictionary<Type,FSMStateType> _fsmStateType=new Dictionary<Type, FSMStateType>();
        //图标
        private Texture2D[] _fsmIcons;

        public FSModuleEditor(string name, Color mainColor, GameMode gameMode) : base(name, mainColor, gameMode)
        {
            List<Type> types=new List<Type>();
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
                        _fsmStateType.Add(t,attr.StateType);
                        if(_fsmTypes.TryGetValue(t.BaseType,out List<Type> fsmStates))
                        {
                            fsmStates.Add(t);
                        }
                        else
                        {
                            fsmStates = new List<Type>();
                            fsmStates.Add(t);
                            _fsmTypes.Add(t.BaseType,fsmStates);
                        }
                    }
                }
            }
        
            types.Clear();

            //图标
            _fsmIcons=new Texture2D[5];
            _fsmIcons[0]= Resources.Load<Texture2D>("icons/fsm_start");
            _fsmIcons[1]= Resources.Load<Texture2D>("icons/fsm_normal");
            _fsmIcons[2]= Resources.Load<Texture2D>("icons/fsm_ignore");
            _fsmIcons[3]= Resources.Load<Texture2D>("icons/fsm_over_start");
            _fsmIcons[4]= Resources.Load<Texture2D>("icons/fsm_run");
        }

        public override void OnDrawGUI()
        {
            GUILayout.BeginVertical("HelpBox",GUILayout.Height(150));
            _scrollPos= GUILayout.BeginScrollView(_scrollPos);
            //正在运行
            //List<string> currentStateFullNames = new List<string>();
            if (EditorApplication.isPlaying)
            {
                // PropertyInfo propertyInfo = typeof(FSManager).GetProperty("_fsms",BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
                // object fsms = propertyInfo.GetValue(GameMode.FSM);
                // PropertyInfo currentStatePropertyInfo = item.Key.GetProperty("CurrentState");
                // currentState = currentStatePropertyInfo.GetValue(GameMode.FSM.GetFSM(item.Key));
            }

            foreach (var item in _fsmTypes)
            {
                GUILayout.BeginVertical("Box");
                GUILayout.Label(item.Key.FullName,EditorStyles.boldLabel);
                for (int i = 0; i < item.Value.Count; i++)
                {
                    Type stateType = item.Value[i];
                    GUILayout.BeginHorizontal();
                    Texture2D ico= _fsmIcons[(int)_fsmStateType[stateType]];
                    // if(currentState!=null && currentState.GetType()==stateType)
                    // {
                    //     ico = _fsmIcons[4];
                    // }
                    GUILayout.Label(ico,GUILayout.Width(20),GUILayout.Height(20));
                    GUILayout.Label( stateType.FullName);
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
          _fsmIcons=null;
        }
    }
}