using System;
using UnityEngine;

public class Wallet
{
    private float _amount = 0.0f;
    public float amount {
        get => _amount;
        set
        {
            _amount = value;
            var e = new WalletValueUpdatedEventArgs();
            e.NewValue = _amount;
            OnWalletValueUpdated(e);
        }
    }
    public bool allowNegativeValue = false;
    public event EventHandler<WalletValueUpdatedEventArgs> WalletValueUpdated;

    public void deposit(float depositAmount)
    {
        amount += depositAmount;
    }

    public bool tryWithdraw(float withdrawalAmount)
    {
        float newValue = amount - withdrawalAmount;
        if ((!allowNegativeValue) && newValue < 0) return false;
        amount = newValue;
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
