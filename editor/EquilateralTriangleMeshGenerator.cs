/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    EquilateralTriangleMeshGenerator.cs

    The EquilateralTriangleMeshGenerator script generates the mesh for a solid equilateral
    triangle based on a given base width. It provides the following functionality:

    - Calculates the vertices, triangles, and normals required to construct the equilateral
      triangle mesh based on the specified base width.
    - Generates a Unity Mesh object representing the solid equilateral triangle.
    - Ensures proper winding order and normal calculation for correct rendering.

    The generated equilateral triangle mesh is used as the base triangle of the TRI-STATE
    structure. It forms the foundation upon which the other elements, such as the side
    triangles and light walls, are built.

    The EquilateralTriangleMeshGenerator script encapsulates the mesh generation logic,
    making it reusable and modular within the TRI-STATE project. It allows for easy
    customization of the base triangle dimensions by modifying the base width parameter.

    The script takes care of the mathematical calculations and mesh construction, providing
    a convenient way to generate the equilateral triangle mesh programmatically. The
    resulting mesh is optimized for rendering and can be easily integrated into the
    TRI-STATE structure.
*/

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquilateralTriangleMeshGenerator : MonoBehaviour
{
    public static Mesh Generate(float baseWidth)
    {
        Mesh mesh = new Mesh();

        // Height of the equilateral triangle
        float height = Mathf.Sqrt(3) / 2 * baseWidth;

        // Vertices
        Vector3[] vertices = new Vector3[]
        {
            // First face
            new Vector3(-baseWidth / 2, 0, 0), // Bottom left
            new Vector3(baseWidth / 2, 0, 0),  // Bottom right
            new Vector3(0, height, 0),         // Top middle

            // Second face (duplicated with reversed order for back face)
            new Vector3(baseWidth / 2, 0, 0),  // Bottom right (reversed order)
            new Vector3(-baseWidth / 2, 0, 0), // Bottom left (reversed order)
            new Vector3(0, height, 0)          // Top middle (reversed order)
        };

        // Triangles
        int[] triangles = new int[]
        {
            // First face
            0, 1, 2,
            // Second face (reversed order)
            3, 4, 5
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // Ensure normals are recalculated for proper lighting

        return mesh;
    }
}
#endif