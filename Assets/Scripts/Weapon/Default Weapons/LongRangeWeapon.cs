using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeapon : Weapon
{
    [SerializeField][Range(0f, 1f)] private float _minProjectileCost = 1f;

    [Header("Long Range Details")]
    [SerializeField] private ProjectileWeapon projectile;
    [SerializeField] private Transform launchLocation; //where the projectile will appear

    [Header("Projectile Details")]
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _projectileCost = 0f;
    [SerializeField][Range(0f, 1f)] private float _stopGapHealth = 0f; //stops making projectiles when the current health reaches at a certain point

    private float _realProjectileCost;

    // Getter and Setters // // // //
    public float speed
    {
        get { return _speed; }
        private set { _speed = value; }
    }

    public float projectileCost
    {
        get { return _projectileCost; }
        private set { _projectileCost = value; }
    }

    public float minProjectileCost
    {
        get { return _minProjectileCost; }
        private set { _minProjectileCost = value; }
    }

    public float realProjectileCost
    {
        get { return _realProjectileCost; }
        private set { _realProjectileCost = value; }
    }

    public float stopGapHealth
    {
        get { return _stopGapHealth; }
        private set { _stopGapHealth = value; }
    }

    // Unity // // // // //
    void Update()
    {
        PerformAttack();
    }

    // Long Range Details // // // // //
    public void IncreaseSpeed(float amount) { speed += amount; }

    public void DecreaseSpeed(float amount) 
    { 
        speed -= amount; 
        if (speed < 0f) speed = 0f;
    }

    public void SetSpeed(float speed) 
    { 
        if (speed < 0f) this.speed = 0f;
        else this.speed = speed;
    }

    public void IncreaseProjectileCost(float amount) { projectileCost += amount; }
    
    public void DecreaseProjectileCost(float amount) 
    { 
        projectileCost -= amount;
        if (projectileCost < 0f) projectileCost = 0f;
    }

    public void SetProjectileCost(float projectileCost) 
    { 
        if (projectileCost < 0f) this.projectileCost = 0f;
        else this.projectileCost = projectileCost;
    }

    public void SetStopGapHealth(float stopGapHealth) 
    { 
        if (stopGapHealth >= 0f && stopGapHealth <= 1f) this.stopGapHealth = stopGapHealth;
        else if (stopGapHealth > 1f) this.stopGapHealth = 1f;
        else this.stopGapHealth = 0f;
    }

    public override void SetRealAmounts()
    {
        base.SetRealAmounts();

        realProjectileCost = GetRealAmount(projectileCost, minProjectileCost);
        //Debug.Log("realProjectileCost: " + realProjectileCost);
    }

    // Attack Details // // // // //
    public override void Attack()
    {
        //whether the weapon can be used or not
        bool useWeapon = weaponUser.currentHealthPercentage > stopGapHealth || projectileCost == 0f;

        if (useWeapon && projectileCost > 0f) weaponUser.TakeDamage(realProjectileCost);

        if (!weaponUser.noHealth && useWeapon)
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
    }

    private void SpawnProjectile()
    {
        GameObject projectileObject = Instantiate(projectile.gameObject, launchLocation.position, transform.rotation);
        ProjectileWeapon wDetails = projectileObject.GetComponent<ProjectileWeapon>();

        wDetails.SetSpeed(speed);
        wDetails.SetDamage(damage);
        wDetails.SetWeaponUser(weaponUser);

        wDetails.Attack();
    }
}
