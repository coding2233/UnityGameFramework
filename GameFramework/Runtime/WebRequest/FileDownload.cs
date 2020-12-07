using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	internal class FileDownload
	{
		/// <summary>
		/// 下载速度 KB/s
		/// </summary>
		public float Speed { get; private set; }

		///// <summary>
		///// 剩余时间 s
		///// </summary>
		//public float RemainingTime { get; private set; }

		/// <summary>
		/// 正在下载
		/// </summary>
		public bool Downloading { get; private set; }

		//需要下载的文件
		private Dictionary<string, string> _needDownloadFiles = new Dictionary<string, string>();
		//正在下载的文件
		private List<string> _downloadingFiles = new List<string>();
		//下载器的个数
		private int _downloaderCount = 3;

		public void OnUpdate()
		{
			if (_needDownloadFiles.Count > 0)
			{
				if (_downloadingFiles.Count < _downloaderCount)
				{
					
				}
			}
		}

		/// <param name="remotePath"></param>
		/// <param name="localPath"></param>
		public void AddDownloadFile(string remotePath,string localPath)
		{
			if (!_needDownloadFiles.ContainsKey(localPath))
			{
				_needDownloadFiles.Add(localPath, remotePath);
				_downloadingFiles.Add(localPath);
			}
		}

	


		#region 内部函数
		private void DownloadFile()
		{
		}
		#endregion

	}
}