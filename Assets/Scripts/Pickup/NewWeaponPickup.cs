using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWeaponPickup : MonoBehaviour
{
    [Header("New Weapon Details")]
    [SerializeField] private StatusUser weaponUser; //identifies who can use the item
    [SerializeField] private int newWeaponAmount;

    void OnTriggerEnter2D(Collider2D collision)
    {
        Status status;
        if (status = collision.GetComponent<Status>())
        {
            if (status.user == weaponUser)
            {
                Debug.Log(collision.gameObject.name + " gains " + newWeaponAmount + " weapons");

                WeaponAtHand weaponAtHand = collision.transform.Find("WeaponHolder").gameObject.GetComponent<WeaponAtHand>();
                weaponAtHand.IncreaseAvailableWeaponLimit(newWeaponAmount);

                Destroy(gameObject);
            }
        }
    }
}
