using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallToGround : MonoBehaviour
{
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float checkGroundDistance = 0.1f;

    private Rigidbody2D rb;
    private RigidbodyConstraints2D initialConstraints;
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
        initialConstraints = rb.constraints;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //ignores collisions from any objects with specific tags
        Collider2D collider = collision.collider;
        if (collider.CompareTag("Enemy") || collider.CompareTag("SavePoint") || collider.CompareTag("Item"))
        {
            Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>());
            return;
        }

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        if (!onGround)
        {
            onGround = CheckForBottom();

            if (!onGround) rb.constraints = initialConstraints;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine((Vector2)raycastOrigin.position, (Vector2)raycastOrigin.position + Vector2.down * checkGroundDistance);
    }

    // Functions // // // //
    public bool CheckForBottom()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll((Vector2)raycastOrigin.position, Vector2.down, checkGroundDistance);

        return hit.Length > 1;
    }
}
