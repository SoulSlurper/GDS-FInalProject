using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeapon : Weapon
{
    [Header("Projectile Details")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform launchOffLocation; //where the projectile will appear
    [SerializeField] private float launchDelay; //the time taken for the projectile to appear
    [SerializeField] private float speed = 1f;

    private float timerDelay = 0f;

    void Start()
    {
        Projectile pComponent = projectile.GetComponent<Projectile>();
        pComponent.SetDamage(GetDamage());
        pComponent.SetProjectileSpeed(speed);

        ResetProjectileDelay();
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
            ResetProjectileDelay();
        }
    }

    private void Attack()
    {
        if (timerDelay > launchDelay)
        {
            SpawnProjectile();

            timerDelay = 0f;
        }
        else
        {
            timerDelay += Time.deltaTime;
        }
    }

    private void SpawnProjectile()
    {
        //Note: the newProjectile gameobject is a sample in place of the actual bullet
        GameObject newProjectile = Instantiate(projectile, launchOffLocation.position, transform.rotation);
        newProjectile.SetActive(true);

        Projectile newPComponent = newProjectile.GetComponent<Projectile>();
        Debug.Log("Projectile Damage: " + newPComponent.GetDamage());
        Debug.Log("Projectile Speed: " + newPComponent.GetProjectileSpeed());
    }

    private void ResetProjectileDelay()
    {
        timerDelay = launchDelay + 1f;
    }
}
