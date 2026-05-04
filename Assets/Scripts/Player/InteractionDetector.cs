using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 1.5f;

    private IInteractable currentInteractable;
    private InteractionHint currentHint;

    private void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        IInteractable foundInteractable = null;
        InteractionHint foundHint = null;

        foreach (var collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Actor actor = collider.GetComponent<Actor>();
                if (actor != null && !CanInteractWithActor(actor)) continue;

                foundInteractable = interactable;
                foundHint = collider.GetComponentInChildren<InteractionHint>(true);
                break;
            }
        }

        if (currentHint != foundHint)
        {
            if (currentHint != null) currentHint.Hide();
            if (foundHint != null) foundHint.Show();
            currentHint = foundHint;
        }

        currentInteractable = foundInteractable;
    }

    private bool CanInteractWithActor(Actor actor)
    {
        if (!actor.needsQuest) return true;

        return QuestManager.Instance != null && QuestManager.Instance.isQuestActive;
    }

    public void TryInteract()
    {
        if (currentInteractable != null)
        {
            Debug.Log("Объект найден. Взаимодействие");
            currentInteractable.Interact();
        }
        else
        {
            Debug.Log("Интерактивный объект не найден.");
        }
    }
}