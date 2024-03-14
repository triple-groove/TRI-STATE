/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    DoubleTriangleGenerator.cs

    The DoubleTriangleGenerator script is responsible for generating a GameObject that
    contains both a solid equilateral triangle mesh and a hollow equilateral triangle mesh.
    It takes in the following parameters:

    - Triangle Width: The width of the base of the equilateral triangle.
    - Outlines Width: The width of the hollow outline of the equilateral triangle.

    The script generates the meshes for the solid and hollow equilateral triangles based on
    the provided parameters. The resulting GameObject combines these meshes to create a
    double triangle structure.

    The generated double triangle GameObject is used to create the side triangles of the
    TRI-STATE structure. It provides a visually appealing combination of a solid triangle
    with a hollow outline, adding depth and dimensionality to the overall structure.

    The DoubleTriangleGenerator script encapsulates the logic for generating the double
    triangle mesh, making it reusable and modular within the TRI-STATE project. It allows
    for easy customization of the triangle dimensions and outlines width to achieve the
    desired aesthetic.
*/

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DoubleTriangleGenerator
{
    public static GameObject Generate(float baseWidth, float thickness)
    {
        GameObject container = new GameObject("DoubleTriangleContainer");

        // Create the solid equilateral triangle mesh
        Mesh solidMesh = EquilateralTriangleMeshGenerator.Generate(baseWidth);
        GameObject solidTriangle = CreateGameObject(container.transform, solidMesh, "SolidTriangle");

        // Create the hollow equilateral triangle mesh
        Mesh hollowMesh = HollowTriangleGenerator.Generate(baseWidth, thickness);
        GameObject hollowTriangle = CreateGameObject(container.transform, hollowMesh, "HollowTriangle");

        return container;
    }

    private static GameObject CreateGameObject(Transform parent, Mesh mesh, string name)
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