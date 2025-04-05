using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Status : MonoBehaviour
{
    //[SerializeField] private StatusAmount _health;
    //[SerializeField] private StatusAmount _stamina;
    [SerializeField] private StatusUser _user;
    [SerializeField] private float health = 100f;

    private float _max;

    // Getter and Setter // // // //
    public StatusUser user
    { 
        get { return _user; }
        private set { _user = value; }
    }

    public bool noHealth
    { 
        get { return health <= 0f; }
        private set { }
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

    // Functions // // // //
    public void TakeDamage(float damage)
    {
        DecreaseHealth(damage);
    }

    public void ResetHealth()
    {
        health = max;
    }

    public void SetHealth(float health) { this.health = health >= 0 && health <= max ? health : this.health; }

    public void SetNewHealth(float health) { this.health = max = health; }

    public void IncreaseHealth(float health) { this.health += health; }

    public void DecreaseHealth(float health) { this.health -= health; }
}
