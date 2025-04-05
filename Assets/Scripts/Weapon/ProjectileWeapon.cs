using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    //maybe make the weapon more flexiable for enemy use

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

        if (collision.gameObject.CompareTag("Weapon")) return;

        MakeDamage(collision);

        bool canDestroy = true;
        Status sDetails;
        if (sDetails = collision.GetComponent<Status>())
        {
            if (sDetails.user == weaponUser.user) canDestroy = false; //prevents weapon from destroying by the weaponUser
        }

        if (canDestroy) Destroy(gameObject);
    }

    // Projectile Details // // // // //
    public void SetSpeed(float speed) { this.speed = speed; }

    // Attack Details // // // // //
    public override void Attack()
    {
        isLaunched = true;
    }
}
