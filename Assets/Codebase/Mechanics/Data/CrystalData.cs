using UnityEngine;

namespace Codebase.Mechanics.Data
{
    [CreateAssetMenu(fileName = "NewCrystal", menuName = "Brewing/Crystal Data")]
    public class CrystalData : ScriptableObject
    {
        [Header("Основные настройки")]
        public string crystalName;
        public GameObject worldPrefab;
        public Sprite icon;

        [Header("Взаимодействие")]
        [Range(1, 10)] public int clicksToIgnite = 3;
        [Tooltip("Множитель скорости варки (1 = стандарт)")]
        public float brewRateMultiplier = 1f;
    }
}