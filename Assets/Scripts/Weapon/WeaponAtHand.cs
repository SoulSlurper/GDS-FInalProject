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
        HandleDirectWeaponSwitching();
        UpdateWeaponOrientation();
        HandleHealthRegeneration();
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
    
    /// <summary>
    /// Returns whether the player is currently in weapon selection mode
    /// Maintained for compatibility with SlimeKnightController
    /// </summary>
    public bool IsSelecting()
    {
        return false;
    }
    
    /// <summary>
    /// Returns whether the weapon selection was recently canceled
    /// Maintained for compatibility with SlimeKnightController
    /// </summary>
    public bool WasSelectionRecentlyCanceled()
    {
        return false;
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
    private void SetupSelectedWeapon(Weapon weaponDetails)
    {
        playerSpriteRenderer.color = playerColor;
        weaponDetails.GetComponent<SpriteRenderer>().color = weaponDetails.color;
        weaponDetails.ShowAllTextDetails(false);
        weaponDetails.enabledAttack = true;
        selectedWeapon = weaponDetails.type;
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
    private void SelectWeapon(int index)
    {
        DeselectWeapon();
        currentWeaponIndex = index;

        GameObject weapon = weapons[index];
        Weapon wDetails = weapon.GetComponent<Weapon>();

        weapon.SetActive(true);
        SetupSelectedWeapon(wDetails);
    }

    /// <summary>
    /// Selects a weapon by its index in the weapons list
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
    /// Selects a weapon by its type
    /// </summary>
    private void SelectWeaponByType(WeaponType type)
    {
        SelectWeaponByIndex(GetWeaponIndex(type));
    }
    
    #endregion

    #region Weapon Cycling Methods
    
    /// <summary>
    /// Get next weapon index, cycling through all weapons
    /// </summary>
    private int GetNextWeaponIndex(int currentIndex)
    {
        int nextIndex = currentIndex + 1;
        if (nextIndex >= availableWeaponsLimit)
            nextIndex = 0;
        
        return nextIndex;
    }
    
    /// <summary>
    /// Get previous weapon index, cycling through all weapons
    /// </summary>
    private int GetPreviousWeaponIndex(int currentIndex)
    {
        int prevIndex = currentIndex - 1;
        if (prevIndex < 0)
            prevIndex = availableWeaponsLimit - 1;
        
        return prevIndex;
    }
    
    #endregion

    #region Direct Weapon Switching
    
    /// <summary>
    /// Directly switches weapons based on mouse wheel input
    /// </summary>
    private void HandleDirectWeaponSwitching()
    {
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        
        if (scrollValue == 0f || weapons.Count <= 1) 
            return;
            
        if (scrollValue > 0f)
        {
            // Scroll up = previous weapon (corrected direction)
            int newIndex = GetPreviousWeaponIndex(currentWeaponIndex);
            SelectWeaponByIndex(newIndex);
        }
        else
        {
            // Scroll down = next weapon (corrected direction)
            int newIndex = GetNextWeaponIndex(currentWeaponIndex);
            SelectWeaponByIndex(newIndex);
        }
    }
    
    /// <summary>
    /// Regenerates health over time when no weapon is equipped
    /// </summary>
    private void HandleHealthRegeneration()
    {
        // Add a check to prevent regeneration if the player's health is 0 or below
        if (selectedWeapon == WeaponType.None && playerStatus != null && !playerStatus.noHealth)
        {
            playerStatus.TakeHealth(healthRegenPerSecond * Time.deltaTime);
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