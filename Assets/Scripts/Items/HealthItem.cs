using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [SerializeField] private StatusUser itemUser; //identifies who can use the item
    [SerializeField] private float healthAmount;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Status status;
        if (status = collision.collider.GetComponent<Status>())
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
