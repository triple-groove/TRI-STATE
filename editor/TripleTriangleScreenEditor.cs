#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class TripleTriangleScreenEditor : EditorWindow
{
    private float baseWidth = 1f;
    private float thickness = 0.01f;
    private float sideOffset = 0.05f;
    private float inwardAngle = 20f;
    private Material screenLeftMaterial;
    private Material screenMiddleMaterial;
    private Material screenRightMaterial;
    private Material outlineTriangleMaterial;

    private static readonly string defaultLeftScreenMaterialPath = "Assets/Materials/TriPanelLeft.mat";
    private static readonly string defaultMiddleScreenMaterialPath = "Assets/Materials/TriPanelMiddle.mat";
    private static readonly string defaultRightScreenMaterialPath = "Assets/Materials/TriPanelRight.mat";
    private static readonly string defaultOutlineTriangleMaterialPath = "Assets/Materials/TrisOutlinesGlow.mat";

    [MenuItem("Tools/Triple Triangle Screen Editor")]
    public static void ShowWindow()
    {
        GetWindow<TripleTriangleScreenEditor>("Triple Triangle Screen Editor");
    }

    private void OnEnable()
    {
        screenLeftMaterial = AssetDatabase.LoadAssetAtPath<Material>(defaultLeftScreenMaterialPath);
        screenMiddleMaterial = AssetDatabase.LoadAssetAtPath<Material>(defaultMiddleScreenMaterialPath);
        screenRightMaterial = AssetDatabase.LoadAssetAtPath<Material>(defaultRightScreenMaterialPath);
        outlineTriangleMaterial = AssetDatabase.LoadAssetAtPath<Material>(defaultOutlineTriangleMaterialPath);
    }

    private void OnGUI()
    {
        GUILayout.Label("Triple Triangle Screen Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        baseWidth = EditorGUILayout.FloatField("Base Width", baseWidth);
        thickness = EditorGUILayout.FloatField("Thickness", thickness);
        EditorGUILayout.Space();
        sideOffset = EditorGUILayout.FloatField("Side Offset", sideOffset);
        inwardAngle = EditorGUILayout.FloatField("Inward Angle", inwardAngle);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
        screenLeftMaterial = (Material)EditorGUILayout.ObjectField("Screen Left Material", screenLeftMaterial, typeof(Material), false);
        screenMiddleMaterial = (Material)EditorGUILayout.ObjectField("Screen Middle Material", screenMiddleMaterial, typeof(Material), false);
        screenRightMaterial = (Material)EditorGUILayout.ObjectField("Screen Right Material", screenRightMaterial, typeof(Material), false);
        outlineTriangleMaterial = (Material)EditorGUILayout.ObjectField("Outline Triangle Material", outlineTriangleMaterial, typeof(Material), false);
        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Triple Triangle Screen"))
        {
            GenerateTripleTriangleScreen();
        }
    }

    private void GenerateTripleTriangleScreen()
    {
        GameObject tripleTriangleScreen = TripleTriangleScreenGenerator.Generate(
            baseWidth,
            thickness,
            sideOffset,
            inwardAngle,
            screenLeftMaterial,
            screenMiddleMaterial,
            screenRightMaterial,
            outlineTriangleMaterial
        );
    }
}
#endif