using UnityEngine;
using Codebase.Mechanics.Data;

namespace Codebase.Mechanics.BlendingNBrewingSystem
{
    /// <summary>
    /// Компонент на источнике кристаллов. Позволяет перетаскивать кристалл в зону установки.
    /// </summary>
    public class CrystalSource : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private CrystalData crystalData;
        private Camera mainCamera;

        private GameObject draggedObject;
        private bool isDragging;
        private Collider2D objectCollider;   // предполагаем 2D физику

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
            if (crystalData == null || crystalData.worldPrefab == null)
            {
                Debug.LogError("CrystalSource: не назначен префаб кристалла");
                return;
            }

            draggedObject = Instantiate(crystalData.worldPrefab, transform.position, Quaternion.identity);
            
            objectCollider = draggedObject.GetComponent<Collider2D>();
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
            worldPos.z = 1;
            draggedObject.transform.position = worldPos;
        }

        private void EndDrag()
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                CrystalDropZone dropZone = hit.collider.GetComponent<CrystalDropZone>();
                if (dropZone != null)
                {
                    dropZone.DropCrystal(crystalData);
                }
            }

            Destroy(draggedObject);
            draggedObject = null;
            isDragging = false;
        }
    }
}