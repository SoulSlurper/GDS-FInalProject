using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        GetAvailableWeapons();
    }

    void Start()
    {
        playerStatus = player.GetComponent<Status>();
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        
        SelectWeaponByType(selectedWeapon);
    }

    void Update()
    {
        ChangingWeapon();

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

            child.gameObject.SetActive(false);
        }
    }

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

    private void SelectWeapon(int index, bool confirm = true)
    {
        GameObject weapon = weapons[index];
        Weapon wDetails = weapon.GetComponent<Weapon>();
        wDetails.enabled = true;

        weapon.SetActive(true);

        if (confirm)
        {
            playerSpriteRenderer.color = Color.white;

            currentWeaponIndex = index;

            weapon.GetComponent<SpriteRenderer>().color = Color.white;

            wDetails.ShowAllTextDetails(false);

            selectedWeapon = wDetails.type;
        }
        else
        {
            playerSpriteRenderer.color = colorSelection;

            tempWeaponIndex = index;

            weapon.GetComponent<SpriteRenderer>().color = colorSelection;

            if (areWeaponsCosting)
            {
                if (wDetails.type == WeaponType.None) wDetails.ShowTextDetails(true, false);
                else wDetails.ShowTextDetails(true, true);
            }
            else wDetails.ShowTextDetails(true, false);
            
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
        //if a weapon is currently being held already
        if (currentWeaponIndex > -1) DeselectWeapon(currentWeaponIndex);
        if (tempWeaponIndex > -1) DeselectWeapon(tempWeaponIndex);

        if (index > -1 && index < weapons.Count) //if the weapon can exist
        {
            int i;
            if (confirm)
            {
                i = currentWeaponIndex = index;
            }
            else
            {
                i = tempWeaponIndex = index;
            }

            SelectWeapon(i, confirm);
        }
        else // if weapon cannot exist
        {
            SelectWeaponByIndex(GetWeaponIndex(WeaponType.None), confirm);
        }
    }

    private void SelectWeaponByType(WeaponType type, bool confirm = true)
    {
        SelectWeaponByIndex(GetWeaponIndex(type), confirm);
    }

    //checks and returns an index that is in the range of the weapons list, where it will be continously used until the newIndex is in range and is not the same as the indexSkip variable
    private int GetIndexInRange(int index, bool isNextCheckIncrement = true, int indexSkip = -1)
    {
        if (index > -1 && index < weapons.Count && !index.Equals(indexSkip)) return index;

        int newIndex = index;

        if (newIndex >= weapons.Count) newIndex = 0;
        else if (newIndex < 0) newIndex = weapons.Count - 1;
        
        if (newIndex.Equals(indexSkip))
        {
            if (isNextCheckIncrement) newIndex++;
            else newIndex--;
        }

        return GetIndexInRange(newIndex, isNextCheckIncrement, indexSkip);
    }

    //when the player enters the weapon selection
    private void OnWeaponMenuEnter()
    {
        tempWeaponIndex = currentWeaponIndex;
        if (skipOnMenuEnter)
        {
            tempWeaponIndex = GetIndexInRange(tempWeaponIndex + 1, true, GetWeaponIndex(WeaponType.None));
        }

        SelectWeaponByIndex(tempWeaponIndex, false);

        isSelecting = true;

        Debug.Log("Weapon Menu opened");
    }

    //increases and decreases the currentWeaponIndex int by the mouse scroll or the up and down arrow keys
    private void OnWeaponMenu()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            DeselectWeapon(tempWeaponIndex);

            if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.UpArrow))
            {
                tempWeaponIndex = GetIndexInRange(tempWeaponIndex - 1, false);
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.DownArrow))
            {
                tempWeaponIndex = GetIndexInRange(tempWeaponIndex + 1);
            }

            SelectWeapon(tempWeaponIndex, false);
        }
    }

    //when the player exits the weapon selection
    private void OnWeaponMenuExit()
    {
        if (tempWeaponIndex == currentWeaponIndex || !isSelecting)
        {
            SelectWeaponByIndex(currentWeaponIndex);
        }
        else
        {
            SelectWeaponByIndex(tempWeaponIndex);

            if (areWeaponsCosting)
                playerStatus.TakeDamage(weapons[currentWeaponIndex].GetComponent<Weapon>().cost);
        }

        isSelecting = false;

        Debug.Log("Weapon Menu exit");
    }

    private void ChangingWeapon()
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
