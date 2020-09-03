//fsm 状态
//wanderer
//

namespace Wanderer.GameFramework
{
    [FSM()]
    public abstract class FSMState<T> where T : FSM<T>
    {
        public virtual void OnInit(FSM<T> fsm)
        { }

        public virtual void OnEnter(FSM<T> fsm)
        { }

        public virtual void OnExit(FSM<T> fsm)
        { }

        public virtual void OnUpdate(FSM<T> fsm)
        { }

        protected void ChangeState<TState>(FSM<T> fsm) where TState : FSMState<T>
        {
            fsm?.ChangeState<TState>();
        }
    }
}