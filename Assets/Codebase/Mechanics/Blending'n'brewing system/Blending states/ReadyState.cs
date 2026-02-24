using Codebase.Mechanics.Data;
using UnityEngine;

namespace Codebase.Mechanics.BlendingNBrewingSystem.BlendingStates
{
    public class ReadyState : IState<IngredientData>
    {
        private readonly BlendingMachine _machine;

        public ReadyState(BlendingMachine machine)
        {
            _machine = machine;
        }

        public void Enter()
        {
            Debug.Log("Enter ReadyState");
        }

        public void Exit()
        {
            Debug.Log("Exit ReadyState");
        }

        public void Interact(IngredientData data)
        {
            Debug.Log("ReadyState: blend is ready, take it.");
            //TODO: Должен менять состояние при запуске заварки
        }
    }
}