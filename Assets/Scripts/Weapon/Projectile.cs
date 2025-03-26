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
        //this ensures the the projectile will always be destroyed upon contact

        Debug.Log("collision tag: " + collision.gameObject.tag);

        if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //this triggers the projectile to attack

        Debug.Log("trigger tag: " + collision.gameObject.tag);

        if (collision.CompareTag("Enemy"))
        {
            Attack(collision);
        }

        if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    private void Attack(Collider2D collision)
    {
        collision.GetComponent<Status>().DecreaseHealth(GetDamage());
    }

    public float GetProjectileSpeed() { return speed; }

    public void SetProjectileSpeed(float speed) { this.speed = speed; }
}
