using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryList {

    public List<GroceryListItem> listItems = new List<GroceryListItem>();

    public delegate void GroceryListUpdated(GroceryList groceryList);
    public event GroceryListUpdated groceryListUpdatedEvent;

    public GroceryList(List<GroceryListItem> listItems) {
        this.listItems = listItems;
    }

    public GroceryListItem GetItemOnList(ItemData itemData) {
        return listItems.Find(item => item.itemData == itemData);
    }

    public bool pickItemOnList(ItemData itemData) {
        GroceryListItem listItem = GetItemOnList(itemData);
        if (listItem == null || listItem.IsFullyPicked) return false;

        listItem.quantityPicked++;

        groceryListUpdatedEvent?.Invoke(this);
        return true;
    }
}
