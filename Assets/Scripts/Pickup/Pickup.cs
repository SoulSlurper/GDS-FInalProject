using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float checkGroundDistance = 0.1f;

    private Rigidbody2D rb;
    private bool _onGround;

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
        onGround = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine((Vector2)raycastOrigin.position, (Vector2)raycastOrigin.position + Vector2.down * checkGroundDistance);
    }

    // Functions // // // //
    public bool CheckForGround()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)raycastOrigin.position, Vector2.down, checkGroundDistance);
        if (hit)
        {
            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Untagged"))
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                
                onGround = true;
                return true;
            }
        }

        onGround = false;
        return false;
    }

    public void SetOnGround(bool onGround) { this.onGround = onGround; }
}
