using Codebase.Mechanics.Data;
using UnityEngine;

namespace Codebase.Mechanics.BlendingNBrewingSystem.BlendingStates
{
    public class IdleState : IState<IngredientData>
    {
        private readonly BlendingMachine _machine;

        public IdleState(BlendingMachine machine)
        {
            _machine = machine;
        }

        public void Enter()
        {
            Debug.Log("Enter IdleState");
        }

        public void Exit()
        {
            Debug.Log("Exit IdleState");
        }

        public void Interact(IngredientData data)
        {
            if (data.Category == IngredientCategory.Base)
            {
                _machine.AddIngredient(data);
                _machine.ChangeState((int)BlendingMachine.BlendingStates.AddingIngredients);
            }
            else
            {
                Debug.Log("IdleState: can only add base ingredient.");
            }
        }
    }
}