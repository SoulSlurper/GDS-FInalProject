using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHp : Status
{
    [Header("Boss DropItem Details")]
    [SerializeField] private int _dropItemAmount = 1;
    [SerializeField] private int _dropItemEndAmount = 1;
    [SerializeField] private float _healthDropItemPercentage = 0f; //the percentage that indicates when the boss will drop the item
                                                                   //Note: must be below or equal to 1

    private float healthPercentCheckpoint = 1f;
    private float healthCheckpoint;

    // Unity // // // //
    public int dropItemAmount
    {
        get { return _dropItemAmount; }
        private set { }
    }

    public float healthDropItemPercentage
    {
        get { return _healthDropItemPercentage; }
        private set { }
    }

    // Unity // // // //
    void Start()
    {
        GetHealthCheckpoint();

        Debug.Log("HealthPercentCheckpoint: " + healthPercentCheckpoint);
        Debug.Log("HealthCheckpoint: " + healthCheckpoint);
    }

    void Update()
    {
        if (noHealth)
        {
            Debug.Log(gameObject.name + " Boss is destroyed");

            for (int i = 0; i < _dropItemEndAmount; i++) CreateDropItem();

            Destroy(gameObject);
        }
        else
        {
            if (health <= healthCheckpoint)
            {
                Debug.Log("Reached HealthCheckpoint: " + healthCheckpoint);

                for (int i = 0; i < _dropItemAmount; i++) CreateDropItem();

                GetHealthCheckpoint();
            }
        }
    }

    // Boss DropItem Details // // // //
    public void SetDropItemAmount(int dropItemAmount) { this.dropItemAmount = dropItemAmount;}

    public void SetHealthDropItemPercentage(float healthDropItemPercentage) { this.healthDropItemPercentage = healthDropItemPercentage; }

    // Health Percentage // // // //
    public void GetHealthCheckpoint()
    {
        if (healthDropItemPercentage > 0f && healthDropItemPercentage < 1f)
        {
            healthPercentCheckpoint -= healthDropItemPercentage;
            healthCheckpoint = max * healthPercentCheckpoint;
        }
        else healthCheckpoint = 0f;
    }
}
