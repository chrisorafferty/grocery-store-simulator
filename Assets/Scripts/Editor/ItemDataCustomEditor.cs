using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

[CustomEditor(typeof(ItemData))]
public class ItemDataCustomEditor : Editor {

    private const string ICON_ASSET_PATH = "Assets/Items/Icons_DO_NOT_REFERENCE_DIRECTLY/";
    private const string ICON_NAME_SEGMENT = "_Icon_";

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        ItemData itemData = (ItemData)target;

        if (itemData != null && itemData.spawnablePrefab != null && GUILayout.Button("Generate Icon")) {
            // Delete any old icons as we are recreating them
            if (itemData.icon != null) {
                itemData.icon = null;
                string[] iconFolder = { ICON_ASSET_PATH };
                foreach (string guid in AssetDatabase.FindAssets(itemData.name + ICON_NAME_SEGMENT, iconFolder)) {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    AssetDatabase.DeleteAsset(path);
                }
            }

            // Setup a preview scene to render in          
            PreviewRenderUtility renderUtility = new PreviewRenderUtility(true, true);
            renderUtility.camera.transform.position = new Vector3(0, itemData.iconGenerationData.camHeight, -itemData.iconGenerationData.camDistance);
            renderUtility.camera.transform.rotation = Quaternion.identity;
            renderUtility.camera.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
            renderUtility.camera.clearFlags = CameraClearFlags.SolidColor;
            renderUtility.camera.fieldOfView = itemData.iconGenerationData.camFov;

            // Setup the object to render
            GameObject targetObj = renderUtility.InstantiatePrefabInScene(itemData.spawnablePrefab.gameObject);
            targetObj.transform.position = Vector3.zero;
            targetObj.transform.Rotate(itemData.iconGenerationData.objRotation);

            // Render the scene and extract the texture
            Rect rect = new Rect(0,0,256,256);
            renderUtility.BeginStaticPreview(rect);
            renderUtility.camera.Render();
            Texture2D icon = renderUtility.EndStaticPreview();
            renderUtility.Cleanup();       

            // Save the texture to disk
            byte[] iconData = icon.EncodeToPNG();
            string iconName = itemData.itemName + ICON_NAME_SEGMENT + System.DateTime.Now.ToString("yyyy-mm-dd_HH-mm-ss") +".png";
            string assetPath = ICON_ASSET_PATH + iconName;
            File.WriteAllBytes(Application.dataPath + "/../" + assetPath, iconData);
            AssetDatabase.Refresh();

            // Set the import settings on the texture to convert it to a sprite
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            textureImporter.spritePixelsPerUnit = 100;
            textureImporter.mipmapEnabled = false;
            textureImporter.textureType = TextureImporterType.Sprite;
            EditorUtility.SetDirty(textureImporter);
            textureImporter.SaveAndReimport();

            Sprite loadedTexture = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

            // Save the icon in the ScriptableObject
            EditorUtility.SetDirty(target);
            itemData.icon = loadedTexture;
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        // Render a preview of the icon
        if (itemData.icon != null) {
            GUILayout.Label(itemData.icon.texture);
        }
    }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
        ItemData itemData = (ItemData)target;

        if (itemData != null && itemData.icon != null) {
            RenderTexture rt = new RenderTexture(width, height, 24);
            RenderTexture.active = rt;
            Graphics.Blit(itemData.icon.texture, rt);
            Texture2D result = new Texture2D(width, height);
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();
            return result;
        }

        return base.RenderStaticPreview(assetPath, subAssets, width, height);
    }
}
