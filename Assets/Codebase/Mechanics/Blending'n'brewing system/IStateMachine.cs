namespace Codebase.Mechanics.BlendingNBrewingSystem
{
    public interface IStateMachine<T>
    {
        IState<T> CurrentState{get;}
        void ChangeState(int stateID);
    }
}