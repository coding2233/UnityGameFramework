using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LitJson;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public class UIModelView : UIView
    {
        [SerializeField]
        private string _jsonText;
        private JsonData _jsonData;
        public JsonData Json
        {
            get
            {
                if (_jsonData == null)
                {
                    if (string.IsNullOrEmpty(_jsonText))
                    {
                        _jsonData = new JsonData();
                        _jsonData["Active"] = true;
                    }
                    else
                    {
                        _jsonData = JsonMapper.ToObject(_jsonText);
                    }
                }
                return _jsonData;
            }
            set
            {
                _jsonText=value.ToJson();
            }
        }

        private Dictionary<string, IBindingSet> _bindingSets = new Dictionary<string, IBindingSet>();


        private void Awake()
        {
        }

        public void Set(string key, JsonData value)
        {
            if (_bindingSets.TryGetValue(key, out IBindingSet bindingSet))
            {
                bindingSet.Set(value);
                Json[key] = value;
            }
            else
            {
                Json[key] = value;
            }
        }

        public JsonData Get(string key)
        {
            return Json[key];
        }


        private BindResult Bind(string key,GameObject target)
        {
            BindResult buildResult = new BindResult(key,Json[key], target);
            if (!_bindingSets.ContainsKey(key))
            {
                _bindingSets.Add(key, buildResult);
            }
            return buildResult;
        }


        //private ModelViewBind<TValue, TComponent> Bind<TValue, TComponent>(string key,TComponent target, TValue value = default(TValue))
        //{
        //    var mvb = new ModelViewBind<TValue, TComponent>(target);
        //    if (!value.Equals(default(TValue)))
        //    {
        //        mvb.Variable = value;
        //    }
        //    return mvb;
        //}

    }
}