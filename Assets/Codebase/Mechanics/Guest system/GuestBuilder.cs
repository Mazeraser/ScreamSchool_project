using UnityEngine;
using Codebase.Mechanics.Data;
using System.Collections;

namespace Codebase.Mechanics.GuestSystem
{
    public class GuestBuilder : IResetable
    {
        private MomentData _momentData;
        public GuestPresenter GuestPresenter{ get; private set; }
        public Guest Guest{ get; private set; }
        private GameObject guestPresenterObject;
        private float _moveDuration;
        private Vector3 _startOffset;

        public GuestBuilder(MomentData momentData, float moveDuration, Vector3 startOffset)
        {
            _momentData = momentData;
            _moveDuration = moveDuration;
            _startOffset = startOffset;
        }

        public void CreateGuest(Transform position=null)
        {
            if (position == null)
            {
                guestPresenterObject = Object.Instantiate(_momentData.Prefab);
            }
            else
            {
                Vector3 startPosition = position.position + _startOffset;
                guestPresenterObject = Object.Instantiate(_momentData.Prefab, startPosition, Quaternion.identity);
                GuestPresenter = guestPresenterObject.GetComponent<GuestPresenter>();
                Guest = new Guest(_momentData);
                
                GuestPresenter.StartCoroutine(MoveToPosition(position.position));
            }
        }
        public void SetEmotion(){
            GuestPresenter.SetEmotion(_momentData.Emotion);
        }
        public void ResetMachine(){
            GuestPresenter.OnClicked=null;
            GuestPresenter=null;
            Guest=null;
            GameObject.Destroy(guestPresenterObject);
        }

        private IEnumerator MoveToPosition(Vector3 targetPosition)
        {
            float elapsedTime = 0f;
            Vector3 startingPosition = guestPresenterObject.transform.position;
            
            while (elapsedTime < _moveDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / _moveDuration;
                
                t = 1 - Mathf.Pow(1 - t, 2);
                
                guestPresenterObject.transform.position = Vector3.Lerp(startingPosition, targetPosition, t);
                yield return null;
            }
            
            // Фиксируем конечную позицию
            guestPresenterObject.transform.position = targetPosition;
        }
    }
}