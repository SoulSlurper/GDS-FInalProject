using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeapon : Weapon
{
    [Header("Long Range Details")]
    [SerializeField] private ProjectileWeapon projectile;
    [SerializeField] private Transform launchLocation; //where the projectile will appear
    public bool killsUser = false;

    [Header("Projectile Details")]
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _projectileCost = 0f;

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

    // Attack Details // // // // //
    public override void Attack()
    {
        bool damagesUser = true;
        if (!killsUser)
        {
            damagesUser = false;
        }

        if (damagesUser) weaponUser.TakeDamage(projectileCost);

        if (!weaponUser.noHealth)
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
