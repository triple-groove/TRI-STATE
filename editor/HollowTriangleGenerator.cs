/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    HollowTriangleGenerator.cs

    The HollowTriangleGenerator script generates the mesh for a hollow equilateral triangle
    based on the specified parameters. It provides the following functionality:

    - Generates a hollow equilateral triangle mesh with customizable inner base width and
      thickness.
    - Calculates the vertices, triangles, and normals required to construct the hollow
      equilateral triangle mesh based on the provided parameters.
    - Ensures proper winding order and normal calculation for correct rendering.
    - Generates a Unity Mesh object representing the hollow equilateral triangle.

    The generated hollow equilateral triangle mesh is used to create the hollow outlines for
    the side triangles of the TRI-STATE structure. It adds visual depth and dimensionality
    to the side triangles, enhancing the overall aesthetic of the structure.

    The HollowTriangleGenerator script encapsulates the mesh generation logic, making it
    reusable and modular within the TRI-STATE project. It allows for easy customization of
    the hollow triangle dimensions by modifying the inner base width and thickness
    parameters.

    The script takes care of the mathematical calculations and mesh construction, providing
    a convenient way to generate the hollow equilateral triangle mesh programmatically. The
    resulting mesh is optimized for rendering and can be seamlessly integrated into the side
    triangles of the TRI-STATE structure.
*/

#if UNITY_EDITOR
using UnityEngine;

public static class HollowTriangleGenerator
{
    public static Mesh Generate(float innerBaseWidth, float thickness)
    {
        Mesh mesh = new Mesh();

        // Calculate the height of the inner equilateral triangle
        float innerHeight = Mathf.Sqrt(3f) / 2f * innerBaseWidth;
        float halfInnerBaseWidth = innerBaseWidth / 2f;

        // Inner vertices of the triangle
        Vector3 innerTopVertex = new Vector3(0, innerHeight, 0);
        Vector3 innerBottomLeftVertex = new Vector3(-halfInnerBaseWidth, 0, 0);
        Vector3 innerBottomRightVertex = new Vector3(halfInnerBaseWidth, 0, 0);

        // Calculate outward direction for each vertex to expand the triangle
        Vector3 directionToOuterTop = (innerTopVertex - (innerBottomLeftVertex + innerBottomRightVertex) / 2).normalized;
        Vector3 directionToOuterBottomLeft = (innerBottomLeftVertex - (innerTopVertex + innerBottomRightVertex) / 2).normalized;
        Vector3 directionToOuterBottomRight = (innerBottomRightVertex - (innerTopVertex + innerBottomLeftVertex) / 2).normalized;

        // Calculate outer vertices based on thickness
        Vector3 outerTopVertex = innerTopVertex + directionToOuterTop * thickness;
        Vector3 outerBottomLeftVertex = innerBottomLeftVertex + directionToOuterBottomLeft * thickness;
        Vector3 outerBottomRightVertex = innerBottomRightVertex + directionToOuterBottomRight * thickness;

        // Define vertices array
        Vector3[] vertices = new Vector3[]
        {
            // Outer vertices
            outerBottomLeftVertex, outerBottomRightVertex, outerTopVertex,
            // Inner vertices
            innerBottomLeftVertex, innerBottomRightVertex, innerTopVertex
        };

        // Define triangles - create two triangles for each side of the original triangle to form a "quad" for thickness
        int[] triangles = new int[]
        {
            // Front side triangles
            0, 3, 4,
            0, 4, 1,
            // Right side triangles
            1, 4, 5,
            1, 5, 2,
            // Left side triangles
            2, 5, 3,
            2, 3, 0,
            // Back side triangles (mirrored from front)
            4, 3, 0,
            1, 4, 0,
            5, 4, 1,
            2, 5, 1,
            3, 5, 2,
            0, 3, 2
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
#endif