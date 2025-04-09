using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCloseRangeWeapon : WeaponUpgrade
{
    // Unity // // // //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!onGround) SetOnGround(CheckForGround());

        if (!collision.CompareTag("Player")) return;

        CloseRangeWeapon weapon = GetWeapon(collision.gameObject);
        UpgradeWeapon(weapon);
        Debug.Log(weapon);

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
        Debug.Log("Upgrade " + weapon.gameObject.name);

        weapon.IncreaseDamage(increaseDamageBy);
        weapon.DecreaseCost(improveCostsBy);
    }
}
