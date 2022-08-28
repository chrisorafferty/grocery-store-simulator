using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroceryListManager : MonoBehaviour {
    public static GroceryListManager instance;

    public int maxQuantityOfItem = 5;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(instance);
        }

        instance = this;
    }

    public static GroceryList GenerateRandomGroceryList() {
        List<ItemData> potentialItems = new List<ItemData>(GameManager.instance.items);

        int numItems = Random.Range(1, potentialItems.Count);
        List<GroceryListItem> listItems = new List<GroceryListItem>(numItems);

        for (int i = 0; i < numItems; i++) {
            ItemData itemData = potentialItems[Random.Range(0, potentialItems.Count)];
            int quantity = Random.Range(1, instance.maxQuantityOfItem + 1);
            listItems.Add(new GroceryListItem(itemData, quantity));

            potentialItems.Remove(itemData);
        }

        return new GroceryList(listItems);
    }
}
