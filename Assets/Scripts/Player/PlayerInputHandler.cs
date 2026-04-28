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
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("OnInteract вызван, Phase: " + context.phase);

        if (context.performed)
        {
            Debug.Log("Нажата клавиша E");
            interactionDetector?.TryInteract();
        }
    }
}