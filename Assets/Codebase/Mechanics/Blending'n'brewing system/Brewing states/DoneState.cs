namespace Codebase.Mechanics.BlendingNBrewingSystem.BrewingStates
{
    public class DoneState : IState<BlendMemento>
    {
        private readonly BrewingMachine _machine;

        public DoneState(BrewingMachine machine)
        {
            _machine = machine;
        }

        public void Enter()
        {
            UnityEngine.Debug.Log("BrewingMachine: Done – варка завершена");
        }

        public void Exit() { }

        public void Interact(BlendMemento _) { }
    }
}