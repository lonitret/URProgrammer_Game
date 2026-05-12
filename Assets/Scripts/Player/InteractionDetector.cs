using UnityEngine;
using System;

public class InteractionDetector : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 1.5f;

    public static event Action<Transform, string> OnInteractableFound;

    private IInteractable currentInteractable;
    private Transform currentTarget;

    private void Update()
    {
        DetectInteractable();
    }

    public void TryInteract()
    {
        if (currentInteractable != null)
        {
            Debug.Log("Взаимодействие с: " + currentTarget.name);
            currentInteractable.Interact();
        }
        else
        {
            Debug.Log("Не с чем взаимодействовать.");
        }
    }

    private void DetectInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius);
        IInteractable foundInteractable = null;
        Transform foundTarget = null;

        foreach (var collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Actor actor = collider.GetComponent<Actor>();
                if (actor != null && !CanInteractWithActor(actor)) continue;

                foundInteractable = interactable;
                foundTarget = collider.transform;
                break;
            }
        }

        if (currentTarget != foundTarget)
        {
            currentTarget = foundTarget;
            currentInteractable = foundInteractable;

            string hintText = "";
            if (currentTarget != null)
            {
                var actor = currentTarget.GetComponent<Actor>();
                hintText = (actor != null) ? actor.GetInteractionText() : "[E] Взаимодействовать";
            }

            OnInteractableFound?.Invoke(currentTarget, hintText);
        }
    }

    private bool CanInteractWithActor(Actor actor)
    {
        if (!actor.needsQuest) return true;

        bool permanentDone = QuestManager.Instance != null && QuestManager.Instance.isCoffeeMachineRepaired;
        return permanentDone || (QuestManager.Instance != null && QuestManager.Instance.isQuestActive);
    }
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, interactionRadius);
    //}
}