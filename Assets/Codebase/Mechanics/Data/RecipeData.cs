using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Codebase.Mechanics.Data
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Brewing/Recipe Data")]
    public class RecipeData : ScriptableObject
    {
        public string recipeName;
        public List<IngredientData> requiredIngredients;
        public CrystalData crystal;
        [Range(0f, 1f)] public float targetStrength = 0.7f;
        public GameObject finalTeaPrefab;

        [EnumToggleButtons, LabelText("Эмоция от напитка")]
        public GuestEmotions Emotion;
    }
}