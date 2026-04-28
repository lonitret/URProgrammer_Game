using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Текущий квест")]
    public string currentQuestDescription = "Нет активных задач";
    public bool isQuestActive = false;
    public bool isTaskCompleted = false;

    private NPCQuestGiver currentGiver;

    private int pendingRep;
    private float pendingAnger;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI questTextUI;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        currentQuestDescription = "Нет активных задач";
        UpdateQuestUI();
    }

    public void AcceptQuest(string description, NPCQuestGiver giver)
    {
        isQuestActive = true;
        isTaskCompleted = false;
        currentGiver = giver;
        currentQuestDescription = description;
        UpdateQuestUI();
    }

    public void MarkTaskAsDone(int rep, float anger)
    {
        isTaskCompleted = true;
        pendingRep = rep;
        pendingAnger = anger;

        currentQuestDescription = "Вернись к нпс";
        UpdateQuestUI();
    }

    public void GiveRewardAndFinish()
    {
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ChangeReputation(pendingRep);
            StatsManager.Instance.ChangeAnger(-pendingAnger);
        }

        isQuestActive = false;
        isTaskCompleted = false;
        currentQuestDescription = "Нет активных задач";

        if (currentGiver != null) currentGiver.MarkAsCompleted();

        UpdateQuestUI();
    }

    private void UpdateQuestUI()
    {
        if (questTextUI != null)
            questTextUI.text = currentQuestDescription;
    }
}