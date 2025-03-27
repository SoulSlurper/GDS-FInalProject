using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    [SerializeField] private float health;

    public float GetHealth() { return health; }

    public void SetHealth(float health) { this.health = health; }

    public void DecreaseHealth(float amount)
    {
        health -= amount;

        Debug.Log(gameObject.name + " Health: " + health);
    }

    public void IncreaseHealth(float amount)
    {
        health += amount;

        Debug.Log(gameObject.name + " Health: " + health);
    }
}
