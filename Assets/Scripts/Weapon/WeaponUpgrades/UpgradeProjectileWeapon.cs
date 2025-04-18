using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeProjectileWeapon : WeaponUpgrade
{
    [Header("Projectile Details")]
    [SerializeField] private float increaseSpeedBy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!onGround) SetOnGround(CheckForGround());

        if (!collision.CompareTag("Player")) return;

        UpgradeWeapon(GetWeapon(collision.gameObject));

        Destroy(gameObject);
    }


    private ProjectileWeapon GetWeapon(GameObject player)
    {
        foreach (Transform weapon in player.transform.Find("WeaponHolder"))
        {
            ProjectileWeapon wDetails;
            if (wDetails = weapon.GetComponent<ProjectileWeapon>())
            {
                if (wDetails.type == type) return wDetails;
            }
        }

        return null;
    }

    private void UpgradeWeapon(ProjectileWeapon weapon)
    {
        Debug.Log("Upgrade " + weapon.gameObject.name);

        weapon.IncreaseDamage(increaseDamageBy);
        weapon.DecreaseCost(improveCostsBy);
        weapon.IncreaseLaunchForce(increaseSpeedBy);
    }
}
