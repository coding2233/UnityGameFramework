//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #游戏状态管理类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 16点48分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Wanderer.GameFramework
{
    public sealed class GameStateManager : GameFrameworkModule, IUpdate, IFixedUpdate
    {
        #region 属性
        private GameStateContext _stateContext;
        private GameState _startState;
        bool _hasOverStart = false;
        /// <summary>
        /// 当前的游戏状态
        /// </summary>
        public GameState CurrentState
        {
            get
            {
                if (_stateContext == null)
                    return null;
                return _stateContext.CurrentState;
            }
        }
        #endregion

        #region 外部接口
         /// <summary>
        /// 创建游戏状态的环境
        /// </summary>
        public void CreateContext()
        {
            if (_stateContext == null)
            {
                _stateContext=new GameStateContext();
            }

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                AddAssemblyStates(assemblies[i]);
            }
        }

        /// <summary>
        /// 创建游戏状态的环境
        /// </summary>
        /// <param name="assembly">重写游戏状态所在的程序集</param>
        public void AddAssemblyStates(Assembly assembly)
        {
           // GameStateContext stateContext = new GameStateContext();
            List<GameState> listState = new List<GameState>();

            Type[] types = assembly.GetTypes();
          
            foreach (var item in types)
            {
                object[] attribute = item.GetCustomAttributes(typeof(GameStateAttribute), true);
                if (attribute.Length <= 0 || item.IsAbstract)
                    continue;
                GameStateAttribute stateAttribute = (GameStateAttribute)attribute[0];
                if (stateAttribute.StateType == GameStateType.Ignore)
                    continue;
                object obj = Activator.CreateInstance(item);
                GameState gs = obj as GameState;
                if (gs != null)
                {
                    listState.Add(gs);
                    if (stateAttribute.StateType == GameStateType.Start)
                    {
                        if(!_hasOverStart)
                            _startState = gs;
                    }
                    //如果有overstar 则以OverStart为准
                    else if(stateAttribute.StateType == GameStateType.OverStart)
                    {
                        _startState = gs;
                        _hasOverStart=true;
                    }
                }
            }
            _stateContext.AddStateRange(listState);
           // _stateContext = stateContext;
        }

       

        /// <summary>
        /// 设置状态开始
        /// </summary>
        public void SetStateStart()
        {
            if (_stateContext != null && _startState != null)
                _stateContext.SetStartState(_startState);
            else
            {
                throw new GameException("Can't find StateContext or StartState!!");
            }
        }
        #endregion

        #region 重写函数
        /// <summary>
        /// 渲染帧函数
        /// </summary>
        public void OnUpdate()
        {
            if (_stateContext != null)
                _stateContext.Update();
        }
        /// <summary>
        /// 固定帧函数
        /// </summary>
        public void OnFixedUpdate()
        {
            if (_stateContext != null)
                _stateContext.FixedUpdate();
        }
        /// <summary>
        /// 关闭
        /// </summary>
        public override void OnClose()
        {
            _stateContext.Close();
            _stateContext = null;
        }
        #endregion
    }
}
