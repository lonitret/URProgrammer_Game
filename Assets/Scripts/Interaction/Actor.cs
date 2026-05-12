using UnityEngine;
using System.Collections.Generic;

public class Actor : MonoBehaviour, IInteractable
{
    [SerializeField] private List<InteractiveModule> modules;
    [SerializeField] public bool needsQuest = true;

    public void Interact()
    {
        bool permanentDone = QuestManager.Instance != null && QuestManager.Instance.isCoffeeMachineRepaired;
        bool taskDone = QuestManager.Instance != null && QuestManager.Instance.isTaskCompleted;

        if (needsQuest && !permanentDone && !taskDone)
        {
            if (QuestManager.Instance == null || !QuestManager.Instance.isQuestActive)
            {
                Debug.Log("Я не буду это чинить просто так.");
                return;
            }
        }

        foreach (var module in modules)
        {
            if (module != null && module.IsActive)
            {
                module.Interact();
                return;
            }
        }
    }

    public string GetInteractionText()
    {
        bool permanentDone = QuestManager.Instance != null && QuestManager.Instance.isCoffeeMachineRepaired;
        bool taskDone = QuestManager.Instance != null && QuestManager.Instance.isTaskCompleted;

        if (permanentDone || taskDone || !needsQuest) return "[E] Взять кофе";

        return "[E] Починить";
    }
}