using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float idleSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float detectionRange = 7f;
    [SerializeField] private float hoverAmplitude = 0.5f;
    [SerializeField] private float hoverFrequency = 2f;
    [SerializeField] private float directionChangeTime = 3f;
    [SerializeField] private int contactDamage = 10;
    
    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.25f;
    [SerializeField] private float invincibilityDuration = 0.5f;
    
    // References
    private Rigidbody2D rb;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    // State variables
    private Vector2 moveDirection;
    private bool isChasing = false;
    private float hoverOffset;
    private bool hasDetectedPlayer = false;
    private bool isKnockedBack = false;
    private bool isInvincible = false;
    private Vector2 currentKnockbackVelocity;
    private float knockbackTimer = 0f;
    private float invincibilityTimer = 0f;
    
    // Animation parameters
    private readonly string IS_CHASING = "IsChasing";
    private readonly string IS_HURT = "IsHurt";
    
    private void Awake()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        // Random hover offset for variation
        hoverOffset = Random.Range(0f, 2f * Mathf.PI);
        
        // Find player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject?.transform;
        
        // Initial direction and start behavior loop
        SetRandomDirection();
        StartCoroutine(ChangeDirectionRoutine());
    }
    
    private void Update()
    {
        // Handle knockback and invincibility timers
        UpdateKnockbackState();

        // Only process AI behavior if not knocked back
        if (!isKnockedBack && playerTransform != null)
            UpdateAIBehavior();
    }
    
    private void FixedUpdate()
    {
        // Apply knockback if in knockback state
        if (isKnockedBack)
        {
            rb.velocity = currentKnockbackVelocity;
            return;
        }
        
        // Normal movement behavior
        if (isChasing)
            ChasePlayer();
        else
            IdleWander();
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
                if (animator != null) 
                    animator.SetBool(IS_HURT, false);
            }
        }
        
        // Update invincibility timer and visual feedback
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
                float alpha = Mathf.PingPong(Time.time * 10f, 1f) * 0.5f + 0.5f;
                spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            }
        }
    }
    
    private void UpdateAIBehavior()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        bool currentlyChasing = distanceToPlayer <= detectionRange;

        // Handle detection events
        if (currentlyChasing && !hasDetectedPlayer)
        {
            hasDetectedPlayer = true;
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayEnemyDetectedSound();
                StartCoroutine(PlayDashAfterDelay(0.5f));
            }
        }
        else if (!currentlyChasing)
        {
            hasDetectedPlayer = false;
        }

        // Update state
        isChasing = currentlyChasing;
        if (animator != null)
            animator.SetBool(IS_CHASING, isChasing);

        UpdateFacingDirection();
    }
    
    public void ApplyKnockback(Vector2 sourcePosition, float force = 0f)
    {
        if (isKnockedBack || isInvincible) return;
        
        // Calculate knockback
        float knockbackStrength = force > 0 ? force : knockbackForce;
        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        currentKnockbackVelocity = direction * knockbackStrength + Vector2.up * knockbackStrength * 0.3f;
        
        // Set state
        isKnockedBack = true;
        isInvincible = true;
        knockbackTimer = knockbackDuration;
        invincibilityTimer = invincibilityDuration;
        
        // Set animation
        if (animator != null) 
            animator.SetBool(IS_HURT, true);
    }
    
    private void IdleWander()
    {
        // Hover motion for floating effect
        float hoverY = Mathf.Sin((Time.time + hoverOffset) * hoverFrequency) * hoverAmplitude;
        Vector2 hoverMotion = new Vector2(0, hoverY);
        
        // Apply movement
        rb.velocity = moveDirection * idleSpeed + hoverMotion;
    }
    
    private void ChasePlayer()
    {
        if (playerTransform == null) return;
        
        // Direction to player
        Vector2 directionToPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        
        // Less pronounced hover during chase
        float hoverY = Mathf.Sin((Time.time + hoverOffset) * hoverFrequency) * (hoverAmplitude * 0.5f);
        Vector2 hoverMotion = new Vector2(0, hoverY);
        
        // Apply chase velocity
        rb.velocity = directionToPlayer * chaseSpeed + hoverMotion;
    }
    
    private void SetRandomDirection()
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    
    private IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            // Only change direction when not chasing
            if (!isChasing)
                SetRandomDirection();
            
            yield return new WaitForSeconds(directionChangeTime);
        }
    }
    
    private void UpdateFacingDirection()
    {
        if (spriteRenderer == null) return;
        
        // Direction to face
        Vector2 direction = isChasing ? 
            (Vector2)playerTransform.position - (Vector2)transform.position : 
            rb.velocity;
            
        // Flip sprite based on direction
        spriteRenderer.flipX = direction.x < -0.1f;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Bounce off surfaces
        ContactPoint2D contact = collision.contacts[0];
        rb.velocity = Vector2.Reflect(rb.velocity, contact.normal) * 0.7f;
        
        // Damage and knockback player on contact
        if (collision.gameObject.CompareTag("Player"))
        {
            SlimeKnightController playerController = collision.gameObject.GetComponent<SlimeKnightController>();
            if (playerController != null && !playerController.IsInvincible())
                playerController.ApplyKnockback(transform.position);
        }
    }
    
    private IEnumerator PlayDashAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance?.PlayEnemyDashSound();
    }
    
    public bool IsKnockedBack() => isKnockedBack;
    public bool IsInvincible() => isInvincible;
    
    // Visualize detection range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}