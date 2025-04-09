using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Status
{
    //may change where enemies could have weapons
    [Header("Collision Damage")]
    [SerializeField] private float _enemyDamage;
    [SerializeField] private float _bossDamage;
    [SerializeField] private float _lavaDamage;
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

    public float lavaDamage
    {
        get { return _lavaDamage; }
        private set { _lavaDamage = value; }
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
    void Update()
    {
        Respawn();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BossProjectile"))
        {
            TakeDamage(bossProjectileDamage);
            Respawn();
        }

        if (collision.CompareTag("Lava"))
        {
            // Instantly kill the player when touching lava
            TakeDamage(lavaDamage);
            Respawn();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Boss"))
        {
            TakeDamage(bossDamage);
        }

        if (collision.collider.CompareTag("Enemy"))
        {
            TakeDamage(enemyDamage);
        }
    }

    // Collision Damage // // // //
    public void SetEnemyDamage(float enemyDamage) { this.enemyDamage = enemyDamage; }
    public void SetBossDamage(float bossDamage) { this.bossDamage = bossDamage; }
    public void SetEnemyProjectileDamage(float enemyProjectileDamage) { this.enemyProjectileDamage = enemyProjectileDamage; }
    public void SetBossProjectileDamage(float bossProjectileDamage) { this.bossProjectileDamage = bossProjectileDamage; }

    // Other functions // // // //
    public void Respawn()
    {
        if (!noHealth) return;

        GameObject Boss = GameObject.FindWithTag("Boss");
        if (Boss)
        {

            this.transform.position = new Vector2(117, -10);
            Destroy(Boss);
            
            BossTrigger.hasSpawnedBoss = false;
        }
        else
        {
            this.transform.position = new Vector2(22, -1);
        }

        ResetHealth();
    }
}
