/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    HollowCircleStackGenerator.cs

    The HollowCircleStackGenerator script generates a stack of hollow circles based on the
    provided parameters. It offers the following functionality:

    - Generates a stack of hollow circles with customizable radius, thickness, height,
      number of segments, number of circles, spacing, and material.
    - Utilizes the HollowCircleMeshGenerator to generate the mesh for each individual hollow
      circle in the stack.
    - Positions the hollow circles vertically with the specified spacing to form a cohesive
      stack.
    - Applies the specified material to each hollow circle in the stack.
    - Generates a GameObject representing the entire stack of hollow circles.

    The generated stack of hollow circles is used to create the circle elevator structure in
    the TRI-STATE project. The script provides a convenient way to customize and generate
    the circle elevator with various parameters, allowing for flexibility in design and
    appearance.

    The HollowCircleStackGenerator script encapsulates the logic for creating the stack of
    hollow circles, making it reusable and modular within the project. It abstracts the
    complexity of generating and positioning multiple hollow circles, providing a
    high-level interface for creating the circle elevator structure.

    By leveraging the HollowCircleMeshGenerator script, the HollowCircleStackGenerator
    ensures that each hollow circle in the stack is generated accurately and efficiently.
    The resulting stack of hollow circles is optimized for rendering and can be easily
    integrated into the TRI-STATE scene.
*/

#if UNITY_EDITOR
using UnityEngine;

public static class HollowCircleStackGenerator
{
    public static GameObject Generate(float radius, float thickness, float height, int segments, int numCircles, float spacing, Material material)
    {
        int colliderSegments = Mathf.FloorToInt(segments / 4);

        GameObject container = new GameObject("Circle Stack");

        // Create the base circle with collider
        GameObject baseCircle = CreateBaseCircleCollider(container.transform, radius, height, segments);

        float totalHeight = (numCircles - 1) * (spacing);
        Mesh stackColliderMesh = HollowCircleMeshGenerator.Generate(radius, thickness, totalHeight, colliderSegments);
        GameObject stackColliderObject = CreateRingCollider(container.transform, stackColliderMesh, "StackCollider");
        stackColliderObject.transform.position = new Vector3(0, (totalHeight - height) / 2, 0); // Align the stack collider with the other rings

        for (int i = 0; i < numCircles; i++)
        {
            Mesh circleMesh = HollowCircleMeshGenerator.Generate(radius, thickness, height, segments);
            GameObject circleObject = CreateRingMesh(container.transform, circleMesh, $"Circle_{i}");
            circleObject.transform.position = new Vector3(0, i * spacing + height / 2, 0);

            MeshRenderer renderer = circleObject.GetComponent<MeshRenderer>();
            if (material != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        return container;
    }

    private static GameObject CreateBaseCircleCollider(Transform parent, float radius, float height, int segments)
    {
        int colliderSegments = Mathf.FloorToInt(segments / 4);

        Mesh baseCircleMesh = HollowCircleMeshGenerator.Generate(radius, 0f, height, colliderSegments);

        GameObject baseCircleObject = new GameObject("BaseCircleCollider");
        baseCircleObject.transform.SetParent(parent, false);

        MeshCollider collider = baseCircleObject.AddComponent<MeshCollider>();
        collider.sharedMesh = baseCircleMesh;

        return baseCircleObject;
    }

    private static GameObject CreateRingMesh(Transform parent, Mesh mesh, string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = new Material(Shader.Find("Standard"));

        return obj;
    }

    private static GameObject CreateRingCollider(Transform parent, Mesh mesh, string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        MeshCollider collider = obj.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;

        return obj;
    }
}
#endif