using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Status _weaponUser;

    [Header("Weapon Details")]
    [SerializeField] private WeaponType _type;
    [SerializeField] private float _damage;
    [SerializeField] private float _cost;
    public bool isHeld = true; //whether the user is holding the weapon to use

    [Header("Delay Attacks / Cooldown")]
    //delayed attacks could be made by making the attackMaxLimit to 1 and the cooldownColor to white, where the attackRecoveryTime is the amount that the attack is delayed
    [SerializeField] private float _attackRecoveryTime; //the time taken for the user to attack again
    [SerializeField] private int _attackMaxLimit = 0; //the number of attacks that can be made before it goes in a cooldown state, 0 means infinite
    [SerializeField] private Color _cooldownColor = Color.gray; //the color state of when the weapon is in cooldown, which is reaching the max attack limit

    [Header("Knockback Settings")]
    [SerializeField] private bool _applyKnockback = true;
    [SerializeField] private float _knockbackForce = 8f;
    [SerializeField] private float _bossKnockbackMultiplier = 0.5f;

    [Header("Focus Settings")]
    [SerializeField] private float _focusDamageMultiplier = 1.25f; // 25% damage increase when focusing

    [Header("Details by User Health")]
    [SerializeField][Range(0f, 1f)] private float _minDamage = 1f;
    [SerializeField][Range(0f, 1f)] private float _minCost = 1f;

    private bool _enabledAttack = true;
    private float _realDamage;
    private float _realCost;
    
    private float attackRecoveryTimer = 0f; //tracks the time that passes for the attackRecoveryTime
    private int attackCount = 0; //counts the number of attacks made before reaching the attackRecoveryTime

    private List<GameObject> textDetails = new List<GameObject>();
    private enum TDIndex { type, cost };

    private Color _color;
    private SlimeKnightController playerController;

    private SpriteRenderer spriteRenderer;

    #region Properties
    public Status weaponUser { get => _weaponUser; private set => _weaponUser = value; }
    public WeaponType type { get => _type; private set => _type = value; }
    public float damage { get => _damage; private set => _damage = value; }
    public bool applyKnockback { get => _applyKnockback; set => _applyKnockback = value; }
    public float knockbackForce { 
        get => _knockbackForce; 
        set => _knockbackForce = value > 0 ? value : _knockbackForce; 
    }
    public float minDamage { get => _minDamage; private set => _minDamage = value; }
    public float realDamage { get => _realDamage; private set => _realDamage = value; }
    public float cost { get => _cost; private set => _cost = value; }
    public float minCost { get => _minCost; private set => _minCost = value; }
    public float realCost { get => _realCost; private set => _realCost = value; }
    public float attackRecoveryTime { get => _attackRecoveryTime; private set => _attackRecoveryTime = value; }
    public Color cooldownColor { get => _cooldownColor; private set => _cooldownColor = value; }
    public int attackMaxLimit { get => _attackMaxLimit; private set => _attackMaxLimit = value; }
    public bool enabledAttack { get => _enabledAttack; set => _enabledAttack = value; }
    public Color color { get => _color; private set => _color = value; }
    public float focusDamageMultiplier { get => _focusDamageMultiplier; set => _focusDamageMultiplier = value; }
    #endregion

    #region Unity
    void Awake()
    {
        GetTextDetailGameObjects();
        SetAllTextDetails();
        ShowAllTextDetails(false);

        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
        
        // Find player controller for focus check
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<SlimeKnightController>();
        }
    }

    void LateUpdate()
    {
        if (attackRecoveryTimer > attackRecoveryTime)
        {
            enabledAttack = true;
            spriteRenderer.color = color;

            if (attackCount > 0) //regain attack availability overtime
            {
                attackCount--;
                attackRecoveryTimer = 0f;
            }
        }
        else attackRecoveryTimer += Time.deltaTime;
        //Debug.Log(gameObject.name + " attackRecoveryTimer: " + attackRecoveryTimer);
        //Debug.Log(gameObject.name + " enabledAttack: " + enabledAttack);

        SetRealAmounts();

        Debug.Log("attackCount: " + attackCount);
    }
    #endregion

    #region Text management
    private void GetTextDetailGameObjects()
    {
        Transform labelCanvas = transform.Find("LabelCanvas");
        if (labelCanvas == null) return;
        
        Transform backgroundImage = labelCanvas.Find("BackgroundImage");
        if (backgroundImage == null) return;
        
        foreach (Transform child in backgroundImage)
            textDetails.Add(child.gameObject);
    }

    private void SetTextDetail(TDIndex index, string text)
    {
        if (textDetails.Count <= (int)index) return;
        
        Transform textDetail = textDetails[(int)index].transform;
        if (textDetail == null) return;

        TMP_Text detail = textDetail.GetComponent<TMP_Text>();
        if (detail == null)
            detail = textDetail.Find("ValueText")?.GetComponent<TMP_Text>();

        if (detail != null)
            detail.text = text;
    }

    private void SetTypeTextDetail(WeaponType type) => SetTextDetail(TDIndex.type, type.ToString());
    private void SetCostTextDetail(float cost) => SetTextDetail(TDIndex.cost, cost.ToString("#.###"));

    private void SetAllTextDetails()
    {
        SetTypeTextDetail(type);
        SetCostTextDetail(cost);
    }

    private void SetActiveTextDetail(TDIndex index, bool active)
    {
        if (textDetails.Count <= (int)index) return;
        textDetails[(int)index].SetActive(active);
    }

    public void ShowTextDetails(bool showType, bool showCost)
    {
        SetActiveTextDetail(TDIndex.type, showType);
        SetActiveTextDetail(TDIndex.cost, showCost);
    }

    public void ShowAllTextDetails(bool show)
    {
        SetActiveTextDetail(TDIndex.type, show);
        SetActiveTextDetail(TDIndex.cost, show);
    }
    #endregion

    #region Weapon properties management
    public void SetWeaponUser(Status weaponUser) => this.weaponUser = weaponUser;

    public void IncreaseDamage(float amount) => damage += amount;

    public void DecreaseDamage(float amount)
    {
        damage -= amount;
        if (damage < 0) damage = 0;
    }
    
    public void SetDamage(float damage) => this.damage = damage < 0 ? 0 : damage;

    public void IncreaseCost(float amount)
    {
        cost += amount;
        SetCostTextDetail(cost);
    }
    
    public void DecreaseCost(float amount)
    {
        cost -= amount;
        if (cost < 0) cost = 0;
        SetCostTextDetail(cost);
    }
    
    public void SetCost(float cost)
    {
        this.cost = cost < 0 ? 0 : cost;
        SetCostTextDetail(this.cost);
    }

    // Calculate actual values based on player health
    public float GetRealAmount(float amount, float minAmount)
    {
        if (minAmount >= 1f || weaponUser == null) return amount;
        
        float actualMinAmount = amount * minAmount;
        float remainingAmount = amount - actualMinAmount;
        return actualMinAmount + remainingAmount * weaponUser.currentHealthPercentage;
    }

    public virtual void SetRealAmounts()
    {
        // Calculate base damage based on health
        float baseDamage = GetRealAmount(damage, minDamage);
        
        // Apply focusing damage bonus if player is focusing
        if (playerController != null && playerController.IsFocusing())
        {
            realDamage = baseDamage * focusDamageMultiplier;
        }
        else
        {
            realDamage = baseDamage;
        }
        
        // Calculate cost
        realCost = GetRealAmount(cost, minCost);
        SetCostTextDetail(realCost);
    }
    #endregion

    #region Attack methods
    public virtual void Attack() => Debug.Log("Attack");

    private bool CanAttack()
    {
        if (Input.GetMouseButtonDown(0) && isHeld)
        {
            if (enabledAttack)
            {
                attackCount++;
                attackRecoveryTimer = 0f;

                if (attackCount > attackMaxLimit && !attackMaxLimit.Equals(0))
                {
                    enabledAttack = false;
                    attackCount = 0;
                    spriteRenderer.color = cooldownColor;
                }
                else return true;
            }
        }

        return false;
    }

    public bool PerformAttack()
    {
        if (CanAttack())
        {
            Attack();

            return true;
        }

        return false;
    }

    // Apply damage and knockback
    public void MakeDamage(Collider2D collision)
    {
        if (collision == null) return;
        
        Status status = collision.GetComponent<Status>();
        if (status == null || (weaponUser != null && status.user == weaponUser.user)) 
            return;

        // Apply damage
        status.TakeDamage(realDamage);
        
        // Apply knockback if enabled
        if (!applyKnockback) return;
            
        Vector2 weaponPos = transform.position;
        
        // Try to apply knockback to an enemy
        EnemyController enemyController = collision.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            float force = knockbackForce;
            if (collision.CompareTag("Boss"))
                force *= _bossKnockbackMultiplier;
                
            enemyController.ApplyKnockback(weaponPos, force);
            return;
        }
        
        // Or try to apply knockback to the player
        SlimeKnightController playerController = collision.GetComponent<SlimeKnightController>();
        if (playerController != null)
            playerController.ApplyKnockback(weaponPos, knockbackForce);
    }
    #endregion
}