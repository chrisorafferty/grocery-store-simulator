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
            walletAmountUpdatedEvent?.Invoke(_amount);
        }
    }
    public bool allowNegativeValue = false;

    public delegate void WalletAmountUpdated(float newAmount);
    public event WalletAmountUpdated walletAmountUpdatedEvent;

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
}
