using System.Collections;
using UnityEngine;

public class PlayerHealth : Status
{
    [Header("Damage Sources")]
    [SerializeField] private float _enemyDamage = 10f;
    [SerializeField] private float _bossDamage = 20f;
    [SerializeField] private float _lavaDamage = 100f; // Instant death
    [SerializeField] private float _spikeDamage = 15f;
    [SerializeField] private float _enemyProjectileDamage = 10f;
    [SerializeField] private float _bossProjectileDamage = 15f;
    
    [Header("Size by Health")]
    [SerializeField] [Range(0, 1f)] private float minSize = 0.7f;
    
    // References
    private Vector3 initialSize;
    private SlimeKnightController playerController;
    private WeaponAtHand weaponHandler;
    
    // Status effect trackers
    private Coroutine currentDamageFlashRoutine;
    
    // Lava damage control
    private float lavaDamageTimer = 0f;
    private float lavaDamageInterval = 0.5f; // Apply damage every 0.5 seconds

    void Start()
    {
        initialSize = transform.localScale;
        playerController = GetComponent<SlimeKnightController>();
        weaponHandler = GetComponentInChildren<WeaponAtHand>();
        
        // Subscribe to damage events
        OnDamageTaken += HandleDamageTaken;
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        OnDamageTaken -= HandleDamageTaken;
    }

    void Update()
    {
       // Update player size based on health
       UpdatePlayerSize();
    }
    
    // Handles damage event with knockback
    private void HandleDamageTaken(float amount, Vector2 damageSource)
    {
        if (playerController != null)
        {
            // Apply knockback when damage is taken
            float knockbackForce = amount / 5f; // Force proportional to damage
            playerController.ApplyKnockback(damageSource, knockbackForce);
        }
    }
    
    // Override base damage method to check invincibility
    public override float TakeDamage(float damage, Vector2? damageSource = null)
    {
        // Skip damage if player is invincible
        if (playerController != null && playerController.IsInvincible())
        {
            return 0;
        }
        
        return base.TakeDamage(damage, damageSource);
    }
    
    // Handle various collision types
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Get contact point for directional effects
        Vector2 contactPoint = collision.ClosestPoint(transform.position);
        
        if (collision.CompareTag("Spike"))
        {
            if (weaponHandler != null && 
                weaponHandler.selectedWeapon == WeaponType.Sword && 
                Input.GetMouseButton(0))
            {
                return;
            }
            
            TakeDamage(_spikeDamage, contactPoint);
        }
        else if (collision.CompareTag("BossProjectile"))
        {
            TakeDamage(_bossProjectileDamage, contactPoint);
        }
        else if (collision.CompareTag("EnemyProjectile"))
        {
            TakeDamage(_enemyProjectileDamage, contactPoint);
        }
        else if (collision.CompareTag("Lava"))
        {
            // Initial damage from lava
            TakeDamage(_lavaDamage, contactPoint);
            // Reset timer to delay next damage
            lavaDamageTimer = 0f;
        }
    }
    
    // FIX BUG 2: Implement continuous damage for Lava
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Lava"))
        {
            lavaDamageTimer += Time.deltaTime;
            
            // Apply periodic damage while in lava
            if (lavaDamageTimer >= lavaDamageInterval)
            {
                Vector2 contactPoint = collision.ClosestPoint(transform.position);
                TakeDamage(_lavaDamage, contactPoint);
                lavaDamageTimer = 0f;
            }
        }
    }
    
    // Reset timer when exiting hazards
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Lava"))
        {
            // Reset timer to ensure damage is applied immediately on next entry
            lavaDamageTimer = lavaDamageInterval;
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Get contact point for knockback direction
        Vector2 contactPoint = collision.GetContact(0).point;
        
        if (collision.collider.CompareTag("Boss"))
        {
            TakeDamage(_bossDamage, contactPoint);
        }
        else if (collision.collider.CompareTag("Enemy"))
        {
            TakeDamage(_enemyDamage, contactPoint);
        }
    }
    
    // Update player size based on health percentage
    private void UpdatePlayerSize()
    {
        Vector3 newSize = initialSize;
        if (currentHealth < maxHealth)
        {
            Vector3 actualMinSize = initialSize * minSize;
            Vector3 remainingSize = initialSize - actualMinSize;
            newSize = actualMinSize + remainingSize * currentHealthPercentage;
        }
        
        transform.localScale = newSize;
    }
}