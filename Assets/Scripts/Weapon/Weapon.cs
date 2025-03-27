using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Details")]
    [SerializeField] private WeaponType type;
    [SerializeField] private float damage;
    [SerializeField] private bool attackContinously = false;
    //[SerializeField] private float healthCost; //the amount of health taken when the player creates the weapon

    private bool isAttacking = false;

    public WeaponType GetWeaponType() { return type; }

    public float GetDamage() { return damage; }

    public void SetDamage(float damage) { this.damage = damage; }

    //public float GetHealthCost() { return healthCost; }

    //public void SetHealthCost(float healthCost) { this.healthCost = healthCost; }

    public bool IsAttacking() { return isAttacking; }

    public void WeaponFaceMouse()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.right = mousePosition - (Vector2)transform.position;
    }

    public bool CanAttack()
    {
        int mouseCode = 0; //for left mouse clicks

        if (Input.GetMouseButtonDown(mouseCode))
        {
            isAttacking = true;
        }
        else if (Input.GetMouseButtonUp(mouseCode))
        {
            isAttacking = false;
        }
        else if (isAttacking && !attackContinously)
        {
            isAttacking = false;
        }

        return isAttacking;
    }

}
