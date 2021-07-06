//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #加载资源状态# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年7月8日 14点39分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Wanderer.GameFramework
{
    [FSM()]
    public class LoadResourceState : FSMState<GameStateContext>
    {
        //  private FSM<GameStateContext> _fsmGameStateContext;
        private bool _flag = false;

        #region 重写函数
        public override void OnInit(FSM<GameStateContext> fsm)
        {
            base.OnInit(fsm);
            //  _fsmGameStateContext=fsm;
        }

        public override void OnEnter(FSM<GameStateContext> fsm)
        {
            base.OnEnter(fsm);

            _flag = false;

            GameMode.Event.AddListener<ResourceAssetPathsMapReadyEventArgs>(OnResourceAssetPathsMapReady);

            //设置ab包的加载方式
         //   GameMode.Resource.SetResourceHelper(new BundleResourceHelper());
            //资源管理类的初始设置
            //GameMode.Resource.Asset.SetResource();
        }

        public override void OnExit(FSM<GameStateContext> fsm)
        {
            GameMode.Event.RemoveListener<ResourceAssetPathsMapReadyEventArgs>(OnResourceAssetPathsMapReady);

            base.OnExit(fsm);
        }



        public override void OnUpdate(FSM<GameStateContext> fsm)
        {
            base.OnUpdate(fsm);

            //切换到预加载的状态
            if (_flag)
            {
                ChangeState<PreloadState>(fsm);
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }


        #endregion

        #region  事件回调
        //资源准备完毕
        private void OnResourceAssetPathsMapReady(object sender, IEventArgs e)
        {
            _flag = true;
        }
        #endregion

    }
}
