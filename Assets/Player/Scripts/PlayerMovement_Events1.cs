using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement_Events1 : MonoBehaviour
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
    //private bool JumpHeld = false;
    //private bool Jumping = false;
    //private bool JumpPressed = false;
    //private bool JumpReleased = false;
    // Coyote Time
    private float CoyoteTimer;
    [SerializeField] float CoyoteTime = 0.3f;
    private bool CoyoteTimeBool = true;
    // Jump Buffering
    private float JumpBufferTimer;
    [SerializeField] float JumpBufferTime = 0.2f;
    // private bool JumpBufferBool = false;
    // Fall feel
    private bool JumpFallFeelOnce = false;
    [SerializeField] float JumpFeelCut = 0.5f;
    [SerializeField] float TerminalSpeed = 15f;
    [SerializeField] float FallMultiplier = 2.5f;
    // [SerializeField] float LowJumpMult = 2f;

    // private int frameCount = 0;

    // Event Vars
    private bool _isGrounded = false;

    private bool IsJumpingThisFrame = false;


    


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
        //JumpHeld = false;
        CoyoteTimer = 0;
        JumpBufferTimer = 0;
        //Jumping = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // If we never found the RigidBody2D then we can't do any movement.
        if (!UseRB) return;

        Move();
        // If Jump is in the Update function, then the IsGrounded check does not work properly.
        //JumpChecks();
        // Making the fall feel heavier goes in the FixedUpdate function as it messes with gravity which is part of the games physics.
        HeavyFall();

        // frameCount++;
    }

    private void Update()
    {
        JumpChecks();
    }

    private void JumpChecks()
    {
        

        // JumpChecks()
        //      A function that every frame,
        //      (1) checks if we've left the ground without jumping and start the coyote time counter and
        //      (2) when we land on the ground, check if the jump buffer timer > 0, if so perform a jump.

        // Is the player currently grounded?
        bool IsCurrentlyGrounded = isGrounded();
        if (IsJumpingThisFrame) IsCurrentlyGrounded = false;


        // This ensures that the cut in velocity as a result of letting go of the jump button in the air can only happen once.
        // Also resets coyote time bool. This ensures that we only have a coyote time jump off a ledge and not after jumping.
        if (IsCurrentlyGrounded)
        {
            JumpFallFeelOnce = true;
            CoyoteTimeBool = true;
        }

        // If on the previous frame the player was grounded and on the current frame they are not
        if (_isGrounded && !IsCurrentlyGrounded)
        {
            // Start the coyote timer if the coyotetimebool is true
            CoyoteTimer = (CoyoteTimeBool) ? CoyoteTime : 0;
        }
        
        // Debug.Log("Jumpcheck: " + CoyoteTimeBool + " CoyoteTimer: " + CoyoteTimer + " frame count: " + frameCount + " IsJumpThisFrameBool: " + IsJumpingThisFrame);
        
        // Decrease the coyote timer or keep it at zero if it goes under zero.
        CoyoteTimer = (CoyoteTimer > 0f) ? CoyoteTimer - Time.deltaTime : 0f;

        // If on the previous frame the player was not grounded but this frame they were then we have just reached the ground.
        if (!_isGrounded && IsCurrentlyGrounded)
        {
            // Check if there is a jump buffered.
            if (JumpBufferTimer > 0)
            {
                PerformJump();
                
                // If the jump button is not pressed when the buffered jump happens, then we need to cut the velocity to ensure a small hop happens.
                // This would normally be handled by the ReleaseJump() function but it can't be called since this jump uniquely, is performed already released.
                if (jump.ReadValue<float>() == 0)
                {
                    rb.linearVelocityY *= JumpFeelCut;
                    JumpFallFeelOnce = false;
                }

            }
        }
        // Decrease the jump buffer timer or keep it at zero if it goes under zero.
        JumpBufferTimer = (JumpBufferTimer > 0f) ? JumpBufferTimer - Time.deltaTime : 0f;

        // Make _isGrounded the grounded state of the current frame.
        _isGrounded = IsCurrentlyGrounded;

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
        direction = (movementDir != 0) ? (int) movementDir : direction;
    }
    
    private int RoundDirection(float input)
    {
        // Although I am expecting the movement values to be -1, 0, or 1, this ensures that the behaviour of the sprite is as expected if the values span decimal numbers from -1 to 1.
        return (input > 0) ? (int) Math.Ceiling(input): (input < 0) ? (int) Math.Floor(input): 0;
    }
    


    // This function will be called whenever space bar is pressed.
    private void PressJump(InputAction.CallbackContext action)
    {
        if (action.performed)
        {


            // If the player is on the ground or if the coyote timer is more than zero.
            if (isGrounded() || CoyoteTimer > 0)
            {
                // Jump
                PerformJump();
            }
            else
            {
                // Else, buffer a jump by starting the timer for one.
                JumpBufferTimer = JumpBufferTime;
            }
        }
    }
    // This function will be called whenever space bar is released.
    private void ReleaseJump(InputAction.CallbackContext action)
    {
        if (action.performed)
        {
            // If the player is moving up, when the jump button has been released, then cut the upwards velocity in half.
            if (rb.linearVelocityY > 0 && JumpFallFeelOnce)
            {
                rb.linearVelocityY *= JumpFeelCut;
                JumpFallFeelOnce = false;
            }
        }   
    }

    private bool isGrounded()
    {
        if (GroundedRay) Debug.DrawRay(feet.position, Vector2.down * GroundRaycastDistance, Color.red);
        return Physics2D.Raycast(feet.position, Vector2.down, GroundRaycastDistance, GroundLayer);
    }

    private void PerformJump()
    {
        // Add the jump force here.
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);

        // Makes sure player can't coyote time jump after jumping.
        CoyoteTimeBool = false;
        CoyoteTimer = 0;

        IsJumpingThisFrame = true;

        // Debug.Log("Inside perform jump. " + CoyoteTimeBool + " frame count: " + frameCount + " IsJumpThisFrameBool: " + IsJumpingThisFrame);
    }

    private void HeavyFall()
    {
        // If linear velocity in the y direction is negative, add some extra gravity.
        // Make sure it is clamped to a terminal velocity.
        if (rb.linearVelocityY < 0f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (FallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        /*
        else if (rb.linearVelocityY > 0 && JumpHeld)
        {
            rb.linearVelocity += Physics2D.gravity.y * (LowJumpMult - 1) * Time.fixedDeltaTime * Vector2.up;
        }
        */

        if (rb.linearVelocityY < -TerminalSpeed) rb.linearVelocityY = -TerminalSpeed;
    }
}
