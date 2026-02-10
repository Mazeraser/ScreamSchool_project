using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Codebase.Infrastructure
{    
    public class CameraService : MonoBehaviour
    {
        private int Location1=Animator.StringToHash("Base Layer.Location 1");
        private int Location2=Animator.StringToHash("Base Layer.Location 2");

        private Animator _animator;
        [SerializeField]private CanvasGroup _canvasGroup;
        [SerializeField]private float _fadeTime=1f;

        private void Start(){
            _animator = GetComponent<Animator>();
        }

        private async UniTask FadeIn(float fadeTime)
        {
            if(_canvasGroup!=null)
                await _canvasGroup.DOFade(1f,fadeTime).OnComplete(()=>{Debug.Log("Fade in");}).AsyncWaitForCompletion();;
        }
        private async UniTask FadeOut(float fadeTime)
        {
            if(_canvasGroup!=null)
                await _canvasGroup.DOFade(0f,fadeTime).OnComplete(()=>{Debug.Log("Fade out");}).AsyncWaitForCompletion();;
        }

        [Button]
        public async void SetDirection(int dir)
        {
            await FadeOut(_fadeTime);

            if(dir>0)
                _animator.SetTrigger("Right");
            else if(dir<0)
                _animator.SetTrigger("Left");
            await UniTask.WaitUntil(() => {  
                var animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                return animatorStateInfo.fullPathHash == Location1&&dir<0
                    || animatorStateInfo.fullPathHash == Location2&&dir>0;  
            });
            
            await FadeIn(_fadeTime);
        }
        
    }
}