using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Codebase.Mechanics.Data;

namespace Codebase.Mechanics.RecipeBook
{
    public class RecipeBookView : MonoBehaviour
    {
        [SerializeField] private RectTransform entriesContainer;
        [SerializeField] private RecipeBookEntryView entryPrefab;
        [SerializeField] private RecipePopupView popupPrefab;
        [SerializeField] private GameObject sectionHeaderPrefab;

        private RecipeBookPresenter presenter;
        private List<RecipeBookEntryView> entries = new List<RecipeBookEntryView>();
        private Dictionary<RecipeData, RecipeBookEntryView> entryMap = new Dictionary<RecipeData, RecipeBookEntryView>();

        private readonly EmotionCategory[] categoryOrder = new EmotionCategory[]
        {
            EmotionCategory.Joy,
            EmotionCategory.Sadness,
            EmotionCategory.Anger,
            EmotionCategory.Fear
        };

        public void Initialize(RecipeBookPresenter presenter)
        {
            this.presenter = presenter;
        }

        public void BuildList(IEnumerable<(RecipeData recipe, bool unlocked)> recipesWithStatus)
        {
            ClearEntries();

            var grouped = recipesWithStatus
                .GroupBy(item => item.recipe.emotionCategory)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            foreach (var category in categoryOrder)
            {
                if (!grouped.ContainsKey(category) || grouped[category].Count == 0)
                    continue;
                
                var entriesParent = CreateSectionHeader(category);
                
                foreach (var (recipe, unlocked) in grouped[category])
                {
                    var entry = Instantiate(entryPrefab, entriesParent.transform);
                    entry.Initialize(recipe, unlocked, presenter);
                    entries.Add(entry);
                    entryMap[recipe] = entry;
                }
            }
        }

        private GameObject CreateSectionHeader(EmotionCategory category)
        {
            if (sectionHeaderPrefab != null)
            {
                var headerObj = Instantiate(sectionHeaderPrefab, entriesContainer);
                var headerText = headerObj.GetComponent<TMP_Text>();
                if (headerText != null)
                    headerText.text = GetCategoryDisplayName(category);
                return headerObj;
            }
            else
            {
                var go = new GameObject("Header_" + category.ToString());
                go.transform.SetParent(entriesContainer, false);
                var text = go.AddComponent<TMP_Text>();
                text.text = GetCategoryDisplayName(category);
                text.fontSize = 24;
                text.fontStyle = TMPro.FontStyles.Bold;

                var rect = go.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(400, 40);

                return go;
            }
        }

        private string GetCategoryDisplayName(EmotionCategory category)
        {
            switch (category)
            {
                case EmotionCategory.Joy: return "Радость";
                case EmotionCategory.Sadness: return "Грусть";
                case EmotionCategory.Anger: return "Злость";
                case EmotionCategory.Fear: return "Страх";
                default: return category.ToString();
            }
        }

        public void UpdateEntry(RecipeData recipe, bool unlocked)
        {
            if (entryMap.TryGetValue(recipe, out var entry))
                entry.RefreshUnlockStatus(unlocked);
        }

        private void ClearEntries()
        {
            foreach (var entry in entries)
                Destroy(entry.gameObject);
            entries.Clear();
            entryMap.Clear();
            
            foreach (Transform child in entriesContainer)
                Destroy(child.gameObject);
        }
        
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}