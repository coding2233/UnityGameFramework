//fsm context
//wanderer
//

using System;
using System.Collections.Generic;

namespace Wanderer.GameFramework
{
    public abstract class Fsm<T> : FsmBase where T : Fsm<T>
    {
        protected FsmState<T> _curState;
        protected readonly Dictionary<Type, FsmState<T>> _allState = new Dictionary<Type, FsmState<T>>();
        protected FsmState<T> _startState;
        public T Context { get; private set; }

        public Fsm()
        {
            Type[] types = typeof(T).Assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type t = types[i];
                if (t.BaseType != typeof(FsmState<T>))
                    continue;

                object[] objs = t.GetCustomAttributes(typeof(FsmAttribute), true);
                if (objs != null && objs.Length > 0)
                {
                    FsmAttribute attr = objs[0] as FsmAttribute;
                    if (attr != null)
                    {
                        if (attr.StateType != FsmStateType.Ignore)
                        {
                            FsmState<T> instance = (FsmState<T>)System.Activator.CreateInstance(t);
                            _allState[t] = instance;
                            if (attr.StateType == FsmStateType.Start)
                                _startState = instance;
                        }
                    }
                }
            }

            //context
            Context = (T)this;
        }


        public override void OnBegin()
        {
            _curState?.OnExit(this);
            _curState = _startState;
            _curState?.OnEnter(this);
        }

        public override void OnUpdate()
        {
            _curState?.OnUpdate(this);
        }

        public override void OnStop()
        {
            _curState?.OnExit(this);
            _curState = null;
        }

        public virtual void ChangeState<TState>() where TState : FsmState<T>
        {
            _curState?.OnExit(this);

            if (_allState.TryGetValue(typeof(TState), out _curState))
            {
                _curState.OnEnter(this);
            }
        }
    }
}