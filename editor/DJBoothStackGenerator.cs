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
using UnityEngine.UIElements;


// public static Mesh Generate(float width, float depth, float stepWidth, float stepDepth, float thickness)

public static class DJBoothStackGenerator
{
    public static GameObject Generate(float width, float height, float depth, float stepWidthMax, float stepDepth, float thickness, int numRectangles, int numSteps, Material material)
    {
        GameObject container = new GameObject("Rectangle Stack");
        float spacing = (height - thickness) / (numRectangles + numSteps - 1);
        float basePosition = -height / 2 + thickness / 2;

        CreateRectangleStackCollider(container.transform, width, height, depth);
        CreateTrapezoidalPrismCollider(container.transform, width, depth, stepWidthMax, stepDepth, thickness, numSteps, spacing, basePosition);

        for (int i = 0; i < (numSteps + numRectangles); i++)
        {
            Mesh layerMesh;
            GameObject layerObject;

            if (i < numSteps)
            {
                float stepHorzSpacing = ((stepWidthMax - width) - thickness) / (numSteps - 1);
                float stepWidth = stepWidthMax - i * stepHorzSpacing;
                layerMesh = HollowTShapeMeshGenerator.Generate(width, depth, stepWidth, stepDepth, thickness);
            }
            else
            {
                layerMesh = HollowRectangleMeshGenerator.Generate(width, depth, thickness);
            }

            layerObject = CreateLayerMesh(container.transform, layerMesh, $"Layer_{i}");
            layerObject.transform.position = new Vector3(0, i * spacing + basePosition, 0);

            MeshRenderer renderer = layerObject.GetComponent<MeshRenderer>();
            if (material != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        return container;
    }

    private static void CreateTrapezoidalPrismCollider(Transform parent, float width, float depth, float stepWidthMax, float stepDepth, float thickness, int numSteps, float spacing, float basePosition)
    {
        float trapezoidalColiderHeight = (numSteps - 1) * spacing;
        Mesh trapezoidalColiderMesh = TrapezoidalPrismMeshGenerator.Generate(trapezoidalColiderHeight, width, depth, stepWidthMax, stepDepth, thickness);

        // Create a new GameObject for the collider
        GameObject colliderObject = new GameObject("TrapezoidalCollider");
        MeshCollider meshCollider = colliderObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = trapezoidalColiderMesh;
        meshCollider.convex = true;
        colliderObject.transform.position = new Vector3(0, basePosition, 0);
        colliderObject.transform.SetParent(parent, false);
    }

    private static void CreateRectangleStackCollider(Transform parent, float width, float height, float depth)
    {
        GameObject colliderObject = new GameObject("RectangleCollider");
        BoxCollider boxCollider = colliderObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(width, height, depth);
        colliderObject.transform.SetParent(parent, false);
    }

    private static GameObject CreateLayerMesh(Transform parent, Mesh mesh, string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = new Material(Shader.Find("Standard"));

        return obj;
    }
}
#endif