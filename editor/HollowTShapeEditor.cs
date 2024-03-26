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

public class HollowTShapeEditor : EditorWindow
{
    private float width = 1f;
    private float depth = 2f;
    private float stepWidth = 2f;
    private float stepDepth = 2f;
    private float thickness = 0.1f;
    private Material material;

    private static readonly string defaultMaterialPath = "Assets/Materials/LightLines.mat";

    [MenuItem("Tools/Hollow T-Shape Editor")]
    public static void ShowWindow()
    {
        GetWindow<HollowTShapeEditor>("Hollow T-Shape Editor");
    }

    void OnEnable()
    {
        material = AssetDatabase.LoadAssetAtPath<Material>(defaultMaterialPath);
    }

    private void OnGUI()
    {
        GUILayout.Label("Hollow T-Shape Editor", EditorStyles.boldLabel);

        width = EditorGUILayout.FloatField("Width", width);
        depth = EditorGUILayout.FloatField("Depth", depth);
        stepWidth = EditorGUILayout.FloatField("Step Width", stepWidth);
        stepDepth = EditorGUILayout.FloatField("Step Depth", stepDepth);
        thickness = EditorGUILayout.FloatField("Thickness", thickness);
        material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), false);

        if (GUILayout.Button("Generate Hollow T-Shape"))
        {
            GenerateHollowTShape();
        }
    }

    private void GenerateHollowTShape()
    {
        Mesh mesh = HollowTShapeMeshGenerator.Generate(width, depth, stepWidth, stepDepth, thickness);

        GameObject hollowTShape = new GameObject("Hollow T-Shape");
        MeshFilter meshFilter = hollowTShape.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = hollowTShape.AddComponent<MeshRenderer>();
        if (material != null)
        {
            meshRenderer.sharedMaterial = material;
        }
    }
}
#endif