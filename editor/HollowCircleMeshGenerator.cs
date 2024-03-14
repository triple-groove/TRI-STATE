/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    HollowCircleMeshGenerator.cs

    The HollowCircleMeshGenerator script generates the mesh for a hollow circle based on the
    specified parameters. It provides the following functionality:

    - Generates a hollow circle mesh with customizable outer radius, inner radius, height,
      and number of segments.
    - Calculates the vertices, triangles, and normals required to construct the hollow
      circle mesh based on the provided parameters.
    - Ensures proper winding order and normal calculation for correct rendering.
    - Generates a Unity Mesh object representing the hollow circle.

    The generated hollow circle mesh is used by the HollowCircleStackGenerator to create the
    circle elevator structure. By stacking multiple hollow circles vertically with a
    specified spacing, the circle elevator is formed.

    The HollowCircleMeshGenerator script encapsulates the mesh generation logic, making it
    reusable and modular within the TRI-STATE project. It allows for easy customization of
    the hollow circle dimensions and level of detail by modifying the outer radius, inner
    radius, height, and number of segments.

    The script takes care of the mathematical calculations and mesh construction, providing
    a convenient way to generate the hollow circle mesh programmatically. The resulting mesh
    is optimized for rendering and can be seamlessly integrated into the circle elevator
    structure.
*/

#if UNITY_EDITOR
using UnityEngine;

public static class HollowCircleMeshGenerator
{
    public static Mesh Generate(float outerRadius, float innerRadius, float height, int segments)
    {
        Mesh mesh = new Mesh();

        int numVertices = segments * 2;
        int numTriangles = segments * 2 * 6; // 2 triangles per segment (top and bottom)
        numTriangles += segments * 2 * 6; // 2 triangles per segment (outer and inner sides)

        Vector3[] vertices = new Vector3[numVertices * 2]; // 2 vertices per segment (top and bottom)
        int[] triangles = new int[numTriangles];
        Vector3[] normals = new Vector3[numVertices * 2];

        float angleStep = 2 * Mathf.PI / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle);
            float z = Mathf.Sin(angle);

            // Outer vertices (top and bottom)
            vertices[i] = new Vector3(x * outerRadius, height / 2, z * outerRadius);
            vertices[i + segments] = new Vector3(x * outerRadius, -height / 2, z * outerRadius);

            // Inner vertices (top and bottom)
            vertices[i + numVertices] = new Vector3(x * innerRadius, height / 2, z * innerRadius);
            vertices[i + numVertices + segments] = new Vector3(x * innerRadius, -height / 2, z * innerRadius);

            // Normals
            normals[i] = Vector3.up;
            normals[i + segments] = Vector3.down;
            normals[i + numVertices] = Vector3.up;
            normals[i + numVertices + segments] = Vector3.down;
        }

        int triangleIndex = 0;
        for (int i = 0; i < segments; i++)
        {
            int nextIndex = (i + 1) % segments;

            // Top triangles
            triangles[triangleIndex++] = i;
            triangles[triangleIndex++] = i + numVertices;
            triangles[triangleIndex++] = nextIndex;

            triangles[triangleIndex++] = nextIndex;
            triangles[triangleIndex++] = i + numVertices;
            triangles[triangleIndex++] = nextIndex + numVertices;

            // Bottom triangles
            triangles[triangleIndex++] = i + segments;
            triangles[triangleIndex++] = nextIndex + segments;
            triangles[triangleIndex++] = i + numVertices + segments;

            triangles[triangleIndex++] = nextIndex + segments;
            triangles[triangleIndex++] = nextIndex + numVertices + segments;
            triangles[triangleIndex++] = i + numVertices + segments;

            // Outer side triangles
            triangles[triangleIndex++] = i;
            triangles[triangleIndex++] = nextIndex;
            triangles[triangleIndex++] = i + segments;

            triangles[triangleIndex++] = nextIndex;
            triangles[triangleIndex++] = nextIndex + segments;
            triangles[triangleIndex++] = i + segments;

            // Inner side triangles
            triangles[triangleIndex++] = i + numVertices;
            triangles[triangleIndex++] = i + numVertices + segments;
            triangles[triangleIndex++] = nextIndex + numVertices;

            triangles[triangleIndex++] = nextIndex + numVertices;
            triangles[triangleIndex++] = i + numVertices + segments;
            triangles[triangleIndex++] = nextIndex + numVertices + segments;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.RecalculateBounds();

        return mesh;
    }
}
#endif