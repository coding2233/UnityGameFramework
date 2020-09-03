//fsm context
//wanderer
//

using System;
using System.Collections.Generic;

namespace Wanderer.GameFramework
{
    public abstract class FSM<T> : FSMBase where T : FSM<T>
    {
        protected FSMState<T> _curState;
        protected readonly Dictionary<Type, FSMState<T>> _allState = new Dictionary<Type, FSMState<T>>();
        protected FSMState<T> _startState;
        public T Context { get; private set; }

        public FSM()
        {
            Type[] types = typeof(T).Assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type t = types[i];
                if (t.BaseType != typeof(FSMState<T>) || t.IsAbstract)
                    continue;

                object[] objs = t.GetCustomAttributes(typeof(FSMAttribute), true);
                if (objs != null && objs.Length > 0)
                {
                    FSMAttribute attr = objs[0] as FSMAttribute;
                    if (attr != null)
                    {
                        if (attr.StateType != FSMStateType.Ignore)
                        {
                            FSMState<T> instance = (FSMState<T>)System.Activator.CreateInstance(t);
                            _allState[t] = instance;
                            instance.OnInit(this);
                            if (attr.StateType == FSMStateType.Start)
                                _startState = instance;
                        }
                    }
                }
            }

            //

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

        public virtual void ChangeState<TState>() where TState : FSMState<T>
        {
            _curState?.OnExit(this);

            if (_allState.TryGetValue(typeof(TState), out _curState))
            {
                _curState.OnEnter(this);
            }
        }



    }
}