using UnityEngine;
using UnityEngine.UI;
using Codebase.Mechanics.Data;
using Codebase.Mechanics.GuestSystem;
using System.Collections.Generic;

namespace Codebase.Mechanics.BlendingNBrewingSystem
{
    public class BrewedTea : MonoBehaviour
    {
        private RecipeData _recipeData;
        private Transform _guestServePoint;

        public void Initialize(RecipeData data, Transform guestServePoint)
        {
            _recipeData = data;
            _guestServePoint = guestServePoint;
        }

        public void ServeToGuest()
        {
            if (_recipeData == null)
            {
                Debug.LogWarning("[BrewedTea] Нельзя подать напиток: данные рецепта отсутсвуют.", this);
                return;
            }

            var facade = FindObjectOfType<GuestFacade>(); //TODO: Переписать.
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

        private void OnMouseDown(){
            if(_guestServePoint!=null && transform.position!=_guestServePoint.position)
                transform.position = _guestServePoint.position;
            else
                ServeToGuest();
        }
    }
}