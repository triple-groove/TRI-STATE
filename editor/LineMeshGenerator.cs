/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    LineMeshGenerator.cs

    The LineMeshGenerator script generates a mesh for a line with a specified length,
    height, and thickness. It provides the following functionality:

    - Generates a line mesh with customizable length, height, and thickness.
    - Calculates the vertices, triangles, and normals required to construct the line mesh
      based on the provided parameters.
    - Ensures proper winding order and normal calculation for correct rendering.
    - Generates a Unity Mesh object representing the line.

    The generated line mesh is used by the LightWallGenerator script to create the
    individual cubes that form the light walls of the TRI-STATE structure. By generating a
    line mesh with the desired dimensions, the script provides a building block for
    constructing the light walls.

    The LineMeshGenerator script encapsulates the mesh generation logic, making it reusable
    and modular within the TRI-STATE project. It allows for easy customization of the line
    dimensions by modifying the length, height, and thickness parameters.

    The script takes care of the mathematical calculations and mesh construction, providing
    a convenient way to generate the line mesh programmatically. The resulting mesh is
    optimized for rendering and can be efficiently utilized by the LightWallGenerator script
    to create the light walls.

    triple_groove - meow btw...if you even care
*/

#if UNITY_EDITOR
using UnityEngine;

public class LineMeshGenerator : MonoBehaviour
{
    public Mesh GenerateLineMesh(float length, float height, float thickness)
    {
        Mesh mesh = new Mesh();

        // Vertices
        Vector3[] vertices = new Vector3[24];

        // Side vertices
        vertices[0] = new Vector3(-length / 2, 0, -thickness / 2); // Bottom front left
        vertices[1] = new Vector3(length / 2, 0, -thickness / 2); // Bottom front right
        vertices[2] = new Vector3(length / 2, 0, thickness / 2); // Bottom back right
        vertices[3] = new Vector3(-length / 2, 0, thickness / 2); // Bottom back left
        vertices[4] = new Vector3(-length / 2, height, -thickness / 2); // Top front left
        vertices[5] = new Vector3(length / 2, height, -thickness / 2); // Top front right
        vertices[6] = new Vector3(length / 2, height, thickness / 2); // Top back right
        vertices[7] = new Vector3(-length / 2, height, thickness / 2); // Top back left

        // End vertices (bottom)
        vertices[8] = new Vector3(-length / 2, 0, -thickness / 2); // Bottom front left
        vertices[9] = new Vector3(length / 2, 0, -thickness / 2); // Bottom front right
        vertices[10] = new Vector3(length / 2, 0, thickness / 2); // Bottom back right
        vertices[11] = new Vector3(-length / 2, 0, thickness / 2); // Bottom back left

        // End vertices (top)
        vertices[12] = new Vector3(-length / 2, height, -thickness / 2); // Top front left
        vertices[13] = new Vector3(length / 2, height, -thickness / 2); // Top front right
        vertices[14] = new Vector3(length / 2, height, thickness / 2); // Top back right
        vertices[15] = new Vector3(-length / 2, height, thickness / 2); // Top back left

        // Triangles
        int[] triangles = new int[72];

        // Side faces
        triangles[0] = 0; triangles[1] = 1; triangles[2] = 5;
        triangles[3] = 0; triangles[4] = 5; triangles[5] = 4;

        triangles[6] = 1; triangles[7] = 2; triangles[8] = 6;
        triangles[9] = 1; triangles[10] = 6; triangles[11] = 5;

        triangles[12] = 2; triangles[13] = 3; triangles[14] = 7;
        triangles[15] = 2; triangles[16] = 7; triangles[17] = 6;

        triangles[18] = 3; triangles[19] = 0; triangles[20] = 4;
        triangles[21] = 3; triangles[22] = 4; triangles[23] = 7;

        // Bottom end face
        triangles[24] = 8; triangles[25] = 9; triangles[26] = 10;
        triangles[27] = 8; triangles[28] = 10; triangles[29] = 11;

        // Top end face
        triangles[30] = 12; triangles[31] = 14; triangles[32] = 13;
        triangles[33] = 12; triangles[34] = 15; triangles[35] = 14;

        // End faces (bottom)
        triangles[36] = 8; triangles[37] = 11; triangles[38] = 9;
        triangles[39] = 9; triangles[40] = 11; triangles[41] = 10;

        // End faces (top)
        triangles[42] = 12; triangles[43] = 13; triangles[44] = 14;
        triangles[45] = 12; triangles[46] = 14; triangles[47] = 15;

        // Normals
        Vector3[] normals = new Vector3[24];

        // Side face normals
        normals[0] = Vector3.Cross(vertices[1] - vertices[0], vertices[4] - vertices[0]).normalized; // Front left bottom
        normals[1] = Vector3.Cross(vertices[5] - vertices[1], vertices[0] - vertices[1]).normalized; // Front right bottom
        normals[2] = Vector3.Cross(vertices[6] - vertices[2], vertices[1] - vertices[2]).normalized; // Back right bottom
        normals[3] = Vector3.Cross(vertices[3] - vertices[7], vertices[2] - vertices[3]).normalized; // Back left bottom
        normals[4] = Vector3.Cross(vertices[0] - vertices[4], vertices[7] - vertices[4]).normalized; // Front left top
        normals[5] = Vector3.Cross(vertices[4] - vertices[5], vertices[7] - vertices[5]).normalized; // Front right top
        normals[6] = Vector3.Cross(vertices[5] - vertices[6], vertices[4] - vertices[6]).normalized; // Back right top
        normals[7] = Vector3.Cross(vertices[7] - vertices[3], vertices[4] - vertices[7]).normalized; // Back left top

        // End face normals (bottom)
        normals[8] = -Vector3.up;
        normals[9] = -Vector3.up;
        normals[10] = -Vector3.up;
        normals[11] = -Vector3.up;

        // End face normals (top)
        normals[12] = Vector3.up;
        normals[13] = Vector3.up;
        normals[14] = Vector3.up;
        normals[15] = Vector3.up;

        // Assign vertices, triangles, and normals to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        return mesh;
    }
}
#endif