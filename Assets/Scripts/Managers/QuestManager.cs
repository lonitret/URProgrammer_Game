using UnityEngine;
using TMPro;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Текущий квест")]
    public string currentQuestDescription = "Нет активных задач";
    public bool isQuestActive = false;
    public bool isTaskCompleted = false;
    public bool isCoffeeMachineRepaired = false;

    private NPCQuestGiver currentGiver;

    private int pendingRep;
    private float pendingAnger;

    public static event Action<string> OnQuestUpdated;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        OnQuestUpdated?.Invoke(currentQuestDescription);
    }

    public void AcceptQuest(string description, NPCQuestGiver giver)
    {
        isQuestActive = true;
        isTaskCompleted = false;
        currentGiver = giver;
        currentQuestDescription = description;
        OnQuestUpdated?.Invoke(currentQuestDescription);
    }

    public void MarkTaskAsDone(int rep, float anger)
    {
        isTaskCompleted = true;
        pendingRep = rep;
        pendingAnger = anger;

        currentQuestDescription = "Вернись к нпс";
        OnQuestUpdated?.Invoke(currentQuestDescription);
    }

    public void GiveRewardAndFinish()
    {
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ChangeReputation(pendingRep);
            StatsManager.Instance.ChangeAnger(-pendingAnger);
        }

        isCoffeeMachineRepaired = true;
        isQuestActive = false;
        isTaskCompleted = false;
        currentQuestDescription = "Нет активных задач";

        if (currentGiver != null) currentGiver.MarkAsCompleted();

        OnQuestUpdated?.Invoke(currentQuestDescription);
    }
}