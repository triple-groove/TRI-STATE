/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    The HollowRectangleMeshGenerator script is a utility class that generates a hollow
    rectangle mesh based on the provided width, depth, and thickness parameters.
    It is designed to be used in conjunction with the HollowRectangleEditor script
    or other scripts that require the creation of hollow rectangle meshes.

    The script uses a set of predefined vertex positions and triangle indices to
    construct the hollow rectangle mesh. It calculates the necessary positions and
    offsets based on the given parameters to ensure the mesh is generated correctly.

    The generated mesh consists of a hollow rectangular shape with the specified
    dimensions and thickness. The mesh is oriented along the YZ plane, with the
    width extending along the X-axis.

    The script provides a static method called Generate() that takes the width,
    depth, and thickness as parameters and returns the generated Mesh object.
    This method can be called from other scripts or editor tools to create hollow
    rectangle meshes programmatically.
*/

#if UNITY_EDITOR
using UnityEngine;

public static class HollowRectangleMeshGenerator
{
    public static Mesh Generate(float width, float depth, float thickness)
    {
        Mesh mesh = new Mesh();

        float halfOuterWidth = width * 0.5f;
        float halfOuterDepth = depth * 0.5f;
        float halfInnerWidth = halfOuterWidth - thickness;
        float halfInnerDepth = halfOuterDepth - thickness;
        float halfThickness = thickness * 0.5f;

        Vector3[] vertices = {
            // Bottom Outer vertices
            new Vector3(-halfOuterWidth, -halfThickness, halfOuterDepth), // Bottom front left
            new Vector3(halfOuterWidth, -halfThickness, halfOuterDepth),  // Bottom front right
            new Vector3(halfOuterWidth, -halfThickness, -halfOuterDepth), // Bottom back right
            new Vector3(-halfOuterWidth, -halfThickness, -halfOuterDepth), // Bottom back left

            // Bottom Inner vertices
            new Vector3(-halfInnerWidth, -halfThickness, halfInnerDepth), // Bottom front left
            new Vector3(halfInnerWidth, -halfThickness, halfInnerDepth),  // Bottom front right
            new Vector3(halfInnerWidth, -halfThickness, -halfInnerDepth), // Bottom back right
            new Vector3(-halfInnerWidth, -halfThickness, -halfInnerDepth), // Bottom back left

            // Top Outer vertices
            new Vector3(-halfOuterWidth, halfThickness, halfOuterDepth), // Top front left
            new Vector3(halfOuterWidth, halfThickness, halfOuterDepth),  // Top front right
            new Vector3(halfOuterWidth, halfThickness, -halfOuterDepth), // Top back right
            new Vector3(-halfOuterWidth, halfThickness, -halfOuterDepth), // Top back left

            // Top Inner vertices
            new Vector3(-halfInnerWidth, halfThickness, halfInnerDepth), // Top front left
            new Vector3(halfInnerWidth, halfThickness, halfInnerDepth),  // Top front right
            new Vector3(halfInnerWidth, halfThickness, -halfInnerDepth), // Top back right
            new Vector3(-halfInnerWidth, halfThickness, -halfInnerDepth) // Top back left
        };

        int[] triangles = {
            // Bottom face
            1,0,5,
            0,4,5,
            0,3,7,
            7,4,0,
            2,7,3,
            2,6,7,
            2,1,6,
            1,5,6,

            // Top face
            8,9,13,
            12,8,13,
            11,8,15,
            12,15,8,
            15,10,11,
            14,10,15,
            9,10,14,
            13,9,14,

            // Outer side faces
            9,8,0,
            1,9,0,
            10,9,1,
            2,10,1,
            11,10,2,
            3,11,2,
            8,11,3,
            0,8,3,

            // Inner side faces
            5,4,13,
            12,13,4,
            6,5,14,
            13,14,5,
            7,6,15,
            14,15,6,
            4,7,12,
            15,12,7
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
#endif