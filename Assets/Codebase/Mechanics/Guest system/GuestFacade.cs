using System.Collections;
using UnityEngine;
using Codebase.Mechanics.Data;
using Sirenix.OdinInspector;

namespace Codebase.Mechanics.GuestSystem
{
    public class GuestFacade : MonoBehaviour
    {
        [LabelText("Очередь гостей на этот уровень")]
        [SerializeField]private MomentData[] guestsQueue;
        
        [FoldoutGroup("Настройки создания персонажей")]
        [LabelText("Точка, в которой будет стоять гость")]
        [SerializeField]
        private Transform guestPoint;

        [FoldoutGroup("Настройки создания персонажей")]
        [LabelText("Время, за которое он дойдет до точки")]
        [SerializeField]
        private float moveDuration=0.5f;

        [FoldoutGroup("Настройки создания персонажей")]
        [LabelText("Начальное отклонение(откуда спавнится и начинает идти)")]
        [SerializeField]
        private Vector3 startOffset = new Vector3(-10f, 0f, 0f);

        private bool _dialogueWasStarted;
        private GuestBuilder _guestBuilder;
        private int _currentGuestIndex = -1;

        private void SpawnNextGuest()
        {
            _currentGuestIndex++;
            if (_currentGuestIndex < guestsQueue.Length)
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
            _guestBuilder=new GuestBuilder(moment, moveDuration, startOffset);
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
            SpawnNextGuest();
        }
    }
}