
/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    This positions the t rapezoidal prism specifically for the DJ Booth, IDFC
*/

#if UNITY_EDITOR
using UnityEngine;

public static class TrapezoidalPrismMeshGenerator
{
    public static Mesh Generate(float height, float width, float depth, float stepWidth, float stepDepth, float thickness)
    {
        Mesh mesh = new Mesh();

        float halfWidth     = width * 0.5f;
        float halfDepth     = depth * 0.5f;
        float halfStepWidth = stepWidth * 0.5f;
        // float halfStepDepth = stepDepth * 0.5f;

        // x<n> y<n> where n corresponds
        // to the back face vertex numbers
        float x0 = -halfStepWidth;
        float x1 = -halfWidth;
        float x2 = halfWidth;
        float x3 = halfStepWidth;
        float y0 = 0;
        float y1 = height;
        float y2 = height;
        float y3 = 0;

        float frontY = -halfDepth + thickness;
        float backY  = -halfDepth - stepDepth + thickness;

        /*
            le vertex diagram
        
               1___________2
               /           \
              /  back face  \
             /_______________\
            0                 3
        
              bottom      top
               0__3       5__6
               |__|       |__|
               4  7       1  2
        
            left side  right side
               5__1       2__6
               |__|       |__|
               4  0       3  7
        */

        Vector3[] vertices = {
            // Back Face
            new Vector3(x0, y0, backY), // 0
            new Vector3(x1, y1, backY), // 1
            new Vector3(x2, y2, backY), // 2
            new Vector3(x3, y3, backY), // 3

            // Front Face
            new Vector3(x0, y0, frontY),  // 4
            new Vector3(x1, y1, frontY),  // 5
            new Vector3(x2, y2, frontY),  // 6
            new Vector3(x3, y3, frontY),  // 7
        };

        int[] triangles = {
            // Front face
            6,5,4,
            6,4,7,

            // // Back face
            0,1,2,
            0,2,3,

            // Bottom
            0,3,4,
            3,7,4,

            // Top
            1,5,6,
            1,6,2,

            // Left side
            0,5,1,
            5,0,4,

            // Right side
            3,2,6,
            3,6,7,
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
#endif