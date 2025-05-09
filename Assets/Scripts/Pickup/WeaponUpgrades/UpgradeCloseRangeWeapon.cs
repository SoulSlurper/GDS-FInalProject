using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCloseRangeWeapon : WeaponUpgrade
{
    // Unity // // // //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        UpgradeWeapon(GetWeapon(collision.gameObject));

        ResetCameraSize();

        Destroy(gameObject);
    }

    // Functions // // // //
    private CloseRangeWeapon GetWeapon(GameObject player)
    {
        foreach (Transform weapon in player.transform.Find("WeaponHolder"))
        {
            CloseRangeWeapon wDetails;
            if (wDetails = weapon.GetComponent<CloseRangeWeapon>())
            {
                if (wDetails.type == type) return wDetails;
            }
        }

        return null;
    }

    private void UpgradeWeapon(CloseRangeWeapon weapon)
    {
        if (weapon == null) return;

        Debug.Log("Upgrade " + weapon.gameObject.name);

        weapon.IncreaseDamage(increaseDamageBy);
        weapon.DecreaseCost(improveCostsBy);
    }
}
