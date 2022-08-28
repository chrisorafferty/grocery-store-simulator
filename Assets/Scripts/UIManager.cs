using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI groceryListText;
    public GameObject groceryListGameObject;

    // Start is called before the first frame update
    void Start()
    {
        SetMoneyTextFromValue(GameManager.playerWallet.amount);
        GameManager.playerWallet.WalletValueUpdated += OnWalletValueUpdated;
        GameManager.playerWallet.deposit(100);
    }

    void Awake() {
        UpdateGroceryListUI(null);
    }

    void UpdateGroceryListUI(GroceryList groceryList) {
        groceryListGameObject.SetActive(groceryList != null);
        if (groceryList == null) return;

        string listString = "";
        foreach(GroceryListItem item in groceryList.listItems) {
            if (item.quantityPicked == 0) {
                listString += item.quantity + "x " + item.itemData.itemName;
            } else if (item.quantityPicked == item.quantity) {
                listString += "<s>" + item.quantity + "x " + item.itemData.itemName + "</s>";
            } else {
                listString += "<s>" + item.quantity + "</s> " + (item.quantity - item.quantityPicked) + "x " + item.itemData.itemName;
            }
            listString += "\n";
        }
        groceryListText.SetText(listString);
    }

    void OnPlayerGroceryListUpdated(GroceryList groceryList) {        
        UpdateGroceryListUI(groceryList);
    }

    public void OnWalletValueUpdated(object sender, WalletValueUpdatedEventArgs e)
    {
        SetMoneyTextFromValue(e.NewValue);
    }

    public void SetMoneyTextFromValue(float value)
    {
        moneyText.text = $"Money: {value:C2}";
    }
}
