namespace Codebase.Mechanics.BlendingNBrewingSystem.BrewingStates
{
    public class IdleState : IState<BlendMemento>
    {
        private readonly BrewingMachine _machine;

        public IdleState(BrewingMachine machine)
        {
            _machine = machine;
        }

        public void Enter()
        {
            UnityEngine.Debug.Log("BrewingMachine: Idle");
            _machine.UpdateFlameVisual();
        }

        public void Exit() { }

        public void Interact(BlendMemento interactObject)
        {
            _machine.LoadBlend(interactObject);
        }
    }
}