using UnityEngine;
using Codebase.Mechanics.Data;

namespace Codebase.Mechanics.BlendingNBrewingSystem
{
    public class IngredientSource : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private IngredientData ingredientData;
        private Camera mainCamera;

        private GameObject draggedObject;
        private bool isDragging;
        private Collider objectCollider;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void OnMouseDown()
        {
            if (isDragging) return;
            StartDrag();
        }

        private void StartDrag()
        {
            draggedObject = Instantiate(ingredientData.WorldPrefab, transform.position, Quaternion.identity);
            
            objectCollider = draggedObject.GetComponent<Collider>();
            if (objectCollider != null)
                objectCollider.enabled = false;

            UpdateDragPosition();
            isDragging = true;
        }

        private void Update()
        {
            if (isDragging && Input.GetMouseButton(0))
            {
                UpdateDragPosition();
            }
            else if (isDragging && Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }

        private void UpdateDragPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            worldPos.z=1;
            draggedObject.transform.position = worldPos;
        }

        private void EndDrag()
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                IngredientDropZone dropZone = hit.collider.GetComponent<IngredientDropZone>();
                if (dropZone != null)
                {
                    dropZone.DropIngredient(ingredientData);
                }
            }

            Destroy(draggedObject);
            draggedObject = null;
            isDragging = false;
        }
    }
}