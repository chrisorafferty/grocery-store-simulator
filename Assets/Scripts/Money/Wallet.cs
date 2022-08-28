using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Wallet", menuName = "Custom/Wallet", order = 1)]
public class Wallet : ScriptableObject
{
    public float value {
        get => value;
        set
        {
            this.value = value;
            var e = new WalletValueUpdatedEventArgs();
            e.NewValue = value;
            OnWalletValueUpdated(e);
        }
    }
    public bool allowNegativeValue = false;
    public event EventHandler<WalletValueUpdatedEventArgs> WalletValueUpdated;

    public void deposit(float additionalValue)
    {
        value += additionalValue;
    }

    public bool tryWithdraw(float subtractValue)
    {
        float newValue = value - subtractValue;
        if ((!allowNegativeValue) && newValue < 0) return false;
        value = newValue;
        return true;
    }

    protected virtual void OnWalletValueUpdated(WalletValueUpdatedEventArgs e)
    {
        EventHandler<WalletValueUpdatedEventArgs> handler = WalletValueUpdated;
        if (handler != null)
        {
            handler(this, e);
        }
    }

}
