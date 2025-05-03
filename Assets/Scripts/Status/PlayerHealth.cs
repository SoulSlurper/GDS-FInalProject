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
    private SavePoint savePoint;
    private Vector2 initialPosition;
    private Vector3 initialSize;
    private Camera mainCamera;
    private SlimeKnightController playerController;
    
    // Status effect trackers
    private Coroutine currentDamageFlashRoutine;

    void Start()
    {
        initialPosition = transform.position;
        initialSize = transform.localScale;
        mainCamera = Camera.main;
        playerController = GetComponent<SlimeKnightController>();
        
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
        // Handle death/respawn
        if (noHealth || Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
        
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
        // Save point logic
        if (collision.CompareTag("SavePoint"))
        {
            SavePoint encounteredSavePoint = collision.gameObject.GetComponent<SavePoint>();
            if (encounteredSavePoint != null && !encounteredSavePoint.Equals(savePoint))
            {
                if (savePoint) savePoint.isActive = false;
                savePoint = encounteredSavePoint;
                savePoint.isActive = true;
                Debug.Log("SavePoint Recorded: " + savePoint.position);
            }
        }
        
        // Handle damage from various sources
        Vector2 contactPoint = collision.ClosestPoint(transform.position);
        
        if (collision.CompareTag("BossProjectile"))
        {
            TakeDamage(_bossProjectileDamage, contactPoint);
        }
        else if (collision.CompareTag("EnemyProjectile"))
        {
            TakeDamage(_enemyProjectileDamage, contactPoint);
        }
        else if (collision.CompareTag("Lava"))
        {
            // Instant death from lava
            TakeDamage(_lavaDamage, contactPoint);
        }
        else if (collision.CompareTag("Spike"))
        {
            TakeDamage(_spikeDamage, contactPoint);
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
    
    // Player respawn logic
    private void Respawn()
    {
        // Move player to save point or initial position
        transform.position = (savePoint != null) ? savePoint.position : initialPosition;
        
        // If player died, reset everything
        if (noHealth)
        {
            // Remove boss if present
            GameObject boss = GameObject.FindWithTag("Boss");
            if (boss)
            {
                boss.GetComponent<Status>().healthBar.SetActiveState(false);
                Destroy(boss);
                
                // Reset camera
                mainCamera.orthographicSize = 3f;
                BossTrigger.hasSpawnedBoss = false;
                
                // Reset any background music
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.ResumeBackgroundMusic();
                }
            }
            
            // Reset health and player state
            ResetHealth();
            if (playerController != null)
            {
                playerController.ResetPlayerState();
            }
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