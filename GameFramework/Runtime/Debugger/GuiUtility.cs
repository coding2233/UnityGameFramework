using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class GuiUtility
    {
        private static StringBuilder _drawTextBuilder = new StringBuilder();
        private static bool _record=false;
        /// <summary>
        /// 记录显示的文本
        /// </summary>
        public static void RecordTextStart()
        {
            _drawTextBuilder.Clear();
            _record = true;
        }

        /// <summary>
        /// 停止记录
        /// </summary>
        /// <returns></returns>
        public static string RecordTextStop()
        {
            _record = false;
            return _drawTextBuilder.ToString();
        }

        public static void DrawItem(string key, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(key, GUILayout.Width(200));
            GUILayout.Label(value);
            GUILayout.EndVertical();
            //记录操作
            if (_record)
            {
                _drawTextBuilder.AppendLine($"{key}:{value}");
            }
        }


    }
}
