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
    public void SetAmount(float amount) { this.amount = amount >= 0 && amount <= max ? amount : this.amount; }

    public void SetNewAmount(float amount) { this.amount = max = amount; }

    public void IncreaseAmount(float amount) { this.amount += amount; }

    public void DecreaseAmount(float amount) { this.amount -= amount; }

    private void SetMax(float amount) 
    { 
        if (!isMaxSet)
        {
            max = amount;

            isMaxSet = true;
        }
    }

    //this is to be placed in the Update unity function
    public void RegainingAmount(float seconds)
    {
        if (isPassive && amount < max)
        {
            if (timer > passiveDelay)
            {
                ResetTimer();

                IncreaseAmount(passiveAmount);

                if (amount > max) SetAmount(max);
            }

            IncreaseTimer(seconds);
        }
    }




    // Passive // // // //
    public void SetPassiveAmount(float passiveAmount) { this.passiveAmount = passiveAmount; }

    public void SetPassiveDelay(float passiveDelay) { this.passiveDelay = passiveDelay; }



    // Timer // // // //
    public void IncreaseTimer(float seconds) { timer += seconds; }

    public void DecreaseTimer(float seconds) { timer -= seconds; }

    public void ResetTimer() { timer = 0f; }

    public bool TimerAboveDelay() { return timer > passiveDelay; }
}
