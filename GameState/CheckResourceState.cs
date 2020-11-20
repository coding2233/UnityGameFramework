//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 wanderer. All rights reserved.
// </copyright>
// <describe> #检查资源状态# </describe>
// <email> dutifulwanderer@gmail.com </email>
// <time> #2018年7月8日 13点20分# </time>
//-----------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Wanderer.GameFramework
{
	[FSM()]
	public class CheckResourceState : FSMState<GameStateContext>
	{
		#region 属性
		//更新标记
		private bool _updateFlag = false;
	
		#endregion

		#region 重写函数
		public override void OnInit(FSM<GameStateContext> fsm)
		{
			base.OnInit(fsm);
		}

		public override void OnEnter(FSM<GameStateContext> fsm)
		{
			_updateFlag = false;

			base.OnEnter(fsm);
			//检查资源版本信息
			GameMode.Resource.Version.CheckResource((needUpdate,localVersion,remoteVersion)=> {
				if (needUpdate)
				{
					GameMode.Resource.Version.UpdateResource(OnResourceUpdateCallback);
				}
				_updateFlag = !needUpdate;
			});
		}

		public override void OnExit(FSM<GameStateContext> fsm)
		{
			base.OnExit(fsm);
		}

		public override void OnUpdate(FSM<GameStateContext> fsm)
		{
			base.OnUpdate(fsm);
			if (_updateFlag)
			{
				//  切换到加载界面
				ChangeState<LoadResourceState>(fsm);
			}
		}
		#endregion


		#region 事件回调
		/// <summary>
		/// 资源下载的回调
		/// </summary>
		/// <param name="result">是否报错</param>
		/// <param name="progress">当前进度 0.0-1.0</param>
		/// <param name="speed">下载速度 KB/s</param>
		/// <param name="size">文件大小 KB</param>
		private void OnResourceUpdateCallback(bool result, float progress, float speed, ulong size)
		{
			if (!result)
			{
				Log.Warning($"资源下载失败,网络错误!!");
				return;
			}
			//下载完成
			if (progress >= 1.0f)
			{
				_updateFlag = true;
			}
		}
		#endregion

		#region 内部函数

		////获取文件
		//private string[] GetFiles(string path)
		//{
		//	List<string> files = new List<string>();
		//	files.AddRange(Directory.GetFiles(path));
		//	foreach (var item in Directory.GetDirectories(path))
		//	{
		//		files.AddRange(GetFiles(item));
		//	}
		//	return files.ToArray();
		//}

		////移动物体
		//private void MoveFiles(string srcPath, string dstPath)
		//{
		//	string[] files = GetFiles(srcPath);
		//	foreach (var item in files)
		//	{
		//		string targetPath = item.Replace(srcPath, dstPath);
		//		string dirPath = Path.GetDirectoryName(targetPath);
		//		if (!Directory.Exists(dirPath))
		//			Directory.CreateDirectory(dirPath);
		//		File.Copy(item, targetPath, true);
		//	}
		//}
		#endregion

	}
}
