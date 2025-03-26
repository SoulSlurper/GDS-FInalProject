using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeWeapon : Weapon
{
    [Header("Close Range Details")]
    [SerializeField] private Transform raycastCircleOrigin;
    [SerializeField] private float raycastCircleRadius;

    void Start()
    {
        
    }

    void Update()
    {
        WeaponFaceMouse();

        if (CanAttack())
        {
            Attack();
        }
    }

    void OnDrawGizmosSelected()
    {
        //shows the raycast range when the gizmos is selected in the scene

        Gizmos.color = Color.blue;
        Vector3 position = raycastCircleOrigin == null ? Vector3.zero : raycastCircleOrigin.position;
        Gizmos.DrawWireSphere(position, raycastCircleRadius);
    }

    private void Attack()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(raycastCircleOrigin.position, raycastCircleRadius))
        {
            Debug.Log(collider.name + " - " + collider.tag);

            Status status;
            if (status = collider.GetComponent<Status>())
            {
                status.DecreaseHealth(GetDamage());
            }
        }
    }
}
