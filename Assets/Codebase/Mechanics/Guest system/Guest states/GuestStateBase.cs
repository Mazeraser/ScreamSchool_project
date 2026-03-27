namespace Codebase.Mechanics.GuestSystem.GuestStates
{
    public abstract class GuestStateBase : IState<Guest>
    {
        protected Guest _guest;
        public int ID { get; private set; }

        protected GuestStateBase(Guest guest, GuestStateID id)
        {
            _guest = guest;
            ID = (int)id;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }

        public abstract void Interact(Guest guest);
    }
}