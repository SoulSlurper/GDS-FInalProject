using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponAtHand : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private bool areWeaponsCosting = false;

    [Header("Selection")]
    [SerializeField] private WeaponType selectedWeapon = WeaponType.None;
    [SerializeField] private Color colorSelection = Color.grey;
    [SerializeField] private bool skipOnMenuEnter = false;

    private List<GameObject> weapons = new List<GameObject>();
    private int currentWeaponIndex = -1;
    private int tempWeaponIndex = -1;

    private Status playerStatus;
    private SpriteRenderer playerSpriteRenderer;

    private bool isSelecting = false;

    void Awake()
    {
        GetExistingWeapons();
    }

    void Start()
    {
        playerStatus = player.GetComponent<Status>();
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        SelectWeaponByType(selectedWeapon);
    }

    void Update()
    {
        WeaponMenu();

        WeaponFacePointer();
    }

    //gets all the children gameobjects that have the Weapon component
    private void GetExistingWeapons()
    {
        foreach (Transform t in transform)
        {
            GameObject child = t.gameObject;
            if (child.GetComponent<Weapon>())
            {
                //Debug.Log(child.name);
                weapons.Add(child);
            }

            child.SetActive(false);
        }
    }

    //gets the index from the weapons List based on the WeaponType enum
    private int GetWeaponIndex(WeaponType type)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].GetComponent<Weapon>().type == type)
            {
                return i;
            }
        }

        return -1;
    }

    //sets the settings of when the player have their desired weapon
    private void ConfirmSelectedWeapon(Weapon weaponDetails)
    {
        playerSpriteRenderer.color = Color.white;

        weaponDetails.GetComponent<SpriteRenderer>().color = Color.white;

        weaponDetails.ShowAllTextDetails(false);

        weaponDetails.enabledAttack = true;

        selectedWeapon = weaponDetails.type;
    }

    //sets the settings of when the player not have their desired weapon
    private void UnconfirmSelectedWeapon(Weapon weaponDetails)
    {
        playerSpriteRenderer.color = colorSelection;

        weaponDetails.GetComponent<SpriteRenderer>().color = colorSelection;

        if (areWeaponsCosting)
        {
            if (weaponDetails.type == WeaponType.None) weaponDetails.ShowTextDetails(true, false);
            else weaponDetails.ShowTextDetails(true, true);
        }
        else weaponDetails.ShowTextDetails(true, false);

        weaponDetails.enabledAttack = false;
    }

    //deselects the current weapon
    private void DeselectWeapon()
    {
        if (currentWeaponIndex < 0) return;

        weapons[currentWeaponIndex].SetActive(false);
        selectedWeapon = WeaponType.None;
    }

    //shows the selected weapon, where there can only be one selected
    private void SelectWeapon(int index, bool confirm = true)
    {
        DeselectWeapon(); //if a weapon is currently being held

        currentWeaponIndex = index;

        GameObject weapon = weapons[index];
        Weapon wDetails = weapon.GetComponent<Weapon>();

        weapon.SetActive(true);

        if (confirm) ConfirmSelectedWeapon(wDetails);
        else UnconfirmSelectedWeapon(wDetails);
    }

    private void SelectWeaponByIndex(int index, bool confirm = true)
    {
        if (index > -1 && index < weapons.Count) //if the weapon does exist
        {
            SelectWeapon(index, confirm);
        }
        else // if weapon does not exist
        {
            selectedWeapon = WeaponType.None;
        }
    }

    private void SelectWeaponByType(WeaponType type, bool confirm = true)
    {
        SelectWeaponByIndex(GetWeaponIndex(type), confirm);
    }

    //checks and returns an index that is in the range of the weapons list, where it will be continously used until the newIndex is in range and is not the same as the indexSkip variable
    private int GetIndexInRange(int index, int indexSkip = -1, bool isCheckIncrement = true)
    {
        if (index > -1 && index < weapons.Count && !index.Equals(indexSkip)) return index;

        int newIndex = index;

        if (newIndex >= weapons.Count) newIndex = 0;
        else if (newIndex < 0) newIndex = weapons.Count - 1;
        
        if (newIndex.Equals(indexSkip))
        {
            if (isCheckIncrement) newIndex++;
            else newIndex--;
        }

        return GetIndexInRange(newIndex, indexSkip, isCheckIncrement);
    }

    //when the player enters the weapon selection
    private void OnWeaponMenuEnter()
    {
        tempWeaponIndex = currentWeaponIndex;

        int newCurrentIndex = currentWeaponIndex;
        if (skipOnMenuEnter)
        {
            newCurrentIndex = GetIndexInRange(tempWeaponIndex + 1, GetWeaponIndex(WeaponType.None), true);
        }

        SelectWeaponByIndex(newCurrentIndex, false);

        isSelecting = true;

        Debug.Log("Weapon Menu opened");
    }

    //increases and decreases the currentWeaponIndex int by the mouse scroll or the up and down arrow keys
    private void OnWeaponMenu()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            int newCurrentIndex = currentWeaponIndex;
            if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.UpArrow))
            {
                newCurrentIndex = GetIndexInRange(newCurrentIndex - 1);
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.DownArrow))
            {
                newCurrentIndex = GetIndexInRange(newCurrentIndex + 1);
            }

            SelectWeaponByIndex(newCurrentIndex, false);
        }
    }

    //when the player exits the weapon selection
    private void OnWeaponMenuExit()
    {
        if (!isSelecting)
        {
            currentWeaponIndex = tempWeaponIndex;
        }
        else
        {
            if (areWeaponsCosting)
                playerStatus.TakeDamage(weapons[currentWeaponIndex].GetComponent<Weapon>().cost);

            isSelecting = false;
        }

        SelectWeaponByIndex(currentWeaponIndex);

        Debug.Log("Weapon Menu exit");
    }

    private void WeaponMenu()
    {
        //enters selection and exit with the current weapon by mouse right click
        if (Input.GetMouseButtonDown(1) && weapons.Count > 1)
        {
            isSelecting = !isSelecting;

            if (isSelecting) OnWeaponMenuEnter();
            else OnWeaponMenuExit();
        }

        if (isSelecting)
        {
            OnWeaponMenu();

            //exit selection with the selected weapon by mouse left click
            if (Input.GetMouseButtonDown(0)) OnWeaponMenuExit();
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

            foreach (Transform child in transform)
            {
                Transform labelCanvas = child.Find("LabelCanvas");

                Vector2 scaleLabel = labelCanvas.localScale;

                scaleLabel.x *= -1;

                labelCanvas.localScale = scaleLabel;
            }
        }

        transform.localScale = scale;
    }
}
