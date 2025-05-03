using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserManager : MonoBehaviour
{
    // Laser damage amount
    [SerializeField] private float damage = 15f;
    
    // Knockback settings
    [SerializeField] private float knockbackForce = 12f;
    
    // Reference to the boss transform for knockback direction
    private Transform bossTransform;
    
    void Start()
    {
        // Find the boss to use as knockback source
        GameObject boss = GameObject.FindWithTag("Boss");
        if (boss != null)
        {
            bossTransform = boss.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Destroy laser if it hits a clear object
        if (collider.gameObject.CompareTag("BossProjectileClear")) 
        {
            Destroy(gameObject);
            return;
        }
        
        // Player hit detection
        if (collider.CompareTag("Player"))
        {
            // Get player controller and check if not invincible
            SlimeKnightController playerController = collider.GetComponent<SlimeKnightController>();
            if (playerController != null && !playerController.IsInvincible())
            {
                // Apply damage to player
                Status playerStatus = collider.GetComponent<Status>();
                if (playerStatus != null)
                {
                    playerStatus.TakeDamage(damage);
                }
                
                // Apply knockback - use boss position as source if available
                Vector2 knockbackSource = bossTransform != null ? 
                    bossTransform.position : transform.position;
                playerController.ApplyKnockback(knockbackSource, knockbackForce);
                
                // Play laser hit sound if available
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayEnemyLaserShootSound(transform.position);
                }
            }
        }
    }
}