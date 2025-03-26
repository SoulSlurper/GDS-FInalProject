using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : Weapon
{
    //there's no need to set the damamge for the projectile if it is used by a long-range weapon

    private float speed = 1f; //the speed the projectile is traveling

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Attack(collision);
        }

        if (!collision.gameObject.CompareTag("Weapon")) //needs to have a Weapon tag so that it doesn't get destroyed when two projectiles make contact
        {
            Destroy(gameObject);
        }
    }

    private void Attack(Collision2D collision)
    {
        collision.gameObject.GetComponent<Status>().DecreaseHealth(GetDamage());
    }

    public float GetProjectileSpeed() { return speed; }

    public void SetProjectileSpeed(float speed) { this.speed = speed; }
}
