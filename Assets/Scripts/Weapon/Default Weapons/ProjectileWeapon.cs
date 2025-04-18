using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    //maybe make the weapon more flexiable for enemy use

    [Header("Projectile Details")]
    [SerializeField] private bool _isLaunched = false;
    [SerializeField] private bool _usesGravity = false;
    [SerializeField] private float _launchForce = 1f; //the force that the projectile is traveling
    [SerializeField] private GameObject _dropItem;

    private Rigidbody2D rb;

    // Getter and Setters // // // //
    public bool isLaunched
    {
        get { return _isLaunched; }
        private set { _isLaunched = value; }
    }

    public bool usesGravity
    {
        get { return _usesGravity; }
        private set { _usesGravity = value; }
    }

    public float launchForce
    {
        get { return _launchForce; }
        private set { _launchForce = value; }
    }

    public GameObject dropItem
    {
        get { return _dropItem; }
        private set { _dropItem = value; }
    }

    // Unity // // // //
    void Update()
    {
        if (isLaunched && !usesGravity) transform.position += transform.right * launchForce * Time.deltaTime;
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

        if (canDestroy)
        {
            CreateDropItem();

            Destroy(gameObject);
        }
    }

    // Projectile Details // // // // //
    public void SetUsesGravity(bool usesGravity)
    {
        this.usesGravity = usesGravity;
    }

    public void IncreaseLaunchForce(float amount) { launchForce += amount; }

    public void DecreaseLaunchForce(float amount)
    {
        launchForce += amount;
        if (launchForce < 0f) launchForce = 0f;
    }

    public void SetLaunchForce(float launchForce)
    {
        if (launchForce < 0f) this.launchForce = 0f;
        else this.launchForce = launchForce;
    }

    public void SetDropItem(GameObject dropItem) { this.dropItem = dropItem; }

    private void CreateDropItem()
    {
        if (dropItem == null) return;

        Instantiate(dropItem, transform.position, Quaternion.identity);
    }

    // Attack Details // // // // //
    public override void Attack()
    {
        isLaunched = true;

        if (usesGravity)
        {
            if (!rb) rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 1f;
            rb.velocity = transform.right * launchForce;
        }
    }
}
