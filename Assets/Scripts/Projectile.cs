using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : Weapon
{
    private float speed = 1f;

    void Update()
    {
        transform.position += -transform.right * speed * Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    public float GetProjectileSpeed() { return speed; }

    public void SetProjectileSpeed(float speed) { this.speed = speed; }
}
