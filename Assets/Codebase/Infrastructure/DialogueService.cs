using System;
using System.Collections;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem; // New Input System

namespace Codebase.Infrastructure
{
    public class DialogueManager : MonoBehaviour
    {
        //BUG: Сделать заморозку камеры
        public static DialogueManager Instance { get; private set; }

        [Header("UI Elements")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button continueButton;
        
        [Header("Settings")]
        [SerializeField] private float textSpeed = 0.05f;
        
        private Story currentStory;
        private bool isDialogueActive = false;
        public bool IsDialogueActive => isDialogueActive;
        
        private bool isTyping = false;
        private Coroutine typingCoroutine;

        public Action onDialogueFinished;
        private Action onDialogueStarted;
        public event Action<string> OnDialogueLineShown;

        // New Input System action
        private InputAction continueAction;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
                
            if (continueButton != null)
                continueButton.onClick.AddListener(ContinueDialogue);

            // Настройка Input Action
            continueAction = new InputAction("Continue");
            continueAction.AddBinding("<Keyboard>/space");
            continueAction.AddBinding("<Keyboard>/enter");
            continueAction.AddBinding("<Mouse>/leftButton");
            continueAction.performed += SkipTyping;
        }

        private void OnEnable()
        {
            continueAction?.Enable();
        }

        private void OnDisable()
        {
            continueAction?.Disable();
        }

        private void OnDestroy()
        {
            if (continueAction != null)
            {
                continueAction.performed -= SkipTyping;
                continueAction.Dispose();
            }
        }

        private string ParseRichText(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return raw;

            bool boldOpen = false;
            string withBold = System.Text.RegularExpressions.Regex.Replace(raw, @"\(b\)", match =>
            {
                boldOpen = !boldOpen;
                return boldOpen ? "<b>" : "</b>";
            });

            bool italicOpen = false;
            string withItalic = System.Text.RegularExpressions.Regex.Replace(withBold, @"\(c\)", match =>
            {
                italicOpen = !italicOpen;
                return italicOpen ? "<i>" : "</i>";
            });

            return withItalic;
        }

        private IEnumerator TypeText(string fullText)
        {
            isTyping = true;
            dialogueText.text = fullText;
            dialogueText.maxVisibleCharacters = 0;
            
            // Ждём обновления текстового компонента, чтобы получить корректный characterCount
            yield return null;
            dialogueText.ForceMeshUpdate();
            int totalVisibleChars = dialogueText.textInfo.characterCount;
            
            int visibleCount = 0;
            while (visibleCount < totalVisibleChars)
            {
                float timer = textSpeed;
                while (timer > 0f)
                {
                    if (!isTyping) // если был сброс печати (пропуск)
                    {
                        dialogueText.maxVisibleCharacters = totalVisibleChars;
                        isTyping = false;
                        if (continueButton != null && currentStory.currentChoices.Count == 0)
                            continueButton.gameObject.SetActive(true);
                        yield break;
                    }
                    timer -= Time.deltaTime;
                    yield return null;
                }
                visibleCount++;
                dialogueText.maxVisibleCharacters = visibleCount;
            }
            
            isTyping = false;
            if (continueButton != null && currentStory.currentChoices.Count == 0)
                continueButton.gameObject.SetActive(true);
        }

        // Показать текст с эффектом печати
        private void ShowDialogueText(string rawText)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            
            string richText = ParseRichText(rawText);
            OnDialogueLineShown?.Invoke(nameText.text+": "+richText);
            typingCoroutine = StartCoroutine(TypeText(richText));
        }

        // Немедленно показать весь текст (пропуск печати)
        private void SkipTyping(InputAction.CallbackContext context)
        {
            if (typingCoroutine != null)
            {
                isTyping = false;
            }
        }

        private void EndDialogue()
        {
            isDialogueActive = false;
            
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
            
            onDialogueFinished?.Invoke();
            
            onDialogueFinished = null;
            onDialogueStarted = null;
            currentStory = null;
        }

        public void ContinueDialogue()
        {
            if (!isDialogueActive || currentStory == null) return;
            
            // Скрываем кнопку продолжения, пока грузится следующая реплика
            if (continueButton != null)
                continueButton.gameObject.SetActive(false);
            
            if (currentStory.canContinue)
            {
                string text = currentStory.Continue();
                ShowDialogueText(text);
            }
            else
            {
                EndDialogue();
            }
        }

        public void StartDialogue(TextAsset inkJSON, Action onFinished = null, Action onStarted = null, string name="")
        {
            onDialogueFinished = onFinished;
            onDialogueStarted = onStarted;

            if(nameText != null)
                nameText.text = name;

            if (inkJSON == null)
            {
                Debug.LogError("DialogueManager: Ink JSON is null!");
                onDialogueFinished?.Invoke();
                return;
            }
            
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            
            currentStory = new Story(inkJSON.text);
            isDialogueActive = true;

            if (dialoguePanel != null)
                dialoguePanel.SetActive(true);
                
            onDialogueStarted?.Invoke();
            
            ContinueDialogue();
        }
    }
}