using System;
using UnityEngine;

[Serializable]
public class StatusAmount
{
    [SerializeField] private float _amount = 100f;

    private float _max;

    private bool isMaxSet = false;

    // Getter and Setter // // // //
    public float amount
    {
        get { return _amount; }
        private set { _amount = value; }
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
}
