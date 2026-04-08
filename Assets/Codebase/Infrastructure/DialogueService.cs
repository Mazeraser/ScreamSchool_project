using System;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Codebase.Infrastructure
{
    public class DialogueManager : MonoBehaviour
    {
        //BUG: Сделать заморозку камеры
        //TODO: Добавить имя персонажа в диалоговое окно
        public static DialogueManager Instance { get; private set; }

        [Header("UI Elements")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button continueButton;
        
        [Header("Settings")]
        [SerializeField] private float textSpeed = 0.05f;
        
        private Story currentStory;
        private bool isDialogueActive = false;
        public bool IsDialogueActive => isDialogueActive;
        
        private bool isTyping = false;
        private string currentText;
        private float typingTimer;
        private int currentCharIndex;

        private Action onDialogueFinished;
        private Action onDialogueStarted;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
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
        }
        private void Update()
        {
            if (!isDialogueActive) return;
            
            if (isTyping)
            {
                typingTimer -= Time.deltaTime;
                if (typingTimer <= 0)
                {
                    if (currentCharIndex < currentText.Length)
                    {
                        dialogueText.text += currentText[currentCharIndex];
                        currentCharIndex++;
                        typingTimer = textSpeed;
                    }
                    else
                    {
                        isTyping = false;
                        // Показываем кнопку продолжения, если нет активных выборов
                        if (continueButton != null && currentStory.currentChoices.Count == 0)
                            continueButton.gameObject.SetActive(true);
                    }
                }
            }
        }

        private void ShowDialogueText(string text)
        {
            currentText = text;
            dialogueText.text = "";
            currentCharIndex = 0;
            isTyping = true;
            typingTimer = 0;
        }
        private void EndDialogue()
        {
            isDialogueActive = false;
            
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
        public void StartDialogue(TextAsset inkJSON, Action onFinished = null, Action onStarted = null)
        {
            onDialogueFinished = onFinished;
            onDialogueStarted = onStarted;

            if (inkJSON == null)
            {
                Debug.LogError("DialogueManager: Ink JSON is null!");
                onDialogueFinished?.Invoke();
                return;
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