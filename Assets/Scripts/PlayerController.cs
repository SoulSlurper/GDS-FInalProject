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
    
    // Collision check positions - calculated automatically from sprite size
    private Vector2 spriteSize;
    private Vector2 groundCheckPos;
    private Vector2 wallCheckLeftPos;
    private Vector2 wallCheckRightPos;
    
    // Core components and state
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWallLeft;
    private bool isTouchingWallRight;
    private float horizontalInput;
    private Vector2 velocity = Vector2.zero;
    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    // Physics materials
    private PhysicsMaterial2D noFriction;
    private PhysicsMaterial2D fullFriction;
    
    // Animation parameters
    private readonly string IS_RUNNING = "IsRunning";
    private readonly string IS_JUMPING = "IsJumping";
    private readonly string IS_FALLING = "IsFalling";
    private readonly string IS_GROUNDED = "IsGrounded";
    
    private void Awake()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        // Get sprite bounds
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            spriteSize = new Vector2(
                spriteRenderer.sprite.bounds.size.x,
                spriteRenderer.sprite.bounds.size.y
            );
        }
        else
        {
            // Default size if sprite not available
            spriteSize = new Vector2(0.3f, 0.2f);
        }
        
        // Calculate check positions based on sprite size
        groundCheckPos = new Vector2(0, -spriteSize.y/2f);
        wallCheckLeftPos = new Vector2(-spriteSize.x/2f, 0);
        wallCheckRightPos = new Vector2(spriteSize.x/2f, 0);
        
        // Create physics materials
        noFriction = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
        fullFriction = new PhysicsMaterial2D("FullFriction") { friction = 0.4f, bounciness = 0f };
    }
    
    private void Update()
    {
        // Get horizontal input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool(IS_JUMPING, true);
        }

        if (!Input.GetButtonDown("Jump"))
        {
            animator.SetBool(IS_JUMPING, false);
        }


        // Better jump physics
        if (rb.velocity.y < 0)
        {
            // Apply higher gravity when falling
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Apply lower gravity when jumping and button released (for variable jump height)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        
        // Update animation parameters
        UpdateAnimationState();
        
        // Flip character based on movement direction
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
        // Calculate world positions for checks
        Vector2 worldGroundCheckPos = (Vector2)transform.position + groundCheckPos;
        Vector2 worldWallCheckLeftPos = (Vector2)transform.position + wallCheckLeftPos;
        Vector2 worldWallCheckRightPos = (Vector2)transform.position + wallCheckRightPos;
        
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(worldGroundCheckPos, groundCheckRadius, groundLayer);
        
        // Check for wall collisions
        isTouchingWallLeft = Physics2D.Raycast(worldWallCheckLeftPos, Vector2.left, wallCheckDistance, groundLayer);
        isTouchingWallRight = Physics2D.Raycast(worldWallCheckRightPos, Vector2.right, wallCheckDistance, groundLayer);
        
        // Adjust physics material based on collision
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            // Use no friction when in air or moving against a wall
            if (!isGrounded || (isTouchingWallLeft && horizontalInput < 0) || (isTouchingWallRight && horizontalInput > 0))
            {
                collider.sharedMaterial = noFriction;
            }
            else
            {
                collider.sharedMaterial = fullFriction;
            }
        }
        
        // Calculate target velocity
        float targetVelocityX = horizontalInput * moveSpeed;
        
        // Prevent "sticking" to walls
        if ((isTouchingWallLeft && horizontalInput < 0) || (isTouchingWallRight && horizontalInput > 0))
        {
            // Apply wall slide when against walls in air
            if (!isGrounded && rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * wallSlideMultiplier);
            }
            
            // Don't apply horizontal force against the wall
            targetVelocityX = 0;
        }
        
        Vector2 targetVelocity = new Vector2(targetVelocityX, rb.velocity.y);
        
        // Smoothly move towards the target velocity
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, smoothMovementTime);
    }
    
    private void FlipCharacter()
    {
        facingRight = !facingRight;

        // Flip using sprite renderer
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !facingRight;
        }

        // Alternative: Flip using transform.localScale
        //transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // Simple and clear animation state management
    private void UpdateAnimationState()
    {
        if (animator == null)
            return;
            
        // Set all animation states based on current physics state
        animator.SetBool(IS_GROUNDED, isGrounded);
        animator.SetBool(IS_RUNNING, Mathf.Abs(horizontalInput) > 0.1f && isGrounded);
        animator.SetBool(IS_FALLING, rb.velocity.y < -0.1f && !isGrounded);
    }
    
    // Visualize check points in editor
    private void OnDrawGizmosSelected()
    {
        if (spriteRenderer == null || !Application.isPlaying)
        {
            // Use default size when not in play mode
            spriteSize = new Vector2(0.3f, 0.2f);
            groundCheckPos = new Vector2(0, -spriteSize.y/2f);
            wallCheckLeftPos = new Vector2(-spriteSize.x/2f, 0);
            wallCheckRightPos = new Vector2(spriteSize.x/2f, 0);
        }
        
        // Draw ground check
        Gizmos.color = Color.green;
        Vector2 worldGroundCheckPos = (Vector2)transform.position + groundCheckPos;
        Gizmos.DrawWireSphere(worldGroundCheckPos, groundCheckRadius);
        
        // Draw wall checks
        Gizmos.color = Color.blue;
        Vector2 worldWallCheckLeftPos = (Vector2)transform.position + wallCheckLeftPos;
        Vector2 worldWallCheckRightPos = (Vector2)transform.position + wallCheckRightPos;
        Gizmos.DrawRay(worldWallCheckLeftPos, Vector2.left * wallCheckDistance);
        Gizmos.DrawRay(worldWallCheckRightPos, Vector2.right * wallCheckDistance);
    }
}