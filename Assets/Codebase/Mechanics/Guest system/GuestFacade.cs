using System.Collections;
using UnityEngine;
using Codebase.Mechanics.Data;

namespace Codebase.Mechanics.GuestSystem
{
    public class GuestFacade : MonoBehaviour
    {
        //TODO: Прикрутить Один
        [SerializeField]private MomentData[] momentData; //TODO: Переделать в очередь
        [SerializeField]private Transform guestPoint;

        private bool _dialogueWasStarted;
        private GuestBuilder _guestBuilder;
        private int _currentGuestIndex = -1;

        private void SpawnNextGuest()
        {
            _currentGuestIndex++;
            if (_currentGuestIndex < guestsQueue.Count)
            {
                StartCoroutine(WaitUntilSpawn(guestsQueue[_currentGuestIndex]));
            }
            else
            {
                Debug.Log("Все гости на сегодня обслужены");
                // Здесь можно вызвать завершение дня
            }
        }
        public void OnGuestFinished()
        {
            SpawnNextGuest();
        }
        private void CreateGuest(MomentData moment){
            _dialogueWasStarted=false;
            _guestBuilder?.ResetMachine();
            _guestBuilder=new GuestBuilder(moment);
            _guestBuilder.CreateGuest(guestPoint);
            _guestBuilder.SetEmotion();

            _guestBuilder.GuestPresenter.OnClicked=OnPresenterClicked;
        }
        public void StartDialogue()
        {
            if(!_dialogueWasStarted){
                _guestBuilder.Guest.Interact(); 
                _dialogueWasStarted=true;
            }
        }
        public void StartEvaluation(RecipeData recipe)
        {
            _guestBuilder.Guest.ServedRecipe=recipe;
            _guestBuilder.Guest.Interact();
        }
        public void OnPresenterClicked(){
            StartDialogue();
        }

        private IEnumerator WaitUntilSpawn(MomentData moment){
            yield return new WaitForSeconds(5f);
            CreateGuest(moment);
        }

        private void Start(){
            if (guestsQueue != null && guestsQueue.Count > 0)
                SpawnNextGuest();
        }
    }
}