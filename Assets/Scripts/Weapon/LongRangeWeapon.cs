using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeapon : Weapon
{
    [Header("Long Range Details")]
    [SerializeField] private ProjectileWeapon projectile;
    [SerializeField] private Transform launchOffLocation; //where the projectile will appear

    [Header("Projectile Details")]
    [SerializeField] private float speed = 1f;
    //[SerializeField] private bool infiniteAmount = false;
    //[SerializeField] private int amount = 10;
    //[SerializeField] private float reloadDelay = 1f;

    //private int maxAmount;
    //private float reloadTimer;

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

        wDetails.SetSpeed(speed);
        wDetails.SetDamage(GetDamage());

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
