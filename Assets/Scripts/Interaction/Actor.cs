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
        bool permanentDone = QuestManager.Instance != null
            && QuestManager.Instance.isCoffeeMachineRepaired
            && canInteractAfterCoffeeUnlock;
        bool isCurrentQuestTarget = QuestManager.Instance != null && QuestManager.Instance.IsCurrentQuestTarget(this);
        bool taskDone = QuestManager.Instance != null && QuestManager.Instance.isTaskCompleted && isCurrentQuestTarget;

        if (needsQuest && !permanentDone && !isCurrentQuestTarget)
        {
            if (QuestManager.Instance == null || !QuestManager.Instance.isQuestActive)
            {
                Debug.Log("Я не буду это чинить просто так.");
                return;
            }

            Debug.Log("Это не цель текущего задания.");
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
        bool permanentDone = QuestManager.Instance != null
            && QuestManager.Instance.isCoffeeMachineRepaired
            && canInteractAfterCoffeeUnlock;
        bool isCurrentQuestTarget = QuestManager.Instance != null && QuestManager.Instance.IsCurrentQuestTarget(this);
        bool taskDone = QuestManager.Instance != null && QuestManager.Instance.isTaskCompleted && isCurrentQuestTarget;

        if (taskDone) return "[E] Задание выполнено";
        if (permanentDone) return "[E] Взять кофе";
        if (!needsQuest) return "[E] Взаимодействовать";

        return "[E] Починить";
    }
}
