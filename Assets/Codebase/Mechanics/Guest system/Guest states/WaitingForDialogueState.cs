namespace Codebase.Mechanics.GuestSystem.GuestStates
{
    public class WaitingForDialogueState : GuestStateBase
{
    public WaitingForDialogueState(Guest guest) : base(guest, GuestStateID.WaitingForDialogue) { }

    public override void Interact(Guest guest)
    {
        guest.StateMachine.ChangeState((int)GuestStateID.Dialogue);
    }
}
}