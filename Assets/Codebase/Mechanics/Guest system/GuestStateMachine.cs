using Codebase.Mechanics.Data;
using System.Collections.Generic;
using Codebase.Mechanics.GuestSystem.GuestStates;

namespace Codebase.Mechanics.GuestSystem
{
    public enum GuestStateID
    {
        WaitingForDialogue=0, // Ожидание диалога
        Dialogue=1,           // Диалог
        WaitingForOrder=2,    // Ожидание заказа
        Evaluation=3          // Оценка напитка
    }
    public class GuestStateMachine : IStateMachine<Guest>
    {
        public IState<Guest> CurrentState { get; private set; }

        private Dictionary<int, IState<Guest>> _states;

        public GuestStateMachine(Guest guest)
        {
            _states = new Dictionary<int, IState<Guest>>
            {
                { (int)GuestStateID.WaitingForDialogue, new WaitingForDialogueState(guest) },
                { (int)GuestStateID.Dialogue, new DialogueState(guest) },
                { (int)GuestStateID.WaitingForOrder, new WaitingForOrderState(guest) },
                { (int)GuestStateID.Evaluation, new EvaluationState(guest) }
            };

            // Начальное состояние
            ChangeState((int)GuestStateID.WaitingForDialogue);
        }

        public void ChangeState(int stateID)
        {
            if (_states.TryGetValue(stateID, out var newState))
            {
                CurrentState?.Exit();
                CurrentState = newState;
                CurrentState.Enter();
            }
        }
    }
}
    