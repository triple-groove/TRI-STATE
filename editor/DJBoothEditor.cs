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
    private float height = 1.3f;
    private float depth = 1.5f;
    private float thickness = 0.1f;
    private int numRectangles = 5;
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
        thickness = EditorGUILayout.FloatField("Thickness", thickness);
        numRectangles = EditorGUILayout.IntField("Number of Rectangles", numRectangles);
        material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), false);

        if (GUILayout.Button("Generate DJ Booth"))
        {
            GenerateDJBooth();
        }
    }

    private void GenerateDJBooth()
    {
        GameObject djBooth = new GameObject("DJ Booth");
        GameObject rectangleStack = HollowRectangleStackGenerator.Generate(width, height, depth, thickness, numRectangles, material);
        rectangleStack.transform.SetParent(djBooth.transform);
    }
}
#endif
