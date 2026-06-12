using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    private float speedMultiplier = 1f;
    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private Animator animator;
    private Coroutine speedBoostCoroutine;
    private bool isMovementLocked;

    private void OnEnable()
    {
        BlockModule.OnMinigameVisibilityChanged += SetMovementLocked;
    }

    private void OnDisable()
    {
        BlockModule.OnMinigameVisibilityChanged -= SetMovementLocked;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }
    void FixedUpdate()
    {
        if (rb == null || inputHandler == null) return;

        if (isMovementLocked)
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimation(Vector2.zero);
            return;
        }

        Vector2 movement = inputHandler.moveInput;
        rb.linearVelocity = movement.normalized * (moveSpeed * speedMultiplier);
        UpdateAnimation(movement);
    }
    public void UpdateAnimation(Vector2 move)
    {
        if (animator == null) return;
        bool isWalking = move.sqrMagnitude > 0.01f;
        animator.SetBool("isWalking", isWalking);
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
    }
    public void ApplyTemporarySpeedBoost(float multiplier, float duration)
    {
        if (multiplier <= 1f || duration <= 0f) return;
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }
        speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }
    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
        speedBoostCoroutine = null;
    }

    private void SetMovementLocked(bool isLocked)
    {
        isMovementLocked = isLocked;

        if (isLocked && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimation(Vector2.zero);
        }
    }
}
