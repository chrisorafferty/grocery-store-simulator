using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public Wallet playerWallet;

    // Start is called before the first frame update
    void Start()
    {
        SetMoneyTextFromValue(playerWallet.value);
        playerWallet.WalletValueUpdated += OnWalletValueUpdated;
        playerWallet.deposit(100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnWalletValueUpdated(object sender, WalletValueUpdatedEventArgs e)
    {
        SetMoneyTextFromValue(e.NewValue);
    }

    public void SetMoneyTextFromValue(float value)
    {
        moneyText.text = $"Money: ${value:0.2f}";
    }
}
