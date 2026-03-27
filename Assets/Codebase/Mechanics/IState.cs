namespace Codebase.Mechanics
{
    public interface IState<T>
    {
        void Enter();
        void Exit();
        void Interact(T interactObject); //TODO: Можно вынести в отдельный интерфейс
    }
}