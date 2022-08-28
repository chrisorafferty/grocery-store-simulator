using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {

    public TextMeshProUGUI groceryListText;

    void UpdateGroceryListUI(GroceryList groceryList) {
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

    void OnEnable() {
        GameManager.playerGroceryListUpdatedEvent += OnPlayerGroceryListUpdated;
    }

    void OnDisable() {
        GameManager.playerGroceryListUpdatedEvent -= OnPlayerGroceryListUpdated;
    }
}
