using UnityEngine;
using UnityEngine.UI;

namespace Codebase.Infrastructure{
    [RequireComponent(typeof(Button))]
    public class AutoMouseDetect : MonoBehaviour
    {
        private void OnMouseEnter(){
            GetComponent<Button>().onClick.Invoke();
        }
    }
}