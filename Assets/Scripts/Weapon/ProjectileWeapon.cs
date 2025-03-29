using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [Header("Projectile Details")]
    [SerializeField] private bool canLaunch = true;
    [SerializeField] private float speed = 1f; //the speed the projectile is traveling

    void Update()
    {
        if (canLaunch) transform.position += transform.right * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger tag: " + collision.gameObject.tag);

        if (collision.CompareTag("Enemy"))
        {
            Attack(collision);
        }

        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Weapon"))
        {
            Destroy(gameObject);
        }
    }

    private void Attack(Collider2D collision)
    {
        collision.GetComponent<Status>().health.DecreaseAmount(GetDamage());
    }

    public float GetProjectileSpeed() { return speed; }

    public void SetProjectileSpeed(float speed) { this.speed = speed; }

    public void LaunchProjectile() { canLaunch = true; }
}
