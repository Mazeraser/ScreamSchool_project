using Codebase.Mechanics.Data;
using UnityEngine;

namespace Codebase.Mechanics.GuestSystem
{
    public class Guest
    {
        public int Loyalty { get; set; }
        public MomentData CurrentMoment { get; private set; }
        public RecipeData ServedRecipe;
        public IStateMachine<Guest> StateMachine { get; private set; }

        private const string KEY_PREFIX = "Loyalty_";

        public Guest(MomentData currentMoment)
        {
            StateMachine = new GuestStateMachine(this);
            CurrentMoment = currentMoment;
            Loyalty = GetLoyalty(currentMoment.GuestId);
        }
        public void Interact()
        {
            StateMachine.CurrentState.Interact(this);
        }
        public void SaveLoyalty()
        {
            SetLoyalty(CurrentMoment.GuestId, Loyalty);
        }

        public static void SetLoyalty(string guestId, int value)
        {
            PlayerPrefs.SetInt(KEY_PREFIX + guestId, value);
            PlayerPrefs.Save();
        }
        public static int GetLoyalty(string guestId, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(KEY_PREFIX + guestId, defaultValue);
        }
    }
}
    