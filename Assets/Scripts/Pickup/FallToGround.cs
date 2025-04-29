using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallToGround : MonoBehaviour
{
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float checkGroundDistance = 0.1f;

    private Rigidbody2D rb;
    private bool _onGround = false;

    // Getter and Setters // // // //
    public bool onGround
    {
        get { return _onGround; }
        private set { _onGround = value; }
    }

    // Unity // // // //
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("SavePoint") || collision.CompareTag("Item")) return;

        if (!onGround) 
        {
            RigidbodyConstraints2D initialConstraints = rb.constraints;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            if (CheckForGround())
            {
                onGround = true;
            }
            else
            {
                rb.constraints = initialConstraints;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine((Vector2)raycastOrigin.position, (Vector2)raycastOrigin.position + Vector2.down * checkGroundDistance);
    }

    // Functions // // // //
    public bool CheckForGround()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll((Vector2)raycastOrigin.position, Vector2.down, checkGroundDistance);

        return hit.Length > 1;
    }
}
