using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class RemoveMissingScripts
    {
        [MenuItem("Assets/Remove MissingScripts Recursively")]
        private static void RemoveScripts()
        {
            GameObject[] go = Selection.gameObjects;
            if (go == null)
                return;
            int count = 0;
            foreach (GameObject g in go)
            {
                count += RemoveScriptsFromGameObject(g);
            }
            Debug.Log($"Remove {count} MissingScripts!");
        }

        //从GameObject上移除MissingScripts
        private static int RemoveScriptsFromGameObject(GameObject go)
        {
            int count = 0;
            count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            foreach (Transform item in go.transform)
            {
                count += RemoveScriptsFromGameObject(item.gameObject);
            }
            return count;
        }

    }

}