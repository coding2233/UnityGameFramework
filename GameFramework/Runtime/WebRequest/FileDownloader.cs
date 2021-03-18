using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public class FileDownloader
	{
		private IDownloader _downloader;

		/// <summary>
		/// 下载速度 KB/s
		/// </summary>
		public double Speed { get; private set; }

		///// <summary>
		///// 剩余时间 s
		///// </summary>
		//public float RemainingTime { get; private set; }
		/// <summary>
		/// 正在下载
		/// </summary>
		public bool Downloading { get; private set; }

		/// <summary>
		/// 需要下载的文件总数
		/// </summary>
		public int NeedDownloadFileCount { get; private set; }

		/// <summary>
		/// 剩余下载的文件总数
		/// </summary>
		public int RemainingFileCount { 
			get {
				return _remainingFiles.Count;
			} 
		}

		//需要下载的文件
		private Dictionary<string, string> _needDownloadFiles = new Dictionary<string, string>();
		//剩余下载的文件
		private List<string> _remainingFiles = new List<string>();
		//正在下载的文件
		private Dictionary<string,ulong> _downloadingFiles = new Dictionary<string, ulong>();
		//下载器的个数
		private int _downloaderCount = 3;
		//下载开始时间
		private float _downloadStartTime = 0;
		//下载的文件大小
		private ulong _downloadSize = 0;
		//下载进度 <当前正在下载的文件名称,下载的文件大小(KB),下载的时间(s),下载的速度(KB/s)>
		private Action<string, double, float, double> _downloadCallback;
		//下载完成的回调
		private Action<string> _downloadCompleteCallback;
		//下载失败的回调 <下载到本地的文件路径，下载错误>
		private Action<string, string> _downloadErrorCallback;

		internal FileDownloader(IDownloader downloader)
		{
			_downloader = downloader;
		}

		public void OnUpdate()
		{
			if (Downloading)
			{
				if (_remainingFiles.Count > 0)
				{
					if (_downloadingFiles.Count < _downloaderCount)
					{
						string localPath = _remainingFiles[0];
						_remainingFiles.RemoveAt(0);
						//下载文件
						DownloadFile(localPath);
					}
				}
				else
				{
					if (_downloadingFiles.Count == 0)
					{
						StopDownload();
					}
				}
			}
		}

		#region 外部接口
		/// <param name="remotePath"></param>
		/// <param name="localPath"></param>
		public void AddDownloadFile(string remotePath,string localPath)
		{
			if (Downloading)
				return;
			if (!_needDownloadFiles.ContainsKey(localPath))
			{
				_needDownloadFiles.Add(localPath, remotePath);
				_remainingFiles.Add(localPath);
			}
		}

		/// <summary>
		/// 开始下载
		/// </summary>
		public bool StartDownload(Action<string, double, float, double> downloadCallback,Action<string> downloadCompleteCallback, Action<string, string> downloadErrorCallback)
		{
			if (_needDownloadFiles.Count <= 0|| Downloading)
				return false;

			//下载回调
			_downloadCallback = downloadCallback;
			_downloadCompleteCallback = downloadCompleteCallback;
			_downloadErrorCallback = downloadErrorCallback;
			//开始下载
			Downloading = true;
			_downloadStartTime = Time.realtimeSinceStartup;
			NeedDownloadFileCount = _needDownloadFiles.Count;
			_downloadSize = 0;
			return true;
		}


		/// <summary>
		/// 停止下载
		/// </summary>
		public void StopDownload()
		{
			if (Downloading)
			{
				//下载回调
				_downloadCallback = null;
				_downloadCompleteCallback = null;
				_downloadErrorCallback = null;
				_downloadingFiles.Clear();
				_remainingFiles.Clear();
				_needDownloadFiles.Clear();
				Downloading = false;
			}
		}

		#endregion

		#region 事件回调

		//下载回调
		private void OnDownloadCallback(string localPath, bool isDone, ulong downloadedBytes, float downloadProgress)
		{
			_downloadingFiles[localPath] = downloadedBytes;

			float downloadTime = Time.realtimeSinceStartup - _downloadStartTime;
			ulong downloadSize = _downloadSize;
			foreach (var item in _downloadingFiles)
			{
				downloadSize += item.Value;
			}
			double downloadKBSize = downloadSize / 1024.0f;
			Speed = downloadKBSize / downloadTime;
			_downloadCallback?.Invoke(localPath, downloadKBSize, downloadTime, Speed);
			if (isDone)
			{
				_downloadSize += downloadedBytes;
				//清理下载完成的文件
				RemoveDownloadFile(localPath);
				//下载完成回调
				_downloadCompleteCallback?.Invoke(localPath);
			}
		}

		//下载错误
		private void OnDownloadError(string localPath, string error)
		{
			_downloadErrorCallback?.Invoke(localPath, error);
			//清理下载完成的文件
			RemoveDownloadFile(localPath);
		}

		#endregion

		#region 内部函数
		//下载文件
		private void DownloadFile(string localPath)
		{
			_downloadingFiles.Add(localPath, 0);
			string remotePath = _needDownloadFiles[localPath];
			_downloader.Download(remotePath, localPath, OnDownloadCallback, OnDownloadError);
		}

		//移除下载文件
		private void RemoveDownloadFile(string localPath)
		{
			//清理下载完成的文件
			_downloadingFiles.Remove(localPath);
			_needDownloadFiles.Remove(localPath);
		}
		#endregion

	}
}
