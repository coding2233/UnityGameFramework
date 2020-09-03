//fsm 管理
//wanderer
//

using System;
using System.Collections.Generic;

namespace Wanderer.GameFramework
{
    public sealed class FSMannager : GameFrameworkModule, IUpdate
    {
        private readonly Dictionary<Type, FSMBase> _fsms = new Dictionary<Type, FSMBase>();

        public T GetFSM<T>() where T : FSMBase, new()
        {
            FSMBase fsm;
            T t;
            if (_fsms.TryGetValue(typeof(T), out fsm))
            {
                t = fsm as T;
            }
            else
            {
                t = new T();
                _fsms.Add(typeof(T), t);
            }
            return t;
        }

        public void RemoveFSM<T>() where T : FSM<T>, new()
        {
            FSMBase fsmBase;
            if (_fsms.TryGetValue(typeof(T), out fsmBase))
            {
                FSM<T> fsm = fsmBase as FSM<T>;
                fsm.OnStop();
                fsmBase = null;
                _fsms.Remove(typeof(T));
            }
        }

        public void OnUpdate()
        {
            foreach (var item in _fsms.Values)
            {
                item.OnUpdate();
            }
        }

        public override void OnClose()
        {
            foreach (var item in _fsms.Values)
            {
                item.OnStop();
            }
            _fsms.Clear();
        }


    }
}