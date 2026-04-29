using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Codebase.Mechanics.Data;

namespace Codebase.Mechanics.RecipeBook
{
    public class RecipeBookEntryView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private GameObject lockOverlay;

        private RecipeData recipe;
        private bool isUnlocked;
        private RecipeBookPresenter presenter;

        public void Initialize(RecipeData recipe, bool unlocked, RecipeBookPresenter presenter)
        {
            this.recipe = recipe;
            this.isUnlocked = unlocked;
            this.presenter = presenter;

            if (iconImage != null && recipe.Icon != null) iconImage.sprite = recipe.Icon;
            if (nameText != null) nameText.text = recipe.recipeName;
        }

        public void RefreshUnlockStatus(bool unlocked)
        {
            isUnlocked = unlocked;
            if (unlocked)
            {
                if (nameText != null) nameText.text = recipe.recipeName;
                if (lockOverlay != null) lockOverlay.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            presenter.ShowPopup(recipe, isUnlocked, transform.position);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            presenter.HidePopup();
        }
    }
}