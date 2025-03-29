using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Details")]
    [SerializeField] private WeaponType type;
    [SerializeField] private float damage;
    [SerializeField] private float cost; //amount needed to get the weapon
    //[SerializeField] private Animator animator;

    [Header("Text Details")]
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text costText;

    [Header("Attack Details")]
    [SerializeField] private bool attackContinously = false;
    [SerializeField] private float attackDelay = 1f; //time taken for the attack to be performed

    private bool isAttacking = false;
    private float attackTimer = 0f;

    void Awake()
    {
        SetText();
    }

    // Weapon details functions // // // // //
    public WeaponType GetWeaponType() { return type; }

    public float GetDamage() { return damage; }

    public void SetDamage(float damage) { this.damage = damage; }

    public float GetCost() { return cost; }

    public void SetCost(float cost) { this.cost = cost; }

    //public Animator GetAnimator() { return animator; }

    // Text functions // // // // //
    public void SetText()
    {
        typeText.text = type.ToString();
        costText.text = cost.ToString();
    }

    // Attack time functions // // // // //
    public float GetAttackDelay() { return attackDelay; }

    public void SetAttackDelay(float attackDelay) { this.attackDelay = attackDelay; }

    public float GetAttackTimer() { return attackTimer; }

    public void SetAttackTimer(float attackTimer) { this.attackTimer = attackTimer; }

    public void IncreaseAttackTimer() { attackTimer += Time.deltaTime; }

    public void DecreaseAttackTimer() { attackTimer -= Time.deltaTime; }

    public bool AttackTimerReachDelay() { return attackTimer >= attackDelay; }

    // Attack performance functions // // // // //
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
    public bool IsAttacking() { return isAttacking; }
}
