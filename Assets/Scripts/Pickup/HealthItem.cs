using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : PickupItem
{
    [Header("Health Item Details")]
    [SerializeField] private StatusUser itemUser; //identifies who can use the item
    [SerializeField] private float healthAmount;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!onGround) CheckForGround();

        Status status;
        if (status = collision.GetComponent<Status>())
        {
            if (status.user == itemUser)
            {
                Debug.Log(collision.gameObject.name + " Health increased");

                status.TakeHealth(healthAmount);
                Destroy(gameObject);
            }
        }
    }
}
