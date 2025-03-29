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
    //[SerializeField] private bool infiniteAmount = false;
    //[SerializeField] private int amount = 10;
    //[SerializeField] private float reloadDelay = 1f;

    //private int maxAmount;
    //private float reloadTimer;

    // Getter and Setters // // // //
    public float speed
    {
        get { return _speed; }
        private set { _speed = value; }
    }



    // Unity // // // // //
    void Update()
    {
        PerformAttack();
    }



    // Long Range Details // // // // //
    public void SetSpeed(float speed) { this.speed = speed; }



    // Attack Details // // // // //
    public override void Attack(Collider2D collider = null)
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

        wDetails.LaunchProjectile();
    }
}
