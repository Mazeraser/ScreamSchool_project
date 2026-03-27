using Codebase.Mechanics.Data;
using Codebase.Infrastructure;
using UnityEngine;

namespace Codebase.Mechanics.GuestSystem.GuestStates
{
    public class DialogueState : GuestStateBase
    {
        public DialogueState(Guest guest) : base(guest, GuestStateID.Dialogue) { }

        private void OnDialogueStarted()
        {
            Debug.Log($"Диалог с гостем начат");
        }
        private void OnDialogueFinished()
        {
            Debug.Log($"Диалог с гостем завершен");
            Interact(_guest);
        }

        public override void Enter()
        {
            //TODO: Запуск диалога из MomentData.Dialogue
            //TODO: Внедрить ink
            DialogueManager.Instance.StartDialogue(
                _guest.CurrentMoment.Dialogue,
                OnDialogueFinished,
                OnDialogueStarted
            );
        }

        public override void Interact(Guest guest)
        {
            guest.StateMachine.ChangeState((int)GuestStateID.WaitingForOrder);
        }
    }
}