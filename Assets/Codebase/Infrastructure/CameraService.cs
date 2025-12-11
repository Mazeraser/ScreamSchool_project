using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;

namespace Codebase.Infrastructure
{    
    public class CameraService : MonoBehaviour
    {
        private Animator _animator;

        [Button]
        public void SetDirection(int dir) {
            if(dir>0)
                _animator.SetTrigger("Right");
            else if(dir<0)
                _animator.SetTrigger("Left");
        } 
        
        private void Start(){
            _animator = GetComponent<Animator>();
        }    
    }
}