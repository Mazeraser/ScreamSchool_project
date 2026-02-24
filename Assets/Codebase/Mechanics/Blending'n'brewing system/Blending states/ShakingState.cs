using Codebase.Mechanics.Data;
using UnityEngine;

namespace Codebase.Mechanics.BlendingNBrewingSystem.BlendingStates
{
    public class ShakingState : IState<IngredientData>
    {
        private readonly BlendingMachine _machine;

        public ShakingState(BlendingMachine machine)
        {
            _machine = machine;
        }

        public void Enter()
        {
            Debug.Log("Enter ShakingState");
            _machine.StartShaking();
        }

        public void Exit()
        {
            Debug.Log("Exit ShakingState");
        }

        public void Interact(IngredientData data)
        {
            Debug.Log("ShakingState: cannot add ingredients while shaking.");
        }
    }
}