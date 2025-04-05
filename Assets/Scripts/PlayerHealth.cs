using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Status
{
    //may change where enemies could have weapons
    [Header("Collision Damage")]
    [SerializeField] private float _enemyDamage;
    [SerializeField] private float _bossDamage;
    [SerializeField] private float _enemyProjectileDamage;
    [SerializeField] private float _bossProjectileDamage;

    // Getter and Setter // // // //
    public float enemyDamage
    {
        get { return _enemyDamage; }
        private set { _enemyDamage = value; }
    }

    public float bossDamage
    {
        get { return _bossDamage; }
        private set { _bossDamage = value; }
    }

    public float enemyProjectileDamage
    {
        get { return _enemyProjectileDamage; }
        private set { _enemyProjectileDamage = value; }
    }

    public float bossProjectileDamage
    {
        get { return _bossProjectileDamage; }
        private set { _bossProjectileDamage = value; }
    }

    // Unity // // // //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BossProjectile"))
        {
            TakeDamage(bossProjectileDamage);
            respawn();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Boss"))
        {
            TakeDamage(bossDamage);
            respawn();
        }
        if (collision.collider.CompareTag("Enemy"))
        {
            TakeDamage(enemyDamage);
            respawn();
        }
    }

    // Collision Damage // // // //
    public void SetEnemyDamage(float enemyDamage) { this.enemyDamage = enemyDamage; }
    public void SetBossDamage(float bossDamage) { this.bossDamage = bossDamage; }
    public void SetEnemyProjectileDamage(float enemyProjectileDamage) { this.enemyProjectileDamage = enemyProjectileDamage; }
    public void SetBossProjectileDamage(float bossProjectileDamage) { this.bossProjectileDamage = bossProjectileDamage; }

    // Other functions // // // //
    public void respawn()
    {
        GameObject Boss = GameObject.FindWithTag("Boss");
        if (noHealth && Boss)
        {

            this.transform.position = new Vector2(117, -10);
            Destroy(Boss);
            
            ResetHealth();
            BossTrigger.hasSpawnedBoss = false;

        }
        else if (noHealth && !Boss)
        {
            this.transform.position = new Vector2(22, -1);

            ResetHealth();
        }
    }
}
