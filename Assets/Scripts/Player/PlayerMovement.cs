using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private Animator animator;
   
    void Start()
    {
        Debug.Log("Скрипт PlayerMovement запущен на: " + gameObject.name);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        Vector2 movement = inputHandler.moveInput;
        rb.linearVelocity = movement * moveSpeed;
        UpdateAnimation(movement);
        //Debug.Log("Velocity: " +  rb.linearVelocity);
    }

    public void UpdateAnimation(Vector2 move)
    {
        if (animator == null) return;

        bool isWalking = move.sqrMagnitude > 0.01f;
        //animator.SetBool("isWalking", isWalking);
        animator.SetBool("isWalking", true);

        if (isWalking)
        {
            animator.SetFloat("LastInputX", move.x);
            animator.SetFloat("LastInputY", move.y);

            animator.SetFloat("InputX", move.x);
            animator.SetFloat("InputY", move.y);
            
        }
        else
        {
            animator.SetFloat("InputX", 0);
            animator.SetFloat("InputY", 0);
        }
        //Debug.Log($"Аниматор получает: {move}");
    }

}
