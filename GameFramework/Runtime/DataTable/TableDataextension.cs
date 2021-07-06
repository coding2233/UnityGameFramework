using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public static class TableDataExtension
	{
		#region Explicit Conversions
		public static bool ToBool(this TableData tableData)
		{
			return (bool)tableData;
		}
		public static int ToInt(this TableData tableData)
		{
			return (int)tableData;
		}
		public static long ToLong(this TableData tableData)
		{
			return (long)tableData;
		}
		public static float ToFloat(this TableData tableData)
		{
			return (float)tableData;
		}
		public static double ToDouble(this TableData tableData)
		{
			return (double)tableData;
		}
		public static Vector2 ToVector2(this TableData tableData)
		{
			return (Vector2)tableData;
		}
		public static Vector3 ToVector3(this TableData tableData)
		{
			return (Vector3)tableData;
		}
		public static Color ToColor(this TableData tableData)
		{
			return (Color32)tableData;
		}
		#endregion
	}
}