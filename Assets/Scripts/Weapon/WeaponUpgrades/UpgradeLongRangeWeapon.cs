using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeLongRangeWeapon : WeaponUpgrade
{
    [Header("Projectile Details")]
    [SerializeField] private float increaseSpeedBy;
    [SerializeField] private float improveProjectileCostBy;

    // Unity // // // //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!onGround) SetOnGround(CheckForGround());

        if (!collision.CompareTag("Player")) return;

        UpgradeWeapon(GetWeapon(collision.gameObject));

        Destroy(gameObject);
    }

    // Functions // // // //
    private LongRangeWeapon GetWeapon(GameObject player)
    {
        foreach (Transform weapon in player.transform.Find("WeaponHolder"))
        {
            LongRangeWeapon wDetails;
            if (wDetails = weapon.GetComponent<LongRangeWeapon>())
            {
                if (wDetails.type == type) return wDetails;
            }
        }

        return null;
    }

    private void UpgradeWeapon(LongRangeWeapon weapon)
    {
        Debug.Log("Upgrade " + weapon.gameObject.name);

        weapon.IncreaseDamage(increaseDamageBy);
        weapon.DecreaseCost(improveCostsBy);
        weapon.IncreaseSpeed(increaseSpeedBy);
        weapon.DecreaseProjectileCost(improveProjectileCostBy);
    }
}
