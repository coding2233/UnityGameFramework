using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public interface IBindingSet
    {
        void Set(JsonData jsonData);

    }


    public class BindResult: IBindingSet
    {
        private string _key;
        private JsonData _json;
        private  GameObject _target;
        private Action<GameObject, JsonData> _setCallback;
        private Action<GameObject> _getCallback;

        public BindResult(string key,JsonData json,GameObject target)
        {
            _key = key;
            _json = json;
            _target = target;
        }

        public BindResult To(Action<GameObject> getCallback)
        {
            _getCallback = getCallback;
            return this;
        }

        public BindResult OnChanged(Action<GameObject, JsonData> setCallback)
        {
            _setCallback += setCallback;
            return this;
        }

        public void FromObject()
        {
            _getCallback?.Invoke(_target);
        }

        public void Set(JsonData jsonData)
        {
            _setCallback?.Invoke(_target, jsonData);
        }
    }
 

}