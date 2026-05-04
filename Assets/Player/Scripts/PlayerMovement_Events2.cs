using System;
using UnityEngine;
using UnityEngine.InputSystem;

// This script's jump functionalities don't work.
// The jump is written out in psuedo code.
// I will likely get back to this.

public class PlayerMovement_Events2 : MonoBehaviour
{
    // Input Actions
    private ProjectActions actionSystem;
    private InputAction move;
    private InputAction jump;
    private InputAction dash;

    // RigidBody2D Check
    private bool UseRB = false;
    private Rigidbody2D rb;

    // Movement
    private int direction;
    [SerializeField] float movementSpeed = 6f;
    [SerializeField] float movementAcceleration = 6f;
    private float currentSpeed = 0f;
    [SerializeField] private float minSpeed = 0.05f;

    // Jump
    [SerializeField] float JumpForce = 10f;
    // Ground
    [SerializeField] private Transform feet;
    [SerializeField] private float GroundRaycastDistance = 0.1f;
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] private bool GroundedRay = false;
    private bool JumpHeld = false;
    private bool Jumping = false;
    private bool JumpPressed = false;
    private bool JumpReleased = false;
    // Coyote Time
    private float CoyoteTimer;
    [SerializeField] float CoyoteTime = 0.3f;
    // Jump Buffering
    private float JumpBufferTimer;
    [SerializeField] float JumpBufferTime = 0.2f;
    // Fall feel
    // private bool JumpFallFeelOnce = false;
    [SerializeField] float JumpFeelCut = 0.5f;
    [SerializeField] float TerminalSpeed = 15f;
    [SerializeField] float FallMultiplier = 2.5f;
    [SerializeField] float LowJumpMult = 2f;


    private void Awake()
    {
        actionSystem = new ProjectActions();
    }

    private void OnEnable()
    {
        move = actionSystem.Player.Move;
        jump = actionSystem.Player.Jump;
        dash = actionSystem.Player.Dash;

        jump.performed += PressJump;
        jump.canceled += ReleaseJump;

        move.Enable();
        jump.Enable();
        dash.Enable();
    }
    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        dash.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        direction = 1;

        if (TryGetComponent(out rb)) { UseRB = true; }

        rb = GetComponent<Rigidbody2D>();
        JumpHeld = false;
        CoyoteTimer = 0;
        JumpBufferTimer = 0;
        Jumping = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // If we never found the RigidBody2D then we can't do any movement.
        if (!UseRB) return;

        Move();
        HeavyFall();
    }

    private void Update()
    {
        JumpChecks();
    }

    private void JumpChecks()
    {
        bool IsGrounded = isGrounded();

        if (IsGrounded)
        {
            CoyoteTimer = CoyoteTime;
            Jumping = false;
        }
        else
        {
            CoyoteTimer -= Time.deltaTime;
        }

        if (JumpPressed)
        {
            JumpBufferTimer = JumpBufferTime;
            JumpPressed = false;
        }
        else
        {
            JumpBufferTimer -= Time.deltaTime;
        }

        if (JumpBufferTimer > 0f && CoyoteTimer > 0f && !Jumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce); // Adjust the jump force as needed
            Jumping = true; // Set jump input as active
            CoyoteTimer = 0f; // Reset coyote time counter after jumping
            JumpBufferTimer = 0f; // Reset jump buffer counter after jumping
        }

        if (JumpReleased && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * JumpFeelCut); // Reduce upward velocity for variable jump height
            JumpReleased = false; // Reset jump release input
        }

        JumpReleased = false; // Reset jump release input at the end of the frame to prevent unintended behavior
    }

    private void Move()
    {
        // 0 means there's no movement whereas -1 and 1 means there is movement in a direction.
        float movement = move.ReadValue<float>();
        SetDirection(movement);

        float speed_ref = 0f;

        if (movement != 0)
            // This happens when we move
            currentSpeed = Mathf.SmoothDamp(currentSpeed, movementSpeed, ref speed_ref, movementAcceleration * Time.fixedDeltaTime);

        else
            // This happens when we let go of move.
            currentSpeed = Mathf.SmoothDamp(currentSpeed, 0, ref speed_ref, movementAcceleration * Time.fixedDeltaTime);

        // As the speed approaches zero on slowdown, set it to zero eventually.
        if (currentSpeed <= minSpeed) currentSpeed = 0f;

        // Translate towards the direction we are moving (or the direction we were previously moving towards) at the assigned speed.
        rb.linearVelocity = new Vector2(direction * currentSpeed, rb.linearVelocity.y);
    }

    private void SetDirection(float movement)
    {
        // If the input is 0 that means we are not moving so keep the direction as is.
        // If the input is 1 or -1 then we are moving and we want to change the way the player sprite is facing.
        float movementDir = RoundDirection(movement);

        // The direction is either set to the key that the player is currently holding or to the last key the player held.
        direction = (movementDir != 0) ? (int)movementDir : direction;
    }

    private int RoundDirection(float input)
    {
        // Although I am expecting the movement values to be -1, 0, or 1, this ensures that the behaviour of the sprite is as expected if the values span decimal numbers from -1 to 1.
        return (input > 0) ? (int)Math.Ceiling(input) : (input < 0) ? (int)Math.Floor(input) : 0;
    }

    // This function will be called whenever space bar is pressed.
    private void PressJump(InputAction.CallbackContext action)
    {
        if (action.performed)
        {
            JumpPressed = true;
            JumpHeld = true;
        }
    }
    // This function will be called whenever space bar is released.
    private void ReleaseJump(InputAction.CallbackContext action)
    {
        if (action.performed)
        {
            JumpReleased = true;
            JumpHeld = false;
        }
    }

    private bool isGrounded()
    {
        if (GroundedRay) Debug.DrawRay(feet.position,  GroundRaycastDistance * Vector2.down, Color.red);
        return Physics2D.Raycast(feet.position, Vector2.down, GroundRaycastDistance, GroundLayer);
    }

    private void HeavyFall()
    {
        // If linear velocity in the y direction is negative, add some extra gravity.
        // Make sure it is clamped to a terminal velocity.
        if (rb.linearVelocityY < 0f)
        {
            rb.linearVelocity += Physics2D.gravity.y * (FallMultiplier - 1f) * Time.fixedDeltaTime * Vector2.up;
        }
        else if (rb.linearVelocityY > 0 && JumpHeld)
        {
            rb.linearVelocity += Physics2D.gravity.y * (LowJumpMult - 1) * Time.fixedDeltaTime * Vector2.up;
        }

        if (rb.linearVelocityY < -TerminalSpeed) rb.linearVelocityY = -TerminalSpeed;
    }
}
