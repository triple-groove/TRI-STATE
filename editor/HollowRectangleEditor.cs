/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care
*/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class HollowRectangleEditor : EditorWindow
{
    private float width = 1f;
    private float depth = 2f;
    private float thickness = 0.1f;
    private Material material;

    private static readonly string defaultMaterialPath = "Assets/Materials/LightLines.mat";

    [MenuItem("Tools/Hollow Rectangle Editor")]
    public static void ShowWindow()
    {
        GetWindow<HollowRectangleEditor>("Hollow Rectangle Editor");
    }

    void OnEnable()
    {
        material = AssetDatabase.LoadAssetAtPath<Material>(defaultMaterialPath);
    }

    private void OnGUI()
    {
        GUILayout.Label("Hollow Rectangle Settings", EditorStyles.boldLabel);

        width = EditorGUILayout.FloatField("Width", width);
        depth = EditorGUILayout.FloatField("Height", depth);
        thickness = EditorGUILayout.FloatField("Thickness", thickness);
        material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), false);

        if (GUILayout.Button("Generate Hollow Rectangle"))
        {
            GenerateHollowRectangle();
        }
    }

    private void GenerateHollowRectangle()
    {
        Mesh mesh = HollowRectangleMeshGenerator.Generate(width, depth, thickness);

        GameObject hollowRectangle = new GameObject("Hollow Rectangle");
        MeshFilter meshFilter = hollowRectangle.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = hollowRectangle.AddComponent<MeshRenderer>();
        if (material != null)
        {
            meshRenderer.sharedMaterial = material;
        }
    }
}
#endif