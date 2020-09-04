//fsm 管理
//wanderer
//

using System;
using System.Collections.Generic;

namespace Wanderer.GameFramework
{
    public sealed class FSManager : GameFrameworkModule, IUpdate
    {
        private readonly Dictionary<Type, FSMBase> _fsms = new Dictionary<Type, FSMBase>();
        private List<IUpdate> _updates=new List<IUpdate>();

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
               t= null;
            }
            return t;
        }

        public FSMBase GetFSM(Type type)
        {
            FSMBase fsm;
            if (_fsms.TryGetValue(type, out fsm))
            {
               return fsm;
            }
            return null;
        }
        
        public bool HasFSM<T>()where T : FSM<T>, new()
        {
            return _fsms.ContainsKey(typeof(T));
        }

        public void AddFSM<T>()where T : FSM<T>, new()
        {
            T t = new T();
            _fsms.Add(typeof(T), t);
            _updates.Add(t);
        }

        public void RemoveFSM<T>() where T : FSM<T>, new()
        {
            FSMBase fsmBase;
            if (_fsms.TryGetValue(typeof(T), out fsmBase))
            {
                FSM<T> fsm = fsmBase as FSM<T>;
                _updates.Remove(fsm);
                fsm.OnStop();
                fsmBase = null;
                _fsms.Remove(typeof(T));
            }
        }

        public void OnUpdate()
        {
            for (int i = 0; i < _updates.Count; i++)
            {
                _updates[i].OnUpdate();
            }
        }

        public override void OnClose()
        {
            _updates.Clear();
            foreach (var item in _fsms.Values)
            {
                item.OnStop();
            }
            _fsms.Clear();
        }


    }
}