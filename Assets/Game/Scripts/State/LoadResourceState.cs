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

namespace GameFramework.Taurus
{
	[GameState]
	public class LoadResourceState : GameState
	{
		#region 重写函数
		public override void OnEnter(params object[] parameters)
		{
			base.OnEnter(parameters);
            
		    string localPath = Path.Combine(GameMode.Resource.LocalPath, "AssetVersion.txt");
		    AssetBundleVersionInfo versionInfo = JsonUtility.FromJson<AssetBundleVersionInfo>(File.ReadAllText(localPath));
            
            //设置ab包的加载方式
            GameMode.Resource.SetResourceHelper(new BundleResourceHelper());
            //加载ab包的mainfest文件
		    GameMode.Resource.SetMainfestAssetBundle(versionInfo.ManifestAssetBundle, versionInfo.IsEncrypt);
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
		}

		public override void OnInit()
		{
			base.OnInit();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
		}
		#endregion
	}
}
