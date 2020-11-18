using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Wanderer.GameFramework
{
	public class Log
	{
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