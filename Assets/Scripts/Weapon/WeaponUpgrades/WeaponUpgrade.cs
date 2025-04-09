using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgrade : MonoBehaviour
{
    [SerializeField] private WeaponType _type;
    [SerializeField] private float _increaseDamageBy;
    [SerializeField] private float _improveCostsBy;

    private Rigidbody2D rb;
    private bool _onGround;

    // Getter and Setters // // // //
    public WeaponType type
    {
        get { return _type; }
        private set { _type = value; }
    }

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

    public bool onGround
    {
        get { return _onGround; }
        private set { _onGround = value; }
    }


    // Unity // // // //
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        onGround = false;
    }

    // Functions // // // //
    public bool CheckForGround()
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

    public void SetOnGround(bool onGround) { this.onGround = onGround; }
}
