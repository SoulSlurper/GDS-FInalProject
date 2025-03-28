using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponInHand : MonoBehaviour
{
    [SerializeField] private WeaponType selectedWeapon;

    private List<GameObject> weapons = new List<GameObject>();
    private int currentWeaponIndex = -1;

    private bool isSelecting = false;

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
        ChangeWeapon();

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

    private int GetWeaponIndex(WeaponType type)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].GetComponent<Weapon>().GetWeaponType() == type)
            {
                return i;
            }
        }

        return -1;
    }

    private void SelectWeapon(bool confirm = true)
    {
        GameObject weapon = weapons[currentWeaponIndex];
        Weapon wDetails = weapon.GetComponent<Weapon>();

        weapon.SetActive(true);

        if (confirm)
        {
            weapon.GetComponent<SpriteRenderer>().color = Color.white;

            wDetails.enabled = true;
            selectedWeapon = wDetails.GetWeaponType();
        }
        else
        {
            weapon.GetComponent<SpriteRenderer>().color = Color.gray;

            wDetails.enabled = false;
        }
    }

    private void DeselectWeapon()
    {
        weapons[currentWeaponIndex].SetActive(false);

        selectedWeapon = WeaponType.None;
    }

    private void SelectWeapon(int index)
    {
        if (index > -1) //if the weapon has been found
        {
            if (currentWeaponIndex > -1)
            {
                DeselectWeapon(); //if a weapon is currently being held already
            }

            currentWeaponIndex = index;

            SelectWeapon();
        }
    }

    private void SelectWeapon(WeaponType type)
    {
        SelectWeapon(GetWeaponIndex(type));
    }

    //increases and decreases the currentWeaponIndex int by the mouse scroll, which returns true if the scroll is performed
    private void ScrollForWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            int maxIndex = weapons.Count - 1;

            DeselectWeapon();

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (currentWeaponIndex >= maxIndex) currentWeaponIndex = 0;
                else currentWeaponIndex++;

            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (currentWeaponIndex <= 0) currentWeaponIndex = maxIndex;
                else currentWeaponIndex--;
            }

            SelectWeapon(false);
        }
    }

    private bool SelectForWeapon()
    {
        isSelecting = !isSelecting;

        return isSelecting;
    }

    private void ChangeWeapon()
    {
        ScrollForWeapon();
    }

    private Vector2 PointerPosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return (mousePosition - (Vector2)transform.position).normalized;
    }

    private void WeaponFacePointer()
    {
        Vector2 direction = PointerPosition();

        transform.right = direction;

        Vector2 scale = transform.localScale;
        if ((direction.x < 0 && scale.y > 0) || (direction.x > 0 && scale.y < 0))
        {
            scale.y *= -1;
        }

        transform.localScale = scale;
    }
}
