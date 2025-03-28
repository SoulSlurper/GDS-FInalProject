using System;
using UnityEngine;

[Serializable]
public class StatusAmount
{
    [SerializeField] private float _amount = 100f;

    [SerializeField] private bool _isPassive = false;
    [SerializeField] private float _passiveAmount = 1f;
    [SerializeField] private float _passiveDelay = 1f;

    private float _max;
    private float _timer;

    private bool isMaxSet = false;

    public float amount 
    { 
        get 
        {
            SetMax(_amount);

            return _amount; 
        }
        private set { _amount = value; }
    }

    public bool isPassive
    { 
        get { return _isPassive; }
        set { _isPassive = value; }
    }
    public float passiveAmount
    { 
        get { return _passiveAmount; }
        private set { _passiveAmount = value; }
    }
    public float passiveDelay
    {
        get { return _passiveDelay; }
        private set { _passiveDelay = value; }
    }

    public float max
    {
        get
        {
            SetMax(_amount);

            return _max;
        }
        private set { _max = value; }
    }
    public float timer
    { 
        get { return _timer; }
        private set { _timer = value; }
    }




    // Amount // // // //
    public void SetAmount(float amount) { _amount = amount >= 0 && amount <= _max ? amount : _amount; }

    public void SetNewAmount(float amount) { _amount = _max = amount; }

    public void IncreaseAmount(float amount) { _amount += amount; }

    public void DecreaseAmount(float amount) { _amount -= amount; }

    private void SetMax(float amount) 
    { 
        if (!isMaxSet)
        {
            _max = amount;

            isMaxSet = true;
        }
    }




    // Passive // // // //
    public void SetIsPassive(bool isPassiveAmount) { _isPassive = isPassiveAmount; }

    public void SetPassiveAmount(float passiveAmount) { _passiveAmount = passiveAmount; }

    public void SetPassiveDelay(float passiveDelay) { _passiveDelay = passiveDelay; }



    // Timer // // // //
    public void IncreaseTimer(float seconds) { _timer += seconds; }

    public void DecreaseTimer(float seconds) { _timer -= seconds; }

    public void ResetTimer() { _timer = 0f; }

    public bool TimerAboveDelay() { return _timer > _passiveDelay; }
}
