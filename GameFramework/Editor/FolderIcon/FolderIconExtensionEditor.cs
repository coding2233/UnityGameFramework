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
			//暂时只支持windows
#if UNITY_EDITOR_WIN
			EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
#endif
		}

		private static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			if (AssetDatabase.IsValidFolder(assetPath))
			{
				if (!assetPath.StartsWith("Assets/Game"))
					return;

				bool isSmall = IsIconSmall(selectionRect);
				Rect iconRect = GetIconRect(selectionRect, isSmall);
				Rect addIconRect = GetAddIconRect(iconRect, isSmall);
				//	Rect textRect = GetTextRect(selectionRect, isSmall);
				if (assetPath.StartsWith("assets/game/animation", StringComparison.CurrentCultureIgnoreCase))
				{
					DrawAddIcon(addIconRect, "animation");
				}
				else if (assetPath.StartsWith("assets/game/audio", StringComparison.CurrentCultureIgnoreCase))
				{
					DrawAddIcon(addIconRect, "audio");
				}
				else if (assetPath.StartsWith("assets/game/datatable", StringComparison.CurrentCultureIgnoreCase))
				{
					DrawAddIcon(addIconRect, "excel");
				}
				else if (assetPath.StartsWith("assets/game/font", StringComparison.CurrentCultureIgnoreCase))
				{
					DrawAddIcon(addIconRect, "font");
				}
				else if (assetPath.StartsWith("assets/game/minigame", StringComparison.CurrentCultureIgnoreCase))
				{
					DrawAddIcon(addIconRect, "game");
				}
				else if (assetPath.StartsWith("assets/game/scene", StringComparison.CurrentCultureIgnoreCase))
				{
					DrawAddIcon(addIconRect, "map");
				}
				else if (assetPath.StartsWith("assets/game/spine", StringComparison.CurrentCultureIgnoreCase))
				{
					DrawAddIcon(addIconRect, "spine");
				}
				else if (assetPath.StartsWith("assets/game/texture", StringComparison.CurrentCultureIgnoreCase))
				{
					DrawAddIcon(addIconRect, "picture");
				}
				else if (assetPath.StartsWith("assets/game/ui", StringComparison.CurrentCultureIgnoreCase))
				{
					DrawAddIcon(addIconRect, "phone");
				}
				else if (assetPath.StartsWith("assets/game/xlua", StringComparison.CurrentCultureIgnoreCase))
				{
					DrawAddIcon(addIconRect, "lua");
				}
				else if (assetPath.StartsWith("assets/game/update", StringComparison.CurrentCultureIgnoreCase))
				{
					DrawAddIcon(addIconRect, "update_circle");
				}
				else if (assetPath.StartsWith("assets/game/scripts", StringComparison.CurrentCultureIgnoreCase))
				{
					DrawAddIcon(addIconRect, "script_01");
				}
				else if (assetPath.StartsWith("assets/game/shader", StringComparison.CurrentCultureIgnoreCase))
				{
					GUI.DrawTexture(addIconRect, EditorGUIUtility.IconContent("d_ShaderVariantCollection Icon").image, ScaleMode.ScaleToFit);
				}
				else if (assetPath.StartsWith("assets/game/material", StringComparison.CurrentCultureIgnoreCase))
				{
					GUI.DrawTexture(addIconRect, EditorGUIUtility.IconContent("Material Icon").image, ScaleMode.ScaleToFit);
				}
				else
				{
					DrawAddIcon(addIconRect, "resource");
				}
			}
		}
		#region 内部函数
		//绘制图标
		private static void DrawAddIcon(Rect rect,string texName)
		{
			GUI.DrawTexture(rect, EditorResourceLibrary.GetTexture2D(texName), ScaleMode.ScaleToFit);
		}

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