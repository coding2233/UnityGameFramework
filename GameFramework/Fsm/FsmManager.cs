//fsm 管理
//wanderer
//

using System;
using System.Collections.Generic;

namespace Wanderer.GameFramework
{
    public sealed class FsmManager : GameFrameworkModule
    {
        private readonly Dictionary<Type, FsmBase> _fsms = new Dictionary<Type, FsmBase>();
        public T GetFSM<T>() where T : FsmBase, new()
        {
            FsmBase fsm;
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

        public void RemoveFSM<T>() where T : Fsm<T>, new()
        {
            FsmBase fsmBase;
            if (_fsms.TryGetValue(typeof(T), out fsmBase))
            {
                Fsm<T> fsm = fsmBase as Fsm<T>;
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