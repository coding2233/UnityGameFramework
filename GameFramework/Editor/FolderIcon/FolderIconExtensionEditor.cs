using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wanderer.GameFramework
{
	[InitializeOnLoad]
	public class FolderIconExtensionEditor
	{
		static FolderIconExtensionEditor()
		{
			EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
		}

		private static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			if (AssetDatabase.IsValidFolder(assetPath))
			{
				bool isSmall = IsIconSmall(ref selectionRect);

				//Rect val = selectionRect;
				//if (val.width > 64f)
				//{
				//	float num = (val.width - 64f) * 0.5f;
				//	val.x += num;
				//	val.y += num;
				//	val.width = 64;
				//	val.height = 64;
				//}
				//val = GetBackgroundRect(val, isSmall);
				//GUI.DrawTexture(val, EditorResourceLibrary.GetTexture2D("icons/fsm_ignore"));
				//GUI.DrawTexture(val, Texture2D.whiteTexture);
			}
		}
		#region 内部函数
		private static bool IsIconSmall(ref Rect rect)
		{
			bool small = rect.width > rect.height;
			rect.width = rect.height;

			return small;
		}

		private static Rect GetBackgroundRect(Rect rect, bool isSmall)
		{
			if (isSmall)
			{
				rect.x += 17f;
				rect.width -= 17.0f;
			}
			else
			{
				rect.y += +rect.width;
				rect.height -= rect.width;
			}
			return rect;
		}
		#endregion
	}
}