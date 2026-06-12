using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour, IInteractable
{
    [SerializeField] private List<InteractiveModule> modules;
    [SerializeField] public bool needsQuest = true;
    [SerializeField] private bool canInteractAfterCoffeeUnlock = true;

    public bool CanInteractAfterCoffeeUnlock => canInteractAfterCoffeeUnlock;

    public void Interact()
    {
        bool hasActiveQuest = QuestManager.Instance != null && QuestManager.Instance.isQuestActive;
        bool isCurrentQuestTarget = QuestManager.Instance != null && QuestManager.Instance.IsCurrentQuestTarget(this);
        bool permanentDone = QuestManager.Instance != null
            && QuestManager.Instance.isCoffeeMachineRepaired
            && canInteractAfterCoffeeUnlock;

        if (needsQuest && hasActiveQuest && !isCurrentQuestTarget)
        {
            Debug.Log("Это не цель текущего задания.");
            return;
        }

        if (needsQuest && !permanentDone && !isCurrentQuestTarget)
        {
            Debug.Log("Я не буду это чинить просто так.");
            return;
        }

        if (modules == null) return;

        foreach (InteractiveModule module in modules)
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
        bool hasActiveQuest = QuestManager.Instance != null && QuestManager.Instance.isQuestActive;
        bool isCurrentQuestTarget = QuestManager.Instance != null && QuestManager.Instance.IsCurrentQuestTarget(this);
        bool taskDone = QuestManager.Instance != null && QuestManager.Instance.isTaskCompleted && isCurrentQuestTarget;
        bool permanentDone = QuestManager.Instance != null
            && QuestManager.Instance.isCoffeeMachineRepaired
            && canInteractAfterCoffeeUnlock;

        if (taskDone) return "[E] Вернуться к NPC";
        if (needsQuest && hasActiveQuest && !isCurrentQuestTarget) return "";
        if (permanentDone) return "[E] Взять кофе";
        if (!needsQuest) return "[E] Взаимодействовать";

        return "[E] Починить объект";
    }
}
