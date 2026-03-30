using UnityEngine;
using System.Linq;
using Codebase.Mechanics.Data;

namespace Codebase.Mechanics.GuestSystem.GuestStates
{
    public class EvaluationState : GuestStateBase
    {
        public EvaluationState(Guest guest) : base(guest, GuestStateID.Evaluation) { }
        public const float STRENGTH_TOLERANCY=0.15F;

        private bool IsStrengthCorrect(RecipeData served, MomentData moment)
        {
            return served.targetStrength-STRENGTH_TOLERANCY<=moment.NeedTarget&&moment.NeedTarget<=served.targetStrength+STRENGTH_TOLERANCY;
        }

        public override void Enter()
        {
            int emotionsMatched = _guest.CurrentMoment.Emotion==_guest.ServedRecipe.Emotion ? 1:0;
            int ingredientsMatched = _guest.ServedRecipe.requiredIngredients.All(s => _guest.CurrentMoment.FavoriteIngredients.Contains(s)) ? 1:0;
            int crystalMatch = _guest.ServedRecipe.crystal == _guest.CurrentMoment.Сrystal ? 1:0;
            int strengthMatch = IsStrengthCorrect(_guest.ServedRecipe, _guest.CurrentMoment) ? 1:0;

            int rating = emotionsMatched+ingredientsMatched+crystalMatch+strengthMatch;
            TextAsset evaluationText = null;

            
            if (rating>=3)
            {
                evaluationText = _guest.CurrentMoment.GoodEvaluationDialogue;
                _guest.Loyalty += 1;
            }
            else if (rating>=2&&rating<=3)
            {
                evaluationText = _guest.CurrentMoment.MidEvaluationDialogue;
                _guest.Loyalty += 0;
            }
            else
            {
                rating = 0;
                evaluationText = _guest.CurrentMoment.BadEvaluationDialogue;
                _guest.Loyalty -= -1;
            }
        }
        public override void Interact(Guest guest)
        {
            guest.StateMachine.ChangeState((int)GuestStateID.WaitingForDialogue);
        }
    }
}