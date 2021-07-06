using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Wanderer.GameFramework
{
	public class Log
	{
		//日志专用StringBuilder
		public static StringBuilder StrBuilder = new StringBuilder();

		public static void Info(string message)
		{
			GameFrameworkMode.GetModule<DebuggerManager>().Log?.Info(message);
		}
		public static void Warning(string message)
		{
			GameFrameworkMode.GetModule<DebuggerManager>().Log?.Warning(message);
		}
		public static void Error(string message)
		{
			GameFrameworkMode.GetModule<DebuggerManager>().Log?.Error(message);
		}
	}
}