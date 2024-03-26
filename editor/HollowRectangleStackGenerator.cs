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

public static class HollowRectangleStackGenerator
{
    public static GameObject Generate(float width, float height, float depth, float thickness, int numRectangles, Material material)
    {
        GameObject container = new GameObject("Rectangle Stack");
        float spacing = (height-thickness)/(numRectangles-1);

        CreateRectangleStackCollider(container.transform, width, height, depth);

        for (int i = 0; i < numRectangles; i++)
        {
            Mesh rectangleMesh = HollowRectangleMeshGenerator.Generate(width, depth, thickness);
            GameObject rectangleObject = CreateRectangleMesh(container.transform, rectangleMesh, $"Rectangle_{i}");
            rectangleObject.transform.position = new Vector3(0, i*spacing - height/2 + thickness/2, 0);

            MeshRenderer renderer = rectangleObject.GetComponent<MeshRenderer>();
            if (material != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        return container;
    }

    private static void CreateRectangleStackCollider(Transform parent, float width, float height, float depth)
    {
        GameObject colliderObject = new GameObject("ColliderObject");
        BoxCollider boxCollider = colliderObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(width, height, depth);
        colliderObject.transform.SetParent(parent, false);
    }

    private static GameObject CreateRectangleMesh(Transform parent, Mesh mesh, string name)
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