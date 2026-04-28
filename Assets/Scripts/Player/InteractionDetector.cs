using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 1.5f;
    [SerializeField] private GameObject hintUI;

    private IInteractable currentInteractable;

    private void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position,
            interactionRadius
        );

        currentInteractable = null;

        foreach (var collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                Actor actor = collider.GetComponent<Actor>();
                if (actor != null && !CanInteractWithActor(actor))
                {
                    continue;
                }

                currentInteractable = interactable;
                break;
            }
        }

        if (hintUI != null)
        {
            hintUI.SetActive(currentInteractable != null);
        }
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