using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementManager moveManager;
    [SerializeField] private Collider2D playerCollider;
    private Rigidbody2D rb;

    // movemenet variables
    private Vector2 moveVelocity;
    private bool isFacingRight;

    // collision check variables
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    private bool isGrounded;
    private bool bumpedHead;

    // jump variables
    public float VerticalVelocity {get; private set; }
    private bool isJumping;
    private bool isFastFalling;
    private bool isFalling;
    private float fastFallTime;
    private float fastFallReleaseSpeed;
    private int numberOfJumpsUsed;

    // jump apex variables
    private float apexPoint;
    private float timePastApexThreshold;
    private bool isPastApexThreshold;

    // jump buffer variables
    private float jumpBufferTimer;
    private bool jumpReleasedDuringBuffer;

    // jump coyoted time variables
    private float coyoteTimer;


    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CountTimers();
        JumpChecks();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();

        if (isGrounded) { Move(moveManager.GroundAcceleration, moveManager.GroundDeceleration, InputManager.movement); }
        else { Move(moveManager.AirAcceleration, moveManager.AirDeceleration, InputManager.movement); }
    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);

            Vector2 targetVelocity = Vector2.zero;
            if (InputManager.runIsHeld)
            { targetVelocity = new Vector2(moveInput.x, 0f) * moveManager.MaxRunSpeed; }
            else
            { targetVelocity = new Vector2(moveInput.x, 0f) * moveManager.MaxWalkSpeed; }

            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(moveVelocity.x, rb.velocity.y);
        }

        else if (moveInput == Vector2.zero)
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(moveVelocity.x, rb.velocity.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (isFacingRight && moveInput.x < 0) { Turn(false); }
        else if (!isFacingRight && moveInput.x > 0) { Turn(true); }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }
    
    #endregion

    #region Jump

    private void JumpChecks()
    {
        // on button press
        if (InputManager.jumpWasPressed)
        {
            jumpBufferTimer = moveManager.JumpBufferTime;
            jumpReleasedDuringBuffer = false;
        }

        // on button release
        if (InputManager.jumpWasReleased)
        {
            if (jumpBufferTimer > 0f)
            {
                jumpReleasedDuringBuffer = true;
            }

            if (isJumping && VerticalVelocity > 0f)
            {
                if (isPastApexThreshold)
                {
                    isPastApexThreshold = false;
                    isFastFalling = true;
                    fastFallTime = moveManager.TimeForUpwardCancel;
                    VerticalVelocity = 0f;
                }
                else{
                    isFastFalling = true;
                    fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        // start jump with bufferng and coyote time 
        if (jumpBufferTimer > 0f && !isJumping && (isGrounded || coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (jumpReleasedDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed =  VerticalVelocity;
            }
        }
        // double jump 
        else if (jumpBufferTimer > 0f && isJumping && numberOfJumpsUsed < moveManager.NumberOfJumpsAllowed)
        {
            isFastFalling = false;
            InitiateJump(1);
        }
        // air jump after coyote time elapsed
        else if (jumpBufferTimer > 0f && isFalling && numberOfJumpsUsed < moveManager.NumberOfJumpsAllowed - 1)
        {
           InitiateJump(2);
           isFastFalling = false; 
        }

        // landing 
        if ((isJumping || isFalling) && isGrounded && VerticalVelocity <= 0f)
        {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApexThreshold = false;
            numberOfJumpsUsed = 0;

            VerticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitiateJump(int numberOfJumpsUsed)
    {
        if (!isJumping) { isJumping = true; }

        jumpBufferTimer = 0f;
        numberOfJumpsUsed += numberOfJumpsUsed;
        VerticalVelocity = moveManager.InitialJumpVelocity;
    }

    private void Jump()
    {
        // apply gravity 
        if (isJumping)
        {
            // check for head bump 
            if (bumpedHead) { isFastFalling = true; }

            // gravity of ascending 
            if (VerticalVelocity >= 0f)
            {
                // apex controls
                apexPoint = Mathf.Lerp(moveManager.InitialJumpVelocity, 0f, VerticalVelocity);
                if (apexPoint > moveManager.ApexThreshold)
                {
                    if (!isPastApexThreshold)
                    {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }

                    if (isPastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if (timePastApexThreshold < moveManager.ApexHangTime) { VerticalVelocity = 0f; }
                        else { VerticalVelocity = -0.01f; }
                    }
                }
                // gravity of ascending not past apex threshold
                else 
                {
                    VerticalVelocity += moveManager.Gravity * Time.fixedDeltaTime;
                    if (isPastApexThreshold) { isPastApexThreshold = false; }
                }
            }

            // gravity of descending 
            else if (!isFastFalling)
            {
                VerticalVelocity += moveManager.Gravity * moveManager.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (VerticalVelocity < 0f)
            {
                if (!isFalling) { isFalling = true; }
            }
        }
       
        // jump cut 
        if (isFastFalling)
        {
            if (fastFallTime >= moveManager.TimeForUpwardCancel)
            {
                VerticalVelocity += moveManager.Gravity * moveManager.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (fastFallTime < moveManager.TimeForUpwardCancel)
            {
                VerticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, (fastFallTime / moveManager.TimeForUpwardCancel));
            }

            fastFallTime += Time.fixedDeltaTime;
        }

        // normal gravity while falling 
        if (!isGrounded && !isJumping)
        {
            if (!isFalling) { isFalling = true; }
            VerticalVelocity += moveManager.Gravity * Time.fixedDeltaTime;
        }

        // clamp fall speed
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -moveManager.MaxFallSpeed, 50f);
        rb.velocity = new Vector2(rb.velocity.x, VerticalVelocity);
    }

    #endregion

    #region Collision Checks

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(playerCollider.bounds.size.x, moveManager.GroundDetectionRayLength);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, moveManager.GroundDetectionRayLength, moveManager.GroundLayer);

        if (groundHit.collider != null) { isGrounded = true; }
        else { isGrounded = false; }

        // Debug Visualization
        if (moveManager.DebugShowIsGroundedBox)
        {
            Color rayColor;
            if (isGrounded) { rayColor = Color.green; }
            else { rayColor = Color.red; }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * moveManager.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * moveManager.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - moveManager.GroundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.max.y);
        Vector2 boxCastSize = new Vector2(playerCollider.bounds.size.x * moveManager.HeadWidth, moveManager.HeadDetectionRayLength);

        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, moveManager.HeadDetectionRayLength, moveManager.GroundLayer);
        if (headHit.collider != null) { bumpedHead = true; }
        else { bumpedHead = false; }

        // debug visualization 
        if (moveManager.DebugShowHeadBumpBox)
        {
            float headWidth = moveManager.HeadWidth;

            Color rayColor;
            if (bumpedHead) { rayColor = Color.green; }
            else { rayColor = Color.red; }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * moveManager.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidth, boxCastOrigin.y), Vector2.up * moveManager.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y + moveManager.HeadDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);
        }

    }

    private void CollisionChecks()
    {
        IsGrounded();
    }

    #endregion

    #region Timers

    private void CountTimers()
    {
        jumpBufferTimer -= Time.deltaTime;
        if (!isGrounded) { coyoteTimer -= Time.deltaTime;}
        else { coyoteTimer = moveManager.JumpCoyoteTime; }
    }

    #endregion

    #region Draw Debuggers

    private void DrawJumpArc(float moveSpeed, Color gizmoColor)
    {
        
    }

    #endregion
}
