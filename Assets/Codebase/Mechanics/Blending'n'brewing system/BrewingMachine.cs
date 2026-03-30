using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Codebase.Mechanics.Data;
using System.Linq;
using Codebase.Mechanics.BlendingNBrewingSystem.BrewingStates;
using Codebase.Mechanics.GuestSystem.GuestStates;

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
        [SerializeField] private Image crystalFlame;
        [FoldoutGroup("Компоненты")]
        [SerializeField] private Slider strengthSlider;
        [FoldoutGroup("Компоненты")]
        [SerializeField] private Image filledSliderArea;

        [FoldoutGroup("Настройки")]
        [SerializeField] private Color wrongColor;
        [FoldoutGroup("Настройки")]
        [SerializeField] private Color rightColor;

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
                Debug.LogWarning("Можно загружать купаж только в состоянии Idle");
                return;
            }
            if (!blend.IsBlendSuccessful)
            {
                Debug.Log("Купаж испорчен – не подходит для варки");
                return;
            }

            FindAllMatchingRecipes(blend.Ingredients);
            if (matchedRecipes.Count == 0)
            {
                Debug.Log("Для этого набора ингредиентов нет рецепта");
                return;
            }

            currentBlend = blend;
            ChangeState((int)BrewingStates.BlendLoaded);
        }

        public void PlaceCrystal(CrystalData data)
        {
            if (_currentState != BrewingStates.BlendLoaded)
            {
                Debug.LogWarning("Сначала загрузите купаж");
                return;
            }

            placedCrystal = data;
            ChangeState((int)BrewingStates.CrystalPlaced);
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
                        var matchedRecipe = FindMatchingRecipe(MatchedRecipes,placedCrystal);
                        float target = matchedRecipe.targetStrength;
                        float lower = target - EvaluationState.STRENGTH_TOLERANCY;
                        float upper = target + EvaluationState.STRENGTH_TOLERANCY;
                        bool success = currentStrength >= lower && currentStrength <= upper;

                        filledSliderArea.color=success?rightColor:wrongColor;
                    }
                }
            }
        }
        public void IgniteCrystal()
        {
            if (_currentState != BrewingStates.CrystalPlaced)
            {
                Debug.LogWarning("Сначала поставьте кристалл");
                return;
            }

            isFlameLit = true;
            currentStrength = 0f;
            UpdateStrengthUI();
            ChangeState((int)BrewingStates.Brewing);
        }
        private void EvaluateBrewResult()
        {
            brewedRecipe = FindMatchingRecipe(MatchedRecipes,placedCrystal);

            float target = brewedRecipe.targetStrength;
            float lower = target - EvaluationState.STRENGTH_TOLERANCY;
            float upper = target + EvaluationState.STRENGTH_TOLERANCY;

            bool success = currentStrength >= lower && currentStrength <= upper; 
            Debug.Log($"Текущая крепость-{currentStrength} Необходимо {lower}-{upper}");
            Debug.Log(success ? "Чай удался!" : "Чай испорчен: крепость не та");

            ChangeState((int)BrewingStates.Done);
        }
        public void ClickWhileBrewing()
        {
            if (_currentState != BrewingStates.Brewing) return;

            isFlameLit = false;
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
                Debug.LogWarning("Сначала завершите варку");
                return;
            }

            if (brewedRecipe != null && brewedRecipe.finalTeaPrefab != null)
            {
                var teaObject = Instantiate(brewedRecipe.finalTeaPrefab, brewOutputPoint.position, Quaternion.identity);
                var teaComponent = teaObject.GetComponent<BrewedTea>();
                if (teaComponent != null)
                {
                    teaComponent.Initialize(brewedRecipe);
                }
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