//fsm 状态
//wanderer
//

namespace Wanderer.GameFramework
{
    public abstract class FsmState<T> where T : Fsm<T>
    {
        internal virtual void OnInit(Fsm<T> fsm)
        { }

        internal virtual void OnEnter(Fsm<T> fsm)
        { }

        internal virtual void OnExit(Fsm<T> fsm)
        { }

        internal virtual void OnUpdate(Fsm<T> fsm)
        { }

        protected void ChangeState<TState>(Fsm<T> fsm) where TState : FsmState<T>
        {
            fsm?.ChangeState<TState>();
        }
    }
}