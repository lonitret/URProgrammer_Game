using System;
using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 1.5f;

    public static event Action<Transform, string> OnInteractableFound;

    private IInteractable currentInteractable;
    private Transform currentTarget;

    private void OnEnable()
    {
        QuestManager.OnQuestStateChanged += HandleQuestStateChanged;
    }

    private void OnDisable()
    {
        QuestManager.OnQuestStateChanged -= HandleQuestStateChanged;
    }

    private void Update()
    {
        DetectInteractable();
    }

    public void TryInteract()
    {
        if (currentInteractable != null)
        {
            if (!CanUseCurrentInteractable())
            {
                currentInteractable = null;
                currentTarget = null;
                OnInteractableFound?.Invoke(null, "");
                return;
            }

            Debug.Log("Взаимодействие с: " + currentTarget.name);
            currentInteractable.Interact();
        }
        else
        {
            Debug.Log("Не с чем взаимодействовать.");
        }
    }

    private bool CanUseCurrentInteractable()
    {
        if (currentTarget == null) return false;

        Actor actor = currentTarget.GetComponent<Actor>();
        if (actor == null) actor = currentTarget.GetComponentInParent<Actor>();
        if (actor != null && !CanInteractWithActor(actor)) return false;

        NPCQuestGiver questGiver = currentTarget.GetComponent<NPCQuestGiver>();
        if (questGiver == null) questGiver = currentTarget.GetComponentInParent<NPCQuestGiver>();
        if (questGiver != null && !CanInteractWithQuestGiver(questGiver)) return false;

        return true;
    }

    private void DetectInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius);
        IInteractable foundInteractable = null;
        Transform foundTarget = null;

        foreach (var collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable == null)
            {
                interactable = collider.GetComponentInParent<IInteractable>();
            }

            if (interactable != null)
            {
                Actor actor = collider.GetComponent<Actor>();
                if (actor == null) actor = collider.GetComponentInParent<Actor>();
                if (actor != null && !CanInteractWithActor(actor)) continue;

                NPCQuestGiver questGiver = collider.GetComponent<NPCQuestGiver>();
                if (questGiver == null) questGiver = collider.GetComponentInParent<NPCQuestGiver>();
                if (questGiver != null && !CanInteractWithQuestGiver(questGiver)) continue;

                foundInteractable = interactable;
                foundTarget = ((MonoBehaviour)interactable).transform;
                break;
            }
        }

        if (currentTarget != foundTarget)
        {
            currentTarget = foundTarget;
            currentInteractable = foundInteractable;

            NotifyCurrentTargetChanged();
        }
    }

    private bool CanInteractWithActor(Actor actor)
    {
        if (!actor.needsQuest) return true;

        if (QuestManager.Instance != null && QuestManager.Instance.isQuestActive)
        {
            return QuestManager.Instance.IsCurrentQuestTarget(actor);
        }

        bool permanentDone = QuestManager.Instance != null
            && QuestManager.Instance.isCoffeeMachineRepaired
            && actor.CanInteractAfterCoffeeUnlock;
        return permanentDone;
    }

    private bool CanInteractWithQuestGiver(NPCQuestGiver questGiver)
    {
        if (QuestManager.Instance == null || !QuestManager.Instance.isQuestActive) return true;

        return QuestManager.Instance.IsCurrentQuestGiver(questGiver);
    }

    private void HandleQuestStateChanged(QuestManager.QuestUiState state)
    {
        currentTarget = null;
        currentInteractable = null;
        OnInteractableFound?.Invoke(null, "");
        DetectInteractable();
    }

    private void NotifyCurrentTargetChanged()
    {
        string hintText = "";
        if (currentTarget != null)
        {
            Actor actor = currentTarget.GetComponent<Actor>();
            if (actor == null) actor = currentTarget.GetComponentInParent<Actor>();
            if (actor != null)
            {
                hintText = actor.GetInteractionText();
            }
            else
            {
                hintText = GetFallbackInteractionText(currentTarget);
            }
        }

        OnInteractableFound?.Invoke(currentTarget, hintText);
    }

    private string GetFallbackInteractionText(Transform target)
    {
        MonoBehaviour[] behaviours = target.GetComponentsInParent<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour == null) continue;

            System.Reflection.MethodInfo method = behaviour.GetType().GetMethod(
                "GetInteractionText",
                Type.EmptyTypes);

            if (method != null && method.ReturnType == typeof(string))
            {
                return method.Invoke(behaviour, null) as string;
            }
        }

        return "[E] Взаимодействовать";
    }
}
