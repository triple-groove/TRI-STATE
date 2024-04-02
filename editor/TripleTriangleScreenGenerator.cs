#if UNITY_EDITOR
using UnityEngine;

public static class TripleTriangleScreenGenerator
{
    public static GameObject Generate(float baseWidth, float thickness, float sideOffset, float inwardAngle, Material screenLeftMaterial, Material screenMiddleMaterial, Material screenRightMaterial, Material outlineTriangleMaterial)
    {
        GameObject root = new GameObject("TripleTriangleScreen");
        float sidePanelYoffset = (baseWidth + sideOffset) * Mathf.Sqrt(3) / 4f - thickness;
        float leftPanelXoffset = (baseWidth + sideOffset) * 0.75f - (baseWidth / 2) - thickness;
        float rightPanelXoffset = -(baseWidth + sideOffset) * 0.75f + (baseWidth / 2) + thickness;

        GeneratePanel(root, "LeftPanel", true, false, baseWidth, thickness, screenLeftMaterial, outlineTriangleMaterial);
        GeneratePanel(root, "MiddlePanel", false, false, baseWidth, thickness, screenMiddleMaterial, outlineTriangleMaterial);
        GeneratePanel(root, "RightPanel", false, true, baseWidth, thickness, screenRightMaterial, outlineTriangleMaterial);

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

        return root;
    }

    private static void GeneratePanel(GameObject parent, string panelName, bool leftScreen, bool rightScreen, float baseWidth, float thickness, Material screenMaterial, Material outlineMaterial)
    {
        GameObject panel = new GameObject(panelName);
        panel.transform.SetParent(parent.transform, false);

        // Generate the main triangle
        Mesh triangleMesh = TriMesh3RotGenerator.Generate(baseWidth, 0f, 0f, 0f, leftScreen, rightScreen);
        GameObject triangleObject = new GameObject("Triangle");
        triangleObject.transform.SetParent(panel.transform, false);
        MeshFilter triangleMeshFilter = triangleObject.AddComponent<MeshFilter>();
        triangleMeshFilter.mesh = triangleMesh;
        MeshRenderer triangleRenderer = triangleObject.AddComponent<MeshRenderer>();
        if (screenMaterial != null)
        {
            triangleRenderer.sharedMaterial = screenMaterial;
        }

        // Generate the outline triangle
        Mesh outlineTriangleMesh = TriMeshOutline3RotGenerator.Generate(baseWidth, thickness, 0f, 0f, 0f);
        GameObject outlineTriangleObject = new GameObject("OutlineTriangle");
        outlineTriangleObject.transform.SetParent(panel.transform, false);
        MeshFilter outlineTriangleMeshFilter = outlineTriangleObject.AddComponent<MeshFilter>();
        outlineTriangleMeshFilter.mesh = outlineTriangleMesh;
        MeshRenderer outlineTriangleRenderer = outlineTriangleObject.AddComponent<MeshRenderer>();
        if (outlineMaterial != null)
        {
            outlineTriangleRenderer.sharedMaterial = outlineMaterial;
        }
    }
}
#endif