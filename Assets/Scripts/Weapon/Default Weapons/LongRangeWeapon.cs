using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeapon : Weapon
{
    [SerializeField][Range(0f, 1f)] private float _minProjectileCost = 1f;

    [Header("Long Range Details")]
    [SerializeField] private ProjectileWeapon projectile;
    [SerializeField] private Transform launchLocation; //where the projectile will appear
    [SerializeField] public Animator animator;

    [Header("Projectile Details")]
    [SerializeField] private float _launchForce = 1f;
    [SerializeField] private bool _usesGravity = false;
    [SerializeField] private float _projectileCost = 0f;
    [SerializeField][Range(0f, 1f)] private float _stopGapHealth = 0f; //stops making projectiles when the current health reaches at a certain point

    private float _realProjectileCost;

    // Getter and Setters // // // //
    public float launchForce
    {
        get { return _launchForce; }
        private set { _launchForce = value; }
    }

    public bool usesGravity
    {
        get { return _usesGravity; }
        private set { _usesGravity = value; }
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
    public void IncreaseLaunchForce(float amount) { launchForce += amount; }

    public void DecreaseLaunchForce(float amount) 
    { 
        launchForce -= amount; 
        if (launchForce < 0f) launchForce = 0f;
    }

    public void SetLaunchForce(float launchForce) 
    { 
        if (launchForce < 0f) this.launchForce = 0f;
        else this.launchForce = launchForce;
    }

    public void SetUseGravity(bool usesGravity) 
    {
        this.usesGravity = usesGravity;
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

            animator.SetTrigger("Fire");

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

        wDetails.SetWeaponUser(weaponUser);
        wDetails.SetUsesGravity(usesGravity);
        wDetails.SetLaunchForce(launchForce);
        
        // Use realDamage instead of damage to apply aiming bonus to projectiles
        wDetails.SetDamage(realDamage);

        wDetails.Attack();
    }
}