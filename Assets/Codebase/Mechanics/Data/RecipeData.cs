using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Codebase.Mechanics.Data
{
    public enum EmotionCategory
    {
        Joy,      // Радость
        Sadness,  // Грусть
        Anger,    // Злость
        Fear      // Страх
    }

    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Brewing/Recipe Data")]
    public class RecipeData : ScriptableObject
    {
        public string recipeName;
        public int recipeID;
        public List<IngredientData> requiredIngredients;
        public CrystalData crystal;
        [Range(0f, 1f)] public float targetStrength = 0.7f;
        public GameObject finalTeaPrefab;
        public Sprite Icon;

        [LabelText("Эмоция от напитка")]
        public string Emotion;
        [LabelText("Тэглайн")]
        public string Tagline;
        [EnumToggleButtons, LabelText("Emotion")]
        public EmotionCategory emotionCategory;
    }
}