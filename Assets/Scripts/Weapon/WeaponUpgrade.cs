using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgrade : MonoBehaviour
{
    [SerializeField] private WeaponType type;
    [SerializeField] private float _increaseDamageBy;
    [SerializeField] private float _improveCostsBy;

    private Rigidbody2D rb;
    private bool onGround;

    // Getter and Setters // // // //
    public float increaseDamageBy
    {
        get { return _increaseDamageBy; }
        private set { _increaseDamageBy = value; }
    }

    public float improveCostsBy
    {
        get { return _improveCostsBy; }
        private set { _improveCostsBy = value; }
    }


    // Unity // // // //
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        onGround = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!onGround) onGround = CheckForGround();

        if (!collision.CompareTag("Player")) return;

        Weapon weapon = GetWeapon(collision.gameObject);
        UpgradeWeapon(weapon);
        Debug.Log(weapon);

        Destroy(gameObject);
    }

    private bool CheckForGround()
    {
        float distance = transform.localScale.y + 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance);
        if (hit)
        {
            if (hit.collider.CompareTag("Wall"))
            {
                rb.gravityScale = 0f;

                return true;
            }
        }

        return false;
    }

    private Weapon GetWeapon(GameObject player)
    {
        foreach (Transform weapon in player.transform.Find("WeaponHolder"))
        {
            Weapon wDetails;
            if (wDetails = weapon.GetComponent<Weapon>())
            {
                if (wDetails.type == type) return wDetails;
            }
        }

        return null;
    }

    public virtual void UpgradeWeapon(Weapon weapon)
    {
        weapon.IncreaseDamage(increaseDamageBy);
        weapon.DecreaseCost(improveCostsBy);
    }
}
