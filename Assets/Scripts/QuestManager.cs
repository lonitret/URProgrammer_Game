using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Текущий квест")]
    public string currentQuestDescription = "Нет активных задач";
    public bool isQuestActive = false;

    private NPCQuestGiver currentGiver;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI questTextUI;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        UpdateQuestUI();
    }
    public void AcceptQuest(string description, NPCQuestGiver giver)
    {
        isQuestActive = true;
        currentGiver = giver;
        currentQuestDescription = description;
        UpdateQuestUI();
    }

    public void CompleteQuest(int repReward, float angerReduction)
    {
        if (!isQuestActive) return;

        isQuestActive = false;

        if (currentGiver != null)
        {
            currentGiver.MarkAsCompleted();
            currentGiver = null;
        }

        currentQuestDescription = "Нет активных задач";
        UpdateQuestUI();

        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ChangeReputation(repReward);
            StatsManager.Instance.ChangeAnger(-angerReduction);
        }
    }

    private void UpdateQuestUI()
    {
        if (questTextUI != null)
            questTextUI.text = currentQuestDescription;
    }
}