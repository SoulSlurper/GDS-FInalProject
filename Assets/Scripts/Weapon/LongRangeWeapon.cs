using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeapon : Weapon
{
    [Header("Long Range Details")]
    [SerializeField] private ProjectileWeapon projectile;
    [SerializeField] private Transform launchLocation; //where the projectile will appear

    [Header("Projectile Details")]
    [SerializeField] private float _speed = 1f;

    [Header("Ammunition Details")]
    public bool infiniteAmmo = true;
    [SerializeField] private int _ammo = 10;
    [SerializeField] private int _reloadAmmo = 1;
    [SerializeField] private float _reloadDelay = 1f;

    private int _maxAmmo;
    private float _reloadTimer;

    // Getter and Setters // // // //
    public float speed
    {
        get { return _speed; }
        private set { _speed = value; }
    }

    public int ammo
    {
        get { return _ammo; }
        private set { _ammo = value; }
    }

    public int maxAmmo
    {
        get { return _maxAmmo; }
        private set { _maxAmmo = value; }
    }

    public int reloadAmmo
    {
        get { return _reloadAmmo; }
        private set { _reloadAmmo = value; }
    }

    public float reloadDelay
    {
        get { return _reloadDelay; }
        private set { _reloadDelay = value; }
    }

    public float reloadTimer
    {
        get { return _reloadTimer; }
        private set { _reloadTimer = value; }
    }



    // Unity // // // // //
    void Start()
    {
        maxAmmo = ammo;
    }

    void Update()
    {
        if (infiniteAmmo) PerformAttack();
        else
        {
            Debug.Log("ammo > 0: " + (ammo > 0));
            if (ammo > 0)
            {
                if (PerformAttack()) ammo--;

                if (ammo <= 0) SetIsAttacking(false);
            }
            else
            {
                InitiateAttackTimer();
            }

            RegainAmmo();
        }
    }



    // Long Range Details // // // // //
    public void SetSpeed(float speed) { this.speed = speed; }



    // Projectile Details // // // // //
    public void SetNewAmmo(int ammo) { this.ammo = maxAmmo = ammo; }

    public void SetReloadDelayDelay(float reloadDelay) { this.reloadDelay = reloadDelay; }

    public void SetReloadTimer(float reloadTimer) { this.reloadTimer = reloadTimer; }

    public void InitiateReloadTimer() { reloadTimer = 0f; }

    private void RegainAmmo()
    {
        if (ammo < maxAmmo)
        {
            Debug.Log("regaining");
            if (reloadTimer > reloadDelay)
            {
                ammo += reloadAmmo;
                InitiateReloadTimer();
            }

            reloadTimer += Time.deltaTime;
        }
        else InitiateReloadTimer();
    }


    // Attack Details // // // // //
    public override void Attack()
    {
        SpawnProjectile();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayShootSound();
        }
        else
        {
            Debug.LogWarning("SoundManager instance is null! Shoot sound won't play.");
        }
    }

    private void SpawnProjectile()
    {
        GameObject projectileObject = Instantiate(projectile.gameObject, launchLocation.position, transform.rotation);
        ProjectileWeapon wDetails = projectileObject.GetComponent<ProjectileWeapon>();

        wDetails.SetSpeed(speed);
        wDetails.SetDamage(damage);

        wDetails.Attack();
    }
}
