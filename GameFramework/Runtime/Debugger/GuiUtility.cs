using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class GuiUtility
    {
        public static void DrawItem(string key, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(key, GUILayout.Width(200));
            GUILayout.Label(value);
            GUILayout.EndVertical();
        }


    }
}
