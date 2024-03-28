#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class TriangleScreenEditor : EditorWindow
{
    private float baseWidth = 1f;
    private float thickness = 0.3f;
    private Material triangleMaterial;
    private Material outlineTriangleMaterial;

    private static readonly string defaultTriangleMaterialPath = "Assets/Materials/TriPanelRight.mat";
    private static readonly string defaultOutlineTriangleMaterialPath = "Assets/Materials/TrisOutlinesGlow.mat";

    [MenuItem("Tools/Triangle Screen Editor")]
    public static void ShowWindow()
    {
        GetWindow<TriangleScreenEditor>("Triangle Screen Editor");
    }

    private void OnEnable()
    {
        triangleMaterial = AssetDatabase.LoadAssetAtPath<Material>(defaultTriangleMaterialPath);
        outlineTriangleMaterial = AssetDatabase.LoadAssetAtPath<Material>(defaultOutlineTriangleMaterialPath);
    }

    private void OnGUI()
    {
        GUILayout.Label("Triangle Screen Settings", EditorStyles.boldLabel);

        baseWidth = EditorGUILayout.FloatField("Base Width", baseWidth);
        thickness = EditorGUILayout.FloatField("Thickness", thickness);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
        triangleMaterial = (Material)EditorGUILayout.ObjectField("Triangle Material", triangleMaterial, typeof(Material), false);
        outlineTriangleMaterial = (Material)EditorGUILayout.ObjectField("Outline Triangle Material", outlineTriangleMaterial, typeof(Material), false);

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Triangle Screen"))
        {
            GenerateTriangleScreen();
        }
    }

    private void GenerateTriangleScreen()
    {
        GameObject panel = new GameObject("Panel");

        // Generate the main triangle
        Mesh triangleMesh = TriMeshGenerator.Generate(baseWidth);
        GameObject triangleObject = new GameObject("Triangle");
        triangleObject.transform.SetParent(panel.transform, false);
        MeshFilter triangleMeshFilter = triangleObject.AddComponent<MeshFilter>();
        triangleMeshFilter.mesh = triangleMesh;
        MeshRenderer triangleRenderer = triangleObject.AddComponent<MeshRenderer>();
        if (triangleMaterial != null)
        {
            triangleRenderer.sharedMaterial = triangleMaterial;
        }

        // Generate the outline triangle
        Mesh outlineTriangleMesh = TriMeshOutlineGenerator.Generate(baseWidth, thickness);
        GameObject outlineTriangleObject = new GameObject("OutlineTriangle");
        outlineTriangleObject.transform.SetParent(panel.transform, false);
        MeshFilter outlineTriangleMeshFilter = outlineTriangleObject.AddComponent<MeshFilter>();
        outlineTriangleMeshFilter.mesh = outlineTriangleMesh;
        MeshRenderer outlineTriangleRenderer = outlineTriangleObject.AddComponent<MeshRenderer>();
        if (outlineTriangleMaterial != null)
        {
            outlineTriangleRenderer.sharedMaterial = outlineTriangleMaterial;
        }
    }
}
#endif