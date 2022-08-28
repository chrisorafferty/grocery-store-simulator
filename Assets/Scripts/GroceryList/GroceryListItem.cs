using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryListItem {

    public bool IsFullyPicked { get { return quantity <= quantityPicked; } }

    public ItemData itemData;
    public int quantity;
    public int quantityPicked;

    public GroceryListItem(ItemData itemData, int quantity) {
        this.itemData = itemData;
        this.quantity = quantity;
    }
}
