using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private float weaponDamage;
    [SerializeField] private float weaponHealthCost;

    public virtual void AttackEnemy()
    {
        Debug.Log("Attack Enemy");
    }

    public virtual void AttackEnemy(Collider2D collision)
    {
        Debug.Log("Attack Enemy");
    }
}
