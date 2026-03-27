namespace Codebase.Mechanics.GuestSystem.GuestStates
{
    public class WaitingForOrderState : GuestStateBase
    {
        public WaitingForOrderState(Guest guest) : base(guest, GuestStateID.WaitingForOrder) { }

        public override void Interact(Guest guest)
        {
            guest.StateMachine.ChangeState((int)GuestStateID.Evaluation);
        }
    }
}