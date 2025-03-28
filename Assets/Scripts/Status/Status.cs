using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    //[SerializeField] private float stamina = 100f;

    [Header("Passive Regain")]
    [SerializeField] private bool isPassiveHealth = false;
    [SerializeField] private float passiveHealthAmount = 1f;
    [SerializeField] private float passiveHealthDelay = 1f;

    private float maxHealth = -1;
    private float healthTimer = 0f;

    void Start()
    {
        maxHealth = health;
    }

    void Update()
    {
        if (healthTimer > passiveHealthDelay)
        {
            if (isPassiveHealth && health < maxHealth)
            {
                health += passiveHealthAmount;

                if (health > maxHealth) health = maxHealth;
            }

            healthTimer = 0f;
        }

        healthTimer += Time.deltaTime;
    }

    public float GetHealth() { return health; }

    public float GetMaxHealth() { return maxHealth; }

    public void SetNewHealth(float health) { this.health = maxHealth = health; }

    public bool GetIsPassiveHealth() { return isPassiveHealth; }

    public void SetIsPassiveHealth(bool isPassiveHealth) { this.isPassiveHealth = isPassiveHealth; }

    public float GetPassiveHealthAmount() { return passiveHealthAmount; }

    public void SetPassiveHealthAmount(float passiveHealthAmount) { this.passiveHealthAmount = passiveHealthAmount; }

    public float GetPassiveHealthDelay() { return passiveHealthDelay; }

    public void SetPassiveHealthDelay(float passiveHealthDelay) { this.passiveHealthDelay = passiveHealthDelay; }

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
