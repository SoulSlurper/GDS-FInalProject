using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponInHand : MonoBehaviour
{
    [SerializeField] private Status playerStatus;
    [SerializeField] private WeaponType selectedWeapon;

    private List<GameObject> weapons = new List<GameObject>();
    private int currentWeaponIndex = -1;
    private int tempWeaponIndex = -1;

    private bool isSelecting = false;

    void Awake()
    {
        GetAvailableWeapons();
    }

    void Start()
    {
        SelectWeaponByType(selectedWeapon);
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

    private void SelectWeapon(int index, bool confirm = true)
    {
        GameObject weapon = weapons[index];
        Weapon wDetails = weapon.GetComponent<Weapon>();

        weapon.SetActive(true);

        if (confirm)
        {
            currentWeaponIndex = index;

            weapon.GetComponent<SpriteRenderer>().color = Color.white;

            wDetails.enabled = true;
            selectedWeapon = wDetails.GetWeaponType();

            playerStatus.DecreaseHealth(wDetails.GetCost());
        }
        else
        {
            tempWeaponIndex = index;

            weapon.GetComponent<SpriteRenderer>().color = Color.gray;

            wDetails.enabled = false;
        }
    }

    private void DeselectWeapon(int index, bool confirm = true)
    {
        weapons[index].SetActive(false);

        if (confirm)
        {
            selectedWeapon = WeaponType.None;
        }
    }

    private void SelectWeaponByIndex(int index, bool confirm = true)
    {
        if (index > -1) //if the weapon has been found
        {
            if (index > -1) //if a weapon is currently being held already
            {
                DeselectWeapon(currentWeaponIndex);
                DeselectWeapon(tempWeaponIndex);
            }

            if (confirm)
            {
                currentWeaponIndex = index;

                SelectWeapon(currentWeaponIndex);
            }
            else
            {
                tempWeaponIndex = index;

                SelectWeapon(tempWeaponIndex);
            }
        }
    }

    private void SelectWeaponByType(WeaponType type, bool confirm = true)
    {
        SelectWeapon(GetWeaponIndex(type));
    }

    //increases and decreases the currentWeaponIndex int by the mouse scroll, which returns true if the scroll is performed
    private void ScrollForWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            int maxIndex = weapons.Count - 1;

            DeselectWeapon(tempWeaponIndex);

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (tempWeaponIndex >= maxIndex) tempWeaponIndex = 0;
                else tempWeaponIndex++;

            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (tempWeaponIndex <= 0) tempWeaponIndex = maxIndex;
                else tempWeaponIndex--;
            }

            SelectWeapon(tempWeaponIndex, false);
        }
    }

    private void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isSelecting = !isSelecting;

            if (isSelecting)
            {
                tempWeaponIndex = currentWeaponIndex;
                Debug.Log("tempWeaponIndex: " + tempWeaponIndex);

                SelectWeapon(tempWeaponIndex, false);
            }
            else
            {
                SelectWeaponByIndex(currentWeaponIndex);
            }
        }

        if (isSelecting)
        {
            ScrollForWeapon();

            if (Input.GetMouseButtonDown(1)) //confirms selection by clicking the right mouse
            {
                isSelecting = false;

                SelectWeaponByIndex(tempWeaponIndex);
            }
        }
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
