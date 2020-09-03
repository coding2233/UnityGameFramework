//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #加载资源状态# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年7月8日 14点39分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Wanderer.GameFramework
{
    [FSM()]
    public class LoadResourceState : FSMState<GameStateContext>
    {
      
        #region 重写函数
        public override void OnEnter(FSM<GameStateContext> fsm)
        {
            base.OnEnter(fsm);

            string localPath = Path.Combine(GameMode.Resource.LocalPath, Utility.GetPlatformName(), "AssetVersion.txt");
            if (!File.Exists(localPath))
                throw new GameException($"can't find AssetVersion: {localPath}");
            AssetBundleVersionInfo versionInfo = JsonUtility.FromJson<AssetBundleVersionInfo>(File.ReadAllText(localPath));

            //设置ab包的加载方式
            GameMode.Resource.SetResourceHelper(new BundleResourceHelper());
            //加载ab包的mainfest文件
            GameMode.Resource.SetMainfestAssetBundle(versionInfo.ManifestAssetBundle);//versionInfo.IsEncrypt

            //切换到预加载的状态
            ChangeState<PreloadState>(fsm);
        }

        public override void OnExit(FSM<GameStateContext> fsm)
        {
            base.OnExit(fsm);
        }

        public override void OnInit(FSM<GameStateContext> fsm)
        {
            base.OnInit(fsm);
        }

        public override void OnUpdate(FSM<GameStateContext> fsm)
        {
            base.OnUpdate(fsm);
        }

        public override string ToString()
        {
            return base.ToString();
        }


        #endregion
    }
}
