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
    [SerializeField] private GameObject _dropItem; // object that appears midbattle or in death
    public HealthBar healthBar;
    public int maxHealth = 100;
    public int currentHealth;

    private float _max;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

    }


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

    public bool noHealth
    { 
        get { return health <= 0f; }
        private set { }
    }

    public float health
    { 
        get { return _health; }
        private set { _health = value; }
    }

    public float max
    { 
        get { return _max; }
        private set { _max = value; }
    }

    // Unity // // // //
    void Awake()
    {
        max = health;
    }

    // Health functions // // // //
    public void TakeDamage(float damage)
    {
        DecreaseHealth(damage);
    }

    public void TakeHealth(float amount)
    {
        IncreaseHealth(amount);

        if (health > max) health = max;
    }

    public void ResetHealth()
    {
        health = max;
    }

    public void SetHealth(float health) 
    { 
        this.health = health >= 0 && health <= max ? health : this.health;

        currentHealth = (int)this.health; 
        healthBar.SetHealth(currentHealth);
    }

    public void SetNewHealth(float health) 
    { 
        this.health = max = health;

        currentHealth = (int)this.health; 
        healthBar.SetHealth(currentHealth);
    }

    public void IncreaseHealth(float health) 
    { 
        this.health += health;

        currentHealth = (int)this.health;
        healthBar.SetHealth(currentHealth);
    }

    public void DecreaseHealth(float health) 
    { 
        this.health -= health;

        currentHealth = (int)this.health;
        healthBar.SetHealth(currentHealth);
    }

    // dropItem functions // // // //
    public void SetDropItem(GameObject dropItem) { this.dropItem = dropItem; }

    public void CreateDropItem()
    {
        Debug.Log(dropItem.name + " Item created");

        Instantiate(dropItem, transform.position, Quaternion.identity);
    }
}
