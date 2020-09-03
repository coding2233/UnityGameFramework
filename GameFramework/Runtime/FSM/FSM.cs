//fsm context
//wanderer
//

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Wanderer.GameFramework
{
    public abstract class FSM<T> : FSMBase where T : FSM<T>
    {
        protected FSMState<T> _curState;
        public FSMState<T> CurrentState
        {
            get
            {
                return _curState;
            }
        }
        protected readonly Dictionary<Type, FSMState<T>> _allState = new Dictionary<Type, FSMState<T>>();
        protected FSMState<T> _startState;
        //是否有覆盖当前的开始状态
        protected bool _hasOverStartState=false;
        public T Context { get; private set; }

        public FSM()
        {
            List<Type> types=new List<Type>();
            //获取所有程序的类型
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                types.AddRange(assemblies[i].GetTypes());
            }  
            //整理类型是否满足状态
            for (int i = 0; i < types.Count; i++)
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
                            {
                                if(!_hasOverStartState)
                                    _startState = instance;
                            }
                            else if(attr.StateType == FSMStateType.OverStart)
                            {
                                _startState = instance;
                                _hasOverStartState=true;
                            }
                                
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
            if(_startState==null)
            {
                throw new GameException($"[{typeof(T).FullName}] FSM can't find [StartState] !!");
            }
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

        public override void OnClose()
        {
            _allState.Clear();
            _hasOverStartState=false;
            Context=null;
            _startState=null;
            _curState=null;
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