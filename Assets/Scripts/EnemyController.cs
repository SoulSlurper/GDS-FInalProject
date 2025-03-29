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
    
    [Header("Behavior")]
    [SerializeField] private float directionChangeTime = 3f;
    [SerializeField] private int contactDamage = 10;
    
    private Rigidbody2D rb;
    private Transform playerTransform;
    private Vector2 moveDirection;
    private bool isChasing = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private float hoverOffset;
    
    // Animation parameter
    private readonly string IS_CHASING = "IsChasing";
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        // Random hover offset for variation between enemies
        hoverOffset = Random.Range(0f, 2f * Mathf.PI);
        
        // Find player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        
        // Initial random direction
        SetRandomDirection();
        
        // Start direction change routine
        StartCoroutine(ChangeDirectionRoutine());
    }
    
    private void Update()
    {
        if (playerTransform != null)
        {
            // Check if player is in detection range
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            isChasing = distanceToPlayer <= detectionRange;
            
            // Update animation
            if (animator != null)
            {
                animator.SetBool(IS_CHASING, isChasing);
            }
            
            // Update facing direction
            UpdateFacingDirection();
        }
    }
    
    private void FixedUpdate()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            IdleWander();
        }
    }
    
    private void IdleWander()
    {
        // Calculate hover motion
        float hoverY = Mathf.Sin((Time.time + hoverOffset) * hoverFrequency) * hoverAmplitude;
        Vector2 hoverMotion = new Vector2(0, hoverY);
        
        // Apply movement and hover
        rb.velocity = moveDirection * idleSpeed + hoverMotion;
    }
    
    private void ChasePlayer()
    {
        // Direction to player
        Vector2 directionToPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        
        // Less pronounced hover during chase
        float hoverY = Mathf.Sin((Time.time + hoverOffset) * hoverFrequency) * (hoverAmplitude * 0.5f);
        Vector2 hoverMotion = new Vector2(0, hoverY);
        
        // Apply chase velocity with hover
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
            {
                SetRandomDirection();
            }
            
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
        if (direction.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (direction.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Bounce off surfaces
        ContactPoint2D contact = collision.contacts[0];
        Vector2 normal = contact.normal;
        rb.velocity = Vector2.Reflect(rb.velocity, normal) * 0.7f;
        
        // Damage player on contact
        if (collision.gameObject.CompareTag("Player"))
        {
            // TODO: Add damage logic here when you implement your health system
            // Example: Player takes contactDamage amount of damage
            Debug.Log("Player hit for " + contactDamage + " damage");
        }
    }
    
    // Visualize detection range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}