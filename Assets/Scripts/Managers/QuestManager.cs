using System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public struct QuestUiState
    {
        public bool HasActiveQuest;
        public string Title;
        public string GiverName;
        public string CurrentStep;
        public string Status;
        public int CompletedSteps;
        public int TotalSteps;
    }

    public static QuestManager Instance;

    [Header("Текущий квест")]
    public string currentQuestDescription = "Нет активных задач";
    public bool isQuestActive = false;
    public bool isTaskCompleted = false;
    public bool isCoffeeMachineRepaired = false;

    private NPCQuestGiver currentGiver;
    private Actor currentTargetActor;
    private int pendingRep;
    private float pendingAnger;
    private bool currentQuestUnlocksCoffee;
    private QuestUiState currentUiState;

    public static event Action<string> OnQuestUpdated;
    public static event Action<QuestUiState> OnQuestStateChanged;
    public static event Action OnQuestCompleted;

    public bool IsCurrentQuestGiver(NPCQuestGiver giver)
    {
        return currentGiver == giver;
    }

    public bool IsCurrentQuestTarget(Actor actor)
    {
        return currentTargetActor == actor;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        PublishQuestState();
    }

    public void AcceptQuest(
        string description,
        NPCQuestGiver giver,
        bool unlocksCoffeeAfterCompletion = false,
        Actor targetActor = null)
    {
        isQuestActive = true;
        isTaskCompleted = false;
        currentGiver = giver;
        currentTargetActor = targetActor;
        currentQuestUnlocksCoffee = unlocksCoffeeAfterCompletion;
        currentQuestDescription = description;

        currentUiState = new QuestUiState
        {
            HasActiveQuest = true,
            Title = description,
            GiverName = giver != null ? giver.NpcName : "NPC",
            CurrentStep = "Выполни задание",
            Status = "В процессе",
            CompletedSteps = 0,
            TotalSteps = 1
        };

        PublishQuestState();
    }

    public void MarkTaskAsDone(int rep, float anger)
    {
        if (isTaskCompleted) return;

        isTaskCompleted = true;
        pendingRep = rep;
        pendingAnger = anger;
        currentQuestDescription = currentGiver != null
            ? $"Вернись к {currentGiver.NpcName}"
            : "Вернись к NPC";

        currentUiState.HasActiveQuest = true;
        currentUiState.CurrentStep = currentQuestDescription;
        currentUiState.Status = "Можно сдавать";
        currentUiState.CompletedSteps = 1;
        currentUiState.TotalSteps = 1;

        PublishQuestState();
    }

    public void CompleteQuestImmediately(int rep, float anger)
    {
        MarkTaskAsDone(rep, anger);
        GiveRewardAndFinish();
    }

    public void GiveRewardAndFinish()
    {
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ChangeReputation(pendingRep);
            StatsManager.Instance.ChangeAnger(-pendingAnger);
        }

        if (currentQuestUnlocksCoffee)
        {
            isCoffeeMachineRepaired = true;
        }

        isQuestActive = false;
        isTaskCompleted = false;
        currentQuestDescription = "Нет активных задач";
        if (currentGiver != null) currentGiver.MarkAsCompleted();
        currentGiver = null;
        currentTargetActor = null;

        currentUiState = new QuestUiState
        {
            HasActiveQuest = false,
            Title = "Нет активных задач",
            GiverName = "",
            CurrentStep = "Поговори с коллегами, чтобы получить новую задачу",
            Status = "Свободно",
            CompletedSteps = 0,
            TotalSteps = 0
        };

        PublishQuestState();
    }

    public void NotifyQuestTurnedIn()
    {
        OnQuestCompleted?.Invoke();
    }

    private void PublishQuestState()
    {
        if (string.IsNullOrWhiteSpace(currentUiState.Title))
        {
            currentUiState = new QuestUiState
            {
                HasActiveQuest = false,
                Title = currentQuestDescription,
                GiverName = "",
                CurrentStep = "Поговори с коллегами, чтобы получить задачу",
                Status = "Свободно",
                CompletedSteps = 0,
                TotalSteps = 0
            };
        }

        OnQuestUpdated?.Invoke(currentQuestDescription);
        OnQuestStateChanged?.Invoke(currentUiState);
    }
}
