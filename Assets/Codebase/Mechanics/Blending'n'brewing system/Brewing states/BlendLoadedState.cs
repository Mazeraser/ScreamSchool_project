namespace Codebase.Mechanics.BlendingNBrewingSystem.BrewingStates
{
    public class BlendLoadedState : IState<BlendMemento>
    {
        private readonly BrewingMachine _machine;

        public BlendLoadedState(BrewingMachine machine)
        {
            _machine = machine;
        }

        public void Enter()
        {
            UnityEngine.Debug.Log("BrewingMachine: BlendLoaded – купаж загружен");
        }

        public void Exit() { }

        public void Interact(BlendMemento _) { }
    }
}