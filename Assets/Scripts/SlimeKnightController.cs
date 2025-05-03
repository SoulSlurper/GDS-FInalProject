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

    [Header("Knockback & Invincibility Settings")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.25f;
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float flashDuration = 1.5f;
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
    
    // Knockback state
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private Vector2 currentKnockbackVelocity;
    
    // Invincibility state
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    
    // Visual effect state
    private Coroutine flashRoutine;

    // Animation parameters
    private readonly string IS_RUNNING = "IsRunning";
    private readonly string IS_JUMPING = "IsJumping";
    private readonly string IS_FALLING = "IsFalling";
    private readonly string IS_GROUNDED = "IsGrounded";
    private readonly string IS_HURT = "IsHurt";
    private readonly string IS_AIMING = "IsAiming";

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
        soundManager = FindObjectOfType<SoundManager>();
    }

    private void Update()
    {
        // Update state timers
        UpdateStateTimers();
        
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
            // Allow deceleration over time for more natural feeling
            currentKnockbackVelocity *= 0.92f;  // Gradual reduction
            return;
        }
        
        // Ground and wall detection
        CheckGroundAndWalls();
        
        // Handle movement physics
        HandleMovementPhysics();
    }

    // Update state timers and effects
    private void UpdateStateTimers()
    {
        // Update knockback timer
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                // Reset animation if needed
                if (animator != null && playHurtAnimation)
                {
                    animator.SetBool(IS_HURT, false);
                }
            }
        }
        
        // Update invincibility timer
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                EndInvincibility();
            }
        }
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
    
    // Apply knockback with configurable force
    public void ApplyKnockback(Vector2 sourcePosition, float force = 0f)
    {
        // If already knocked back or invincible, don't apply again
        if (isKnockedBack || isInvincible) return;
        
        // Calculate knockback
        float knockbackStrength = force > 0 ? force : knockbackForce;
        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        currentKnockbackVelocity = direction * knockbackStrength + Vector2.up * (knockbackStrength * 0.5f);
        
        // Set knockback state
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        
        // Set invincibility
        StartInvincibility(invincibilityDuration);
        
        // Set animation
        if (animator != null && playHurtAnimation) 
            animator.SetBool(IS_HURT, true);
        
        // Play sound effect
        SoundManager.Instance?.PlaySlimeHitSound(transform.position);
    }
    
    // Start invincibility period
    public void StartInvincibility(float duration)
    {
        isInvincible = true;
        invincibilityTimer = duration;
        
        // Start flash effect
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashEffect(duration));
    }
    
    // End invincibility
    private void EndInvincibility()
    {
        isInvincible = false;
        
        // Ensure sprite is fully visible
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        
        // Stop flash effect if still running
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }
    }
    
    // Flash effect coroutine
    private IEnumerator FlashEffect(float duration)
    {
        if (spriteRenderer == null) yield break;
        
        float timer = 0;
        
        // Flash between visible and partially transparent
        while (timer < duration)
        {
            // Toggle visibility
            spriteRenderer.color = spriteRenderer.color.a >= 1.0f ? 
                new Color(1f, 1f, 1f, 0.3f) : Color.white;
            
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }
        
        // Ensure sprite is visible at the end
        spriteRenderer.color = Color.white;
        flashRoutine = null;
    }
    
    // Public state checks
    public bool IsMoving() => Mathf.Abs(horizontalInput) > 0.1f;
    public bool IsAiming() => isAiming;
    public bool IsKnockedBack() => isKnockedBack;
    public bool IsInvincible() => isInvincible;
    
    // Reset player state (called after respawn)
    public void ResetPlayerState()
    {
        // Reset knockback state
        isKnockedBack = false;
        knockbackTimer = 0f;
        
        // Reset invincibility
        EndInvincibility();
        
        // Reset animations
        if (animator != null)
        {
            animator.SetBool(IS_HURT, false);
        }
        
        // Reset velocity
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
}