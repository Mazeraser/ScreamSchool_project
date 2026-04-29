using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Codebase.Mechanics.Data;
using Codebase.Mechanics.GuestSystem;

namespace Codebase.Mechanics.RecipeBook
{
    public class RecipePopupView : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text emotionText;
        [SerializeField] private TMP_Text taglineText;
        [SerializeField] private TMP_Text ingredientsText;
        [SerializeField] private TMP_Text strengthText;
        [SerializeField] private TMP_Text crystallText;
        [SerializeField] private Image iconImage;
        [SerializeField] private CanvasGroup canvasGroup;

        public void Show(RecipeData recipe, bool isUnlocked, Vector2 position)
        {
            transform.position = position;

            if (!isUnlocked)
            {
                titleText.text =  recipe.recipeName;
                emotionText.text = recipe.Emotion;
                taglineText.text = recipe.Tagline;
                crystallText.text = $"Температура: ???";
                ingredientsText.text = FormatIngredientsHidden(recipe);
                strengthText.text = $"Крепость: ???";
                if (iconImage != null) iconImage.sprite = null;
            }
            else
            {
                titleText.text = recipe.recipeName;
                emotionText.text = recipe.Emotion;
                taglineText.text = recipe.Tagline;
                crystallText.text = $"Температура: {recipe.crystal.crystalName}";
                ingredientsText.text = FormatIngredientsKnown(recipe);
                strengthText.text = $"Крепость: {FormatStrength(recipe.targetStrength)}";
                if (iconImage != null && recipe.Icon != null) iconImage.sprite = recipe.Icon;
            }

            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = false; // чтобы не мешать кликам по книге
        }

        public void Hide()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
        }

        private string FormatIngredientsKnown(RecipeData recipe)
        {
            if (recipe.requiredIngredients == null || recipe.requiredIngredients.Count == 0)
                return "—";

            var grouped = new Dictionary<IngredientData, int>();
            foreach (var ing in recipe.requiredIngredients)
            {
                if (grouped.ContainsKey(ing))
                    grouped[ing]++;
                else
                    grouped[ing] = 1;
            }

            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (var kvp in grouped)
            {
                sb.Append(kvp.Key.IngredientName);
                if (kvp.Value > 1)
                    sb.Append($" ({kvp.Value})");
                if (index < grouped.Count - 1)
                    sb.Append("\n");
                index++;
            }
            return sb.ToString();
        }
        private string FormatIngredientsHidden(RecipeData recipe)
        {
            if (recipe.requiredIngredients == null || recipe.requiredIngredients.Count == 0)
                return "—";

            Dictionary<IngredientCategory, List<IngredientData>> categoryGroups = new Dictionary<IngredientCategory, List<IngredientData>>();
            foreach (var ing in recipe.requiredIngredients)
            {
                if (!categoryGroups.ContainsKey(ing.Category))
                    categoryGroups[ing.Category] = new List<IngredientData>();
                categoryGroups[ing.Category].Add(ing);
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("Чайная база для расы гостя");
            sb.Append("\n");

            if (categoryGroups.ContainsKey(IngredientCategory.Aroma))
            {
                int totalAromaCount = categoryGroups[IngredientCategory.Aroma].Count;
                int uniqueAromaCount = categoryGroups[IngredientCategory.Aroma].Distinct().Count();
                for(int i=0;i<uniqueAromaCount;i++)
                    sb.Append("Арома\n");
                sb.Append(new string(totalAromaCount.ToString()));
                sb.Append("\n");
            }

            if (categoryGroups.ContainsKey(IngredientCategory.Magic))
            {
                sb.Append("Ингредиент магии\n");
            }

            return sb.ToString().TrimEnd('\n');
        }
        private string FormatStrength(float strength)
        {
            if(strength<=0.33)
                return "Низкая";
            else if(strength<=0.66)
                return "Средняя";
            else if(strength<=1)
                return "Высокая";
            else
                return "???";
        }

        private void Awake()
        {
            Hide();
        }
    }
}