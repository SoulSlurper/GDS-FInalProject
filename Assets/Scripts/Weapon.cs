using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Details")]
    [SerializeField] private WeaponType type;
    [SerializeField] private float damage;
    [SerializeField] private float healthCost;

    public WeaponType GetWeaponType() { return type; }

    public float GetDamage() { return damage; }

    public void SetDamage(float damage) { this.damage = damage; }

    public float GetHealthCost() { return healthCost; }

    public void SetHealthCost(float healthCost) { this.healthCost = healthCost; }

    public virtual void AttackEnemy()
    {
        Debug.Log("Attack Enemy");
    }

    public virtual void AttackEnemy(Collider2D collision)
    {
        Debug.Log("Attack Enemy");
    }
}
