using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Weapon : MonoBehaviour
{
    //maybe make the weapon more flexiable for enemy use in the future

    [SerializeField] private Status _weaponUser; //identifies who holds the weapon

    [Header("Weapon Details")]
    [SerializeField] private WeaponType _type;
    [SerializeField] private float _damage;
    [SerializeField] private float _cost; //amount needed to get the weapon
    //[SerializeField] private Animator animator;

    //Text Details
    private List<GameObject> textDetails = new List<GameObject>();
    private enum TDIndex { type, cost };

    // Getter and Setters // // // //
    public Status weaponUser
    {
        get { return _weaponUser; }
        private set { _weaponUser = value; }
    }

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

    // Unity // // // // //

    void Awake()
    {
        GetTextDetailGameObjects();

        SetAllTextDetails();

        ShowAllTextDetails(false);
    }



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
        SetTextDetail((int)TDIndex.type, type.ToString());
        SetTextDetail((int)TDIndex.cost, cost.ToString());
    }

    private void SetActiveTextDetail(int index, bool active)
    {
        textDetails[index].SetActive(active);
    }

    public void ShowTextDetails(bool showType, bool showCost)
    {
        SetActiveTextDetail((int)TDIndex.type, showType);
        SetActiveTextDetail((int)TDIndex.cost, showCost);
    }

    public void ShowAllTextDetails(bool show)
    {
        SetActiveTextDetail((int)TDIndex.type, show);
        SetActiveTextDetail((int)TDIndex.cost, show);
    }

    // Set details functions // // // // //

    public void SetWeaponUser(Status weaponUser) { this.weaponUser = weaponUser; }

    public void IncreaseDamage(float amount) { damage += amount; }

    public void DecreaseDamage(float amount) 
    { 
        damage -= amount; 
        if (damage < 0) damage = 0;
    }
    
    public void SetDamage(float damage) 
    { 
        if (damage < 0) this.damage = 0;
        else this.damage = damage;
    }

    public void IncreaseCost(float amount) 
    { 
        cost += amount;

        SetTextDetail((int)TDIndex.cost, cost.ToString());
    }
    
    public void DecreaseCost(float amount) 
    { 
        cost -= amount; 
        if (cost < 0) cost = 0;

        SetTextDetail((int)TDIndex.cost, cost.ToString());
    }

    public void SetCost(float cost) 
    {        
        if (cost < 0) this.cost = 0;
        else this.cost = cost;

        SetTextDetail((int)TDIndex.cost, cost.ToString());
    }

    // Attack performance functions // // // // //

    //describes how the weapon will attack, including sounds
    public virtual void Attack()
    {
        Debug.Log("Attack");
    }

    //return a true if an attack is being made
    private bool CanAttack()
    {
        int mouseCode = 0; //for left mouse clicks

        if (Input.GetMouseButtonDown(mouseCode)) return true;

        return false;
    }

    //return a true if an attack is being made
    public bool PerformAttack()
    {
        if (CanAttack())
        {
            Attack();

            return true;
        }

        return false;
    }

    //affects the health of the collided object
    public void MakeDamage(Collider2D collision)
    {
        Status status;
        if (status = collision.GetComponent<Status>())
        {
            if (status.user != weaponUser.user) //prevents damage on the user
            {
                status.TakeDamage(damage);
            }
        }
    }
}
