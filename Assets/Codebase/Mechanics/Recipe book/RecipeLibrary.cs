using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Codebase.Mechanics.Data;

namespace Codebase.Mechanics.RecipeBook
{
    [CreateAssetMenu(fileName = "RecipeLibrary", menuName = "Brewing/Recipe Library")]
    public class RecipeLibrary : ScriptableObject
    {
        [SerializeField, Required, ListDrawerSettings(ShowFoldout = true)]
        private List<RecipeData> allRecipes = new List<RecipeData>();

        public IReadOnlyList<RecipeData> AllRecipes => allRecipes;

        public RecipeData GetRecipeByID(int id)
        {
            return allRecipes.Find(r => r.recipeID == id); // Используем InstanceID, но лучше добавить собственное поле recipeID
        }
    }
}