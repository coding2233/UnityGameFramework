using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
using LitJson;

namespace Wanderer.GameFramework
{
    [CustomEditor(typeof(UIModelView))]
    public class UIModeleEditor : Editor
    {
        private UIModelView _uiModel;

        private LitJsonEditor _litJsonEditor;

        public void OnEnable()
        {
            _uiModel = target as UIModelView;
            _litJsonEditor = new LitJsonEditor("UI-Model", _uiModel.Json);
        }

        public override void OnInspectorGUI()
        {
            if (_uiModel != null)
            {
                if (_litJsonEditor.OnDraw())
                {
                    _uiModel.Json = _litJsonEditor.GetJson;
                    EditorUtility.SetDirty(_uiModel);
                }
            }
            else
            {
                base.OnInspectorGUI();
            }
        }

      
    }
}
