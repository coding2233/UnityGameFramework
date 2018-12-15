//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #游戏状态管理类# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月25日 10点54分# </time>
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Reflection;

namespace HotFix.Taurus
{
    public sealed class GameStateManager : GameFrameworkModule, IUpdate, IFixedUpdate
    {
        #region 属性
        private GameStateContext _stateContext;
        private GameState _startState;
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
        /// <param name="assembly">重写游戏状态所在的程序集</param>
        public void CreateContext(Type[] types)
        {
            if (_stateContext != null)
                return;
			UnityEngine.Debug.Log(types.Length);
            GameStateContext stateContext = new GameStateContext();
            List<GameState> listState = new List<GameState>();
            
            foreach (var item in types)
            {
                object[] attribute = item.GetCustomAttributes(typeof(GameStateAttribute), false);
                if (attribute.Length <= 0 || item.IsAbstract)
                    continue;
				
				GameStateAttribute stateAttribute = attribute[0] as GameStateAttribute;
				if (stateAttribute == null)
					continue;

				if (stateAttribute.StateType == GameStateType.Ignore)
                    continue;
                object obj = Activator.CreateInstance(item);
				GameState gs = obj as GameState;
				if (gs!=null)
                {
                    listState.Add(gs);
                    if (stateAttribute.StateType == GameStateType.Start)
                        _startState = gs;
                }
            }
            stateContext.SetAllState(listState.ToArray());
            _stateContext = stateContext;
        }
        /// <summary>
        /// 设置状态开始
        /// </summary>
        public void SetStateStart()
        {
            if (_stateContext != null && _startState != null)
                _stateContext.SetStartState(_startState);
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