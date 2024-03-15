/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

   TRI-STATE Editor

   The TRI-STATE Editor is a Unity tool that allows users to generate a the VRChat world in
   the Unity scene. It provides a user interface for customizing various aspects of the
   structure and generates the geometry based on the specified parameters.

   Main Components:

   1. TriStateEditor.cs:
      - The main editor window class that provides the user interface for generating the
        TRI-STATE structure.
      - Allows the user to adjust parameters such as triangle width, outlines width, number
        of outlines, spacing, materials, and light wall settings.
      - Generates the TRI-STATE structure when the "Generate TRI-STATE" button is clicked.

   2. EquilateralTriangleMeshGenerator.cs:
      - Generates the mesh for a solid equilateral triangle based on a given base width.
      - Used to create the base triangle of the TRI-STATE structure.

   3. HollowTriangleGenerator.cs:
      - Generates the mesh for a hollow equilateral triangle based on an inner base width
        and thickness.
      - Used to create the hollow outlines for the side triangles of the TRI-STATE structure.

   4. DoubleTriangleGenerator.cs:
      - Generates a GameObject containing both a solid equilateral triangle mesh and a
        hollow equilateral triangle mesh.
      - Used to create the side triangles of the TRI-STATE structure, combining the solid
        and hollow triangle meshes.

   Usage:

   1. Open the Unity editor and navigate to the "Tools" menu.
   2. Select "TRI-STATE Editor" from the menu to open the editor window.
   3. Adjust the desired parameters for the TRI-STATE structure, such as triangle width,
      outlines width, number of outlines, spacing, materials, and light wall settings.
   4. Click the "Generate TRI-STATE" button to generate the structure in the Unity scene.

   The generated TRI-STATE structure will be created as a hierarchy of GameObjects in the
   scene, with the specified geometry, materials, and colliders applied.
