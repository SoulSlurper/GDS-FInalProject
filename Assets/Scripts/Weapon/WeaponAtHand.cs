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

    //increases and decreases the currentWeaponIndex int by the mouse scroll or the up and down arrow keys
    private void ScrollForWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            int maxIndex = weapons.Count - 1;

            DeselectWeapon(tempWeaponIndex);

            if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (tempWeaponIndex <= 0) tempWeaponIndex = maxIndex;
                else tempWeaponIndex--;
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (tempWeaponIndex >= maxIndex) tempWeaponIndex = 0;
                else tempWeaponIndex++;
            }

            SelectWeapon(tempWeaponIndex, false);
        }
    }

    private void SwitchingWeapons()
    {
        if (isSelecting)
        {
            ScrollForWeapon();

            if (Input.GetMouseButtonDown(1)) //confirms the selection by clicking the right mouse
            {
                isSelecting = false;

                if (tempWeaponIndex == currentWeaponIndex)
                {
                    SelectWeaponByIndex(currentWeaponIndex);
                }
                else
                {
                    SelectWeaponByIndex(tempWeaponIndex);

                    if (areWeaponsCosting)
                        playerStatus.health.DecreaseAmount(weapons[currentWeaponIndex].GetComponent<Weapon>().cost);
                }
            }
        }
    }

    private void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.E) && weapons.Count > 1)
        {
            isSelecting = !isSelecting;

            if (isSelecting)
            {
                tempWeaponIndex = currentWeaponIndex;
                if (skipOnMenuEnter)
                {
                    tempWeaponIndex++;

                    if (tempWeaponIndex >= weapons.Count) tempWeaponIndex = 0;

                    if (weapons[tempWeaponIndex].GetComponent<Weapon>().type == WeaponType.None && weapons.Count > 2)
                    {
                        tempWeaponIndex++;
                    }

                    if (tempWeaponIndex >= weapons.Count) tempWeaponIndex = 0;

                    if (tempWeaponIndex == currentWeaponIndex) tempWeaponIndex++;
                }

                SelectWeaponByIndex(tempWeaponIndex, false);

                Debug.Log("Weapon Menu opened");
            }
            else
            {
                SelectWeaponByIndex(currentWeaponIndex);
            }
        }

        SwitchingWeapons();
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
