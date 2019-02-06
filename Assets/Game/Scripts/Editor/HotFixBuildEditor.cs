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
		private static string _luaFilePath = "Game/Scripts/Lua";
		private static string _hotFixPath = "Game/HotFix";

		static HotFixBuildEditor()
		{
			//强制设置unity后台运行
			PlayerSettings.runInBackground = true;

			_luaFilePath = Path.Combine(Application.dataPath, _luaFilePath);
			_hotFixPath = Path.Combine(Application.dataPath, _hotFixPath);

			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(_luaFilePath, "*.lua");
			fileSystemWatcher.Changed += UpdateLuaTxtFile;
			fileSystemWatcher.Created += UpdateLuaTxtFile;
			fileSystemWatcher.Deleted += DelecteLuaTxtFile;

			//启动事件
			fileSystemWatcher.EnableRaisingEvents = true;
		}

		//删除lua文件
		private static void DelecteLuaTxtFile(object sender, FileSystemEventArgs e)
		{
			string path =Path.Combine(_hotFixPath,Path.GetFileName(e.Name) + ".txt");
			if (File.Exists(path))
				File.Delete(path);
		}

		//更新lua文件
		private static void UpdateLuaTxtFile(object sender, FileSystemEventArgs e)
		{
			string path = Path.Combine(_hotFixPath, Path.GetFileName(e.Name) + ".txt");
			File.Copy(e.FullPath, path, true);
			Debug.Log(".lua==>.lua.txt 转换完成");
		}

	}
}