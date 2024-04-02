#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class TriangleScreenEditor : EditorWindow
{
    private string[] positionOptions = { "Left", "Middle", "Right" };
    private int selectedPositionIndex = 0;
    private float baseWidth = 1f;
    private float thickness = 0.3f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private float zRotation = 0f;
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
        EditorGUILayout.Space();
        selectedPositionIndex = EditorGUILayout.Popup("Triangle Position", selectedPositionIndex, positionOptions);
        EditorGUILayout.Space();
        baseWidth = EditorGUILayout.FloatField("Base Width", baseWidth);
        thickness = EditorGUILayout.FloatField("Thickness", thickness);
        EditorGUILayout.Space();
        xRotation = EditorGUILayout.FloatField("X Rotation", xRotation);
        yRotation = EditorGUILayout.FloatField("Y Rotation", yRotation);
        zRotation = EditorGUILayout.FloatField("Z Rotation", zRotation);
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
        bool leftScreen;
        bool rightScreen;
        switch (selectedPositionIndex)
        {
            case 0: // Left
                leftScreen = true;
                rightScreen = false;
                break;
            case 1: // Middle
                leftScreen = false;
                rightScreen = false;
                break;
            default: // Right
                leftScreen = false;
                rightScreen = true;
                break;
        }

        // Generate the main triangle
        Mesh triangleMesh = TriMesh3RotGenerator.Generate(baseWidth, xRotation, yRotation, zRotation, leftScreen, rightScreen);
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
        Mesh outlineTriangleMesh = TriMeshOutline3RotGenerator.Generate(baseWidth, thickness, xRotation, yRotation, zRotation);
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