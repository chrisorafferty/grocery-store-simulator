using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Custom/ItemData", order = 1)]
public class ItemData : ScriptableObject {
    public string itemName;
    public ItemController spawnablePrefab;
    public Texture2D icon;
    public float moneyValue = 1.0f;
    public IconGenerationData iconGenerationData = new IconGenerationData(3, 0.2f, 10, new Vector3(0, 180, 0));
}

[System.Serializable]
public struct IconGenerationData {
    public float camDistance;
    public float camHeight;
    public float camFov;
    public Vector3 objRotation;

    public IconGenerationData(float camDistance, float camHeight, float camFov, Vector3 objRotation) {
        this.camDistance = camDistance;
        this.camHeight = camHeight;
        this.camFov = camFov;
        this.objRotation = objRotation;
    }
}
