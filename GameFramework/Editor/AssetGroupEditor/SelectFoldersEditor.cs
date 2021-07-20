using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class SelectFoldersEditor : EditorWindow
    {
        private static List<string> _selectFolders = new List<string>();
        private static Action<List<string>> _onSelectFolders;
        private string _newFolder="";

        public static void OpenWindow(string[] oldFolders, Action<List<string>> onSelectFolders)
        {
            _selectFolders.Clear();
            if (oldFolders != null)
            {
                _selectFolders.AddRange(oldFolders);
            }
            _onSelectFolders = onSelectFolders;
            GetWindow<SelectFoldersEditor>("Select Folders Editor").ShowAuxWindow();
        }

        private void OnDisable()
        {
            //_onSelectFolders?.Invoke(_selectFolders);
            _selectFolders.Clear();
            _onSelectFolders = null;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save"))
            {
                _onSelectFolders?.Invoke(_selectFolders);
                Close();
            }
            GUILayout.EndHorizontal();


            for (int i = 0; i < _selectFolders.Count; i++)
            {
                GUILayout.BeginHorizontal("box");
                var dirObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_selectFolders[i]);
                var newObject = EditorGUILayout.ObjectField(dirObject, typeof(UnityEngine.Object), false,GUILayout.Width(100));
                if (newObject != dirObject)
                {
                    string newPath = AssetDatabase.GetAssetPath(newObject);
                    if (AssetDatabase.IsValidFolder(newPath))
                    {
                        _selectFolders[i] = newPath;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Tip", "Please select the folder.","OK");
                    }
                }
                GUILayout.Space(10);
                GUILayout.Label(_selectFolders[i]);
                GUILayout.Space(10);
                if (GUILayout.Button("-", GUILayout.Width(80)))
                {
                    if (EditorUtility.DisplayDialog("Warning", "Delete data?","Yes","No"))
                    {
                        _selectFolders.RemoveAt(i);
                        GUIUtility.ExitGUI();
                        break;
                    }
                    
                }
                GUILayout.EndHorizontal();
            }


            GUILayout.BeginHorizontal("box");


            var targetObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_newFolder);
            var newTargetObject = EditorGUILayout.ObjectField(targetObject, typeof(UnityEngine.Object), false, GUILayout.Width(100));
            if (newTargetObject != targetObject)
            {
                string newPath = AssetDatabase.GetAssetPath(newTargetObject);
                if (AssetDatabase.IsValidFolder(newPath))
                {
                    _newFolder = newPath;
                }
                else
                {
                    EditorUtility.DisplayDialog("Tip", "Please select the folder.", "OK");
                }
            }
            GUILayout.Space(10);
            GUILayout.Label(_newFolder);
            GUILayout.Space(10);
            if (GUILayout.Button("+",GUILayout.Width(80)))
            {
                if (!string.IsNullOrEmpty(_newFolder))
                {
                    _selectFolders.Add(_newFolder);
                    _newFolder = "";
                }
            }
            GUILayout.EndHorizontal();

        }
    }

}