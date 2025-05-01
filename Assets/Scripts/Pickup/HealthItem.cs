using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [Header("Health Item Details")]
    [SerializeField] private StatusUser itemUser; //identifies who can use the item
    [SerializeField] private float healthAmount;

    #region Unity
    void OnTriggerEnter2D(Collider2D collision)
    {
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
    #endregion

    #region Functions
    public void SetHealthAmount(float healthAmount) { this.healthAmount = healthAmount < 0 ? 0 : healthAmount; }
    #endregion
}
