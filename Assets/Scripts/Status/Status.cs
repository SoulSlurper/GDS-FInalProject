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
    [SerializeField] private HealthBar healthBar;
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

    // Unity // // // //
    void Awake()
    {
        maxHealth = health;

        if (healthBar)
        {
            healthBar.SetMaxHealth(health);
        }
    }

    // Health functions // // // //
    public void TakeDamage(float damage)
    {
        DecreaseHealth(damage);
    }

    public void TakeHealth(float amount)
    {
        IncreaseHealth(amount);
    }

    public void ResetHealth()
    {
        health = maxHealth;

        healthBar.SetHealth(health);
    }

    public void SetHealth(float health) 
    { 
        this.health = health >= 0 && health <= maxHealth ? health : this.health;

        healthBar.SetHealth(this.health);
    }

    public void SetNewHealth(float health) 
    { 
        this.health = maxHealth = health;

        healthBar.SetHealth(this.health);
    }

    public void IncreaseHealth(float health) 
    { 
        this.health += health;
        if (health > maxHealth) health = maxHealth;

        healthBar.SetHealth(this.health);
    }

    public void DecreaseHealth(float health) 
    { 
        this.health -= health;
        if (health < 0) health = 0;

        healthBar.SetHealth(this.health);
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
