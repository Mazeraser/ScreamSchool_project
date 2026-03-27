using Codebase.Mechanics.Data;

namespace Codebase.Mechanics.GuestSystem
{
    public class Guest
    {
        public int Loyalty { get; set; }
        public MomentData CurrentMoment { get; private set; }
        public RecipeData ServedRecipe;
        public IStateMachine<Guest> StateMachine { get; private set; }

        public Guest(MomentData currentMoment)
        {
            StateMachine = new GuestStateMachine(this);
            CurrentMoment = currentMoment;
            Loyalty = LoyaltyManager.GetLoyalty(currentMoment.guestId);
        }
        public void Interact()
        {
            StateMachine.CurrentState.Interact(this);
        }
        public void SaveLoyalty()
        {
            LoyaltyManager.SetLoyalty(CurrentMoment.guestId, Loyalty);
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
    