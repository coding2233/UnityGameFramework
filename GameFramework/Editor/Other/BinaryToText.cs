using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Wanderer.GameFramework
{
	public class BinaryToText:EditorWindow
	{
		private string _content;
		private Vector2 _contentScollPos;
		[MenuItem("Tools/Other/Binary To Text")]
		static void OpenWindow()
		{
			GetWindow<BinaryToText>("Binary To Text");
		}

		private void OnGUI()
		{
			if (GUILayout.Button("Open File"))
			{
				string file = EditorUtility.OpenFilePanel("open file","","*");
				if (!string.IsNullOrEmpty(file))
				{
					_content = File.ReadAllText(file).ToEncrypt();
				}
			}
			_contentScollPos = GUILayout.BeginScrollView(_contentScollPos);
			GUILayout.TextArea(_content);
			GUILayout.EndScrollView();
		}
	}
}
