using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Codebase.Mechanics.Data;
using Codebase.Mechanics.BlendingNBrewingSystem.BlendingStates;
using Codebase.UI;
using Sirenix.OdinInspector;

namespace Codebase.Mechanics.BlendingNBrewingSystem
{
    public class BlendingMachine : MonoBehaviour, IStateMachine<IngredientData>, IResetable
    {
        public enum BlendingStates
        {
            Idle = 0,
            AddingIngredients = 1,
            Shaking = 2,
            Ready = 3
        }

        private IState<IngredientData>[] states;

        [ShowInInspector, ReadOnly]
        public IState<IngredientData> CurrentState => states != null ? states[(int)_currentState] : null;

        [FoldoutGroup("Состояние"), LabelText("Текущее состояние"), EnumToggleButtons, ReadOnly]
        [SerializeField] private BlendingStates _currentState;
        public BlendingStates CurrentStateType => _currentState;

        [FoldoutGroup("Компоненты")]
        [SerializeField, Required] private Transform shakeHandle;
        [FoldoutGroup("Компоненты")]
        [SerializeField] private Slider progressSlider;
        [FoldoutGroup("Компоненты")]
        [SerializeField] private Image filledSliderArea;
        [FoldoutGroup("Компоненты")]
        [SerializeField] private BrewingMachine _brewingMachine;

        [FoldoutGroup("Настройки")]
        [SerializeField] private Color wrongColor;
        [FoldoutGroup("Настройки")]
        [SerializeField] private Color rightColor;

        [FoldoutGroup("Настройки")]
        [LabelText("Чувствительность тряски"), Range(0.001f, 0.01f)]
        [SerializeField] private float shakeSensitivity = 0.01f;
        [FoldoutGroup("Настройки")]
        [LabelText("Порог скорости встряхивания"), Range(1f, 20f)]
        [SerializeField] private float shakeSpeedThreshold = 10f;

        [FoldoutGroup("Настройки")]
        [LabelText("Допустимый диапазон"), Range(0f, 0.5f)]
        [SerializeField] private float acceptableRange = 0.1f;

        [FoldoutGroup("Данные"), ReadOnly, ShowInInspector]
        private List<IngredientData> ingredients = new List<IngredientData>();
        public IReadOnlyList<IngredientData> Ingredients => ingredients;

        [FoldoutGroup("Данные"), ReadOnly, ShowInInspector]
        private float currentShakeProgress;

        [FoldoutGroup("Данные"), ReadOnly, ShowInInspector]
        private float targetShakeTotal;

        [FoldoutGroup("Данные"), ReadOnly, ShowInInspector]
        private bool lastBlendSuccessful;

        private Vector3 lastMousePosition;
        private Vector3 shakerPosition;

        public void ChangeState(int stateID)
        {
            if (states == null || stateID < 0 || stateID >= states.Length)
            {
                Debug.LogError($"BlendingMachine: Invalid state ID {stateID}");
                return;
            }

            CurrentState?.Exit();
            _currentState = (BlendingStates)stateID;
            CurrentState.Enter();
        }
        private void RecalculateTarget()
        {
            targetShakeTotal = 0f;
            foreach (var ing in ingredients)
                targetShakeTotal += ing.ShakeContribution;
            if (progressSlider != null)
            {
                progressSlider.maxValue = Mathf.Max(targetShakeTotal*2, 1f);
            }
        }
        public void AddIngredient(IngredientData ingredient)
        {
            ingredients.Add(ingredient);
            RecalculateTarget();
            Debug.Log($"Added {ingredient.IngredientName}, total shake target: {targetShakeTotal}");
        }
        private void UpdateProgressUI()
        {
            if (progressSlider != null){
                progressSlider.value = currentShakeProgress;
                if(filledSliderArea!=null)
                {
                    if(currentShakeProgress==0){
                        filledSliderArea.color=Color.white;
                    }
                    else{
                        float lower = targetShakeTotal - acceptableRange;
                        float upper = targetShakeTotal + acceptableRange;
                        
                        bool success = currentShakeProgress>=lower&&currentShakeProgress<=upper;

                        filledSliderArea.color=success?rightColor:wrongColor;
                    }
                }
            }
            Debug.Log($"Current progress: {currentShakeProgress}. Need: {targetShakeTotal-acceptableRange}-{targetShakeTotal+acceptableRange}");
        }
         public void StartShaking()
        {
            if(_currentState != BlendingStates.AddingIngredients){
                NotificationManager.Instance.ShowNotification("Нечего смешивать – добавьте ингредиенты!", NotificationType.Warning);
                return;
            }
            ChangeState((int)BlendingStates.Shaking);
            lastMousePosition = Input.mousePosition;
            currentShakeProgress = 0f;
            NotificationManager.Instance.ShowNotification("Начинайте трясти!", NotificationType.Info);
            UpdateProgressUI();
        }
        public void UpdateShaking()
        {
            //if (_currentState != BlendingStates.Shaking) return;
            
            Vector3 currentMouse = Input.mousePosition;
            Vector3 delta = currentMouse - lastMousePosition;
            float deltaMagnitude = delta.magnitude * shakeSensitivity;
            lastMousePosition = currentMouse;
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(currentMouse.x,currentMouse.y,1));
            
            float speed = deltaMagnitude / Time.deltaTime; 
            
            if (speed > shakeSpeedThreshold)
            {
                currentShakeProgress += deltaMagnitude;
                UpdateProgressUI();
            }
            
        }
        public void EndShaking()
        {
            if (_currentState != BlendingStates.Shaking) return;
            float lower = targetShakeTotal - acceptableRange;
            float upper = targetShakeTotal + acceptableRange;
            transform.position = shakerPosition;
            ChangeState((int)BlendingStates.Ready);
            if (currentShakeProgress >= lower && currentShakeProgress <= upper)
            {
                NotificationManager.Instance.ShowNotification("Чайный купаж готов! Идеальная консистенция.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Instance.ShowNotification("Купаж испорчен – встряхнуто не так.", NotificationType.Error);
            }
            lastBlendSuccessful = (currentShakeProgress >= lower && currentShakeProgress <= upper);
        }
        [Button("Сбросить машину"), FoldoutGroup("Отладка")]
        public void ResetMachine()
        {
            transform.position = shakerPosition;
            ingredients.Clear();
            currentShakeProgress = 0f;
            targetShakeTotal = 0f;
            UpdateProgressUI();
            ChangeState((int)BlendingStates.Idle);
        }
        public BlendMemento CreateBlendMemento()
        {
            return new BlendMemento(ingredients, lastBlendSuccessful);
        }
        public void DropTeaBlend(){

            if(_brewingMachine==null)
                Debug.LogError("I don see my brewing machine(");
            else if((int)_brewingMachine.CurrentStateType==0)
            {
                _brewingMachine.CurrentState.Interact(CreateBlendMemento());
                ResetMachine();
            }
        }

        private void Awake()
        {
            states = new IState<IngredientData>[]
            {
                new IdleState(this),
                new AddingIngredientsState(this),
                new ShakingState(this),
                new ReadyState(this)
            };
        }

        private void Start()
        {
            shakerPosition = transform.position;
            ChangeState((int)BlendingStates.Idle);
        }
    }
}