using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Wanderer.GameFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomModuleEditor : Attribute
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        /// <value></value>
        public string Name { get; private set; }
        /// <summary>
        /// 模块颜色
        /// </summary>
        /// <value></value>
        public Color Color { get; private set; }
        public CustomModuleEditor(string name)
        {
            Name = name;
            Color = RandomColor();
        }
        public CustomModuleEditor(string name, float x, float y, float z)
        {
            Name = name;
            Color = new Color(x, y, z);
        }

        private Color RandomColor()
        {
            float x = UnityEngine.Random.Range(0, 1.0f);
            float y = 1 - x;
            float z = UnityEngine.Random.Range(0, 1.0f);
            List<float> values = new List<float>();
            values.Add(x);
            values.Add(y);
            values.Add(z);
            while (values.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, values.Count);
                x = values[index];
                values.RemoveAt(index);
            }
            return new Color(x, y, z);
        }
    }
}
