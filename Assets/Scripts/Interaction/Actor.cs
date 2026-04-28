using UnityEngine;
using System.Collections.Generic;

public class Actor : MonoBehaviour, IInteractable
{
    [SerializeField] private List<InteractiveModule> modules;
    [SerializeField] public bool needsQuest = true;

    public void Interact()
    {
        Debug.Log("Actor: попытка взаимодействия");

        if (needsQuest && (QuestManager.Instance == null || !QuestManager.Instance.isQuestActive))
        {
            Debug.Log("Я не буду это чинить просто так.");
            return;
        }

        foreach (var module in modules)
        {
            if (module != null && module.IsActive)
            {
                Debug.Log($"Actor: выполняется модуль {module.GetType().Name}");
                module.Interact();
                return;
            }
        }

        Debug.LogWarning("Actor: активный модуль не найден!");
    }
}