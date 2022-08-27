using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfLocation : MonoBehaviour {

    public ItemController item;

    public bool Filled { get { return item != null; } }

    public bool SetItem(ItemData itemData) {
        if (Filled) return false;

        item = Instantiate(itemData.spawnablePrefab, transform.position, transform.rotation, transform);

        return true;
    }

    public ItemController RemoveItem() {
        ItemController item = this.item;

        Destroy(item.gameObject);
        this.item = null;

        return item;
    }
}
