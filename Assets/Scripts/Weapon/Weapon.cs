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

    private List<GameObject> textDetails = new List<GameObject>();
    private enum textDetailsIndex { type, cost };

    [Header("Attack Details")]
    [SerializeField] private bool attackContinously = false;
    [SerializeField] private float attackDelay = 1f; //time taken for the attack to be performed

    private bool isAttacking = false;
    private float attackTimer = 0f;

    void Awake()
    {
        GetTextDetailGameObjects();

        SetAllTextDetails();

        ShowAllTextDetails(false);
    }

    // Weapon details functions // // // // //
    public WeaponType GetWeaponType() { return type; }

    public float GetDamage() { return damage; }

    public void SetDamage(float damage) { this.damage = damage; }

    public float GetCost() { return cost; }

    public void SetCost(float cost) { this.cost = cost; }

    //public Animator GetAnimator() { return animator; }

    // Text functions // // // // //
    private void GetTextDetailGameObjects()
    {
        foreach(Transform child in transform.Find("LabelCanvas").Find("Image"))
        {
            textDetails.Add(child.gameObject);
        }
    }

    private void SetTextDetail(int index, string text)
    {
        Transform textDetail = textDetails[index].transform;

        TMP_Text detail = textDetail.GetComponent<TMP_Text>();
        if (detail == null)
        {
            Debug.LogWarning(textDetail);
            foreach (Transform child in textDetail) Debug.Log(child);

            detail = textDetail.Find("ValueText").GetComponent<TMP_Text>();
        }

        detail.text = text;
    }

    private void SetAllTextDetails()
    {
        SetTextDetail((int)textDetailsIndex.type, type.ToString());
        SetTextDetail((int)textDetailsIndex.cost, cost.ToString());
    }

    private void SetActiveTextDetail(int index, bool active)
    {
        textDetails[index].SetActive(active);
    }

    public void ShowTextDetails(bool showType, bool showCost)
    {
        SetActiveTextDetail((int)textDetailsIndex.type, showType);
        SetActiveTextDetail((int)textDetailsIndex.cost, showCost);
    }

    public void ShowAllTextDetails(bool show)
    {
        SetActiveTextDetail((int)textDetailsIndex.type, show);
        SetActiveTextDetail((int)textDetailsIndex.cost, show);
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
