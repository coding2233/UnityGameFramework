using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public class UIModel<T> 
    {
        private T _variable;
        private Action<T> _onVariableChanged;

        public T Variable
        {
            get
            {
                return _variable;
            }
            set
            {
                _variable = value;
                _onVariableChanged?.Invoke(_variable);
            }
        }

        public UIModel<T> OnVariableChanged(Action<T> onVariableChanged)
        {
            _onVariableChanged = onVariableChanged;
            return this;
        }
    }

    public class ModelViewBind<T1,T2>
    {
        public UIModel<T1> Model { get; private set; }
        public T2 Target { get; private set; }

        private Func<UIModel<T1>, T2, T1> _getCallback;
        private Action<T1, T2> _setCallback;
        private bool _oneWayBinding;

        public T1 Variable 
        { 
            get 
            {
                T1 t1 = _getCallback(Model, Target);
                if (!t1.Equals(Model.Variable))
                {
                    Model.Variable = t1;
                }
                return t1;
            }
            set
            {
                if (!value.Equals(Model.Variable))
                {
                    Model.Variable = value;
                }
            }
        }

      


        public ModelViewBind(T2 target)
        {
            Model = new UIModel<T1>();

            Model.OnVariableChanged((t1)=> {
                if (!_oneWayBinding)
                {
                    _setCallback?.Invoke(Model.Variable, Target);
                }
            });

            Target = target;
        }

        public ModelViewBind<T1,T2> Get(Func<UIModel<T1>, T2, T1> callback)
        {
            _getCallback = callback;
            return this;
        }

        public ModelViewBind<T1, T2> Set(Action<T1,T2> callback)
        {
            _setCallback = callback;
            return this;
        }


        public ModelViewBind<T1, T2> OneWay()
        {
            _oneWayBinding = true;
            return this;
        }

    }


    public static class ModelViewBinding
    {
        public static ModelViewBind<TValue, TComponent> Bind<TValue, TComponent>(TComponent target, TValue value = default(TValue))
        {
            var mvb = new ModelViewBind<TValue, TComponent>(target);
            if (!value.Equals(default(TValue)))
            {
                mvb.Variable = value;
            }
            return mvb;
        }
    }

}


