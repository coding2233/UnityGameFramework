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
				bool isSmall = IsIconSmall(selectionRect);

				Rect iconRect = GetIconRect(selectionRect, isSmall);
				Rect textRect = GetTextRect(selectionRect, isSmall);
				if (assetPath.Equals("Assets"))
					return;
				//Rect addIcon = new Rect(iconRect.x +20, iconRect.y+20, 16, 16);
				//GUI.DrawTexture(iconRect, EditorResourceLibrary.GetTexture2D("icons/fsm_ignore"));
				Color c = Color.white;
				ColorUtility.TryParseHtmlString("#e76f51", out c);
				//GUI.DrawTexture(GetAddIconRect(iconRect, isSmall), EditorResourceLibrary.GetTexture2D("icons/fsm_run"));
				//	GUI.DrawTexture(iconRect, Resources.Load<Texture2D>("folder"),ScaleMode.ScaleToFit,false,1.2f,c,0,0);
				//if (AssetDatabase.GetSubFolders(assetPath) != null)//FolderOpened Icon
				//{
				//	//GUI.DrawTexture(iconRect, EditorGUIUtility.IconContent("Folder On Icon").image, ScaleMode.ScaleToFit, true, 1.0f, c, 0, 0);
				//}
				//else
				//{
				//	GUI.DrawTexture(iconRect, EditorGUIUtility.IconContent("Folder Icon").image, ScaleMode.ScaleToFit, true, 1.0f, c, 0, 0);
				//}
				//	EditorGUI.DrawRect(selectionRect,);
				//	GUI.DrawTexture
				//	GUI.DrawTexture(GetAddIconRect(iconRect, isSmall), Resources.Load<Texture2D>("android"));
				//GUI.DrawTexture();
				//GUI.Label(textRect, EditorGUIUtility.IconContent("Folder Icon"));
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
			rect.x=rect.width;
			rect.y = rect.width;
			rect.width = rect.height = isSmall?16:16;
			return rect;
		}

		#endregion
	}
}