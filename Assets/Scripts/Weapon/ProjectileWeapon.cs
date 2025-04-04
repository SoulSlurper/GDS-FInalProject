using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [Header("Projectile Details")]
    [SerializeField] private bool _isLaunched = false;
    [SerializeField] private float _speed = 1f; //the speed the projectile is traveling

    // Getter and Setters // // // //
    public bool isLaunched
    {
        get { return _isLaunched; }
        private set { _isLaunched = value; }
    }

    public float speed
    {
        get { return _speed; }
        private set { _speed = value; }
    }

    // Unity // // // //
    void Update()
    {
        if (isLaunched) transform.position += transform.right * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger detects: " + collision.gameObject.name);

        Status status;
        if (status = collision.GetComponent<BossHp>())
        {
            Attack(collision);
        }
        else if (status = collision.GetComponent<EnemyStatus>())
        {
            Attack(collision);
        }
        else if (status = collision.GetComponent<BreakableWall>())
        {
            Attack(collision);
        }

        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Weapon"))
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Status status;
        if (status = collision.gameObject.GetComponent<BossHp>())
        {
            Attack(collision.collider);
        }
        else if (status = collision.gameObject.GetComponent<EnemyStatus>())
        {
            Attack(collision.collider);
        }
        else if (status = collision.gameObject.GetComponent<BreakableWall>())
        {
            Attack(collision.collider);
        }
        MakeDamage(collision);

        Destroy(gameObject);
    }

    // Projectile Details // // // // //
    public void SetSpeed(float speed) { this.speed = speed; }

    // Attack Details // // // // //
    public override void Attack()
    {
        isLaunched = true;
    }
}
