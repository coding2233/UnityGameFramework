using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public static class GameObjectExtensions 
    {
        public static T GetOrAddComponet<T>(this GameObject gameObject) where T:Component
        {
            T t = gameObject.GetComponent<T>();
            if (t == null)
            {
                t = gameObject.AddComponent<T>();
            }
            return t;
        }
    }
}