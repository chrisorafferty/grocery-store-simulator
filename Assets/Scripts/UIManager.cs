using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI groceryListText;
    public GameObject groceryListGameObject;

    // Start is called before the first frame update
    void Start()
    {
        SetMoneyTextFromValue(GameManager.playerWallet.amount);
        GameManager.playerWallet.deposit(100);
    }

    void Awake() {
        UpdateGroceryListUI(null);
    }

    void UpdateGroceryListUI(GroceryList groceryList) {
        groceryListGameObject.SetActive(groceryList != null);
        if (groceryList == null) return;

        StringBuilder listString = new StringBuilder();
        foreach(GroceryListItem item in groceryList.listItems) {
            if (item.quantityPicked == 0) {
                listString.Append($"{item.quantity}x {item.itemData.itemName}");
            } else if (item.quantityPicked == item.quantity) {
                listString.Append($"<s>{item.quantity}x {item.itemData.itemName}</s>");
            } else {
                listString.Append($"<s>{item.quantity}</s>{(item.quantity - item.quantityPicked)}x {item.itemData.itemName}");
            }
            listString.Append("\n");
        }
        groceryListText.SetText(listString.ToString());
    }

    void OnPlayerGroceryListUpdated(GroceryList groceryList) {        
        UpdateGroceryListUI(groceryList);
    }

    public void OnWalletValueUpdated(float newAmount)
    {
        SetMoneyTextFromValue(newAmount);
    }

    public void SetMoneyTextFromValue(float value)
    {
        moneyText.text = $"Money: {value:C2}";
    }

    void OnEnable() {
        GameManager.playerWallet.walletAmountUpdatedEvent += OnWalletValueUpdated;
        GameManager.playerGroceryListUpdatedEvent += OnPlayerGroceryListUpdated;
    }

    void OnDisable() {
        GameManager.playerWallet.walletAmountUpdatedEvent -= OnWalletValueUpdated;
        GameManager.playerGroceryListUpdatedEvent -= OnPlayerGroceryListUpdated;
    }
}
