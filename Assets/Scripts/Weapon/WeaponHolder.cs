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
        SelectWeapon(selectedWeapon);
    }

    void Update()
    {
        if (ChangeWeaponIndex())
        {
            //Debug.Log("weaponIndex: " + weaponIndex);
            ChangeWeapons();
        }

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
            if (weapons[i].GetComponent<Weapon>().GetWeaponType() == type)
            {
                weaponIndex = i;

                return true;
            }
        }

        return false;
    }

    private bool FindDifferentWeapon(int index)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (i != previousWeaponIndex && i == index)
            {
                weaponIndex = i;

                return true;
            }
        }

        return false;
    }

    private void SelectWeapon()
    {
        weapons[weaponIndex].SetActive(true);

        selectedWeapon = weapons[weaponIndex].GetComponent<Weapon>().GetWeaponType();
    }

    private void SelectWeapon(WeaponType type)
    {
        if (FindWeapon(type))
        {
            SelectWeapon();
        }
    }

    private void SelectWeapon(int index)
    {
        if (FindDifferentWeapon(index))
        {
            SelectWeapon();
        }
    }

    private void DeselectWeapon()
    {
        weapons[previousWeaponIndex].SetActive(false);

        selectedWeapon = WeaponType.None;
    }

    //increases and decreases the weaponIndex int by the mouse scroll, which returns true if the scroll is performed
    private bool ChangeWeaponIndex()
    {
        int maxIndex = weapons.Count - 1;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            previousWeaponIndex = weaponIndex;

            if (weaponIndex >= maxIndex) weaponIndex = 0;
            else weaponIndex++;

            return true;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            previousWeaponIndex = weaponIndex;

            if (weaponIndex <= 0) weaponIndex = maxIndex;
            else weaponIndex--;

            return true;
        }

        return false;
    }

    private void ChangeWeapons()
    {
        DeselectWeapon();
        SelectWeapon(weaponIndex);
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
