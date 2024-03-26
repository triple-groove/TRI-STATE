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
using UnityEditor;
using UnityEngine;

public class DJBoothEditor : EditorWindow
{
    private float width = 5.0f;
    private float height = 2.6f;
    private float depth = 1.5f;
    private float stepWidthMax = 9f;
    private float stepDepth = 3f;
    private float thickness = 0.02f;
    private int numRectangles = 8;
    private int numSteps = 8;
    private Material material;

    private static readonly string defaultMaterialPath = "Assets/Materials/LightLines.mat";

    [MenuItem("Tools/DJ Booth Editor")]
    private static void ShowWindow()
    {
        GetWindow<DJBoothEditor>("DJ Booth Editor");
    }

    void OnEnable()
    {
        material = AssetDatabase.LoadAssetAtPath<Material>(defaultMaterialPath);
    }

    private void OnGUI()
    {
        GUILayout.Label("DJ Booth Generator", EditorStyles.boldLabel);

        width = EditorGUILayout.FloatField("Width", width);
        height = EditorGUILayout.FloatField("Height", height);
        depth = EditorGUILayout.FloatField("Depth", depth);
        stepWidthMax = EditorGUILayout.FloatField("Step Width Max", stepWidthMax);
        stepDepth = EditorGUILayout.FloatField("Step Depth", stepDepth);
        thickness = EditorGUILayout.FloatField("Thickness", thickness);
        numRectangles = EditorGUILayout.IntField("Number of Steps", numRectangles);
        numSteps = EditorGUILayout.IntField("Number of Rectangles", numSteps);
        material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), false);

        if (GUILayout.Button("Generate DJ Booth"))
        {
            GenerateDJBooth();
        }
    }

    private void GenerateDJBooth()
    {
        GameObject djBooth = new GameObject("DJ Booth");
        GameObject rectangleStack = DJBoothStackGenerator.Generate(width, height, depth, stepWidthMax, stepDepth, thickness, numRectangles, numSteps, material);
        rectangleStack.transform.SetParent(djBooth.transform);
    }
}
#endif
