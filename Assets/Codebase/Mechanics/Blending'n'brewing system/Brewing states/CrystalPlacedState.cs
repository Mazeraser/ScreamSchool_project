namespace Codebase.Mechanics.BlendingNBrewingSystem.BrewingStates
{
    public class CrystalPlacedState : IState<BlendMemento>
    {
        private readonly BrewingMachine _machine;

        public CrystalPlacedState(BrewingMachine machine)
        {
            _machine = machine;
        }

        public void Enter()
        {
            UnityEngine.Debug.Log("BrewingMachine: CrystalPlaced – кристалл установлен");
            _machine.UpdateFlameVisual();
        }

        public void Exit() { }

        public void Interact(BlendMemento _) { }
    }
}