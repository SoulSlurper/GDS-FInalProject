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
        //Debug.Log("trigger detects: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Weapon")) return;

        MakeDamage(collision);

        bool canDestroy = true; //prevents weapon from destroying by the weaponUser
        Status sDetails;
        if (sDetails = collision.GetComponent<Status>())
        {
            if (sDetails.user == weaponUser.user) canDestroy = false;
        }

        if (canDestroy) Destroy(gameObject);
    }

    // Projectile Details // // // // //
    public void IncreaseSpeed(float amount) { speed += amount; }

    public void DecreaseSpeed(float amount)
    {
        speed += amount;
        if (speed < 0f) speed = 0f;
    }

    public void SetSpeed(float speed)
    {
        if (speed < 0f) this.speed = 0f;
        else this.speed = speed;
    }

    // Attack Details // // // // //
    public override void Attack()
    {
        isLaunched = true;
    }
}
