using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private WeaponType selectedWeapon;

    private List<GameObject> weapons = new List<GameObject>();
    private int weaponIndex = -1;
    private int previousWeaponIndex = -1;

    void Awake()
    {
        GetAvailableWeapons();
    }

    void Start()
    {
        SelectWeapon();
    }

    void Update()
    {
        WeaponFacePointer();
    }

    private void GetAvailableWeapons()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<Weapon>())
            {
                //Debug.Log(child.name);
                weapons.Add(child.gameObject);
            }
        }
    }

    private bool FindWeapon(WeaponType type)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (i != previousWeaponIndex 
                && weapons[i].GetComponent<Weapon>().GetWeaponType() == type)
            {
                previousWeaponIndex = weaponIndex;
                weaponIndex = i;

                return true;
            }
        }

        return false;
    }

    private void SelectWeapon()
    {
        if (FindWeapon(selectedWeapon))
        {
            weapons[weaponIndex].SetActive(true);
        }

        Debug.Log("yes");
    }

    private Vector2 FacePointerPosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return (mousePosition - (Vector2)transform.position).normalized;
    }

    private void WeaponFacePointer()
    {
        Vector2 direction = FacePointerPosition();

        transform.right = direction;

        Vector2 scale = transform.localScale;
        if ((direction.x < 0 && scale.y > 0) || (direction.x > 0 && scale.y < 0))
        {
            scale.y *= -1;
        }

        transform.localScale = scale;
    }
}
