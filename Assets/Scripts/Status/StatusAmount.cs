public class StatusAmount
{
    public float amount { get; private set; }

    public bool isPassive { get; private set; }
    public float passiveAmount { get; private set; }
    public float passiveDelay { get; private set; }

    public float maxAmount { get; private set; }
    public float timer { get; private set; }

    public StatusAmount()
    {
        this.amount = 100f;

        this.isPassive = false;
        this.passiveAmount = 1f;
        this.passiveDelay = 1f;

        this.maxAmount = this.amount;

        this.timer = 0f;
    }

    public StatusAmount(float Amount, bool isPassive = false, float passiveAmount = 1f, float passiveDelay = 1f)
    {
        this.amount = Amount;

        this.isPassive = isPassive;
        this.passiveAmount = passiveAmount;
        this.passiveDelay = passiveDelay;

        this.maxAmount = this.amount;

        this.timer = 0f;
    }



    // Amount // // // //
    public void SetAmount(float amount) { this.amount = amount >= 0 && amount <= maxAmount ? amount : this.amount; }

    public void SetNewAmount(float amount) { this.amount = maxAmount = amount; }

    public void IncreaseAmount(float amount) { this.amount += amount; }

    public void DecreaseAmount(float amount) { this.amount -= amount; }




    // Passive // // // //
    public void SetIsPassive(bool isPassiveAmount) { this.isPassive = isPassiveAmount; }

    public void SetPassiveAmount(float passiveAmount) { this.passiveAmount = passiveAmount; }

    public void SetPassiveDelay(float passiveDelay) { this.passiveDelay = passiveDelay; }



    // Timer // // // //
    public void IncreaseTimer(float seconds) { timer += seconds; }

    public void DecreaseTimer(float seconds) { timer -= seconds; }

    public void ResetTimer() { timer = 0f; }

    public bool TimerAboveDelay() { return timer > passiveDelay; }
}
