using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    // Define enemy states
    public enum EnemyState
    {
        Idle,
        Chase,
        KnockBack,
        Stunned
    }

    [Header("Movement")]
    [SerializeField] private float idleSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float detectionRange = 7f;
    [SerializeField] private float hoverAmplitude = 0.5f;
    [SerializeField] private float hoverFrequency = 2f;
    [SerializeField] private float directionChangeTime = 3f;
    
    [Header("Player Interaction")]
    [SerializeField] private int contactDamage = 10;
    [SerializeField] private float attackCooldown = 0.5f;
    
    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.25f;
    [SerializeField] private float flashDuration = 0.5f;
    
    // References
    private Rigidbody2D rb;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    // State variables
    private EnemyState currentState = EnemyState.Idle;
    private Vector2 moveDirection;
    private float hoverOffset;
    private bool hasDetectedPlayer = false;
    private Vector2 currentKnockbackVelocity;
    private float knockbackTimer = 0f;
    private float flashTimer = 0f;
    private Coroutine directionChangeCoroutine;
    private float lastAttackTime = 0f;
    
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
        StartDirectionChangeRoutine();
    }
    
    private void OnEnable()
    {
        StartDirectionChangeRoutine();
    }
    
    private void OnDisable()
    {
        StopDirectionChangeRoutine();
    }
    
    private void Update()
    {
        UpdateState();
        UpdateTimers();
        
        // Update visuals based on state
        UpdateVisuals();
    }
    
    private void FixedUpdate()
    {
        // Execute behavior based on current state
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleWander();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.KnockBack:
                // Knockback velocity is set when entering state
                break;
            case EnemyState.Stunned:
                // No movement during stun
                break;
        }
    }
    
    private void UpdateState()
    {
        // Skip state change if knocked back or stunned
        if (currentState == EnemyState.KnockBack || currentState == EnemyState.Stunned)
            return;
            
        // Check distance to player for state changes
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            bool shouldChase = distanceToPlayer <= detectionRange;
            
            // Handle detection events
            if (shouldChase && !hasDetectedPlayer)
            {
                hasDetectedPlayer = true;
                PlayDetectionSound();
            }
            else if (!shouldChase)
            {
                hasDetectedPlayer = false;
            }
            
            // Update state
            currentState = shouldChase ? EnemyState.Chase : EnemyState.Idle;
            
            // Update animator
            if (animator != null)
                animator.SetBool(IS_CHASING, currentState == EnemyState.Chase);
        }
    }
    
    private void UpdateTimers()
    {
        // Update knockback timer
        if (currentState == EnemyState.KnockBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                // Return to idle or chase based on player distance
                if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= detectionRange)
                    currentState = EnemyState.Chase;
                else
                    currentState = EnemyState.Idle;
                
                if (animator != null) 
                    animator.SetBool(IS_HURT, false);
            }
        }
        
        // Update flash timer
        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
        }
    }
    
    private void UpdateVisuals()
    {
        // Update facing direction
        if (spriteRenderer == null) return;
        
        // Direction to face
        Vector2 direction;
        
        if (currentState == EnemyState.Chase && playerTransform != null)
            direction = (Vector2)playerTransform.position - (Vector2)transform.position;
        else
            direction = rb.velocity;
            
        // Only flip if velocity is significant
        if (Mathf.Abs(direction.x) > 0.1f)
            spriteRenderer.flipX = direction.x < 0;
            
        // Flashing during damage effect period
        if (flashTimer > 0)
        {
            float alpha = Mathf.PingPong(Time.time * 10f, 1f) * 0.5f + 0.5f;
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
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
    
    private void StartDirectionChangeRoutine()
    {
        StopDirectionChangeRoutine();
        directionChangeCoroutine = StartCoroutine(ChangeDirectionRoutine());
    }
    
    private void StopDirectionChangeRoutine()
    {
        if (directionChangeCoroutine != null)
            StopCoroutine(directionChangeCoroutine);
    }
    
    private IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            // Only change direction when idle
            if (currentState == EnemyState.Idle)
                SetRandomDirection();
            
            yield return new WaitForSeconds(directionChangeTime);
        }
    }
    
    private void PlayDetectionSound()
    {
        SoundManager.Instance?.PlayEnemyDetectedSound();
        StartCoroutine(PlayDashAfterDelay(0.5f));
    }
    
    public void ApplyKnockback(Vector2 sourcePosition, float force = 0f)
    {
        // If already knocked back, don't interrupt
        if (currentState == EnemyState.KnockBack) return;
        
        // Calculate knockback
        float knockbackStrength = force > 0 ? force : knockbackForce;
        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        currentKnockbackVelocity = direction * knockbackStrength + Vector2.up * knockbackStrength * 0.3f;
        
        // Set state
        currentState = EnemyState.KnockBack;
        flashTimer = flashDuration; // Start flash effect
        knockbackTimer = knockbackDuration;
        rb.velocity = currentKnockbackVelocity;
        
        // Set animation
        if (animator != null) 
            animator.SetBool(IS_HURT, true);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Bounce off surfaces
        ContactPoint2D contact = collision.GetContact(0);
        rb.velocity = Vector2.Reflect(rb.velocity, contact.normal) * 0.7f;
        
        // Only damage player if it's a player and we're not cooling down
        if (collision.gameObject.CompareTag("Player") && Time.time > lastAttackTime + attackCooldown)
        {
            SlimeKnightController playerController = collision.gameObject.GetComponent<SlimeKnightController>();
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            
            // Only damage if player isn't invincible
            if (playerHealth != null && playerController != null && !playerController.IsInvincible())
            {
                // Apply damage with this enemy's position as the damage source
                playerHealth.TakeDamage(contactDamage, transform.position);
                
                // Update last attack time
                lastAttackTime = Time.time;
            }
        }
    }
    
    // Stay collision - for continuous damage sources
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Only apply damage at intervals
        if (collision.gameObject.CompareTag("Player") && Time.time > lastAttackTime + attackCooldown)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            SlimeKnightController playerController = collision.gameObject.GetComponent<SlimeKnightController>();
            
            if (playerHealth != null && playerController != null && !playerController.IsInvincible())
            {
                // Apply damage with position
                playerHealth.TakeDamage(contactDamage, transform.position);
                
                // Update last attack time
                lastAttackTime = Time.time;
            }
        }
    }
    
    private IEnumerator PlayDashAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance?.PlayEnemyDashSound();
    }
    
    // State queries
    public bool IsKnockedBack() => currentState == EnemyState.KnockBack;
    
    // This method is kept for compatibility but always returns false now
    public bool IsInvincible() => false;
    
    // Visualize detection range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}