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
    [SerializeField] private bool areWeaponsCosting = false;
    [SerializeField] private WeaponType selectedWeapon = WeaponType.None;
    [SerializeField][Range(1, 4)] private int _availableWeaponsLimit = 3;
    
    [Header("Visual Settings")]
    [SerializeField] private Color colorSelection = Color.grey;
    
    [Header("Health Settings")]
    [SerializeField] private float healthRestoreOnNone = 5f; // Amount of HP restored when switching to None

    // Internal weapon tracking
    private List<GameObject> weapons = new List<GameObject>();
    private int currentWeaponIndex = -1;
    private int tempWeaponIndex = -1;
    private bool isSelecting = false;

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
        HandleWeaponSelection();
        UpdateWeaponOrientation();
    }
    
    #endregion

    #region Public Methods
    
    /// <summary>
    /// Increases the number of weapons the player can access at once
    /// </summary>
    public void IncreaseAvailableWeaponLimit(int amount)
    {
        availableWeaponsLimit += amount;
        if (availableWeaponsLimit > weapons.Count) 
            availableWeaponsLimit = weapons.Count;
    }
    
    // Tracking right-click cancel for aiming conflict prevention
    private float lastCancelTime = 0f;
    private const float RIGHT_CLICK_COOLDOWN = 0.2f; // 200ms cooldown
    
    /// <summary>
    /// Returns whether the player is currently in weapon selection mode
    /// </summary>
    public bool IsSelecting()
    {
        return isSelecting;
    }
    
    /// <summary>
    /// Checks if right-click was recently used to cancel weapon selection
    /// </summary>
    public bool WasSelectionRecentlyCanceled()
    {
        return Time.time - lastCancelTime < RIGHT_CLICK_COOLDOWN;
    }
    
    #endregion

    #region Weapon Management Methods
    
    /// <summary>
    /// Finds all child objects with Weapon component and caches them
    /// </summary>
    private void FindAndCacheWeapons()
    {
        foreach (Transform t in transform)
        {
            GameObject child = t.gameObject;
            Weapon weaponComponent = child.GetComponent<Weapon>();
            
            if (weaponComponent)
            {
                weapons.Add(child);
                child.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Gets component references needed for operation
    /// </summary>
    private void InitializeReferences()
    {
        playerStatus = player.GetComponent<Status>();
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        playerColor = playerSpriteRenderer.color;
    }

    /// <summary>
    /// Gets the index from the weapons list based on the WeaponType enum
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
    /// Applies visual settings and enables attack for active weapon
    /// </summary>
    private void ConfirmSelectedWeapon(Weapon weaponDetails)
    {
        playerSpriteRenderer.color = playerColor;
        weaponDetails.GetComponent<SpriteRenderer>().color = weaponDetails.color;
        weaponDetails.ShowAllTextDetails(false);
        weaponDetails.enabledAttack = true;
        selectedWeapon = weaponDetails.type;
    }

    /// <summary>
    /// Applies selection visual settings and disables attack for preview weapon
    /// </summary>
    private void UnconfirmSelectedWeapon(Weapon weaponDetails)
    {
        playerSpriteRenderer.color = colorSelection;
        weaponDetails.GetComponent<SpriteRenderer>().color = colorSelection;

        if (areWeaponsCosting)
        {
            bool showCost = weaponDetails.type != WeaponType.None;
            weaponDetails.ShowTextDetails(true, showCost);
        }
        else 
        {
            weaponDetails.ShowTextDetails(true, false);
        }

        weaponDetails.enabledAttack = false;
    }

    /// <summary>
    /// Deactivates the current weapon
    /// </summary>
    private void DeselectWeapon()
    {
        if (currentWeaponIndex < 0) return;

        weapons[currentWeaponIndex].SetActive(false);
        selectedWeapon = WeaponType.None;
    }

    /// <summary>
    /// Activates a weapon by index and applies appropriate settings
    /// </summary>
    private void SelectWeapon(int index, bool confirm = true)
    {
        DeselectWeapon();
        currentWeaponIndex = index;

        GameObject weapon = weapons[index];
        Weapon wDetails = weapon.GetComponent<Weapon>();

        weapon.SetActive(true);

        if (confirm) 
            ConfirmSelectedWeapon(wDetails);
        else 
            UnconfirmSelectedWeapon(wDetails);
    }

    /// <summary>
    /// Selects a weapon by its index in the weapons list
    /// </summary>
    private void SelectWeaponByIndex(int index, bool confirm = true)
    {
        if (index > -1 && index < availableWeaponsLimit)
        {
            SelectWeapon(index, confirm);
        }
        else
        {
            selectedWeapon = WeaponType.None;
            currentWeaponIndex = GetWeaponIndex(selectedWeapon);
            
            if (currentWeaponIndex > -1)
            {
                SelectWeapon(currentWeaponIndex, confirm);
            }
        }
    }

    /// <summary>
    /// Selects a weapon by its type
    /// </summary>
    private void SelectWeaponByType(WeaponType type, bool confirm = true)
    {
        SelectWeaponByIndex(GetWeaponIndex(type), confirm);
    }
    
    #endregion

    #region Weapon Cycling Methods
    
    /// <summary>
    /// Get next weapon index, cycling through all weapons except the current one
    /// </summary>
    private int GetNextWeaponIndex(int currentIndex)
    {
        int nextIndex = currentIndex + 1;
        if (nextIndex >= availableWeaponsLimit)
            nextIndex = 0;
            
        // Skip original weapon when cycling
        if (nextIndex == tempWeaponIndex)
        {
            nextIndex++;
            if (nextIndex >= availableWeaponsLimit)
                nextIndex = 0;
        }
        
        return nextIndex;
    }
    
    /// <summary>
    /// Get previous weapon index, cycling through all weapons except the current one
    /// </summary>
    private int GetPreviousWeaponIndex(int currentIndex)
    {
        int prevIndex = currentIndex - 1;
        if (prevIndex < 0)
            prevIndex = availableWeaponsLimit - 1;
            
        // Skip original weapon when cycling
        if (prevIndex == tempWeaponIndex)
        {
            prevIndex--;
            if (prevIndex < 0)
                prevIndex = availableWeaponsLimit - 1;
        }
        
        return prevIndex;
    }
    
    #endregion

    #region Weapon Selection Logic
    
    /// <summary>
    /// Handles entering the weapon selection menu
    /// </summary>
    private void OnWeaponMenuEnter(bool isScrollUp)
    {
        tempWeaponIndex = currentWeaponIndex;
        
        // Get first weapon to display based on scroll direction
        int newCurrentIndex;
        if (isScrollUp)
        {
            // If scrolling up, start with next weapon
            newCurrentIndex = GetNextWeaponIndex(currentWeaponIndex);
        }
        else
        {
            // If scrolling down, start with previous weapon
            newCurrentIndex = GetPreviousWeaponIndex(currentWeaponIndex);
        }
        
        SelectWeaponByIndex(newCurrentIndex, false);
        isSelecting = true;
    }

    /// <summary>
    /// Handles scrolling through weapons in the selection menu
    /// </summary>
    private void OnWeaponMenu()
    {
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        if (scrollValue != 0f)
        {
            if (scrollValue > 0f)
            {
                // Scroll up = next weapon
                int newCurrentIndex = GetNextWeaponIndex(currentWeaponIndex);
                SelectWeaponByIndex(newCurrentIndex, false);
            }
            else
            {
                // Scroll down = previous weapon
                int newCurrentIndex = GetPreviousWeaponIndex(currentWeaponIndex);
                SelectWeaponByIndex(newCurrentIndex, false);
            }
        }
    }

    /// <summary>
    /// Handles exiting the weapon selection menu
    /// </summary>
    private void OnWeaponMenuExit(bool weaponSelected)
    {
        if (weaponSelected)
        {
            // Confirm selection
            SelectWeaponByIndex(currentWeaponIndex);
            
            // Apply weapon switch cost if enabled
            if (areWeaponsCosting)
            {
                float weaponCost = weapons[currentWeaponIndex].GetComponent<Weapon>().cost;
                playerStatus.TakeDamage(weaponCost);
            }
                
            // Restore health when switching to None weapon
            Weapon selectedWeaponComponent = weapons[currentWeaponIndex].GetComponent<Weapon>();
            if (selectedWeaponComponent.type == WeaponType.None && healthRestoreOnNone > 0)
            {
                playerStatus.TakeHealth(healthRestoreOnNone);
            }
        }
        else
        {
            // Cancel and revert to previous weapon
            SelectWeaponByIndex(tempWeaponIndex);
        }

        isSelecting = false;
    }

    /// <summary>
    /// Main method for handling weapon selection input
    /// </summary>
    private void HandleWeaponSelection()
    {
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        
        // Enter selection mode when mouse wheel is scrolled
        if (scrollValue != 0f && !isSelecting && weapons.Count > 1)
        {
            OnWeaponMenuEnter(scrollValue > 0f);
        }

        if (isSelecting)
        {
            OnWeaponMenu();

            // Confirm with left click
            if (Input.GetMouseButtonDown(0))
            {
                OnWeaponMenuExit(true);
            }
            // Cancel with right click - only process right-click when in selection mode
            else if (Input.GetMouseButtonDown(1))
            {
                lastCancelTime = Time.time; // Record time of right-click cancel
                OnWeaponMenuExit(false);
            }
        }
    }
    
    #endregion

    #region Weapon Orientation
    
    /// <summary>
    /// Gets the normalized direction from weapon to mouse pointer
    /// </summary>
    private Vector2 GetPointerDirection()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mousePosition - (Vector2)transform.position).normalized;
    }

    /// <summary>
    /// Flips the text UI elements on weapons when facing direction changes
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
    /// Makes the weapon face towards the pointer's position
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