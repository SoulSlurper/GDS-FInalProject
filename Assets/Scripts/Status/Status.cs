using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    [SerializeField] private StatusAmount _health;

    public StatusAmount health 
    { 
        get { return _health; }
        private set { }
    }

    void Start()
    {
    }

    void Update()
    {
        RegainStatusAmount(_health);

        Debug.LogWarning(gameObject.name);
        Debug.Log("_health.amount: " + _health.amount);
        Debug.Log("_health.max: " + _health.max);
    }

    private void RegainStatusAmount(StatusAmount sAmount)
    {
        if (sAmount.isPassive && sAmount.amount < sAmount.max)
        {
            if (sAmount.TimerAboveDelay())
            {
                sAmount.ResetTimer();

                sAmount.IncreaseAmount(sAmount.passiveAmount);

                if (sAmount.amount > sAmount.max) sAmount.SetAmount(sAmount.max);
            }

            sAmount.IncreaseTimer(Time.deltaTime);
        }
    }
}
