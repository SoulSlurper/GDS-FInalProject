using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Status : MonoBehaviour
{
    //[SerializeField] private StatusAmount _health;
    //[SerializeField] private StatusAmount _stamina;
    [SerializeField] private StatusUser _user;
    [SerializeField] private float _health = 100f;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private GameObject _dropItem; // object that appears midbattle or in death

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

    public bool noHealth
    { 
        get { return health <= 0f; }
        private set { }
    }

    public float maxHealth
    { 
        get { return _maxHealth; }
        private set { _maxHealth = value; }
    }

    public float currentHealthPercentage
    {
        get { return health / maxHealth; }
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
        maxHealth = health;

        if (healthBar)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    // Health functions // // // //
    public virtual void TakeDamage(float damage)
    {
        DecreaseHealth(damage);
    }

    public virtual void TakeHealth(float amount)
    {
        IncreaseHealth(amount);
    }

    private void SetHealthBar()
    {
        if (healthBar)
        {
            healthBar.SetHealth(this.health);
        }
    }

    public void ResetHealth()
    {
        health = maxHealth;

        SetHealthBar();
    }

    public void SetHealth(float health) 
    { 
        this.health = health >= 0 && health <= maxHealth ? health : this.health;

        SetHealthBar();
    }

    public void SetNewHealth(float health) 
    { 
        this.health = maxHealth = health;

        SetHealthBar();
    }

    public void IncreaseHealth(float health) 
    { 
        this.health += health;
        if (this.health > maxHealth) this.health = maxHealth;

        SetHealthBar();
    }

    public void DecreaseHealth(float health) 
    { 
        this.health -= health;
        if (this.health < 0) this.health = 0;

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
