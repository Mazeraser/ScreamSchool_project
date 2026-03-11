using System.Collections.Generic;
using UnityEngine;

namespace Codebase.Mechanics.Data
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Brewing/Recipe Data")]
    public class RecipeData : ScriptableObject
    {
        public string recipeName;
        public List<IngredientData> requiredIngredients;
        [Range(0f, 1f)] public float targetStrength = 0.7f;
        public float strengthTolerance = 0.1f;
        public GameObject finalTeaPrefab;
        public CrystalData crystal;
    }
}