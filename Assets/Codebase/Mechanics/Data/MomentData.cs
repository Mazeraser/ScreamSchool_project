using UnityEngine;
using Sirenix.OdinInspector;

namespace Codebase.Mechanics.Data
{
	public enum GuestEmotions{
        Happyness=0, 
        Sad=1, 
        Fear=2, 
        Angry=3
    }
	[CreateAssetMenu(fileName="NewMoment", menuName="Guests/Moment Data")]
	public class MomentData : ScriptableObject
	{
		[LabelText("Эмоция")]
		public GuestEmotions Emotion;
		[LabelText("Пул необходимых ингредиентов в рецепте")]
		public IngredientData[] FavoriteIngredients;
		[LabelText("Необходимый кристал")]
		public CrystalData Сrystal;
		[LabelText("Стартовый диалоговый текст")]
		public TextAsset Dialogue;
		[LabelText("Оценка напитка(хорошо)")]
		public TextAsset GoodEvaluationDialogue;
		[LabelText("Оценка напитка(средне)")]
		public TextAsset MidEvaluationDialogue;
		[LabelText("Оценка напитка(плохо)")]
		public TextAsset BadEvaluationDialogue;
		[LabelText("Префаб")]
		public GameObject Prefab;
		[LabelText("Айди клиента")]
		public string guestId;
	}
}