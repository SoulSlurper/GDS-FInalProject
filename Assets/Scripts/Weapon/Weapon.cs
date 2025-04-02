using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Details")]
    [SerializeField] private WeaponType _type;
    [SerializeField] private float _damage;
    [SerializeField] private float _cost; //amount needed to get the weapon
    //[SerializeField] private Animator animator;

    [Header("Attack Details")]
    public bool isBreakable = false;
    public bool isAttackContinuous = false;
    [SerializeField] private float _attackDelay = 1f; //time taken for the attack to be performed

    private bool _isAttacking = false;
    private float _attackTimer = 0f;


    //Text Details
    private List<GameObject> textDetails = new List<GameObject>();
    private enum textDetailsIndex { type, cost };



    // Getter and Setters // // // //
    public WeaponType type
    {
        get { return _type; }
        private set { _type = value; }
    }

    public float damage
    {
        get { return _damage; }
        private set { _damage = value; }
    }

    public float cost
    {
        get { return _cost; }
        private set { _cost = value; }
    }

    public float attackDelay
    {
        get { return _attackDelay; }
        private set { _attackDelay = value; }
    }

    public float attackTimer
    {
        get { return _attackTimer; }
        set { _attackTimer = value; }
    }
    public bool isAttacking
    {
        get { return _isAttacking; }
        private set { _isAttacking = value; }
    }


    // Unity // // // // //

    void Awake()
    {
        GetTextDetailGameObjects();

        SetAllTextDetails();

        ShowAllTextDetails(false);

        InitiateAttackTimer();
    }



    // Weapon details functions // // // // //

    public void SetDamage(float damage) { this.damage = damage; }

    public void SetCost(float cost) { this.cost = cost; }




    // Attack time functions // // // // //
    public void SetAttackDelay(float attackDelay) { this.attackDelay = attackDelay; }

    public void SetAttackTimer(float attackTimer) { this.attackTimer = attackTimer; }

    public void InitiateAttackTimer() {  this.attackTimer = attackDelay + 1f; }


    // Text functions // // // // //
    private void GetTextDetailGameObjects()
    {
        foreach (Transform child in transform.Find("LabelCanvas").Find("BackgroundImage"))
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

    // Attack performance functions // // // // //

    public void SetIsAttacking(bool isAttacking) { this.isAttacking = isAttacking; }

    public virtual void Attack()
    {
        Debug.Log("Attack");
    }

    //return a true if an attack is being made
    private bool ContinousAttack()
    {
        if (attackTimer > attackDelay)
        {
            Attack();

            attackTimer = 0f;

            return true;
        }

        attackTimer += Time.deltaTime;

        return false;
    }

    private bool CanAttack()
    {
        int mouseCode = 0; //for left mouse clicks

        if (Input.GetMouseButtonDown(mouseCode))
        {
            isAttacking = true;
        }
        else if (Input.GetMouseButtonUp(mouseCode) || (isAttacking && !isAttackContinuous))
        {
            isAttacking = false;
            InitiateAttackTimer();
        }

        return isAttacking;
    }

    //return a true if an attack is being made
    public bool PerformAttack(Collider2D collider = null)
    {
        if (CanAttack())
        {
            if (isAttackContinuous) return ContinousAttack();
            
            Attack();

            return true;
        }

        return false;
    }

    //affects the health of the collided object
    public void MakeDamage(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.CompareTag("Weapon")) return;

        Status status;
        if (status = collision.GetComponent<Status>())
        {
            collision.GetComponent<Status>().health.DecreaseAmount(damage);
        }
    }
}
