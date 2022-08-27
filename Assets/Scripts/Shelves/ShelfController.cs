using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfController : Interactable {

    public ItemData itemData { get; protected set; }
    public ShelfLocation[] shelfLocations;

    public bool SetItemType(ItemData itemType) {        
        foreach(ShelfLocation location in shelfLocations) {
            if (location.Filled) return false;
        }
        itemData = itemType;
        return true;
    }

    public bool Restock() {
        if (itemData == null) return false;

        foreach(ShelfLocation location in shelfLocations) {
            if (!location.Filled) {
                location.SetItem(itemData);
                return true;
            }
        }
        return false;
    }

    public bool TakeItem() {
        bool tookItem = false;
        bool allEmpty = true;

        foreach(ShelfLocation location in shelfLocations) {
            if (!tookItem && location.Filled) {
                location.RemoveItem();
                tookItem = true;
            }

            if (location.Filled) allEmpty = false;
        }

        if (allEmpty) itemData = null;

        return tookItem;
    }

    public override void OnEnter() {}
    public override void OnExit() {}
}
