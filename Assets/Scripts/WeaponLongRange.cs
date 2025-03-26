using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLongRange : Weapon
{
    [Header("Projectile Details")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform launchOffLocation;
    [SerializeField] private float speed = 1f;

    void Start()
    {
        Projectile pComponent = projectile.GetComponent<Projectile>();
        pComponent.SetDamage(GetDamage());
        pComponent.SetProjectileSpeed(speed);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AttackEnemy();
        }
    }

    public override void AttackEnemy()
    {
        //Note: the newProjectile gameobject is a sample in place of the actual bullet
        GameObject newProjectile = Instantiate(projectile, launchOffLocation.position, transform.rotation);
        newProjectile.SetActive(true);

        Projectile newPComponent = newProjectile.GetComponent<Projectile>();
        Debug.Log("Projectile Damage: " + newPComponent.GetDamage());
        Debug.Log("Projectile Speed: " + newPComponent.GetProjectileSpeed());
    }
}
