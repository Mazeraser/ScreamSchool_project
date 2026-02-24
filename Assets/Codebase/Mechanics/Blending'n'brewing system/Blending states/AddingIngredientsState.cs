using Codebase.Mechanics.Data;
using UnityEngine;

namespace Codebase.Mechanics.BlendingNBrewingSystem.BlendingStates
{
    public class AddingIngredientsState : IState<IngredientData>
    {
        private readonly BlendingMachine _machine;

        public AddingIngredientsState(BlendingMachine machine)
        {
            _machine = machine;
        }

        public void Enter()
        {
            Debug.Log("Enter AddingIngredientsState");
        }

        public void Exit()
        {
            Debug.Log("Exit AddingIngredientsState");
        }

        public void Interact(IngredientData data)
        {
            if (data.Category == IngredientCategory.Base)
            {
                Debug.Log("AddingIngredientsState: base already added, cannot add another base.");
                return;
            }
            
            _machine.AddIngredient(data);
        }
    }
}