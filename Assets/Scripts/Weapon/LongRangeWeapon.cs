using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeapon : Weapon
{
    //Note: Does not set the damage or speed of the projectile

    [Header("Long Range Details")]
    [SerializeField] private ProjectileWeapon projectile;
    [SerializeField] private Transform launchOffLocation; //where the projectile will appear

    void Start()
    {
        SetAttackTimer(GetAttackDelay());
    }

    void Update()
    {
        if (CanAttack())
        {
            Attack();
        }
        else
        {
            SetAttackTimer(GetAttackDelay());
        }
    }

    private void Attack()
    {
        if (AttackTimerReachDelay())
        {
            SpawnProjectile();

            SetAttackTimer(0f);
        }
        else
        {
            IncreaseAttackTimer();
        }
    }

    private void SpawnProjectile()
    {
        GameObject projectileObject = Instantiate(projectile.gameObject, launchOffLocation.position, transform.rotation);
        ProjectileWeapon wDetails = projectileObject.GetComponent<ProjectileWeapon>();

        wDetails.LaunchProjectile();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayShootSound();
        }
        else
        {
            Debug.LogWarning("SoundManager instance is null! Shoot sound won't play.");
        }
    }
}
