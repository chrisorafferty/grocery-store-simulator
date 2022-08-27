using UnityEngine;

[CreateAssetMenu(fileName = "Wallet", menuName = "Custom/Wallet", order = 1)]
public class Wallet : ScriptableObject
{
    public float value = 0.0f;
    public bool allowNegativeValue = false;

    public void deposit(float additionalValue)
    {
        this.value += additionalValue;
    }

    public bool tryWithdraw(float subtractValue)
    {
        float newValue = this.value - subtractValue;
        if ((!this.allowNegativeValue) && newValue < 0) return false;
        this.value = newValue;
        return true;
    }
}
