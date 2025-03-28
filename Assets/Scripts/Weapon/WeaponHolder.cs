using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private WeaponType selectedWeapon;

    private List<GameObject> weapons = new List<GameObject>();
    private int currentWeaponIndex = -1;
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
        if (ChangeCurrentWeaponIndex())
        {
            //Debug.Log("currentWeaponIndex: " + currentWeaponIndex);
            ChangeWeapons();
        }

        WeaponFacePointer();
    }

    //gets all the children gameobjects that have the Weapon component
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

    //changes the currentWeaponIndex when the WeaponType is found, which will return a true
    private bool FindWeaponIndex(WeaponType type)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].GetComponent<Weapon>().GetWeaponType() == type)
            {
                currentWeaponIndex = i;

                return true;
            }
        }

        return false;
    }

    private void SelectWeapon()
    {
        weapons[currentWeaponIndex].SetActive(true);

        selectedWeapon = weapons[currentWeaponIndex].GetComponent<Weapon>().GetWeaponType();
    }

    private void SelectWeapon(WeaponType type)
    {
        if (FindWeaponIndex(type))
        {
            SelectWeapon();
        }
    }

    private void SelectWeapon(int index)
    {
        if (FindDifferentWeaponIndex(index))
        {
            SelectWeapon();
        }
    }

    //increases and decreases the currentWeaponIndex int by the mouse scroll, which returns true if the scroll is performed
    private bool ChangeCurrentWeaponIndex()
    {
        int maxIndex = weapons.Count - 1;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            previousWeaponIndex = currentWeaponIndex;

            if (currentWeaponIndex >= maxIndex) currentWeaponIndex = 0;
            else currentWeaponIndex++;

            return true;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            previousWeaponIndex = currentWeaponIndex;

            if (currentWeaponIndex <= 0) currentWeaponIndex = maxIndex;
            else currentWeaponIndex--;

            return true;
        }

        return false;
    }

    //changes the currentWeaponIndex when the index is found, which will return a true
    //it only works if the previousWeaponIndex has been updated before the currentWeaponIndex changed
    private bool FindDifferentWeaponIndex(int index)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (i != previousWeaponIndex && i == index)
            {
                currentWeaponIndex = i;

                return true;
            }
        }

        return false;
    }

    //deactivates the current weapon that was selected
    //it only works if the previousWeaponIndex has been updated before the currentWeaponIndex changed
    private void DeselectWeapon()
    {
        weapons[previousWeaponIndex].SetActive(false);

        selectedWeapon = WeaponType.None;
    }

    private void ChangeWeapons()
    {
        DeselectWeapon();
        SelectWeapon(currentWeaponIndex);
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
