using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public class InfiniteListViewEditor
    {
        [MenuItem("GameObject/UI/Infinite List View", false, 10)]
        private static void CreateInfiniteListView(MenuCommand menuCommand)
        {
            GameObject infiniteList = null;
            InfiniteListView prefab = AssetDatabase.LoadAssetAtPath<InfiniteListView>("Assets/UnityGameFramework/GameFramework/Editor/UI/InfiniteListView.prefab");
            if (prefab == null)
            {
                infiniteList = new GameObject("InfiniteListView");
                infiniteList.AddComponent<RectTransform>();
                infiniteList.AddComponent<InfiniteListView>();
            }
            else 
            {
                infiniteList = GameObject.Instantiate(prefab.gameObject);
            }
            infiniteList.name = "InfiniteListView";

            GameObject parent = menuCommand.context as GameObject;
            if (parent != null)
            {
                if (parent.GetComponentInParent<Canvas>() == null)
                    parent = null;
            }
            if (parent == null)
            {
                Canvas  findCanvas = GameObject.FindObjectOfType<Canvas>();
                if (findCanvas != null)
                {
                    parent = findCanvas.gameObject;
                }
            }
            if (parent == null)
            {
                parent = new GameObject("Canvas");
                parent.layer = LayerMask.NameToLayer("UI");
                parent.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                parent.AddComponent<CanvasScaler>();
                parent.AddComponent<GraphicRaycaster>();

                EventSystem _es = GameObject.FindObjectOfType<EventSystem>();
                if (!_es)
                {
                    _es = new GameObject("EventSystem").AddComponent<EventSystem>();
                    _es.gameObject.AddComponent<StandaloneInputModule>();
                }
            }

            GameObjectUtility.SetParentAndAlign(infiniteList, parent);
            //×¢²á·µ»ØÊÂ¼þ
            Undo.RegisterCreatedObjectUndo(infiniteList, "Create " + infiniteList.name);
            Selection.activeObject = infiniteList;
        }
    }
}