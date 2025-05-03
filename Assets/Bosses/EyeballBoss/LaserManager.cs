using UnityEngine;

public class LaserManager : MonoBehaviour
{
    [SerializeField] private float damage = 15f;
    [SerializeField] private float knockbackForce = 12f;
    
    private Transform bossTransform;
    
    void Start()
    {
        // Find boss for knockback direction
        GameObject boss = GameObject.FindWithTag("Boss");
        if (boss != null)
        {
            bossTransform = boss.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Destroy laser if hits a clear object
        if (collider.gameObject.CompareTag("BossProjectileClear"))
        {
            Destroy(gameObject);
            return;
        }
        
        // Handle player collision
        if (collider.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
            SlimeKnightController playerController = collider.GetComponent<SlimeKnightController>();
            
            // Only damage if player isn't invincible
            if (playerHealth != null && playerController != null && !playerController.IsInvincible())
            {
                // Determine knockback source position
                Vector2 sourcePosition = bossTransform != null ? 
                    bossTransform.position : transform.position;
                
                // Apply damage - this will also trigger knockback via the damage event
                playerHealth.TakeDamage(damage, sourcePosition);
                
                // Play sound
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayEnemyLaserShootSound(transform.position);
                }
            }
        }
    }
}