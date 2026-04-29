using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Codebase.UI
{
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }

    public class NotificationManager : MonoBehaviour
    {
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private Transform notificationContainer;
        [SerializeField] private float displayDuration = 3f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        private static NotificationManager _instance;
        public static NotificationManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        public void ShowNotification(string message, NotificationType type = NotificationType.Info)
        {
            if (notificationPrefab == null || notificationContainer == null)
            {
                Debug.LogError("NotificationManager: префаб или контейнер не назначены!");
                return;
            }

            GameObject notifGO = Instantiate(notificationPrefab, notificationContainer);
            TextMeshProUGUI tmp = notifGO.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = message;
            else
            {
                Text legacy = notifGO.GetComponentInChildren<Text>();
                if (legacy != null) legacy.text = message;
            }

            Image bg = notifGO.GetComponent<Image>();
            if (bg != null)
            {
                switch (type)
                {
                    case NotificationType.Success:
                        bg.color = new Color(0.2f, 0.8f, 0.2f, 0.9f);
                        break;
                    case NotificationType.Warning:
                        bg.color = new Color(0.9f, 0.6f, 0.1f, 0.9f);
                        break;
                    case NotificationType.Error:
                        bg.color = new Color(0.9f, 0.2f, 0.2f, 0.9f);
                        break;
                    default:
                        bg.color = new Color(0.2f, 0.4f, 0.8f, 0.9f);
                        break;
                }
            }

            notifGO.AddComponent<NotificationAutoDestroy>().Initialize(displayDuration, fadeOutDuration);
        }
    }

    // Вспомогательный компонент для автоматического удаления уведомления
    public class NotificationAutoDestroy : MonoBehaviour
    {
        private float _delay;
        private float _fadeDuration;
        private CanvasGroup _canvasGroup;

        public void Initialize(float delay, float fadeDuration)
        {
            _delay = delay;
            _fadeDuration = fadeDuration;
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            StartCoroutine(AutoDestroyRoutine());
        }

        private IEnumerator AutoDestroyRoutine()
        {
            yield return new WaitForSeconds(_delay);
            float elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = 1f - (elapsed / _fadeDuration);
                if (_canvasGroup != null) _canvasGroup.alpha = alpha;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}