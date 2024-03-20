#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class CircleElevatorEditor : EditorWindow
{
    private float radius = 2.0f;
    private float thickness = 1.9f;
    private float height = 0.05f;
    private int segments = 50;
    private int numCircles = 8;
    private float circleSpacing = 0.25f;
    private Material circleElevatorMaterial;

    private static readonly string lightLineMaterialPath = "Assets/Materials/LightLines.mat";

    [MenuItem("Tools/Circle Elevator Editor")]
    private static void ShowWindow()
    {
        GetWindow<CircleElevatorEditor>("Circle Elevator Editor");
    }

    // This method is called when the script is loaded or a value is changed in the inspector
    void OnEnable()
    {
        circleElevatorMaterial = AssetDatabase.LoadAssetAtPath<Material>(lightLineMaterialPath);
    }

    private void OnGUI()
    {
        GUILayout.Label("Circle Elevator Generator", EditorStyles.boldLabel);

        radius = EditorGUILayout.FloatField("Radius", radius);
        thickness = EditorGUILayout.FloatField("Thickness", thickness);
        height = EditorGUILayout.FloatField("Height", height);
        segments = EditorGUILayout.IntField("Segments", segments);
        numCircles = EditorGUILayout.IntField("Number of Circles", numCircles);
        circleSpacing = EditorGUILayout.FloatField("Circle Spacing", circleSpacing);
        circleElevatorMaterial = (Material)EditorGUILayout.ObjectField("Material", circleElevatorMaterial, typeof(Material), false);

        if (GUILayout.Button("Generate Circle Elevator"))
        {
            GenerateCircleElevator();
        }
    }

    private void GenerateCircleElevator()
    {
        GameObject circleElevator = new GameObject("Circle Elevator");
        GameObject circleStack = HollowCircleStackGenerator.Generate(radius, thickness, height, segments, numCircles, circleSpacing, circleElevatorMaterial);
        circleStack.transform.SetParent(circleElevator.transform);
    }
}
#endif