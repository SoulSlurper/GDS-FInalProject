using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Status : MonoBehaviour
{
    [SerializeField] private StatusUser _user;
    [SerializeField] private float _health = 100f;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private GameObject _dropItem; // object that appears midbattle or in death

    private float _currentHealth;
    private float _maxHealth;

    // Getter and Setter // // // //
    public StatusUser user
    { 
        get { return _user; }
        private set { _user = value; }
    }

    public GameObject dropItem
    { 
        get { return _dropItem; }
        private set { _dropItem = value; }
    }

    public float health
    { 
        get { return _health; }
        private set { _health = value; }
    }

    public float currentHealth
    { 
        get { return _currentHealth; }
        private set { _currentHealth = value; }
    }

    public bool noHealth
    { 
        get { return currentHealth <= 0f; }
        private set { }
    }

    public float maxHealth
    { 
        get { return _maxHealth; }
        private set { _maxHealth = value; }
    }

    public float currentHealthPercentage
    {
        get { return currentHealth / maxHealth; }
        private set { }
    }

    public HealthBar healthBar
    { 
        get { return _healthBar; }
        private set { _healthBar = value; }
    }

    // Unity // // // //
    void Awake()
    {
        maxHealth = currentHealth = health;

        if (healthBar)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    // Health functions // // // //
    public virtual void TakeDamage(float damage)
    {
        DecreaseCurrentHealth(damage);
    }

    public virtual void TakeHealth(float amount)
    {
        IncreaseCurrentHealth(amount);
    }

    private void SetHealthBar()
    {
        if (healthBar)
        {
            healthBar.SetHealth(currentHealth);
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;

        SetHealthBar();
    }

    public void SetCurrentHealth(float currentHealth) 
    { 
        this.currentHealth = currentHealth >= 0 && currentHealth <= maxHealth ? currentHealth : this.currentHealth;

        SetHealthBar();
    }

    public void SetNewHealth(float health) 
    { 
        this.health = currentHealth = maxHealth = health;

        SetHealthBar();
    }

    public void IncreaseCurrentHealth(float currentHealth) 
    { 
        this.currentHealth += currentHealth;
        if (this.currentHealth > maxHealth) this.currentHealth = maxHealth;

        SetHealthBar();
    }

    public void DecreaseCurrentHealth(float currentHealth) 
    { 
        this.currentHealth -= currentHealth;
        if (this.currentHealth < 0) this.currentHealth = 0;

        SetHealthBar();
    }

    // dropItem functions // // // //
    public void SetDropItem(GameObject dropItem) { this.dropItem = dropItem; }

    public void InstantiateItem(GameObject item)
    {
        Debug.Log(item.name + " Item created");

        Instantiate(item, transform.position, Quaternion.identity);
    }

    public void CreateDropItem()
    {
        InstantiateItem(dropItem);
    }
}
