//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #复制.lua=>.lua.txt# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2019年2月5日 17点30分# </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace GameFramework.Taurus
{
	[InitializeOnLoad]
	public static class HotFixBuildEditor
	{
		private static string _hotFixPath = "Game/HotFix";

		static HotFixBuildEditor()
		{
			//强制设置unity后台运行
			PlayerSettings.runInBackground = true;

			string folderPath = Path.Combine(Application.dataPath, _hotFixPath);
			
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(folderPath,"*.lua");
			fileSystemWatcher.Changed += (sender, e) =>
			{
				//if (!EditorApplication.isPlayingOrWillChangePlaymode)
				{
					File.Copy(e.FullPath, e.FullPath + ".txt", true);
					AssetDatabase.Refresh();
					Debug.Log(".lua==>.lua.txt 转换完成");
				}
			};

			fileSystemWatcher.EnableRaisingEvents = true;
			
		}
		

	}
}