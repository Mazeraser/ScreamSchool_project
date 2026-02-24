namespace Codebase.Mechanics.BlendingNBrewingSystem.BrewingStates
{
    public class BrewingState : IState<BlendMemento>
    {
        private readonly BrewingMachine _machine;

        public BrewingState(BrewingMachine machine)
        {
            _machine = machine;
        }

        public void Enter()
        {
            UnityEngine.Debug.Log("BrewingMachine: Brewing – началась варка");
            _machine.UpdateFlameVisual();
        }

        public void Exit()
        {
            // При выходе гасим пламя, если оно ещё горит
            _machine.isFlameLit = false;
            _machine.UpdateFlameVisual();
        }

        public void Interact(BlendMemento _) { }
    }
}