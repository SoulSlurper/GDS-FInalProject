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

    [Header("Size by Health")]
    [SerializeField] [Range(0, 1f)] private float minSize = 1f;

    private SavePoint savePoint;
    private Vector2 initialPosition;
    private Vector3 initialSize;

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
    void Start()
    {
        initialPosition = transform.position;
        initialSize = transform.localScale;
    }

    void Update()
    {
        Respawn();
        ChangeSize();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SavePoint"))
        {
            SavePoint encounteredSavePoint = collision.gameObject.GetComponent<SavePoint>();
            if (!encounteredSavePoint.Equals(savePoint))
            {
                if (savePoint) savePoint.isActive = false;

                savePoint = encounteredSavePoint;
                savePoint.isActive = true;
            }

            Debug.Log("SavePoint Recorded: " + savePoint.position);
        }

        if (collision.CompareTag("BossProjectile"))
        {
            TakeDamage(bossProjectileDamage);
        }

        if (collision.CompareTag("Lava"))
        {
            // Instantly kill the player when touching lava
            TakeDamage(lavaDamage);
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
    private void Respawn()
    {
        if (!noHealth) return;

        GameObject Boss = GameObject.FindWithTag("Boss");
        if (Boss)
        {
            Destroy(Boss);
            
            BossTrigger.hasSpawnedBoss = false;
        }

        if (savePoint) this.transform.position = savePoint.position;
        else this.transform.position = initialPosition;
        
        ResetHealth();
    }

    private void ChangeSize()
    {
        Debug.Log(gameObject.name + " size updated");

        Vector3 newSize = initialSize;
        if (currentHealth < maxHealth)
        {
            Vector3 actualMinSize = initialSize * minSize;
            Vector3 remainingSize = initialSize - actualMinSize;

            newSize = actualMinSize + remainingSize * currentHealthPercentage;
        }

        transform.localScale = newSize;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        ChangeSize();
    }

    public override void TakeHealth(float amount)
    {
        base.TakeHealth(amount);

        ChangeSize();
    }
}
