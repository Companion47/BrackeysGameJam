using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float laneDistance = 3f; // distance between lanes
    public float laneSwitchSpeed = 10f;
    public float jumpForce = 7f;
    public float gravity = -20f;

    private CharacterController controller;
    private float verticalVelocity;
    private int currentLane = 0; // -1 = left, 0 = middle, 1 = right

    [Header("Lives")]
    public int lives = 3;

    // Input system
    private PlayerInputActions inputActions;
    private Vector2 moveInput;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();

        // Read Vector2 input from "Move action"
        inputActions.Player.Moveaction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Moveaction.canceled += ctx => moveInput = Vector2.zero;

        // Jump
        inputActions.Player.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        inputActions.Player.Moveaction.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Moveaction.canceled -= ctx => moveInput = Vector2.zero;
        inputActions.Player.Jump.performed -= OnJump;

        inputActions.Player.Disable();
    }

    void Update()
    {
        // Check lane input (only once per key press)
        if (moveInput.x < 0) // A / Left
        {
            ChangeLane(-1);
            moveInput = Vector2.zero; // consume input so it doesn’t spam
        }
        else if (moveInput.x > 0) // D / Right
        {
            ChangeLane(1);
            moveInput = Vector2.zero;
        }

        // Forward move
        Vector3 move = Vector3.forward * forwardSpeed * Time.deltaTime;

        // Gravity
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        move.y = verticalVelocity * Time.deltaTime;

        // Smooth lane switching
        float targetX = currentLane * laneDistance;
        float newX = Mathf.Lerp(transform.position.x, targetX, laneSwitchSpeed * Time.deltaTime);
        move.x = newX - transform.position.x;

        // Apply move
        controller.Move(move);
    }

    void ChangeLane(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, -1, 1);
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        if (controller.isGrounded)
            verticalVelocity = jumpForce;
    }

    public void TakeDamage()
    {
        lives--;

        // Change color depending on remaining lives
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            if (lives == 2) rend.material.color = Color.yellow;
            else if (lives == 1) rend.material.color = new Color(1f, 0.5f, 0f); // orange
            else if (lives <= 0) rend.material.color = Color.red;
        }

        Debug.Log("Lives left: " + lives);

        if (lives <= 0)
        {
            Debug.Log("Game Over!");
            // TODO: trigger game over state (stop movement / show UI)
        }
    }
}
