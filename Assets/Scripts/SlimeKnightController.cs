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
    [SerializeField] private float groundCheckRadius = 0.05f;
    [SerializeField] private float wallCheckDistance = 0.05f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float smoothMovementTime = 0.05f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.25f;
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private float flashInterval = 0.1f;
    [SerializeField] private bool playHurtAnimation = true;

    private Vector2 spriteSize;
    private Vector2 groundCheckPos, wallCheckLeftPos, wallCheckRightPos;
    private Rigidbody2D rb;
    private bool isGrounded, isTouchingWallLeft, isTouchingWallRight;
    private float horizontalInput;
    private Vector2 velocity = Vector2.zero;
    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private SoundManager soundManager;
    private PhysicsMaterial2D noFriction, fullFriction;

    // Animation parameters
    private readonly string IS_RUNNING = "IsRunning";
    private readonly string IS_JUMPING = "IsJumping";
    private readonly string IS_FALLING = "IsFalling";
    private readonly string IS_GROUNDED = "IsGrounded";
    private readonly string IS_HURT = "IsHurt";

    // State tracking
    private bool isWalking = false, hasJumped = false;
    private bool isKnockedBack = false, isInvincible = false;
    private float knockbackTimer = 0f, invincibilityTimer = 0f;
    private Vector2 currentKnockbackVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        soundManager = FindAnyObjectByType<SoundManager>();

        // Set up collision sizes
        spriteSize = spriteRenderer?.sprite != null 
            ? new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y) 
            : new Vector2(0.3f, 0.2f);
        
        groundCheckPos = new Vector2(0, -spriteSize.y / 2f);
        wallCheckLeftPos = new Vector2(-spriteSize.x / 2f, 0);
        wallCheckRightPos = new Vector2(spriteSize.x / 2f, 0);
        
        // Set up physics materials
        noFriction = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
        fullFriction = new PhysicsMaterial2D("FullFriction") { friction = 0.4f, bounciness = 0f };
    }

    private void Update()
    {
        // Handle knockback and invincibility timers
        UpdateKnockbackState();
        
        // Skip input processing if knocked back
        if (isKnockedBack) return;

        // Process movement input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // Handle jump
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
            animator.SetBool(IS_JUMPING, false);
            
        // Apply better jump physics
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            
        // Update visuals and audio
        UpdateAnimationState();
        HandleWalkingSound();
        
        // Handle character flipping
        if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
            FlipCharacter();
    }

    private void FixedUpdate()
    {
        // Apply knockback if in knockback state
        if (isKnockedBack)
        {
            rb.velocity = currentKnockbackVelocity;
            return;
        }
        
        // Ground and wall detection
        Vector2 worldGroundCheckPos = (Vector2)transform.position + groundCheckPos;
        Vector2 worldWallCheckLeftPos = (Vector2)transform.position + wallCheckLeftPos;
        Vector2 worldWallCheckRightPos = (Vector2)transform.position + wallCheckRightPos;

        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(worldGroundCheckPos, groundCheckRadius, groundLayer);
        isTouchingWallLeft = Physics2D.Raycast(worldWallCheckLeftPos, Vector2.left, wallCheckDistance, groundLayer);
        isTouchingWallRight = Physics2D.Raycast(worldWallCheckRightPos, Vector2.right, wallCheckDistance, groundLayer);

        // Handle physics material
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.sharedMaterial = (!isGrounded || (isTouchingWallLeft && horizontalInput < 0) || 
                                     (isTouchingWallRight && horizontalInput > 0)) ? noFriction : fullFriction;

        // Play landing sound
        if (!wasGrounded && isGrounded && rb.velocity.y < -0.1f)
            SoundManager.Instance?.PlaySplatterSound();

        // Movement
        float targetVelocityX = horizontalInput * moveSpeed;

        // Wall sliding
        if ((isTouchingWallLeft && horizontalInput < 0) || (isTouchingWallRight && horizontalInput > 0))
        {
            if (!isGrounded && rb.velocity.y < 0)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * wallSlideMultiplier);
            targetVelocityX = 0;
        }

        // Apply movement with smoothing
        Vector2 targetVelocity = new Vector2(targetVelocityX, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, smoothMovementTime);
    }

    private void UpdateKnockbackState()
    {
        // Update knockback timer
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                if (animator != null && playHurtAnimation) 
                    animator.SetBool(IS_HURT, false);
            }
        }
        
        // Update invincibility timer and visual effect
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
                if (spriteRenderer != null) 
                    spriteRenderer.color = Color.white;
            }
            else if (spriteRenderer != null)
            {
                float alpha = Mathf.PingPong(Time.time * (1f/flashInterval), 1f) * 0.5f + 0.5f;
                spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            }
        }
    }

    private void FlipCharacter()
    {
        facingRight = !facingRight;
        if (spriteRenderer != null)
            spriteRenderer.flipX = !facingRight;
    }

    private void UpdateAnimationState()
    {
        if (animator == null) return;
        animator.SetBool(IS_GROUNDED, isGrounded);
        animator.SetBool(IS_RUNNING, Mathf.Abs(horizontalInput) > 0.1f);
        animator.SetBool(IS_FALLING, rb.velocity.y < -0.1f && !isGrounded);
    }

    private void HandleWalkingSound()
    {
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f && isGrounded;
        if (isMoving && !isWalking)
        {
            isWalking = true;
            soundManager?.PlayWalkSound();
        }
        else if (!isMoving && isWalking)
        {
            isWalking = false;
            soundManager?.StopWalkSound();
        }
    }
    
    public bool IsMoving() => Mathf.Abs(horizontalInput) > 0.1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
            ApplyKnockback(collision.transform.position);

        if (collision.gameObject.CompareTag("Spike"))
            ApplyKnockback(collision.transform.position);
    }
    
    // Apply knockback with unified API
    public void ApplyKnockback(Vector2 sourcePosition, float force = 0f)
    {
        if (isKnockedBack || isInvincible) return;
        
        float knockbackStrength = force > 0 ? force : knockbackForce;
        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        currentKnockbackVelocity = direction * knockbackStrength + Vector2.up * knockbackStrength * 0.5f;
        
        isKnockedBack = true;
        isInvincible = true;
        knockbackTimer = knockbackDuration;
        invincibilityTimer = invincibilityDuration;
        
        if (animator != null && playHurtAnimation) 
            animator.SetBool(IS_HURT, true);
        
        SoundManager.Instance?.PlaySlimeHitSound(transform.position);
    }
    
    public bool IsKnockedBack() => isKnockedBack;
    public bool IsInvincible() => isInvincible;
}