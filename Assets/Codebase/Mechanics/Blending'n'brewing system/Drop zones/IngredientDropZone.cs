using UnityEngine;
using UnityEngine.EventSystems;
using Codebase.Mechanics.Data;

namespace Codebase.Mechanics.BlendingNBrewingSystem
{
    [RequireComponent(typeof(Collider2D))]
    public class IngredientDropZone : MonoBehaviour
    {
        [SerializeField] private BlendingMachine machine;

        public void DropIngredient(IngredientData data)
        {
            Debug.Log($"{data.IngredientName} was dropped on blending machine.");
            // Передаём ингредиент текущему состоянию машины
            machine.CurrentState?.Interact(data);
        }
    }
}