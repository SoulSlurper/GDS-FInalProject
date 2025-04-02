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

    // Unity // // // // //

    void Awake()
    {
        GetTextDetailGameObjects();

        SetAllTextDetails();

        ShowAllTextDetails(false);
    }



    // Weapon details functions // // // // //

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
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.CompareTag("Weapon")) return;

        Status status;
        if (status = collision.GetComponent<Status>())
        {
            collision.GetComponent<Status>().health.DecreaseAmount(damage);
        }
    }
}
