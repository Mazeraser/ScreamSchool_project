using UnityEngine;
using Sirenix.OdinInspector;

namespace Codebase.Mechanics.BlendingNBrewingSystem
{
    public class DispenseHandle : MonoBehaviour
    {
        [FoldoutGroup("Настройки")]
        [SerializeField, Required] private BrewingMachine brewingMachine;
        
        [FoldoutGroup("Настройки вращения")]
        [SerializeField] private float rotationSpeed = 25f;
        
        [FoldoutGroup("Настройки вращения")]
        [SerializeField] private float minRotation = 0f;
        [FoldoutGroup("Настройки вращения")]
        [SerializeField] private float maxRotation = 360f;
        
        [FoldoutGroup("Настройки активации")]
        [SerializeField] private bool requireMouseDrag = true;
        
        [FoldoutGroup("Настройки активации")]
        [SerializeField] private KeyCode interactionKey = KeyCode.Mouse0;
        
        [FoldoutGroup("Состояние"), ShowInInspector, ReadOnly]
        private float currentRotation;
        
        [FoldoutGroup("Состояние"), ShowInInspector, ReadOnly]
        private bool isDragging;
        
        [FoldoutGroup("Состояние"), ShowInInspector, ReadOnly]
        private bool dispenseTriggered;
        
        [FoldoutGroup("Состояние"), ShowInInspector, ReadOnly]
        private Vector3 initialMousePosition;
        
        [FoldoutGroup("Состояние"), ShowInInspector, ReadOnly]
        private float initialRotation;
        
        private bool IsMouseOverHandle()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100f);
            
            return hit.collider != null && hit.collider.gameObject == gameObject;
        }
        private void TriggerDispense()
        {
            if (brewingMachine != null)
            {
                Debug.Log("DispenseHandle: Полный оборот, разливаем чай");
                brewingMachine.Dispense();
            }
            else
            {
                Debug.LogWarning("DispenseHandle: не привязана BrewingMachine");
            }
        }
        private void HandleMouseInput()
        {
            if (Input.GetKeyDown(interactionKey) && IsMouseOverHandle())
            {
                isDragging = true;
                initialMousePosition = Input.mousePosition;
                initialRotation = currentRotation;
                dispenseTriggered = false;
            }
            else if (Input.GetKeyUp(interactionKey))
            {
                isDragging = false;
            }

            if (!requireMouseDrag)
            {
                // Если не требуется перетаскивание, просто увеличиваем вращение со временем
                if (isDragging)
                {
                    currentRotation += rotationSpeed * Time.deltaTime;
                }
                return;
            }
            if (isDragging)
            {
                float mouseDelta = Input.mousePosition.x-initialMousePosition.x;
                
                currentRotation = initialRotation + mouseDelta * rotationSpeed * 0.01f;
            }
        }
        private void HandleRotation()
        {
            if (currentRotation > maxRotation)
            {
                if (!dispenseTriggered)
                {
                    TriggerDispense();
                    dispenseTriggered = true;
                }
            }
            else if(currentRotation < minRotation)
                currentRotation = minRotation;
            
            transform.localRotation = Quaternion.Euler(0, 0, currentRotation);
        }

        private void Start()
        {
            currentRotation = transform.localEulerAngles.z;
            initialRotation = currentRotation;
        }
        
        private void Update()
        {
            if((int)brewingMachine.CurrentStateType==4)
            {
                HandleMouseInput();
                HandleRotation();
                Debug.Log($"Накручено: {currentRotation}. Достаточно: {maxRotation}");
            }
        }
        
        // Публичный метод для принудительного сброса
        public void ResetHandle()
        {
            currentRotation = initialRotation;
            dispenseTriggered = false;
            isDragging = false;
            transform.localRotation = Quaternion.Euler(0, 0, currentRotation);
        }
        
        [Button("Разлить чай"), FoldoutGroup("Отладка")]
        public void ManualDispense()
        {
            TriggerDispense();
        }
    }
}