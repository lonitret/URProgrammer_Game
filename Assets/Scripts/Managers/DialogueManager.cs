using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    [Header("UI элементы диалога")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [Header("UI выбора квеста")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;
    private Queue<string> sentences;
    private System.Action onDialogueComplete;
    private System.Action onDialogueDeclined;
    private bool hasChoice;
    private int npcVoiceLetterCounter;
    public bool IsWaitingForChoice => choicePanel != null && choicePanel.activeSelf;
    public bool IsDialogueActive => dialoguePanel != null && dialoguePanel.activeSelf;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        sentences = new Queue<string>();
        dialoguePanel.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
        if (acceptButton != null) acceptButton.onClick.AddListener(SelectAccept);
        if (declineButton != null) declineButton.onClick.AddListener(SelectDecline);
    }
    public void StartDialogue(Dialogue dialogue, System.Action onComplete = null, System.Action onDeclined = null)
    {
        dialoguePanel.SetActive(true);
        RefreshCursorState();
        if (choicePanel != null) choicePanel.SetActive(false);
        nameText.text = dialogue.npcName;
        onDialogueComplete = onComplete;
        onDialogueDeclined = onDeclined;
        hasChoice = onComplete != null && onDeclined != null;
        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        if (IsWaitingForChoice) return;
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        npcVoiceLetterCounter = 0;
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            npcVoiceLetterCounter++;

            if (!GameManager.isGameOver && !char.IsWhiteSpace(letter) && npcVoiceLetterCounter % 4 == 0)
            {
                AudioManager.Instance?.PlayVoice();
            }

            yield return null;
        }
        if (sentences.Count == 0)
        {
            if (hasChoice)
            {
                ShowChoices();
            }
        }
    }
    private void ShowChoices()
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
            acceptButton.Select();
        }
        else
        {
            SelectAccept();
        }
    }
    public void SelectAccept()
    {
        AudioManager.Instance?.PlaySFX(SoundType.UIClick);
        choicePanel.SetActive(false);
        dialoguePanel.SetActive(false);
        RefreshCursorState();
        onDialogueComplete?.Invoke();
    }
    public void SelectDecline()
    {
        AudioManager.Instance?.PlaySFX(SoundType.UIClick);
        AudioManager.Instance?.PlaySFX(SoundType.ErrorDecline);
        choicePanel.SetActive(false);
        dialoguePanel.SetActive(false);
        RefreshCursorState();
        onDialogueDeclined?.Invoke();
    }
    private void RefreshCursorState()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RefreshCursorState();
        }
    }
    public void ForceCloseDialogue()
    {
        StopAllCoroutines();
        sentences.Clear();
        onDialogueComplete = null;
        onDialogueDeclined = null;
        hasChoice = false;
        if (choicePanel != null) choicePanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        RefreshCursorState();
    }
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        RefreshCursorState();
        if (onDialogueComplete != null && !IsWaitingForChoice)
        {
            onDialogueComplete.Invoke();
        }
    }
}
