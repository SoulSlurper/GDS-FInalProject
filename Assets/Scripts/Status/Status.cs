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

    public StatusAmount healthStatus { get; private set; }

    void Start()
    {
        healthStatus = new StatusAmount(health, isPassiveHealth, passiveHealthAmount, passiveHealthDelay);
    }

    void Update()
    {
        RegainStatusAmount(healthStatus);
        MaintainHealthStatusVariables(healthStatus);
    }

    private void RegainStatusAmount(StatusAmount _statusAmount)
    {
        if (_statusAmount.isPassive)
        {
            if (_statusAmount.TimerAboveDelay())
            {
                _statusAmount.ResetTimer();

                if (_statusAmount.amount < _statusAmount.maxAmount)
                {
                    _statusAmount.IncreaseAmount(_statusAmount.passiveAmount);

                    if (_statusAmount.amount > _statusAmount.maxAmount) _statusAmount.SetAmount(_statusAmount.maxAmount);
                }
            }

            _statusAmount.IncreaseTimer(Time.deltaTime);
        }
    }

    private void MaintainHealthStatusVariables(StatusAmount _statusAmount)
    {
        if (health != _statusAmount.amount) health = _statusAmount.amount;
        if (isPassiveHealth != _statusAmount.isPassive) isPassiveHealth = _statusAmount.isPassive;
        if (passiveHealthAmount != _statusAmount.passiveAmount) passiveHealthAmount = _statusAmount.passiveAmount;
        if (passiveHealthDelay != _statusAmount.passiveDelay) health = passiveHealthDelay = _statusAmount.passiveDelay;
    }
}
