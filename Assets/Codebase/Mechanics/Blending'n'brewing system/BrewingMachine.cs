using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Codebase.Mechanics.BlendingNBrewingSystem.BrewingStates;
using Codebase.Mechanics.GuestSystem.GuestStates;
using Codebase.Mechanics.Data;
using Codebase.UI;
using Sirenix.OdinInspector;

namespace Codebase.Mechanics.BlendingNBrewingSystem
{
    public class BrewingMachine : MonoBehaviour, IStateMachine<BlendMemento>, IResetable
    {
        public enum BrewingStates
        {
            Idle = 0,           // ждёт купаж
            BlendLoaded = 1,    // купаж загружен, ожидается кристалл
            CrystalPlaced = 2,  // кристалл поставлен, требуется зажигание
            Brewing = 3,        // активная варка
            Done = 4            // завершено, можно разливать
        }
        private IState<BlendMemento>[] states;
        [ShowInInspector, ReadOnly]
        public IState<BlendMemento> CurrentState => states != null ? states[(int)_currentState] : null;

        [FoldoutGroup("Состояние"), LabelText("Текущее состояние"), EnumToggleButtons, ReadOnly]
        [SerializeField] private BrewingStates _currentState;
        public BrewingStates CurrentStateType => _currentState;

        [FoldoutGroup("Компоненты")]
        [SerializeField, Required] private Transform brewOutputPoint;
        [FoldoutGroup("Компоненты")]
        [SerializeField, Required] private Transform guestServePoint;
        [FoldoutGroup("Компоненты")]
        [SerializeField] private Image crystalFlame;
        [FoldoutGroup("Компоненты")]
        [SerializeField] private Slider strengthSlider;
        [FoldoutGroup("Компоненты")]
        [SerializeField] private Image filledSliderArea;
        [FoldoutGroup("Компоненты")]
        [SerializeField] private ParticleSystem particleSystem;

        [FoldoutGroup("Настройки")]
        [SerializeField] private Color lowColor;
        [FoldoutGroup("Настройки")]
        [SerializeField] private Color midColor;
        [FoldoutGroup("Настройки")]
        [SerializeField] private Color highColor;

        [FoldoutGroup("Настройки")]
        [SerializeField,LabelText("Скорость варки")] private float brewRate = 0.1f;

        [FoldoutGroup("Настройки")]
        [SerializeField, Required] private List<RecipeData> availableRecipes;

        [FoldoutGroup("Данные"), ShowInInspector, ReadOnly]
        private BlendMemento currentBlend;
        public BlendMemento CurrentBlend => currentBlend;

        [FoldoutGroup("Данные"), ShowInInspector, ReadOnly]
        private List<RecipeData> matchedRecipes=new List<RecipeData>();
        public List<RecipeData> MatchedRecipes => matchedRecipes;

        private RecipeData brewedRecipe;

        [FoldoutGroup("Данные"), ShowInInspector, ReadOnly]
        private float currentStrength;

        [FoldoutGroup("Данные"), ShowInInspector, ReadOnly]
        private CrystalData placedCrystal;

        [FoldoutGroup("Данные"), ShowInInspector, ReadOnly]
        internal bool isFlameLit;

        public event Action<RecipeData> OnBrewingSuccess;

        public void ChangeState(int stateID)
        {
            if (states == null || stateID < 0 || stateID >= states.Length)
            {
                Debug.LogError($"BrewingMachine: Invalid state ID {stateID}");
                return;
            }

            CurrentState?.Exit();
            _currentState = (BrewingStates)stateID;
            CurrentState.Enter();
        }
        private void FindAllMatchingRecipes(IReadOnlyList<IngredientData> ingredients)
        {
            foreach (var recipe in availableRecipes)
            {
                if (recipe.requiredIngredients.Count != ingredients.Count)
                    continue;

                bool match = recipe.requiredIngredients.All(ing => ingredients.Contains(ing));
                if (match)
                    matchedRecipes.Add(recipe);
            }
        }
        private RecipeData FindMatchingRecipe(List<RecipeData> recipes,CrystalData placedCrystal)
        {
            foreach (var recipe in recipes)
            {
                bool match = recipe.crystal==placedCrystal;
                if (match)
                    return recipe;
            }
            return null;
        }
        public void LoadBlend(BlendMemento blend)
        {
            if (_currentState != BrewingStates.Idle)
            {
                NotificationManager.Instance.ShowNotification("Можно загружать купаж только в состоянии ожидания.", NotificationType.Warning);
                return;
            }
            if (!blend.IsBlendSuccessful)
            {
                NotificationManager.Instance.ShowNotification("Купаж испорчен – не подходит для варки.", NotificationType.Error);
                return;
            }

            FindAllMatchingRecipes(blend.Ingredients);
            if (matchedRecipes.Count == 0)
            {
                NotificationManager.Instance.ShowNotification("Для этого набора ингредиентов нет рецепта.", NotificationType.Warning);
                return;
            }

            currentBlend = blend;
            ChangeState((int)BrewingStates.BlendLoaded);
            NotificationManager.Instance.ShowNotification("Купаж загружен. Теперь поместите магический кристалл.", NotificationType.Success);
        }

