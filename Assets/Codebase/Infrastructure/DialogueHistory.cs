using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

namespace Codebase.Infrastructure
{
    /// <summary>
    /// Хранит и отображает историю всех диалоговых реплик.
    /// </summary>
    public class DialogueHistory : MonoBehaviour
    {
        public static DialogueHistory Instance { get; private set; }

        [Header("UI Elements")]
        [SerializeField] private GameObject historyPanel;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Transform contentParent;
        [SerializeField] private TextMeshProUGUI historyPrefab;

        [Header("Input")]
        [SerializeField] private InputActionReference showHistoryAction;

        private List<string> historyLines = new List<string>();
        private bool isPanelOpen = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (historyPanel != null)
                historyPanel.SetActive(false);

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.OnDialogueLineShown += AddLine;
            }
            else
            {
                Debug.LogWarning("DialogueHistory: DialogueManager.Instance не найден. " +
                                 "Добавляйте реплики вручную через DialogueHistory.AddLine()");
            }
        }

        private void OnDestroy()
        {
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.OnDialogueLineShown -= AddLine;
            if (Instance == this)
                Instance = null;
        }

        private void OnEnable()
        {
            if (showHistoryAction != null)
                showHistoryAction.action.performed += OnToggleHistory;
        }

        private void OnDisable()
        {
            if (showHistoryAction != null)
                showHistoryAction.action.performed -= OnToggleHistory;
        }

        private void OnToggleHistory(InputAction.CallbackContext context)
        {
            ToggleHistoryPanel();
        }

        /// <summary>
        /// Добавляет строку в историю. Если панель открыта, обновляет UI.
        /// </summary>
        public void AddLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return;

            historyLines.Add(line);
            
            if (isPanelOpen)
                RefreshUI();
        }

        /// <summary>
        /// Очищает всю историю.
        /// </summary>
        public void ClearHistory()
        {
            Debug.Log("[DialogueHistory] Очистка истории");
            historyLines.Clear();
            RefreshUI();
        }

        /// <summary>
        /// Открывает или закрывает панель истории.
        /// </summary>
        public void ToggleHistoryPanel()
        {
            if (historyPanel == null)
            {
                Debug.LogError("DialogueHistory: historyPanel не назначен!");
                return;
            }

            isPanelOpen = !historyPanel.activeSelf;
            historyPanel.SetActive(isPanelOpen);

            if (isPanelOpen)
            {
                RefreshUI();
                if (scrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    scrollRect.verticalNormalizedPosition = 0f;
                }
            }
        }

        /// <summary>
        /// Обновляет отображение списка: пересоздаёт все элементы из historyLines.
        /// </summary>
        private void RefreshUI()
        {
            if (contentParent == null || historyPrefab == null)
            {
                Debug.LogError("DialogueHistory: contentParent или historyPrefab не назначены!");
                return;
            }

            foreach (Transform child in contentParent)
                Destroy(child.gameObject);

            foreach (string line in historyLines)
            {
                TextMeshProUGUI item = Instantiate(historyPrefab, contentParent);
                item.text = line;
            }

            if (scrollRect != null)
                Canvas.ForceUpdateCanvases();
        }
    }
}