*/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class TriStateEditor : EditorWindow
{
    private float triangleWidth = 48f;
    private float outlinesWidth = 1f;
    private int numberOfOutlines = 8;
    private float outlineSpacing = 50;
    private static Material triangleMaterial;
    private static Material hollowTriangleMaterial;
    private static Material lightLineMaterial;
    private float lightWallLength = 15000f;
    private float lightWallHeight = 0.15f;
    private int lightWallCount = 25;
    private float lightWallThickness = 0.1f;
    private float circleElevatorRadius = 1.2f;
    private float circleElevatorThickness = 1.15f;
    private float circleElevatorHeight = 0.01f;
    private int circleElevatorSegments = 50;
    private int circleElevatorNumCircles = 8;
    private float circleElevatorSpacing = 0.13f;
    private Material circleElevatorMaterial;

    private static readonly string defaultTriangleMaterialtPath = "Assets/Materials/Walls.mat";
    private static readonly string defaultHollowTriangleMaterialPath = "Assets/Materials/TrisOutlinesGlow.mat";
    private static readonly string lightLineMaterialPath = "Assets/Materials/LightLines.mat";

    private float lightWallSpacing;
    private LightWallGenerator lightWallGenerator;

    [MenuItem("Tools/TRI-STATE Editor")]
    public static void ShowWindow()
    {
        GetWindow<TriStateEditor>("TRI-STATE Editor ");
    }

    // This method is called when the script is loaded or a value is changed in the inspector
    void OnEnable()
    {
        triangleMaterial = AssetDatabase.LoadAssetAtPath<Material>(defaultTriangleMaterialtPath);
        hollowTriangleMaterial = AssetDatabase.LoadAssetAtPath<Material>(defaultHollowTriangleMaterialPath);
        lightLineMaterial = AssetDatabase.LoadAssetAtPath<Material>(lightLineMaterialPath);
        circleElevatorMaterial = AssetDatabase.LoadAssetAtPath<Material>(lightLineMaterialPath);
    }

    private void OnGUI()
    {
        GUILayout.Label("TRI-STATE Generator", EditorStyles.boldLabel);
        GUILayout.Label("", EditorStyles.boldLabel);
        GUILayout.Label("Triangle Wall Settings", EditorStyles.boldLabel);
        triangleWidth = EditorGUILayout.FloatField("Triangle Width", triangleWidth);
        outlinesWidth = EditorGUILayout.FloatField("Outlines Width", outlinesWidth);
        triangleMaterial = (Material)EditorGUILayout.ObjectField("Triangle Material", triangleMaterial, typeof(Material), false);
        GUILayout.Label("", EditorStyles.boldLabel);
        GUILayout.Label("Outline Settings", EditorStyles.boldLabel);
        numberOfOutlines = EditorGUILayout.IntField("Number of Outlines", numberOfOutlines);
        outlineSpacing = EditorGUILayout.FloatField("Outline Spacing", outlineSpacing);
        hollowTriangleMaterial = (Material)EditorGUILayout.ObjectField("Hollow Triangle Material", hollowTriangleMaterial, typeof(Material), false);
        GUILayout.Label("", EditorStyles.boldLabel);
        GUILayout.Label("LightLine Settings", EditorStyles.boldLabel);
        lightWallLength = EditorGUILayout.FloatField("Light Wall Length", lightWallLength);
        lightWallHeight = EditorGUILayout.FloatField("Light Wall Height", lightWallHeight);
        lightWallCount = EditorGUILayout.IntField("Light Wall Count", lightWallCount);
        lightWallThickness = EditorGUILayout.FloatField("Light Wall Thickness", lightWallThickness);
        lightLineMaterial = (Material)EditorGUILayout.ObjectField("Light Wall Material", lightLineMaterial, typeof(Material), false);
        GUILayout.Label("", EditorStyles.boldLabel);
        GUILayout.Label("Circle Elevator Settings", EditorStyles.boldLabel);
        circleElevatorRadius = EditorGUILayout.FloatField("Circle Elevator Radius", circleElevatorRadius);
        circleElevatorThickness = EditorGUILayout.FloatField("Circle Elevator Thickness", circleElevatorThickness);
        circleElevatorHeight = EditorGUILayout.FloatField("Circle Elevator Height", circleElevatorHeight);
        circleElevatorSegments = EditorGUILayout.IntField("Circle Elevator Segments", circleElevatorSegments);
        circleElevatorNumCircles = EditorGUILayout.IntField("Circle Elevator Num Circles", circleElevatorNumCircles);
        circleElevatorSpacing = EditorGUILayout.FloatField("Circle Elevator Spacing", circleElevatorSpacing);
        circleElevatorMaterial = (Material)EditorGUILayout.ObjectField("Circle Elevator Material", circleElevatorMaterial, typeof(Material), false);

        if (GUILayout.Button("Generate TRI-STATE"))
        {
            GenerateTriState();
        }
    }

    private void GenerateTriState()
    {
        GameObject tristate = new GameObject("TRI-STATE");

        // The height of an equilateral triangle
        float triangleHeight = (Mathf.Sqrt(3) / 2) * triangleWidth;

        // The height of the tetrahedron
        float tetrahedronHeight = Mathf.Sqrt(2 / 3.0f) * triangleHeight;

        // Calculate lightWallSpacing based on lightWallCount and triangleHeight
        lightWallSpacing = (triangleHeight - lightWallHeight) / (lightWallCount - 1);

        // Create base triangle (laying flat) and position it so that its base is at y = 0
        GameObject baseTriangle = CreateTriangleAtPosition(Vector3.zero, Quaternion.Euler(90, 0, 0), tristate.transform);
        GameObject baseTriangleCollider = CreateTriangleColliderAtPosition(Vector3.zero, Quaternion.Euler(90, 0, 0), tristate.transform);
        baseTriangle.name = "BaseTriangle";
        baseTriangleCollider.name = "BaseTriangleCollider";

        // Calculate the centroid of the base triangle and elevator initial height
        Vector3 baseTriangleCentroid = new Vector3(0, 0, triangleHeight / 3);
        float initialElevatorElevation = -(circleElevatorNumCircles - 1) * (circleElevatorSpacing);
        Vector3 elevatorPosition = new Vector3(0, initialElevatorElevation, triangleHeight / 3);

        // Create the circle elevator at the centroid of the base triangle
        GameObject circleElevator = CreateCircleElevator(elevatorPosition, circleElevatorRadius, circleElevatorThickness, circleElevatorHeight, tristate.transform);
        circleElevator.name = "CircleElevator";

        // Calculate the positions for the triangles
        Vector3 midpoint0 = Vector3.zero;
        Vector3 midpoint1 = new Vector3(-triangleWidth / 4, 0, triangleHeight / 2);
        Vector3 midpoint2 = new Vector3(triangleWidth / 4, 0, triangleHeight / 2);

        // The dihedral angle of a tetrahedron is the angle between any two faces
        float dihedralAngle = Mathf.Acos(-1.0f / 3.0f) * Mathf.Rad2Deg;
        float inwardTiltAngle = 90f - dihedralAngle;

        // Create the three side triangles, rotated to point towards the apex of the tetrahedron
        GameObject Triangle0 = CreateDoubleTriangleAtPosition(midpoint0, Quaternion.Euler(inwardTiltAngle, 180, 0), tristate.transform);
        GameObject Triangle0Collider = CreateTriangleColliderAtPosition(midpoint0, Quaternion.Euler(inwardTiltAngle, 180, 0), tristate.transform);
        Triangle0.name = "Triangle0";
        Triangle0Collider.name = "Triangle0Collider";

        GameObject Triangle1 = CreateDoubleTriangleAtPosition(midpoint1, Quaternion.Euler(inwardTiltAngle, -60, 0), tristate.transform);
        GameObject Triangle1Collider = CreateTriangleColliderAtPosition(midpoint1, Quaternion.Euler(inwardTiltAngle, -60, 0), tristate.transform);
        Triangle1.name = "Triangle1";
        Triangle1Collider.name = "Triangle1Collider";

        GameObject Triangle2 = CreateDoubleTriangleAtPosition(midpoint2, Quaternion.Euler(inwardTiltAngle, 60, 0), tristate.transform);
        GameObject Triangle2Collider = CreateTriangleColliderAtPosition(midpoint2, Quaternion.Euler(inwardTiltAngle, 60, 0), tristate.transform);
        Triangle2.name = "Triangle2";
        Triangle2Collider.name = "Triangle2Collider";

        // Create outlines and final reference
        CreateHollowOutlinesForTriangle(Triangle0, numberOfOutlines, outlineSpacing, tristate.transform);
        CreateHollowOutlinesForTriangle(Triangle1, numberOfOutlines, outlineSpacing, tristate.transform);
        CreateHollowOutlinesForTriangle(Triangle2, numberOfOutlines, outlineSpacing, tristate.transform);

        // Generate LightLineWalls for each triangle
        GameObject LightLineWall0 = CreateLightLineWall(Triangle0, tristate.transform);
        GameObject LightLineWall1 = CreateLightLineWall(Triangle1, tristate.transform);
        GameObject LightLineWall2 = CreateLightLineWall(Triangle2, tristate.transform);
        LightLineWall0.name = "LightLineWall0";
        LightLineWall1.name = "LightLineWall1";
        LightLineWall2.name = "LightLineWall2";
    }

    GameObject CreateTriangleAtPosition(Vector3 position, Quaternion rotation, Transform parent)
    {
        Mesh triangleMesh = EquilateralTriangleMeshGenerator.Generate(triangleWidth);
        GameObject triangleObject = new GameObject("Triangle");
        triangleObject.transform.SetParent(parent, false);

        // Move object to specified position
        triangleObject.transform.localPosition = position;
        triangleObject.transform.localRotation = rotation;

        MeshFilter meshFilter = triangleObject.AddComponent<MeshFilter>();
        meshFilter.mesh = triangleMesh;
        MeshRenderer renderer = triangleObject.AddComponent<MeshRenderer>();

        // Assign the selected material to the objects
        if (triangleMaterial != null)
        {
            renderer.sharedMaterial = triangleMaterial;
        }
        else
        {
            renderer.sharedMaterial = new Material(Shader.Find("Standard"));
        }

        return triangleObject;
    }

    GameObject CreateTriangleColliderAtPosition(Vector3 position, Quaternion rotation, Transform parent)
    {
        // Generate the triangle mesh
        Mesh triangleMesh = EquilateralTriangleMeshGenerator.Generate(triangleWidth);

        // Create a new GameObject for the triangle collider
        GameObject triangleObject = new GameObject("Triangle");
        triangleObject.transform.SetParent(parent, false);

        // Move object to specified position and rotation
        triangleObject.transform.localPosition = position;
        triangleObject.transform.localRotation = rotation;

        // Add a MeshCollider component for collision detection
        MeshCollider meshCollider = triangleObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = triangleMesh;
        //meshCollider.convex = true; // Ensure convex is set to true for mesh colliders
        meshCollider.inflateMesh = true; // Inflate the mesh collider to prevent tunneling

        return triangleObject;
    }




    GameObject CreateDoubleTriangleAtPosition(Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject doubleTriangleObject = DoubleTriangleGenerator.Generate(triangleWidth, outlinesWidth);
        doubleTriangleObject.transform.SetParent(parent, false);

        // Move object to specified position
        doubleTriangleObject.transform.localPosition = position;
        doubleTriangleObject.transform.localRotation = rotation;

        // Assign materials to solid and hollow triangle meshes
        MeshRenderer solidRenderer = doubleTriangleObject.transform.GetChild(0).GetComponent<MeshRenderer>();
        if (triangleMaterial != null)
        {
            solidRenderer.sharedMaterial = triangleMaterial;
        }
        else
        {
            solidRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        }

        MeshRenderer hollowRenderer = doubleTriangleObject.transform.GetChild(1).GetComponent<MeshRenderer>();
        if (hollowTriangleMaterial != null)
        {
            hollowRenderer.sharedMaterial = hollowTriangleMaterial;
        }
        else
        {
            hollowRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        }

        return doubleTriangleObject;
    }


    GameObject CreateHollowTriangleAtPosition(Vector3 position, Quaternion rotation, Transform parent)
    {
        Mesh hollowTriangleMesh = HollowTriangleGenerator.Generate(triangleWidth, outlinesWidth);
        GameObject hollowTriangleObject = new GameObject("HollowTriangle");
        hollowTriangleObject.transform.SetParent(parent, false);

        // Move object to specified position
        hollowTriangleObject.transform.localPosition = position;
        hollowTriangleObject.transform.localRotation = rotation;

        MeshFilter meshFilter = hollowTriangleObject.AddComponent<MeshFilter>();
        meshFilter.mesh = hollowTriangleMesh;
        MeshRenderer renderer = hollowTriangleObject.AddComponent<MeshRenderer>();

        // Assign the selected material to the objects
        if (hollowTriangleMaterial != null)
        {
            renderer.sharedMaterial = hollowTriangleMaterial;
        }
        else
        {
            renderer.sharedMaterial = new Material(Shader.Find("Standard"));
        }
        return hollowTriangleObject;
    }

    void CreateHollowOutlinesForTriangle(GameObject baseTriangle, int numberOfTriangles, float spacing, Transform parent)
    {
        for (int i = 0; i < numberOfTriangles; i++)
        {
            Vector3 positionOffset = baseTriangle.transform.forward * spacing * (i + 1);
            Vector3 newPosition = baseTriangle.transform.position + positionOffset;

            GameObject newTriangle = CreateHollowTriangleAtPosition(newPosition, baseTriangle.transform.rotation, parent);
            newTriangle.name = baseTriangle.name + "_Hollow_" + i; // Naming the triangles sequentially
        }
    }

    GameObject CreateLightLineWall(GameObject baseTriangle, Transform parent)
    {
        GameObject lightLineWallObject = LightWallGenerator.Generate(lightWallLength, lightWallHeight, lightWallThickness, lightWallSpacing, lightWallCount, lightLineMaterial);

        lightLineWallObject.transform.SetParent(parent, false);

        lightLineWallObject.transform.localPosition = baseTriangle.transform.position + new Vector3(0, lightWallHeight / 2, 0);
        lightLineWallObject.transform.localRotation = baseTriangle.transform.rotation;

        return lightLineWallObject;
    }

    GameObject CreateCircleElevator(Vector3 position, float radius, float thickness, float height, Transform parent)
    {
        GameObject circleElevator = HollowCircleStackGenerator.Generate(radius, thickness, height, circleElevatorSegments, circleElevatorNumCircles, circleElevatorSpacing, circleElevatorMaterial);

        circleElevator.transform.SetParent(parent, false);

        circleElevator.transform.localPosition = position;

        return circleElevator;
    }
}
#endif