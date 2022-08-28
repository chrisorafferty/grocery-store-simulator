using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

[CustomEditor(typeof(ItemData))]
public class ItemDataCustomEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        ItemData itemData = (ItemData)target;

        if (itemData != null && itemData.spawnablePrefab != null && GUILayout.Button("Generate Icon")) {
            if (itemData.icon != null) {
                itemData.icon = null;
                string[] iconFolder = { "Assets/Items/Icons/" };
                foreach (string guid in AssetDatabase.FindAssets(itemData.name + "_Icon_", iconFolder)) {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    AssetDatabase.DeleteAsset(path);
                }
            }

            PreviewRenderUtility renderUtility = new PreviewRenderUtility(true, true);
            renderUtility.camera.transform.position = new Vector3(0, itemData.iconGenerationData.camHeight, -itemData.iconGenerationData.camDistance);
            renderUtility.camera.transform.rotation = Quaternion.identity;
            renderUtility.camera.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
            renderUtility.camera.clearFlags = CameraClearFlags.SolidColor;
            renderUtility.camera.fieldOfView = itemData.iconGenerationData.camFov;

            GameObject targetObj = renderUtility.InstantiatePrefabInScene(itemData.spawnablePrefab.gameObject);
            targetObj.transform.position = Vector3.zero;
            targetObj.transform.Rotate(itemData.iconGenerationData.objRotation);

            Rect rect = new Rect(0,0,256,256);
            renderUtility.BeginStaticPreview(rect);
            renderUtility.camera.Render();
            Texture2D icon = renderUtility.EndStaticPreview();
            renderUtility.Cleanup();

            byte[] iconData = icon.EncodeToPNG();
            string iconName = itemData.itemName + "_Icon_" + System.DateTime.Now.ToString("yyyy-mm-dd_HH-mm-ss") +".png";
            string assetPath = "Assets/Items/Icons/" + iconName;

            File.WriteAllBytes(Application.dataPath + "/../" + assetPath, iconData);
            AssetDatabase.Refresh();
            Texture2D loadedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            EditorUtility.SetDirty(target);
            itemData.icon = loadedTexture;
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        if (itemData.icon != null) {
            GUILayout.Label(itemData.icon);
        }
    }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
        ItemData itemData = (ItemData)target;

        if (itemData != null && itemData.icon != null) {
            RenderTexture rt = new RenderTexture(width, height, 24);
            RenderTexture.active = rt;
            Graphics.Blit(itemData.icon, rt);
            Texture2D result = new Texture2D(width, height);
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();
            return result;
        }

        return base.RenderStaticPreview(assetPath, subAssets, width, height);
    }
}
