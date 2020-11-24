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
				if (!assetPath.StartsWith("Assets/Game"))
					return;

				assetPath = assetPath.ToLower();
				bool isSmall = IsIconSmall(selectionRect);
				Rect iconRect = GetIconRect(selectionRect, isSmall);
				//	Rect textRect = GetTextRect(selectionRect, isSmall);

				if (assetPath.StartsWith("assets/game/datatable"))
				{
					GUI.DrawTexture(GetAddIconRect(iconRect, isSmall), EditorResourceLibrary.GetTexture2D("excel"));
				}
				else if (assetPath.StartsWith("assets/game/ui"))
				{
					GUI.DrawTexture(GetAddIconRect(iconRect, isSmall), EditorResourceLibrary.GetTexture2D("phone"));
				}
				else if (assetPath.StartsWith("assets/game/xlua"))
				{
					GUI.DrawTexture(GetAddIconRect(iconRect, isSmall), EditorResourceLibrary.GetTexture2D("lua"));
				}
				else if (assetPath.StartsWith("assets/game/update"))
				{
					GUI.DrawTexture(GetAddIconRect(iconRect, isSmall), EditorResourceLibrary.GetTexture2D("update_circle"));
				}
				else if (assetPath.StartsWith("assets/game/scripts"))
				{
					GUI.DrawTexture(GetAddIconRect(iconRect, isSmall), EditorResourceLibrary.GetTexture2D("script_01"));
				}
				else
				{
					GUI.DrawTexture(GetAddIconRect(iconRect, isSmall), EditorResourceLibrary.GetTexture2D("resource"));
				}
			}
		}
		#region 内部函数
		//获取是否为小图标
		private static bool IsIconSmall(Rect rect)
		{
			bool small = rect.width > rect.height;
			return small;
		}

		//获取图标的Rect
		private static Rect GetIconRect(Rect rect, bool isSmall)
		{
			if (isSmall)
			{
				if (rect.x < 16)
				{
					rect.x += 3;
				}
				rect.width = rect.height;
			}
			else
			{
				rect.height = rect.width;
			}
			return rect;
		}


		//获取文本的Rect
		private static Rect GetTextRect(Rect rect, bool isSmall)
		{
			if (isSmall)
			{
				rect.x += 17f;
				rect.width -= 17.0f;
			}
			else
			{
				rect.y += rect.width;
				rect.height -= rect.width;
			}
			return rect;
		}

		private static Rect GetAddIconRect(Rect rect, bool isSmall)
		{
			rect.position += rect.size * 0.5f;
			rect.size *= 0.5f;
			return rect;
		}

		#endregion
	}
}