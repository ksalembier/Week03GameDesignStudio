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
    private float jumpReleasedDuringBuffer;

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
        if (isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
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

    }

    private void Jump()
    {

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
}
