using UnityEngine;
using UnityEngine.UI;
using Codebase.Mechanics.Data;
using Codebase.Mechanics.GuestSystem;
using System.Collections.Generic;

namespace Codebase.Mechanics.BlendingNBrewingSystem
{
    public class BrewedTea : MonoBehaviour
    {
        [SerializeField] private RecipeData _recipeData;

        public void Initialize(RecipeData data)
        {
            _recipeData = data;
        }

        public void ServeToGuest()
        {
            if (_recipeData == null)
            {
                Debug.LogWarning("[BrewedTea] Нельзя подать напиток: данные рецепта отсутсвуют.", this);
                return;
            }

            var facade = FindObjectOfType<GuestFacade>();
            if (facade != null)
            {
                facade.StartEvaluation(_recipeData);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("[BrewedTea] GuestFacade не найден на сцене. Не удалось оценить напиток.", this);
            }
        }

        private void OnMouseDrag(){
            Vector3 currentMouse = Input.mousePosition;
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(currentMouse.x,currentMouse.y,1));
        }
    }
}