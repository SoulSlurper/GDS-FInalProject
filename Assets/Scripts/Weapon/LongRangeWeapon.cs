using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeapon : Weapon
{
    [Header("Long Range Details")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform launchOffLocation; //where the projectile will appear

    void Start()
    {
        SetAttackTimer(GetAttackDelay());
    }

    void Update()
    {
        WeaponFaceMouse();

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
        Instantiate(projectile, launchOffLocation.position, transform.rotation);
    }
}
