using System.Collections;
using UnityEngine;
public class SlimeKnightController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float wallSlideMultiplier = 0.1f;

    [Header("Collision Detection")]
    [SerializeField] private float groundCheckRadius = 0.05f;
    [SerializeField] private float wallCheckDistance = 0.05f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Smoothing")]
    [SerializeField] private float smoothMovementTime = 0.05f;

    private Vector2 spriteSize;
    private Vector2 groundCheckPos;
    private Vector2 wallCheckLeftPos;
    private Vector2 wallCheckRightPos;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWallLeft;
    private bool isTouchingWallRight;
    private float horizontalInput;
    private Vector2 velocity = Vector2.zero;
    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private SoundManager soundManager;

    private PhysicsMaterial2D noFriction;
    private PhysicsMaterial2D fullFriction;

    private readonly string IS_RUNNING = "IsRunning";
    private readonly string IS_JUMPING = "IsJumping";
    private readonly string IS_FALLING = "IsFalling";
    private readonly string IS_GROUNDED = "IsGrounded";

    private bool isWalking = false;

    private bool hasJumped = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        soundManager = FindObjectOfType<SoundManager>();

        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            spriteSize = new Vector2(
                spriteRenderer.sprite.bounds.size.x,
                spriteRenderer.sprite.bounds.size.y
            );
        }
        else
        {
            spriteSize = new Vector2(0.3f, 0.2f);
        }
        groundCheckPos = new Vector2(0, -spriteSize.y / 2f);
        wallCheckLeftPos = new Vector2(-spriteSize.x / 2f, 0);
        wallCheckRightPos = new Vector2(spriteSize.x / 2f, 0);
        noFriction = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
        fullFriction = new PhysicsMaterial2D("FullFriction") { friction = 0.4f, bounciness = 0f };
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool(IS_JUMPING, true);
            
            if (soundManager != null)
            {
                soundManager.StopWalkSound();
                soundManager.PlayJumpSound();
            }
            hasJumped = true;
        }
        if (!Input.GetButtonDown("Jump"))
        {
            animator.SetBool(IS_JUMPING, false);
        }
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        UpdateAnimationState();
        HandleWalkingSound();
        if (horizontalInput > 0 && !facingRight)
        {
            FlipCharacter();
        }
        else if (horizontalInput < 0 && facingRight)
        {
            FlipCharacter();
        }
    }

    private void FixedUpdate()
    {
        Vector2 worldGroundCheckPos = (Vector2)transform.position + groundCheckPos;
        Vector2 worldWallCheckLeftPos = (Vector2)transform.position + wallCheckLeftPos;
        Vector2 worldWallCheckRightPos = (Vector2)transform.position + wallCheckRightPos;

        bool wasGrounded = isGrounded;  // Track previous grounded state
        isGrounded = Physics2D.OverlapCircle(worldGroundCheckPos, groundCheckRadius, groundLayer);
        isTouchingWallLeft = Physics2D.Raycast(worldWallCheckLeftPos, Vector2.left, wallCheckDistance, groundLayer);
        isTouchingWallRight = Physics2D.Raycast(worldWallCheckRightPos, Vector2.right, wallCheckDistance, groundLayer);

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            if (!isGrounded || (isTouchingWallLeft && horizontalInput < 0) || (isTouchingWallRight && horizontalInput > 0))
            {
                collider.sharedMaterial = noFriction;
            }
            else
            {
                collider.sharedMaterial = fullFriction;
            }
        }

        // **Play splatter sound only when falling and then landing**
        if (!wasGrounded && isGrounded && rb.velocity.y < -0.1f)
        {
            if (soundManager != null)
            {
                SoundManager.Instance.PlaySplatterSound();
            }
        }

        float targetVelocityX = horizontalInput * moveSpeed;

        if ((isTouchingWallLeft && horizontalInput < 0) || (isTouchingWallRight && horizontalInput > 0))
        {
            if (!isGrounded && rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * wallSlideMultiplier);
            }
            targetVelocityX = 0;
        }

        Vector2 targetVelocity = new Vector2(targetVelocityX, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, smoothMovementTime);
    }


    private void FlipCharacter()
    {
        facingRight = !facingRight;
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !facingRight;
        }
    }

    private void UpdateAnimationState()
    {
        if (animator == null)
            return;

        animator.SetBool(IS_GROUNDED, isGrounded);
        animator.SetBool(IS_RUNNING, Mathf.Abs(horizontalInput) > 0.1f);
        animator.SetBool(IS_FALLING, rb.velocity.y < -0.1f && !isGrounded);
    }

    private void HandleWalkingSound()
    {
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f && isGrounded;

        if (isMoving)
        {
            if (!isWalking)
            {
                isWalking = true;
                soundManager.PlayWalkSound();
            }
        }
        else
        {
            if (isWalking)
            {
                isWalking = false;
                soundManager.StopWalkSound();
            }
        }
    }
    public bool IsMoving()
    {
        return Mathf.Abs(horizontalInput) > 0.1f;
    }

}