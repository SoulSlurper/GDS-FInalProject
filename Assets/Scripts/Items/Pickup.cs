using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool _onGround;
    private float checkGroundDistance = 0.1f;

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
                
                onGround = true;
                return true;
            }
        }

        onGround = false;
        return false;
    }

    public void SetOnGround(bool onGround) { this.onGround = onGround; }
}
