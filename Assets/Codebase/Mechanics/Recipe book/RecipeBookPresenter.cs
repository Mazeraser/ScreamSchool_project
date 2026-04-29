using System.Collections.Generic;
using UnityEngine;
using Codebase.Mechanics.Data;

namespace Codebase.Mechanics.RecipeBook
{
    public class RecipeBookPresenter : MonoBehaviour
    {
        [SerializeField] private RecipeBookView view;
        [SerializeField] private RecipePopupView popup;
        [SerializeField] private RectTransform popupOffsetParent; 
        
        private RecipeBookManager manager;

        private void Start()
        {
            manager = RecipeBookManager.Instance;
            if (manager == null)
            {
                Debug.LogError("[RecipeBookPresenter] RecipeBookManager instance not found!");
                return;
            }

            view.Initialize(this);
            manager.OnRecipeUnlocked += OnRecipeUnlockedHandler;
            RefreshList();
            view.Hide();
        }

        private void OnDestroy()
        {
            if (manager != null)
                manager.OnRecipeUnlocked -= OnRecipeUnlockedHandler;
        }

        private void OnRecipeUnlockedHandler(RecipeData recipe)
        {
            view.UpdateEntry(recipe, true);
        }

        private void RefreshList()
        {
            var allRecipes = manager.GetAllRecipesWithStatus();
            var list = new List<(RecipeData, bool)>();
            foreach (var recipe in allRecipes)
            {
                bool unlocked = manager.IsUnlocked(recipe);
                list.Add((recipe, unlocked));
            }
            view.BuildList(list);
        }

        public void ShowPopup(RecipeData recipe, bool isUnlocked, Vector3 entryPosition)
        {
            Vector2 popupPos = entryPosition + new Vector3(0, 50, 0);
            popup.Show(recipe, isUnlocked, popupPos);
        }

        public void HidePopup()
        {
            popup.Hide();
        }

        public void ToggleBook()
        {
            if (view.gameObject.activeSelf)
                view.Hide();
            else
            {
                RefreshList();
                view.Show();
            }
        }
    }
}