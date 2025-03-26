using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLongRange : Weapon
{
    [Header("Projectile Details")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform launchOffLocation; //where the projectile will appear
    [SerializeField] private float launchDelay; //the time taken for the projectile to appear
    [SerializeField] private float speed = 1f;

    private float timeDelay = 0f;
    private bool triggerAttack = false;

    void Start()
    {
        Projectile pComponent = projectile.GetComponent<Projectile>();
        pComponent.SetDamage(GetDamage());
        pComponent.SetProjectileSpeed(speed);

        ResetProjectileDelay();
    }

    void Update()
    {
        if (CanAttackEnemy(KeyCode.Return))
        {
            AttackEnemy();
        }
        else
        {
            ResetProjectileDelay();
        }
    }

    public override void AttackEnemy()
    {
        if (timeDelay > launchDelay)
        {
            SpawnProjectile();

            timeDelay = 0f;
        }
        else
        {
            timeDelay += Time.deltaTime;
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

    private bool CanAttackEnemy(KeyCode keycode)
    {
        if (Input.GetKeyDown(keycode)) 
        { 
            triggerAttack = true; 
        }
        else if (Input.GetKeyUp(keycode)) 
        { 
            triggerAttack = false; 
        }

        return triggerAttack;
    }

    private void ResetProjectileDelay()
    {
        timeDelay = launchDelay + 1f;
    }
}
