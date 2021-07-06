using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

namespace Wanderer.GameFramework
{
	public class ProjectSettingsConfig
	{
		/// <summary>
		/// 加载配置文件
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string Load(string path)
		{
			string fullPath = FullPath(path);
			if (File.Exists(fullPath))
				return File.ReadAllText(fullPath);
			return "";
		}


		/// <summary>
		/// 保存文件
		/// </summary>
		/// <param name="path"></param>
		/// <param name="content"></param>
		public static void Save(string path,string content)
		{
			if (string.IsNullOrEmpty(path)||string.IsNullOrEmpty(content))
				return;
			string fullPath = FullPath(path);
			if (File.Exists(fullPath))
				File.Delete(fullPath);
			File.WriteAllText(fullPath, content);
		}

		/// <summary>
		/// 获取JsonData
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static JsonData LoadJsonData(string path)
		{
			string fullPath = FullPath(path);
			if (File.Exists(fullPath))
			{
				string content = File.ReadAllText(fullPath);
				if (!string.IsNullOrEmpty(content))
				{
					return JsonMapper.ToObject(content); 
				}
			}
			return null;
		}

		/// <summary>
		/// 保存JsonData
		/// </summary>
		/// <param name="path"></param>
		/// <param name="data"></param>
		public static void SaveJsonData(string path, JsonData data)
		{
			if (string.IsNullOrEmpty(path) || data == null)
				return;

			string fullPath = FullPath(path);
			if (File.Exists(fullPath))
				File.Delete(fullPath);
			string content = JsonMapper.ToJson(data);
			File.WriteAllText(fullPath, content);
		}



		//获取完成路径
		private static string FullPath(string path)
		{
			string fullPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "ProjectSettings", path);
			return fullPath;
		}
	}
}