using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class LoadDataTableEventArgs :GameEventArgs<LoadDataTableEventArgs>
    {
        private bool _success;
        public bool Success{
            get
            {
                
                return _success;
            }
        }

        private IDataTable _data;
        public IDataTable Data{
            get{
                return _data;
            }
        }

        private string _message;
        public string Message{get{
            return _message;
        }}


        public LoadDataTableEventArgs Set(bool success,string message,IDataTable data)
        {
            _success=success;
            _message = message;
            _data=data;
            return this;
        }

    }
}

