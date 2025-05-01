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

    #region Getter and Setters
    public bool onGround
    {
        get { return _onGround; }
        private set { _onGround = value; }
    }
    #endregion

    #region Unity
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialConstraints = rb.constraints;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //ignores collisions from any objects with specific tags
        Collider2D collider = collision.collider;
        if (DetectInvalidObjects(collider))
        {
            Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>());
            return;
        }

        if (!onGround)
        {
            onGround = CheckForBottom();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine((Vector2)raycastOrigin.position, (Vector2)raycastOrigin.position + Vector2.down * checkGroundDistance);
    }
    #endregion

    #region Detection functions
    private bool DetectInvalidObjects(Collider2D collider)
    {
        return collider.CompareTag("Enemy") || collider.CompareTag("Boss") || collider.CompareTag("SavePoint") || collider.CompareTag("Item");
    }

    public bool CheckForBottom()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)raycastOrigin.position, Vector2.down, checkGroundDistance);
        if (hit) return !DetectInvalidObjects(hit.collider);

        return false;
    }
    #endregion
}
