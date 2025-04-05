using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Weapon : MonoBehaviour
{
    //maybe make the weapon more flexiable for enemy use

    [SerializeField] private StatusUser _user; //identifies who holds the weapon

    [Header("Weapon Details")]
    [SerializeField] private WeaponType _type;
    [SerializeField] private float _damage;
    [SerializeField] private float _cost; //amount needed to get the weapon
    //[SerializeField] private Animator animator;

    //Text Details
    private List<GameObject> textDetails = new List<GameObject>();
    private enum TDIndex { type, cost };

    // Getter and Setters // // // //
    public StatusUser user
    {
        get { return _user; }
        private set { _user = value; }
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



    // Set details functions // // // // //

    public void SetUser(StatusUser user) { this.user = user; }

    public void SetDamage(float damage) { this.damage = damage; }

    public void SetCost(float cost) { this.cost = cost; }




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
            if (status.user != user) //prevents damage on the user
            {
                status.TakeDamage(damage);
            }
        }
    }
}
