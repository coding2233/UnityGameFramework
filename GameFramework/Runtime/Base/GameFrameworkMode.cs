﻿//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #游戏模块的管理类# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年6月22日 14点59分# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Wanderer.GameFramework
{
    public static class GameFrameworkMode
    {
        #region 属性
        //所有的子模块
        private static readonly Dictionary<int, GameFrameworkModule> _allGameModules = new Dictionary<int, GameFrameworkModule>();
        //所有渲染帧函数
        private static List<IUpdate> _allUpdates = new List<IUpdate>();
        //所有的固定帧函数
        private static List<IFixedUpdate> _allFixedUpdates = new List<IFixedUpdate>();
        //unity imgui
        private static List<IImGui> _allImGuis = new List<IImGui>();
        #endregion

        #region 外部接口
        /// <summary>
        /// 获取模块
        /// </summary>
        /// <typeparam name="T">游戏模块</typeparam>
        /// <returns></returns>
        public static T GetModule<T>() where T : GameFrameworkModule, new()
        {
            return (T)GetModule(typeof(T));
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            //根据优先级排序
            //初始化的OnInit
            var orderResult = _allGameModules.OrderBy(x => x.Value.Priority);
			foreach (var item in orderResult)
			{
                item.Value.OnInit();
            }
        }

        /// <summary>
        /// 渲染帧
        /// </summary>
        public static void Update()
        {
            for (int i = 0; i < _allUpdates.Count; i++)
                _allUpdates[i].OnUpdate();
        }

        /// <summary>
        /// 固定帧
        /// </summary>
        public static void FixedUpdate()
        {
            for (int i = 0; i < _allFixedUpdates.Count; i++)
            {
                _allFixedUpdates[i].OnFixedUpdate();
            }
        }

        /// <summary>
        /// imgui
        /// </summary>
        public static void ImGui()
        {
            for (int i = 0; i < _allImGuis.Count; i++)
            {
                _allImGuis[i].OnImGui();
            }
        }

        /// <summary>
        /// 关闭游戏的所有模块
        /// </summary>
        public static void ShutDown()
        {
            //根据优先级 调用OnClose
            var orderResult = _allGameModules.OrderBy(x => x.Value.Priority);
            foreach (var item in orderResult)
            {
                item.Value.OnClose();
            }

            _allUpdates.Clear();
            _allFixedUpdates.Clear();
            _allGameModules.Clear();
        }

        #endregion


        #region 内部函数

        //获取模块
        private static GameFrameworkModule GetModule(Type type)
        {
            int hashCode = type.GetHashCode();
            GameFrameworkModule module = null;
            if (_allGameModules.TryGetValue(hashCode, out module))
                return module;
            module = CreateModule(type);
            return module;
        }

        //创建模块
        private static GameFrameworkModule CreateModule(Type type)
        {
            int hashCode = type.GetHashCode();
            GameFrameworkModule module = (GameFrameworkModule)Activator.CreateInstance(type);
            _allGameModules[hashCode] = module;
            //整理含IUpdate的模块
            var update = module as IUpdate;
            if (update != null)
                _allUpdates.Add(update);
            //整理含IFixed的模块
            var fixedUpdate = module as IFixedUpdate;
            if (fixedUpdate != null)
                _allFixedUpdates.Add(fixedUpdate);
            var imgui = module as IImGui;
            if (imgui != null)
                _allImGuis.Add(imgui);
            return module;
        }

        #endregion

    }
}
