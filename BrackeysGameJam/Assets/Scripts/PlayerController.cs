using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float laneDistance = 3f; // distance between lanes
    public float laneSwitchSpeed = 10f;
    public float jumpForce = 7f;
    public LayerMask groundLayer;

    private CharacterController controller;
    private Vector3 moveDirection;
    private int currentLane = 0; // -1 = left, 0 = middle, 1 = right
    private float gravity = -20f;
    private float verticalVelocity;

    [Header("Lives")]
    public int lives = 3; // lose legs mechanic later

    // Input system actions
    private PlayerInputActions inputActions;
    

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.MoveLeft.performed += ctx => ChangeLane(-1);
        inputActions.Player.MoveRight.performed += ctx => ChangeLane(1);
        inputActions.Player.Jump.performed += ctx => Jump();
    }

    void OnDisable()
    {
        inputActions.Player.MoveLeft.performed -= ctx => ChangeLane(-1);
        inputActions.Player.MoveRight.performed -= ctx => ChangeLane(1);
        inputActions.Player.Jump.performed -= ctx => Jump();
        inputActions.Player.Disable();
    }

    void Update()
    {
        // Constant forward movement
        moveDirection.z = forwardSpeed;

        // Gravity + jumping
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = -2f; // small downward force
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;

        // Smooth lane switching
        Vector3 targetPosition = transform.position.z * Vector3.forward + Vector3.up * transform.position.y;
        if (currentLane == -1) targetPosition += Vector3.left * laneDistance;
        else if (currentLane == 1) targetPosition += Vector3.right * laneDistance;

        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * laneSwitchSpeed * Time.deltaTime;

        if (moveDir.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);

        controller.Move(moveDirection * Time.deltaTime);
    }

    void ChangeLane(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, -1, 1);
    }

    void Jump()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    public void TakeDamage()
    {
        lives--;
        Debug.Log("Lives left: " + lives);

        if (lives <= 0)
        {
            Debug.Log("Game Over!");
            // TODO: trigger game over state
        }
    }
}
