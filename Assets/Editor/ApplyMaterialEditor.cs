using UnityEditor;
using UnityEngine;

public class ApplyMaterialEditor : EditorWindow
{
    [MenuItem("Tools/Apply Material To All Children")]
    static void ApplyMaterial()
    {
        if (Selection.activeTransform == null)
        {
            Debug.LogError("No parent object selected! Please select an object in the Hierarchy.");
            return;
        }

        // Changed this line - removed the cast to Material
        EditorGUIUtility.ShowObjectPicker<Material>(null, false, "", 0);
        
        EditorApplication.update += () =>
        {
            if (Event.current != null && Event.current.commandName == "ObjectSelectorClosed")
            {
                Material newMaterial = EditorGUIUtility.GetObjectPickerObject() as Material;
                if (newMaterial == null) return;

                ApplyMaterialRecursively(Selection.activeTransform, newMaterial);
                EditorApplication.update = null;
                Debug.Log("Material applied to all children!");
            }
        };
    }

    static void ApplyMaterialRecursively(Transform parent, Material material)
    {
        Renderer renderer = parent.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }

        foreach (Transform child in parent)
        {
            ApplyMaterialRecursively(child, material);
        }
    }
}