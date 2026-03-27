using UnityEngine;
using System.Linq;

namespace Codebase.Mechanics.GuestSystem.GuestStates
{
    public class EvaluationState : GuestStateBase
    {
        public EvaluationState(Guest guest) : base(guest, GuestStateID.Evaluation) { }

        private bool IsStrengthCorrect(RecipeData served, MomentData moment)
        {
            // Если в MomentData нет целевой крепости, используем целевые параметры рецепта
            // Поскольку крепость хранится в RecipeData, считаем, что гость ожидает именно этот рецепт
            // Можно добавить в MomentData поле expectedStrength или использовать served.targetStrength как ожидание
            // Пока допустим, что если рецепт совпадает, то и крепость подходит (она уже проверена в BrewingMachine)
            return true;
        }

        public override void Enter()
        {
            bool emotionsMatched = _guest.CurrentMoment.Emotion==_guest.ServedRecipe.Emotion;
            //TODO: Реализовать сравнение ингредиентов с итоговым напитком
            bool ingredientsMatched = _guest.ServedRecipe.requiredIngredients.All(s => _guest.CurrentMoment.FavoriteIngredients.Contains(s) );
            bool crystalMatch = served.crystal == moment.Сrystal;
            bool strengthMatch = IsStrengthCorrect(served, moment);

            int rating = emotionsMatched+ingredientsMatched+crystalMatch+strengthMatch;
            TextAsset evaluationText = null;

            
            if (rating>=3)
            {
                evaluationText = moment.GoodEvaluationDialogue;
                _guest.Loyalty += 1;
            }
            else if (rating>=2&&rating<=3)
            {
                evaluationText = moment.MidEvaluationDialogue;
                _guest.Loyalty += -;
            }
            else
            {
                rating = 0;
                evaluationText = moment.BadEvaluationDialogue;
                _guest.Loyalty -= -1;
            }
        }
        public override void Interact(Guest guest)
        {
            guest.StateMachine.ChangeState((int)GuestStateID.WaitingForDialogue);
        }
    }
}