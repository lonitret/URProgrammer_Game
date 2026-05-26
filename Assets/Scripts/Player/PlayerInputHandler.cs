using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 moveInput;
    private InteractionDetector interactionDetector;

    void Awake()
    {
        interactionDetector = GetComponent<InteractionDetector>();
        Debug.Log("InteractionDetector найден: " + (interactionDetector != null));
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsWaitingForChoice)
            {
                return;
            }

            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                DialogueManager.Instance.DisplayNextSentence();
                return;
            }

            interactionDetector?.TryInteract();
        }
    }

    public void OnPause(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (GameManager.isPaused)
                GameManager.Instance.Resume();
            else
                GameManager.Instance.Pause();
        }
    }

    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Вызвана команда использования предмета");

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.UseSelectedItem();
            }
        }
    }
}