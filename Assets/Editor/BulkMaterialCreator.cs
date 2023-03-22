using UnityEditor;
using UnityEngine;

public class BulkMaterialCreator : ScriptableWizard {

    public string PropertyName = "_MainTex";
    public string Suffix;
    public Shader Shader;

    [MenuItem(itemName: "Assets/Bulk Material Creator")]
    public static void CreateWizard() => DisplayWizard(title: "Bulk Material Creator", klass: typeof(BulkMaterialCreator));

    public void OnWizardUpdate() { }

    public void OnWizardCreate() {
        foreach (Object obj in Selection.objects) {
            if (obj.GetType() != typeof(Texture2D)) continue;
            Texture2D texture = (Texture2D)obj;
            Material material = GenerateMaterial(texture);
            var path = GetDirectory(obj) + "/" + material.name + ".mat";
            AssetDatabase.CreateAsset(asset: material, path);
        }
    }

    private Material GenerateMaterial(Texture2D texture) {
        Material material = new (Shader) {
            name = texture.name + Suffix
        };
        material.SetTexture(PropertyName, texture);
        return material;
    }

    private string GetDirectory(Object obj) {
        var path = AssetDatabase.GetAssetPath(assetObject: obj);
        if (path.Contains(value: '/')) {
            path = path.Substring(startIndex: 0, length: path.LastIndexOf(value: '/'));
        }
        return path;
    }
}