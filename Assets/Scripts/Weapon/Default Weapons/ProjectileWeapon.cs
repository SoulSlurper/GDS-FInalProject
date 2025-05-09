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

    #region Getter and Setters
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
    #endregion

    #region Unity
    void Update()
    {
        if (isLaunched && !usesGravity) transform.position += transform.right * launchForce * Time.deltaTime;

        if (usesGravity)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger detects: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Weapon") || collision.CompareTag("Item")) return;

        MakeDamage(collision);

        Status status = collision.GetComponent<Status>();
        if (status != null && weaponUser != null && status.user == weaponUser.user) return;

        CreateDropItem();

        Destroy(gameObject);
    }
    #endregion

    #region Projectile Details
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
    #endregion

    #region Attack Details
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
    #endregion
}
