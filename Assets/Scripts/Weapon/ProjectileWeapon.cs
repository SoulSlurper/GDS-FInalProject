using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [Header("Projectile Details")]
    [SerializeField] private bool isLaunched = false;
    [SerializeField] private float _speed = 1f; //the speed the projectile is traveling

    // Getter and Setters // // // //
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
        Debug.Log("trigger tag: " + collision.gameObject.tag);

        Status status;
        if (status = collision.GetComponent<Status>()) { Attack(collision); }
        else if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            Attack(collision);
        }

        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Weapon"))
        {
            Destroy(gameObject);
        }
    }



    // Projectile Details // // // // //
    public void SetSpeed(float speed) { this.speed = speed; }

    public void LaunchProjectile() { isLaunched = true; }



    // Attack Details // // // // //
    public override void Attack(Collider2D collider = null)
    {
        collider.GetComponent<Status>().health.DecreaseAmount(damage);
    }
}
