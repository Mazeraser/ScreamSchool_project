using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Codebase.Mechanics.Data;
using Codebase.Infrastructure;
using Sirenix.OdinInspector;

namespace Codebase.Mechanics.GuestSystem
{
    public class GuestFacade : MonoBehaviour
    {
        [LabelText("Очередь гостей на этот уровень")]
        [SerializeField] private MomentData[] guestsQueue;

        [FoldoutGroup("Настройки создания персонажей")]
        [LabelText("Точка, в которой будет стоять гость")]
        [SerializeField]
        private Transform guestPoint;

        [FoldoutGroup("Настройки создания персонажей")]
        [LabelText("Время, за которое он дойдет до точки")]
        [SerializeField]
        private float moveDuration = 0.5f;

        [FoldoutGroup("Настройки создания персонажей")]
        [LabelText("Начальное отклонение(откуда спавнится и начинает идти)")]
        [SerializeField]
        private Vector3 startOffset = new Vector3(-10f, 0f, 0f);

        [FoldoutGroup("Переход при окончании")]
        [LabelText("Имя следующей сцены")]
        [SerializeField] private string nextSceneName = "NextScene";

        [FoldoutGroup("Переход при окончании")]
        [LabelText("Длительность затемнения (сек)")]
        [SerializeField] private float fadeDuration = 1f;

        [FoldoutGroup("Переход при окончании")]
        [LabelText("UI-панель затемнения (опционально)")]
        [SerializeField] private CanvasGroup fadeCanvasGroup; // если null, создаётся автоматически

        private bool _dialogueWasStarted;
        private GuestBuilder _guestBuilder;
        private int _currentGuestIndex = -1;
        private bool _isTransitioning = false;

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
                StartFadeAndTransition();
            }
        }

        private void OnGuestFinished()
        {
            if (_guestBuilder?.Guest != null)
                _guestBuilder.Guest.OnGuestFinished -= OnGuestFinished;
            _guestBuilder?.ResetMachine();
            if (DialogueHistory.Instance != null)
                DialogueHistory.Instance.ClearHistory();
            SpawnNextGuest();
        }

        private void CreateGuest(MomentData moment)
        {
            _dialogueWasStarted = false;
            _guestBuilder?.ResetMachine();
            _guestBuilder = new GuestBuilder(moment, moveDuration, startOffset);
            _guestBuilder.CreateGuest(guestPoint);
            _guestBuilder.SetEmotion();

            _guestBuilder.Guest.OnGuestFinished += OnGuestFinished;
            _guestBuilder.GuestPresenter.OnClicked = OnPresenterClicked;
        }

        public void StartDialogue()
        {
            if (!_dialogueWasStarted && _guestBuilder?.Guest != null)
            {
                _guestBuilder.Guest.Interact();
                _dialogueWasStarted = true;
            }
        }

        public void StartEvaluation(RecipeData recipe)
        {
            if (_guestBuilder?.Guest == null) return;
            _guestBuilder.Guest.ServedRecipe = recipe;
            _guestBuilder.Guest.Interact();
        }

        public void OnPresenterClicked()
        {
            StartDialogue();
        }

        private IEnumerator WaitUntilSpawn(MomentData moment)
        {
            yield return new WaitForSeconds(5f);
            CreateGuest(moment);
        }

        private void Start()
        {
            SpawnNextGuest();
        }

        private void StartFadeAndTransition()
        {
            if (_isTransitioning) return;
            _isTransitioning = true;
            StartCoroutine(FadeAndLoadScene());
        }

        private IEnumerator FadeAndLoadScene()
        {
            // Затемнение
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;

            // Загрузка сцены
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}