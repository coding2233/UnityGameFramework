using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class AssetGroupEditor:EditorWindow
    {
        private const string _configName = "AssetGroupEditor.json";
        private JsonData _config;
        private JsonData _newData;
        private JsonData _selectFoldersJsonData;
        private Vector2 _scrollView = Vector2.zero;


    }

}