        public void PlaceCrystal(CrystalData data)
        {
            if (_currentState != BrewingStates.BlendLoaded)
            {
                NotificationManager.Instance.ShowNotification("Сначала загрузите купаж!", NotificationType.Warning);
                return;
            }

            placedCrystal = data;
            ChangeState((int)BrewingStates.CrystalPlaced);
            NotificationManager.Instance.ShowNotification($"Кристалл {data.crystalName} установлен. Зажгите его!", NotificationType.Info);
        }
        private void UpdateStrengthUI()
        {
            if (strengthSlider != null)
            {
                strengthSlider.value = currentStrength;
                strengthSlider.maxValue = 1f;
                if(filledSliderArea!=null)
                {
                    if(currentStrength==0){
                        filledSliderArea.color=Color.white;
                    }
                    else{
                        if(currentStrength<=0.33)
                            filledSliderArea.color=lowColor;
                        else if(currentStrength<=0.66)
                            filledSliderArea.color=midColor;
                        else if(currentStrength<=1)
                            filledSliderArea.color=highColor;
                    }
                }
            }
        }
        public void IgniteCrystal()
        {
            if (_currentState != BrewingStates.CrystalPlaced)
            {
                NotificationManager.Instance.ShowNotification("Сначала поставьте кристалл!", NotificationType.Warning);
                return;
            }

            isFlameLit = true;
            currentStrength = 0f;
            UpdateStrengthUI();
            particleSystem?.Play();
            ChangeState((int)BrewingStates.Brewing);
            NotificationManager.Instance.ShowNotification("Кристалл горит! Начинается варка.", NotificationType.Info);
        }
        private void EvaluateBrewResult()
        {
            brewedRecipe = FindMatchingRecipe(MatchedRecipes,placedCrystal);
            if (brewedRecipe == null)
            {
                NotificationManager.Instance.ShowNotification("Ошибка: рецепт не найден! Что-то пошло не так.", NotificationType.Error);
                ChangeState((int)BrewingStates.Done);
                return;
            }

            float target = brewedRecipe.targetStrength;
            float lower = target - EvaluationState.STRENGTH_TOLERANCY;
            float upper = target + EvaluationState.STRENGTH_TOLERANCY;

            bool success = currentStrength >= lower && currentStrength <= upper; 
            if (success)
            {
                NotificationManager.Instance.ShowNotification($"Чай «{brewedRecipe.recipeName}» удался! Крепость идеальна.", NotificationType.Success);
            }
            
            OnBrewingSuccess?.Invoke(brewedRecipe);

            ChangeState((int)BrewingStates.Done);
        }
        public void ClickWhileBrewing()
        {
            if (_currentState != BrewingStates.Brewing) return;

            isFlameLit = false;
            particleSystem?.Stop();
            EvaluateBrewResult();
        }
        [Button("Сбросить машину"), FoldoutGroup("Отладка")]
        public void ResetMachine()
        {
            currentBlend = null;
            matchedRecipes = new List<RecipeData>();
            currentStrength = 0f;
            placedCrystal = null;
            isFlameLit = false;
            UpdateStrengthUI();
            UpdateFlameVisual();
            ChangeState((int)BrewingStates.Idle);
        }
        public void Dispense()
        {
            if (_currentState != BrewingStates.Done)
            {
                NotificationManager.Instance.ShowNotification("Сначала завершите варку!", NotificationType.Warning);
                return;
            }

            if (brewedRecipe != null && brewedRecipe.finalTeaPrefab != null)
            {
                var teaObject = Instantiate(brewedRecipe.finalTeaPrefab, brewOutputPoint.position, Quaternion.identity);
                var teaComponent = teaObject.GetComponent<BrewedTea>();
                if (teaComponent != null)
                {
                    teaComponent.Initialize(brewedRecipe, guestServePoint);
                }
                NotificationManager.Instance.ShowNotification($"Чай разлит! Можете подавать гостю.", NotificationType.Success);
            }
            else
            {
                NotificationManager.Instance.ShowNotification("Нечего разливать – варка не завершена или рецепт повреждён.", NotificationType.Error);
            }
            ResetMachine();
        }
        public void UpdateFlameVisual()
        {
            if (crystalFlame != null)
                crystalFlame.enabled = isFlameLit;
        }

        private void Awake()
        {
            states = new IState<BlendMemento>[]
            {
                new IdleState(this),
                new BlendLoadedState(this),
                new CrystalPlacedState(this),
                new BrewingState(this),
                new DoneState(this)
            };
        }
        private void Start()
        {
            ChangeState((int)BrewingStates.Idle);
        }
        private void Update()
        {
            if (_currentState == BrewingStates.Brewing && isFlameLit)
            {
                currentStrength += brewRate * Time.deltaTime;
                currentStrength = Mathf.Clamp01(currentStrength);
                UpdateStrengthUI();
            }
        }
    }
}