using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgrade : MonoBehaviour
{
    [SerializeField] private WeaponType _type;
    [SerializeField] private float _increaseDamageBy;
    [SerializeField] private float _improveCostsBy;

    [Header("For Object")]
    [SerializeField] private float checkGroundDistance = 0.1f;

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector2 raycastOrigin = transform.position - new Vector3(0, transform.localScale.y + 0.1f) / 2;
        Gizmos.DrawLine(raycastOrigin, raycastOrigin + Vector2.down * checkGroundDistance);
    }

    // Functions // // // //
    public bool CheckForGround()
    {
        Vector2 raycastOrigin = transform.position - new Vector3(0, transform.localScale.y + 0.1f) / 2;
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, checkGroundDistance);
        if (hit)
        {
            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Untagged"))
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;

                return true;
            }
        }

        return false;
    }

    public void SetOnGround(bool onGround) { this.onGround = onGround; }
}
