using UnityEngine;
using Codebase.Mechanics.Data;
using System;

namespace Codebase.Mechanics.GuestSystem
{
    public class GuestPresenter : MonoBehaviour
    {
        //TODO: Прикрутить Один
        [SerializeField] private Sprite[] emotions = new Sprite[4];
        public Action OnClicked;

        public void SetEmotion(GuestEmotions emotion){
            GetComponent<SpriteRenderer>().sprite = emotions[(int)emotion];
        }
        private void OnMouseDown(){
            OnClicked?.Invoke();
        }
    }
}