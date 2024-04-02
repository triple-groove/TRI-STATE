#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class TripleTriangleScreenEditor : EditorWindow
{
    private float baseWidth = 1f;
    private float thickness = 0.01f;
    private float sideOffset = 0.05f;
    private float inwardAngle = 20f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private float zRotation = 0f;
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
        GameObject root = new GameObject("TripleTriangleScreen");
        float sidePanelYoffset = (baseWidth+sideOffset)*Mathf.Sqrt(3)/4f - thickness;
        float leftPanelXoffset = (baseWidth+sideOffset)*0.75f-(baseWidth/2) - thickness;
        float rightPanelXoffset = -(baseWidth+sideOffset)*0.75f+(baseWidth/2) + thickness;

        GeneratePanel(root, "LeftPanel", true, false);
        GeneratePanel(root, "MiddlePanel", false, false);
        GeneratePanel(root, "RightPanel", false, true);

        // Arrange the panels to form a three-panel screen
        Transform leftPanel = root.transform.Find("LeftPanel");
        Transform middlePanel = root.transform.Find("MiddlePanel");
        Transform rightPanel = root.transform.Find("RightPanel");


        leftPanel.localPosition = new Vector3(leftPanelXoffset, sidePanelYoffset, 0f);
        middlePanel.localPosition = Vector3.zero;
        rightPanel.localPosition = new Vector3(rightPanelXoffset, sidePanelYoffset, 0f);

        leftPanel.localRotation = Quaternion.Euler(0f, 0f, -60f);
        leftPanel.localRotation *= Quaternion.Euler(inwardAngle, 0f, 0f);
        rightPanel.localRotation = Quaternion.Euler(0f, 0f, 60f);
        rightPanel.localRotation *= Quaternion.Euler(inwardAngle, 0f, 0f);
    }

    private void GeneratePanel(GameObject parent, string panelName, bool leftScreen, bool rightScreen)
    {
        GameObject panel = new GameObject(panelName);
        panel.transform.SetParent(parent.transform, false);

        // Generate the main triangle
        Mesh triangleMesh = TriMesh3RotGenerator.Generate(baseWidth, xRotation, yRotation, zRotation, leftScreen, rightScreen);
        GameObject triangleObject = new GameObject("Triangle");
        triangleObject.transform.SetParent(panel.transform, false);
        MeshFilter triangleMeshFilter = triangleObject.AddComponent<MeshFilter>();
        triangleMeshFilter.mesh = triangleMesh;
        MeshRenderer triangleRenderer = triangleObject.AddComponent<MeshRenderer>();
        if (leftScreen)
        {
            if (screenLeftMaterial != null)
            {
                triangleRenderer.sharedMaterial = screenLeftMaterial;
            }
        }
        else if(rightScreen)
        {
            if (screenRightMaterial != null)
            {
                triangleRenderer.sharedMaterial = screenRightMaterial;
            }
        }
        else
        {
            if (screenMiddleMaterial != null)
            {
                triangleRenderer.sharedMaterial = screenMiddleMaterial;
            }
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