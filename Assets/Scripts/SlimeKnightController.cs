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
    
    [Header("Coyote Time & Jump Buffer")]
    [SerializeField] private float coyoteTime = 0.1f; // Time player can jump after leaving a platform
    [SerializeField] private float jumpBufferTime = 0.1f; // Time to buffer jump input before landing

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.25f;
    [SerializeField] private float flashDuration = 1f; // Renamed from invincibilityDuration
    [SerializeField] private float flashInterval = 0.1f;
    [SerializeField] private bool playHurtAnimation = true;

    [Header("Aiming Settings")]
    [SerializeField] private float aimMovementMultiplier = 0.6f; // Slow down to 60% when aiming
    [SerializeField] private float aimJumpMultiplier = 0.7f; // Reduce jump height when aiming
    [SerializeField] private bool allowJumpWhileAiming = true; // Option to enable/disable jumping while aiming

    [Header("Physics Materials")]
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;

    // Cached component references
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private SoundManager soundManager;
    private Collider2D playerCollider;
    private WeaponAtHand weaponHandler;
    
    // State variables
    private Vector2 spriteSize;
    private Vector2 groundCheckPos, wallCheckLeftPos, wallCheckRightPos;
    private bool isGrounded, wasGrounded;
    private bool isTouchingWallLeft, isTouchingWallRight;
    private float horizontalInput;
    private Vector2 velocity = Vector2.zero;
    private bool facingRight = true;
    private bool isWalking = false;
    private bool isAiming = false;
    
    // Jump related variables
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool hasJumped = false;
    
    // Hit state variables
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private float flashTimer = 0f; // Renamed from invincibilityTimer
    private Vector2 currentKnockbackVelocity;

    // Animation parameters
    private readonly string IS_RUNNING = "IsRunning";
    private readonly string IS_JUMPING = "IsJumping";
    private readonly string IS_FALLING = "IsFalling";
    private readonly string IS_GROUNDED = "IsGrounded";
    private readonly string IS_HURT = "IsHurt";
    private readonly string IS_AIMING = "IsAiming"; // New animation parameter for aiming

    private void Awake()
    {
        // Cache component references
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
        weaponHandler = GetComponentInChildren<WeaponAtHand>();
        
        // Setup collision detection sizes
        CalculateCollisionPoints();
        
        // Create default physics materials if not specified
        if (noFriction == null)
            noFriction = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
        if (fullFriction == null)
            fullFriction = new PhysicsMaterial2D("FullFriction") { friction = 0.4f, bounciness = 0f };
    }

    private void Start()
    {
        soundManager = FindAnyObjectByType<SoundManager>();
    }

    private void Update()
    {
        // Handle knockback and flash states
        UpdateKnockbackState();
        
        // Skip input processing if knocked back
        if (isKnockedBack) return;

        // Process movement input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // Handle aiming state
        UpdateAimingState();
        
        // Handle jump input and buffer
        HandleJumpInput();
        
        // Apply better jump physics
        ApplyBetterJumpPhysics();
        
        // Update animation states
        UpdateAnimationState();
        
        // Handle walking sound effects
        HandleWalkingSound();
        
        // Handle character flipping
        HandleCharacterFlip();
        
        // Handle coyote time
        HandleCoyoteTime();
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
        CheckGroundAndWalls();
        
        // Handle movement physics
        HandleMovementPhysics();
    }

    private void UpdateAimingState()
    {
        if (weaponHandler == null)
        {
            weaponHandler = GetComponentInChildren<WeaponAtHand>();
            if (weaponHandler == null) return;
        }
        
        isAiming = Input.GetMouseButton(1);
    }

    private void CalculateCollisionPoints()
    {
        // Setup collision detection sizes
        spriteSize = spriteRenderer?.sprite != null 
            ? new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y) 
            : new Vector2(0.3f, 0.2f);
        
        groundCheckPos = new Vector2(0, -spriteSize.y / 2f);
        wallCheckLeftPos = new Vector2(-spriteSize.x / 2f, 0);
        wallCheckRightPos = new Vector2(spriteSize.x / 2f, 0);
    }

    private void HandleJumpInput()
    {
        // Handle jump buffer
        if (Input.GetButtonDown("Jump") && (!isAiming || allowJumpWhileAiming))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        
        // Jump if buffer is active and player can jump
        if (jumpBufferCounter > 0 && (isGrounded || coyoteTimeCounter > 0))
        {
            Jump();
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
        }
        
        // Reset jump animation
        if (!Input.GetButtonDown("Jump"))
            animator.SetBool(IS_JUMPING, false);
    }

    private void Jump()
    {
        float currentJumpForce = jumpForce;
        
        // Apply jump reduction if aiming
        if (isAiming && allowJumpWhileAiming)
        {
            currentJumpForce *= aimJumpMultiplier;
        }
        
        rb.velocity = new Vector2(rb.velocity.x, currentJumpForce);
        animator.SetBool(IS_JUMPING, true);
        
        if (soundManager != null)
        {
            soundManager.StopWalkSound();
            soundManager.PlayJumpSound();
        }
        hasJumped = true;
    }

    private void HandleCoyoteTime()
    {
        // Handle coyote time - short period to jump after leaving platform
        if (wasGrounded && !isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void ApplyBetterJumpPhysics()
    {
        // Apply better jump physics for more control
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }

    private void CheckGroundAndWalls()
    {
        // Ground and wall detection
        Vector2 worldGroundCheckPos = (Vector2)transform.position + groundCheckPos;
        Vector2 worldWallCheckLeftPos = (Vector2)transform.position + wallCheckLeftPos;
        Vector2 worldWallCheckRightPos = (Vector2)transform.position + wallCheckRightPos;

        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(worldGroundCheckPos, groundCheckRadius, groundLayer);
        isTouchingWallLeft = Physics2D.Raycast(worldWallCheckLeftPos, Vector2.left, wallCheckDistance, groundLayer);
        isTouchingWallRight = Physics2D.Raycast(worldWallCheckRightPos, Vector2.right, wallCheckDistance, groundLayer);

        // Handle physics material
        if (playerCollider != null)
            playerCollider.sharedMaterial = (!isGrounded || (isTouchingWallLeft && horizontalInput < 0) || 
                                     (isTouchingWallRight && horizontalInput > 0)) ? noFriction : fullFriction;

        // Play landing sound
        if (!wasGrounded && isGrounded && rb.velocity.y < -0.1f)
            SoundManager.Instance?.PlaySplatterSound();
    }

    private void HandleMovementPhysics()
    {
        // Calculate target velocity with aiming slowdown if applicable
        float currentMoveSpeed = moveSpeed;
        
        // Only apply aiming slowdown when on ground
        if (isAiming && isGrounded)
        {
            currentMoveSpeed *= aimMovementMultiplier;
        }
        
        float targetVelocityX = horizontalInput * currentMoveSpeed;

        // Wall sliding
        if ((isTouchingWallLeft && horizontalInput < 0) || (isTouchingWallRight && horizontalInput > 0))
        {
            if (!isGrounded && rb.velocity.y < 0)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * wallSlideMultiplier);
            targetVelocityX = 0;
        }

        // Apply movement with smooth damping
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
        
        // Update flash timer and visual effect
        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f)
            {
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

    private void HandleCharacterFlip()
    {
        // Handle character flipping
        if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
            FlipCharacter();
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
        
        // Set aiming animation if parameter exists
        bool hasAimParameter = false;
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == IS_AIMING)
            {
                hasAimParameter = true;
                break;
            }
        }
        
        if (hasAimParameter)
            animator.SetBool(IS_AIMING, isAiming);
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
    
    public bool IsAiming() => isAiming;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // No longer checking invincible state here
        if (collision.gameObject.CompareTag("Enemy"))
            ApplyKnockback(collision.transform.position);

        if (collision.gameObject.CompareTag("Spike"))
            ApplyKnockback(collision.transform.position);
    }
    
    // Apply knockback with configurable force - removed invincibility
    public void ApplyKnockback(Vector2 sourcePosition, float force = 0f)
    {
        if (isKnockedBack) return; // Only check for knockback state, not invincibility
        
        float knockbackStrength = force > 0 ? force : knockbackForce;
        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        currentKnockbackVelocity = direction * knockbackStrength + Vector2.up * knockbackStrength * 0.5f;
        
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        flashTimer = flashDuration; // Start flash effect but no invincibility
        
        if (animator != null && playHurtAnimation) 
            animator.SetBool(IS_HURT, true);
        
        SoundManager.Instance?.PlaySlimeHitSound(transform.position);
    }
    
    public bool IsKnockedBack() => isKnockedBack;
    
    // This method is kept for compatibility, but always returns false now
    public bool IsInvincible() => false;
}