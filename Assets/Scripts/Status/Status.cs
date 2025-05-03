using UnityEngine;

public class Status : MonoBehaviour
{
    [SerializeField] private StatusUser _user;
    [SerializeField] private float health = 100f;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private GameObject _dropItem;

    private float _maxHealth;

    // Event for damage taken - can be used for UI updates, sound effects, etc.
    public delegate void DamageEvent(float amount, Vector2 damageSource);
    public event DamageEvent OnDamageTaken;

    // Property getters/setters
    public StatusUser user { get { return _user; } private set { _user = value; } }
    public GameObject dropItem { get { return _dropItem; } private set { _dropItem = value; } }
    public float currentHealth { get { return health; } private set { health = value; } }
    public bool noHealth { get { return currentHealth <= 0f; } }
    public float maxHealth { get { return _maxHealth; } private set { _maxHealth = value; } }
    public float currentHealthPercentage { get { return currentHealth / maxHealth; } }
    public HealthBar healthBar { get { return _healthBar; } private set { _healthBar = value; } }

    void Awake()
    {
        SetNewHealth(health);
    }

    // Health bar management
    public void SetHealthBar(HealthBar healthBar)
    {
        this.healthBar = healthBar;
    }

    private void SetupHealthBar()
    {
        if (healthBar)
        {
            healthBar.SetActiveState(true);
            healthBar.SetMaxValue(maxHealth);
            healthBar.SetValue(maxHealth);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar)
        {
            healthBar.SetValue(currentHealth);
        }
    }

    // Damage System - returns the actual damage dealt
    public virtual float TakeDamage(float damage, Vector2? damageSource = null)
    {
        if (damage <= 0) return 0;

        float previousHealth = currentHealth;
        DecreaseCurrentHealth(damage);
        
        // Calculate actual damage dealt
        float actualDamage = previousHealth - currentHealth;
        
        // Trigger event if damage was taken and source provided
        if (actualDamage > 0 && damageSource.HasValue)
        {
            OnDamageTaken?.Invoke(actualDamage, damageSource.Value);
        }
        
        return actualDamage;
    }

    // Healing System
    public virtual void TakeHealth(float amount)
    {
        IncreaseCurrentHealth(amount);
    }

    // Health management
    public void InstantDeath()
    {
        currentHealth = 0;
        UpdateHealthBar();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void SetCurrentHealth(float currentHealth)
    {
        this.currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    public void SetNewHealth(float health)
    {
        currentHealth = maxHealth = health;
        SetupHealthBar();
    }

    public void IncreaseCurrentHealth(float amount)
    {
        this.currentHealth = Mathf.Min(this.currentHealth + amount, maxHealth);
        UpdateHealthBar();
    }

    public void DecreaseCurrentHealth(float amount)
    {
        this.currentHealth = Mathf.Max(this.currentHealth - amount, 0);
        UpdateHealthBar();
    }

    // Item drop system
    public void SetDropItem(GameObject dropItem) { this.dropItem = dropItem; }

    public void InstantiateItem(GameObject item)
    {
        Debug.Log(item.name + " Item created");
        Instantiate(item, transform.position, Quaternion.identity);
    }

    public void CreateDropItem()
    {
        if (dropItem)
        {
            InstantiateItem(dropItem);
        }
    }
}