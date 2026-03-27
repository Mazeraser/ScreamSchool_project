using UnityEngine;
using Codebase.Mechanics.Data;

namespace Codebase.Mechanics.GuestSystem
{
    public class GuestBuilder : IResetable
    {
        private MomentData _momentData;
        public GuestPresenter GuestPresenter{ get; private set; }
        public Guest Guest{ get; private set; }
        private GameObject guestPresenterObject;

        public GuestBuilder(MomentData momentData)
        {
            _momentData = momentData;
        }

        public void CreateGuest(Transform position=null)
        {
            guestPresenterObject = Object.Instantiate(_momentData.Prefab, position);
            GuestPresenter = guestPresenterObject.GetComponent<GuestPresenter>();
            Guest=new Guest(_momentData);
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
    }
}