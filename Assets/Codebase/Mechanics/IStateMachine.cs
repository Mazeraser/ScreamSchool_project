namespace Codebase.Mechanics
{
    public interface IStateMachine<T>
    {
        IState<T> CurrentState{get;}
        void ChangeState(int stateID);
    }
}