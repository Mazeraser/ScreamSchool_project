using UnityEngine;

namespace Codebase.Mechanics.BlendingNBrewingSystem.BlendingInteractiveComponents
{
    public class ShakeHandle : MonoBehaviour
    {
        [SerializeField] private BlendingMachine machine;

        private void OnMouseDown()
        {
            GetComponent<Collider2D>().enabled=false;
            machine.StartShaking();
        }

        private void OnMouseDrag()
        {
            machine.UpdateShaking();
        }

        private void OnMouseUp()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            GetComponent<Collider2D>().enabled=true;
            if (hit.collider!=null && hit.collider.CompareTag("Trash"))
            {
                Debug.Log("Шейкер очищен");
                machine.ResetMachine();
                return;
            }
            else
                machine.EndShaking();
        }
    }
}