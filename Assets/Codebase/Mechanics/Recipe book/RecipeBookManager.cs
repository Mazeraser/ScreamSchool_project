using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Codebase.Mechanics.Data;
using Codebase.Mechanics.BlendingNBrewingSystem;

namespace Codebase.Mechanics.RecipeBook
{
    public class RecipeBookManager : MonoBehaviour
    {
        public static RecipeBookManager Instance { get; private set; }

        [SerializeField, Required] private RecipeLibrary recipeLibrary;
        [SerializeField] private BrewingMachine brewingMachine;

        private HashSet<RecipeData> unlockedRecipes = new HashSet<RecipeData>();

        public System.Action<RecipeData> OnRecipeUnlocked;

        private const string SAVE_KEY = "UnlockedRecipes";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            LoadProgress();

            if (brewingMachine == null)
                Debug.LogWarning("[RecipeBookManager] Don't see brewing machine");
            brewingMachine.OnBrewingSuccess += OnBrewingSuccessHandler;
        }

        private void OnDestroy()
        {
            if (brewingMachine != null)
                brewingMachine.OnBrewingSuccess -= OnBrewingSuccessHandler;
        }

        private void OnBrewingSuccessHandler(RecipeData brewedRecipe)
        {
            UnlockRecipe(brewedRecipe);
        }

        public bool IsUnlocked(RecipeData recipe)
        {
            return recipe != null && unlockedRecipes.Contains(recipe);
        }

        public void UnlockRecipe(RecipeData recipe)
        {
            if (recipe == null) return;
            if (unlockedRecipes.Add(recipe))
            {
                Debug.Log($"[RecipeBookManager] Рецепт '{recipe.recipeName}' разблокирован!");
                SaveProgress();
                OnRecipeUnlocked?.Invoke(recipe);
            }
        }

        public IReadOnlyList<RecipeData> GetAllRecipesWithStatus()
        {
            return recipeLibrary.AllRecipes.ToList();
        }

        public bool TryGetRecipeByID(int ID, out RecipeData recipe)
        {
            recipe = recipeLibrary.GetRecipeByID(ID);
            return recipe != null;
        }

        private void SaveProgress()
        {
            List<int> unlockedIDs = unlockedRecipes.Select(r => r.recipeID).ToList();
            string json = JsonUtility.ToJson(new SerializableRecipeList { recipeIDs = unlockedIDs });
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        private void LoadProgress()
        {
            if (!PlayerPrefs.HasKey(SAVE_KEY)) return;

            string json = PlayerPrefs.GetString(SAVE_KEY);
            var wrapper = JsonUtility.FromJson<SerializableRecipeList>(json);
            if (wrapper?.recipeIDs == null) return;

            unlockedRecipes.Clear();
            foreach (int id in wrapper.recipeIDs)
            {
                var recipe = recipeLibrary.GetRecipeByID(id);
                if (recipe != null)
                    unlockedRecipes.Add(recipe);
            }
        }

        [Button]
        private void ClearPlayerprefs(){
            PlayerPrefs.DeleteAll();
        }

        [System.Serializable]
        private class SerializableRecipeList
        {
            public List<int> recipeIDs = new List<int>();
        }
    }
}