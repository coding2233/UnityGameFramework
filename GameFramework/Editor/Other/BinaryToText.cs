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
			if (GUILayout.Button("Save As"))
			{
				string file = EditorUtility.OpenFilePanel("open file", "", "*");
				if (!string.IsNullOrEmpty(file))
				{
					string saveFile = EditorUtility.SaveFilePanel("save file","","","");
					if (!string.IsNullOrEmpty(saveFile))
					{
						int bufferCount = 1024 * 4;
						using (EncryptFileStream fileStream = new EncryptFileStream(file, FileMode.Open, FileAccess.Read, FileShare.None, bufferCount, false))
						{
							using (FileStream writeStream = new FileStream(saveFile, FileMode.OpenOrCreate,FileAccess.Write,FileShare.None, bufferCount))
							{
								byte[] buffer = new byte[bufferCount];
								int count;
								do
								{
									count = fileStream.Read(buffer, 0, buffer.Length);
									writeStream.Write(buffer, 0, count);
								} 
								while (count>0);
							}
						}
					}
				}
			}
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
