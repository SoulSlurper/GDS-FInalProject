using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    [SerializeField] private StatusAmount _health;
    //[SerializeField] private StatusAmount _stamina;

    public StatusAmount health
    { 
        get { return _health; }
        private set { }
    }

    public bool noHealth
    { 
        get { return health.amount <= 0f; }
        private set { }
    }

    void Update()
    {
        health.RegainingAmount(Time.deltaTime);

        if (noHealth)
        {
            Destroy(gameObject);
        }
    }
}
