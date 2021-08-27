using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public class LogFile
	{
		private const string _logDirectory = "logs/";
		//日志路径
		private string _logPath;
		public string LogPath
		{
			get
			{
				if (string.IsNullOrEmpty(_logPath))
				{
					_logPath = Path.Combine(Application.persistentDataPath, _logDirectory);
					if (!Directory.Exists(_logPath))
					{
						Directory.CreateDirectory(_logPath);
					}
				}
				return _logPath;
			}
		}
		private Queue<LogNode> _logNodes = new Queue<LogNode>();

		private FileStream _logFileStream;

		private StringBuilder _strBuilder;

		/// <summary>
		/// Unity Logger 是否开启
		/// </summary>
		private bool _useUnityLogger
		{
			get
			{
				return UnityEngine.Debug.unityLogger.logEnabled;
			}
		}

		/// <summary>
		/// 可以写入
		/// </summary>
		private bool _canWrite
		{
			get
			{
				return _logFileStream != null;
			}
		}

		public void Start()
		{
			//监听UnityEngine.Debug
			Application.logMessageReceived += OnLogMessageReceived;

			//检查日志文件所占的空间大小，如果太大还是需要自动删除
			//CheckLogFileSize();
			//创建文件流
			if (_logFileStream == null)
			{
				_strBuilder = new StringBuilder();

				string logFilePath = Path.Combine(LogPath, $"{SystemInfo.deviceUniqueIdentifier}_{DateTime.Now.ToString("yyyy-MM-dd")}.log");
				if (!File.Exists(logFilePath))
				{
					_strBuilder.AppendLine("[INFORMATION]");
					_strBuilder.AppendLine($"Device Unique ID: {SystemInfo.deviceUniqueIdentifier}");
					_strBuilder.AppendLine($"Device Name: {SystemInfo.deviceName}");
					_strBuilder.AppendLine($"Device Type: {SystemInfo.deviceType.ToString()}");
					_strBuilder.AppendLine($"Device Model: {SystemInfo.deviceModel}");
					_strBuilder.AppendLine($"Processor Type: {SystemInfo.processorType}");
					_strBuilder.AppendLine($"Processor Count: {SystemInfo.processorCount.ToString()}");
					_strBuilder.AppendLine($"Processor Frequency: {string.Format("{0} MHz", SystemInfo.processorFrequency.ToString())}");
					_strBuilder.AppendLine($"System Memory Size: {string.Format("{0} MB", SystemInfo.systemMemorySize.ToString())}");
					_strBuilder.AppendLine($"Processor Count: {SystemInfo.processorCount.ToString()}");
					_strBuilder.AppendLine($"Operating System: {SystemInfo.operatingSystem}");
					_strBuilder.AppendLine($"Product Name: {Application.productName}");
					_strBuilder.AppendLine($"Company Name: {Application.companyName}");
					_strBuilder.AppendLine($"Game Identifier: {Application.identifier}");
					_strBuilder.AppendLine($"Application Version: {Application.version}");
					_strBuilder.AppendLine($"Unity Version: {Application.unityVersion}");
					_strBuilder.AppendLine($"Platform: {Application.platform.ToString()}");
					_strBuilder.AppendLine($"System Language: {Application.systemLanguage.ToString()}");
					_strBuilder.AppendLine($"Target Frame Rate: {Application.targetFrameRate.ToString()}");
					_strBuilder.AppendLine($"Internet Reachability: {Application.internetReachability.ToString()}");
					_strBuilder.AppendLine($"Background Loading Priority: {Application.backgroundLoadingPriority.ToString()}");
					_strBuilder.AppendLine($"Device ID: {SystemInfo.graphicsDeviceID.ToString()}");
					_strBuilder.AppendLine($"Device Name: {SystemInfo.graphicsDeviceName}");
					_strBuilder.AppendLine($"Device Vendor ID: {SystemInfo.graphicsDeviceVendorID.ToString()}");
					_strBuilder.AppendLine($"Device Vendor: {SystemInfo.graphicsDeviceVendor}");
					_strBuilder.AppendLine($"Device Type: {SystemInfo.graphicsDeviceType.ToString()}");
					_strBuilder.AppendLine($"Device Version: {SystemInfo.graphicsDeviceVersion}");
					_strBuilder.AppendLine($"Graphics Memory Size: {SystemInfo.graphicsMemorySize.ToString()}");
					_strBuilder.AppendLine($"Multi Threaded: { SystemInfo.graphicsMultiThreaded.ToString()}");
					_strBuilder.AppendLine($"Shader Level: { SystemInfo.graphicsShaderLevel.ToString()}");
					_strBuilder.AppendLine($"Global Maximum LOD: {Shader.globalMaximumLOD.ToString()}");
					_strBuilder.AppendLine($"Global Render Pipeline: {Shader.globalRenderPipeline}");
					_strBuilder.AppendLine("[INFORMATION]");
					_strBuilder.AppendLine();
					//写入设备信息
					File.WriteAllText(logFilePath,_strBuilder.ToString());
				}
				//文件流
				_logFileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
				Task.Run(WriteTask);
			}
		}

	

		/// <summary>
		/// 普通日志
		/// </summary>
		/// <param name="message"></param>
		public void Info(string message)
		{
			if (_useUnityLogger)
			{
				Debug.Log($"<color=green>[Info]</color> {message}");
			}
			else
			{
				Write(message, LogType.Log);
			}
		}

		/// <summary>
		/// 警告日志
		/// </summary>
		/// <param name="message"></param>
		public void Warning(string message)
		{
			if (_useUnityLogger)
			{
				Debug.LogWarning($"<color=yellow>[Warning]</color> {message}");
			}
			else
			{
				Write(message, LogType.Warning);
			}
		}

		/// <summary>
		/// 错误日志
		/// </summary>
		/// <param name="message"></param>
		public void Error(string message)
		{
			if (_useUnityLogger)
			{
				Debug.LogWarning($"<color=red>[Error]</color> {message}");
			}
			else
			{
				Write(message, LogType.Error);
			}
		}

		public void Write(string message,LogType type)
		{
			if (_canWrite)
			{
				string stack = new System.Diagnostics.StackTrace().ToString();
				_logNodes.Enqueue(LogNodePool.Get(message, stack, type));
			}
		}

		public void Close()
		{
			Application.logMessageReceived -= OnLogMessageReceived;

			if (_logFileStream != null)
			{
				_logFileStream.Close();
				_logFileStream.Dispose();
				_logFileStream = null;
			}

			if (_strBuilder != null)
			{
				_strBuilder = null;
			}
		}


		#region 事件回调
		//log 信息回调
		private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
		{
			_logNodes.Enqueue(LogNodePool.Get(condition, stackTrace, type));
		}
		#endregion


		#region 内部函数

		/// <summary>
		/// logoType 转 符号
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private string LogTypeToSymbol(LogType type)
		{
			string symbol = "";
			switch (type)
			{
				case LogType.Error:
					symbol = "$";
					break;
				case LogType.Assert:
					symbol = "$";
					break;
				case LogType.Warning:
					symbol = "!";
					break;
				case LogType.Log:
					symbol = "#";
					break;
				case LogType.Exception:
					symbol = "$";
					break;
				default:
					break;
			}
			return symbol;
		}
		/// <summary>
		/// 任务写日志
		/// </summary>
		private void WriteTask()
		{
			byte[] buffer = null;
			while (_logFileStream != null)
			{
				if (_logNodes.Count > 0)
				{
					_strBuilder.Clear();
					var logNode = _logNodes.Dequeue();
					//日志
					string nowDateTime = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}]";
					_strBuilder.AppendLine(nowDateTime);
					string str = $"[{LogTypeToSymbol(logNode.LogType)}] [{logNode.LogFrameCount}] [{logNode.LogTime.ToString("yyyy-MM-dd HH:mm:ss:ms")}] [{logNode.LogType.ToString()}]";
					_strBuilder.AppendLine(str);
					_strBuilder.AppendLine(logNode.LogMessage);
					_strBuilder.AppendLine(logNode.StackTrack);
					buffer = System.Text.Encoding.UTF8.GetBytes(_strBuilder.ToString());
					_logFileStream.Write(buffer, 0, buffer.Length);
					LogNodePool.Release(logNode);
					//回收
					GC.Collect();
				}
			}
		}
		
		/// <summary>
		/// 获取日志文件的大小
		/// </summary>
		/// <returns></returns>
		public long GetLogFileSize()
		{
			string todayLog = $"{DateTime.Now.ToString("yyyy-MM-dd")}.log";

			long size = 0;
            string[] files = Directory.GetFiles(LogPath);
            if (files != null )
            {
                for (int i = 0; i < files.Length; i++)
                {
					if (files[i].EndsWith(todayLog))
						continue;
					size += File.ReadAllBytes(files[i]).Length;
				}
			}
			return size;
        }

		/// <summary>
		/// 删除日志文件
		/// </summary>
		public void DeleteLogFiles()
		{
			string todayLog = $"{DateTime.Now.ToString("yyyy-MM-dd")}.log";
			string[] files = Directory.GetFiles(LogPath);
			if (files != null)
			{
				for (int i = 0; i < files.Length; i++)
				{
					if (files[i].EndsWith(todayLog))
						continue;
					File.Delete(files[i]);
				}
			}
		}

		#endregion


	}

}