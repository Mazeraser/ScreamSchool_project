using UnityEngine;
using Sirenix.OdinInspector;

namespace Codebase.Mechanics.Data
{
    [CreateAssetMenu(fileName = "NewIngredient", menuName = "Brewing/Ingredient Data")]
    public class IngredientData : ScriptableObject
    {
        [FoldoutGroup("Основное", expanded: true)]
        [LabelText("Название"), PropertyOrder(0)]
        public string IngredientName;

        [FoldoutGroup("Основное")]
        [LabelText("Иконка"), PreviewField(50), AssetSelector]
        public Sprite Icon;

        [FoldoutGroup("Основное")]
        [LabelText("Префаб"), PreviewField(50), AssetSelector, Required]
        public GameObject WorldPrefab;

        [FoldoutGroup("Основное")]
        [LabelText("Описание"), MultiLineProperty(3)]
        public string Description;

        [FoldoutGroup("Тип"), PropertyOrder(1)]
        [EnumToggleButtons, LabelText("Категория")]
        public IngredientCategory Category;

        [FoldoutGroup("Параметры заварки")]
        [LabelText("Вклад в смешивание"), Range(0, 2)]
        public float ShakeContribution = 1f;
    }

    public enum IngredientCategory { Base, Aroma, Magic }
}