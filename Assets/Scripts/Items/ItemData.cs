using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Custom/ItemData", order = 1)]
public class ItemData : ScriptableObject {
    public string itemName;
    public ItemController spawnablePrefab;
    public Sprite icon;
}
