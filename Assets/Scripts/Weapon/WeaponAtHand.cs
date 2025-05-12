using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles weapon switching and selection for the player
/// </summary>
public class WeaponAtHand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    
    [Header("Weapon Settings")]
    [SerializeField] private WeaponType selectedWeapon = WeaponType.None;
    [SerializeField][Range(1, 4)] private int _availableWeaponsLimit = 3;
    
    [Header("Health Regeneration")]
    [SerializeField] private float healthRegenPerSecond = 1f; // HP regenerated per second when using None weapon

    // Internal weapon tracking
    private List<GameObject> weapons = new List<GameObject>();
    private int currentWeaponIndex = -1;

    // Component references
    private Status playerStatus;
    private SpriteRenderer playerSpriteRenderer;
    private Color playerColor;

    // Properties
    public int availableWeaponsLimit
    {
        get { return _availableWeaponsLimit; }
        private set { _availableWeaponsLimit = value; }
    }

    #region Unity Lifecycle Methods
    
    private void Awake()
    {
        FindAndCacheWeapons();
    }

    private void Start()
    {
        InitializeReferences();
        SelectWeaponByType(selectedWeapon);
    }

    private void Update()
    {
        HandleWeaponSwitching();
        UpdateWeaponOrientation();
        HandleHealthRegeneration();
    }
    
    #endregion

    #region Public Methods
    
    /// <summary>
    /// Increases weapon limit
    /// </summary>
    public void IncreaseAvailableWeaponLimit(int amount)
    {
        availableWeaponsLimit += amount;
        if (availableWeaponsLimit > weapons.Count) 
            availableWeaponsLimit = weapons.Count;
    }
    
    #endregion

    #region Weapon Management Methods
    
    /// <summary>
    /// Finds and caches all weapons
    /// </summary>
    private void FindAndCacheWeapons()
    {
        foreach (Transform t in transform)
        {
            GameObject child = t.gameObject;
            Weapon weaponComponent = child.GetComponent<Weapon>();
            
            if (weaponComponent)
            {
                child.SetActive(true);
                child.GetComponent<SpriteRenderer>().enabled = false;
                weaponComponent.isHeld = false;

                weapons.Add(child);
            }
            else
            {
                child.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Gets required component references
    /// </summary>
    private void InitializeReferences()
    {
        playerStatus = player.GetComponent<Status>();
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        playerColor = playerSpriteRenderer.color;
    }

    /// <summary>
    /// Gets weapon index by type
    /// </summary>
    private int GetWeaponIndex(WeaponType type)
    {
        for (int i = 0; i < availableWeaponsLimit; i++)
        {
            if (weapons[i].GetComponent<Weapon>().type == type)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Sets up active weapon
    /// </summary>
    private void SetupSelectedWeapon(Weapon weaponDetails)
    {
        playerSpriteRenderer.color = playerColor;
        weaponDetails.GetComponent<SpriteRenderer>().color = weaponDetails.color;
        weaponDetails.ShowAllTextDetails(false);
        weaponDetails.isHeld = true;
        selectedWeapon = weaponDetails.type;
    }

    /// <summary>
    /// Deactivates current weapon
    /// </summary>
    private void DeselectWeapon()
    {
        if (currentWeaponIndex < 0) return;

        weapons[currentWeaponIndex].GetComponent<SpriteRenderer>().enabled = false;
        weapons[currentWeaponIndex].GetComponent<Weapon>().isHeld = false;
        selectedWeapon = WeaponType.None;
    }

    /// <summary>
    /// Activates weapon by index
    /// </summary>
    private void SelectWeapon(int index)
    {
        DeselectWeapon();
        currentWeaponIndex = index;
        weapons[index].GetComponent<SpriteRenderer>().enabled = true;

        GameObject weapon = weapons[index];
        Weapon wDetails = weapon.GetComponent<Weapon>();

        SetupSelectedWeapon(wDetails);
    }

    /// <summary>
    /// Selects weapon by index
    /// </summary>
    private void SelectWeaponByIndex(int index)
    {
        if (index > -1 && index < availableWeaponsLimit)
        {
            SelectWeapon(index);
        }
        else
        {
            selectedWeapon = WeaponType.None;
            currentWeaponIndex = GetWeaponIndex(selectedWeapon);
            
            if (currentWeaponIndex > -1)
            {
                SelectWeapon(currentWeaponIndex);
            }
        }
    }

    /// <summary>
    /// Selects weapon by type
    /// </summary>
    private void SelectWeaponByType(WeaponType type)
    {
        SelectWeaponByIndex(GetWeaponIndex(type));
    }
    
    #endregion

    #region Weapon Cycling Methods
    
    /// <summary>
    /// Get next weapon index
    /// </summary>
    private int GetNextWeaponIndex(int currentIndex)
    {
        int nextIndex = currentIndex + 1;
        if (nextIndex >= availableWeaponsLimit)
            nextIndex = 0;
        
        return nextIndex;
    }
    
    /// <summary>
    /// Get previous weapon index
    /// </summary>
    private int GetPreviousWeaponIndex(int currentIndex)
    {
        int prevIndex = currentIndex - 1;
        if (prevIndex < 0)
            prevIndex = availableWeaponsLimit - 1;
        
        return prevIndex;
    }
    
    #endregion

    #region Weapon Switching
    
    /// <summary>
    /// Handles weapon switching with mouse wheel and number keys
    /// </summary>
    private void HandleWeaponSwitching()
    {
        // Handle mouse wheel switching
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        
        if (scrollValue != 0f && weapons.Count > 1)
        {   
            if (scrollValue > 0f)
            {
                // Scroll up = previous weapon
                int newIndex = GetPreviousWeaponIndex(currentWeaponIndex);
                SelectWeaponByIndex(newIndex);
            }
            else
            {
                // Scroll down = next weapon
                int newIndex = GetNextWeaponIndex(currentWeaponIndex);
                SelectWeaponByIndex(newIndex);
            }
        }
        
        // Handle number key switching (1-3)
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SelectWeaponByIndex(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SelectWeaponByIndex(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SelectWeaponByIndex(2);
        }
    }
    
    /// <summary>
    /// Regenerates health when no weapon equipped
    /// </summary>
    private void HandleHealthRegeneration()
    {
        if (selectedWeapon == WeaponType.None && playerStatus != null && !playerStatus.noHealth)
        {
            playerStatus.TakeHealth(healthRegenPerSecond * Time.deltaTime);
        }
    }
    
    #endregion

    #region Weapon Orientation
    
    /// <summary>
    /// Gets direction to mouse pointer
    /// </summary>
    private Vector2 GetPointerDirection()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mousePosition - (Vector2)transform.position).normalized;
    }

    /// <summary>
    /// Flips weapon text elements
    /// </summary>
    private void FlipWeaponTexts()
    {
        foreach (Transform child in transform)
        {
            Transform labelCanvas = child.Find("LabelCanvas");
            if (labelCanvas == null) continue;

            Vector2 scaleLabel = labelCanvas.localScale;
            scaleLabel.x *= -1;
            labelCanvas.localScale = scaleLabel;
        }
    }

    /// <summary>
    /// Makes weapon face towards mouse pointer
    /// </summary>
    private void UpdateWeaponOrientation()
    {
        Vector2 direction = GetPointerDirection();
        transform.right = direction;

        Vector2 scale = transform.localScale;
        bool shouldFlip = (direction.x < 0 && scale.y > 0) || (direction.x > 0 && scale.y < 0);
        
        if (shouldFlip)
        {
            scale.y *= -1;
            transform.localScale = scale;
            FlipWeaponTexts();
        }
    }
    
    #endregion
}