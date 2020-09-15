using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class ConfigManager : GameFrameworkModule
    {
        private JsonData _config;
        public JsonData Config{get{return _config;}}

        public JsonData SetData(string content)
        {
            _config=null;
            if(!string.IsNullOrEmpty(content))
            {
                _config = JsonMapper.ToObject(content);
            }
            return _config;
        }

        public JsonData this[string key] 
        {
            get
            {
                if(_config!=null)
                {
                    return _config[key];
                }
                return null;
            }
        }

        public override void OnClose()
        {
            _config=null;
        }

    }

}
