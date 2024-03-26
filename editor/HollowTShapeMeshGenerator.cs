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

public static class HollowTShapeMeshGenerator
{
    public static Mesh Generate(float width, float depth, float stepWidth, float stepDepth, float thickness)
    {
        Mesh mesh = new Mesh();

        float halfOuterWidth = width * 0.5f;
        float halfOuterDepth = depth * 0.5f;
        float halfInnerWidth = halfOuterWidth - thickness;
        float halfInnerDepth = halfOuterDepth - thickness;
        float halfOuterStepWidth = stepWidth * 0.5f;
        float halfOuterStepDepth = stepDepth * 0.5f;
        float halfInnerStepWidth = halfOuterStepWidth - thickness;
        float halfInnerStepDepth = halfOuterStepDepth - thickness;
        float halfThickness = thickness * 0.5f;


        float x0 = -halfOuterStepWidth;
        float x1 = -halfInnerStepWidth;
        float x2 = -halfOuterWidth;
        float x3 = -halfInnerWidth;
        float x4 = halfInnerWidth;
        float x5 = halfOuterWidth;
        float x6 = halfInnerStepWidth;
        float x7 = halfOuterStepWidth;
        float y0 = halfOuterDepth;
        float y1 = halfInnerDepth;
        float y2 = -halfInnerDepth;
        float y3 = -halfOuterDepth;
        float y4 = y3 - halfInnerStepDepth + thickness + thickness;
        float y5 = y3 - halfInnerStepDepth + thickness;

        float topY =  halfThickness;
        float botY = -halfThickness;

        /*
            le vertex diagram
            
            y5  5____________________________________4
                |                                    |
            y4  |   13__________________________12   |
                |    |                          |    |
                |    |                          |    |
            y3  |   14_______15       10________11   |
                |             |        |             |
            y2  6_________7   |        |   2_________3
                          |   |        |   |
                          |   |        |   |
            y1            |   8________9   |
                          |                |
            y0            0________________1


               x0   x1   x2  x3        x4  x5  x6   x7
        */

        Vector3[] vertices = {
            // Bottom
            new Vector3(x2, botY, y0), //  0
            new Vector3(x5, botY, y0), //  1
            new Vector3(x5, botY, y2), //  2
            new Vector3(x7, botY, y2), //  3
            new Vector3(x7, botY, y5), //  4
            new Vector3(x0, botY, y5), //  5
            new Vector3(x0, botY, y2), //  6
            new Vector3(x2, botY, y2), //  7
            new Vector3(x3, botY, y1), //  8
            new Vector3(x4, botY, y1), //  9
            new Vector3(x4, botY, y3), // 10
            new Vector3(x6, botY, y3), // 11
            new Vector3(x6, botY, y4), // 12
            new Vector3(x1, botY, y4), // 13
            new Vector3(x1, botY, y3), // 14
            new Vector3(x3, botY, y3), // 15

            // Bottom
            new Vector3(x2, topY, y0), // 16
            new Vector3(x5, topY, y0), // 17
            new Vector3(x5, topY, y2), // 18
            new Vector3(x7, topY, y2), // 19
            new Vector3(x7, topY, y5), // 20
            new Vector3(x0, topY, y5), // 21
            new Vector3(x0, topY, y2), // 22
            new Vector3(x2, topY, y2), // 23
            new Vector3(x3, topY, y1), // 24
            new Vector3(x4, topY, y1), // 25
            new Vector3(x4, topY, y3), // 26
            new Vector3(x6, topY, y3), // 27
            new Vector3(x6, topY, y4), // 28
            new Vector3(x1, topY, y4), // 29
            new Vector3(x1, topY, y3), // 30
            new Vector3(x3, topY, y3), // 31
        };

        int[] triangles = {
            // Bottom face
            0,8,9,
            0,9,1,
            9,2,1,
            9,10,2,
            10,11,2,
            2,11,3,
            11,4,3,
            11,12,4,
            12,5,4,
            5,12,13,
            5,13,6,
            6,13,14,
            6,14,15,
            6,15,7,
            7,15,0,
            0,15,8,

            // Top face
            25,24,16,
            17,25,16,
            17,18,25,
            18,26,25,
            18,27,26,
            19,27,18,
            19,20,27,
            20,28,27,
            20,21,28,
            29,28,21,
            22,29,21,
            30,29,22,
            31,30,22,
            23,31,22,
            16,31,23,
            24,31,16,

            // Outer side faces
            0,1,17,
            0,17,16,
            1,2,18,
            1,18,17,
            2,3,19,
            2,19,18,
            3,4,20,
            3,20,19,
            4,5,21,
            4,21,20,
            5,6,22,
            5,22,21,
            6,7,23,
            6,23,22,
            7,0,16,
            7,16,23,

            // Inner side faces
            8,25,9,
            8,24,25,
            9,26,10,
            9,25,26,
            10,27,11,
            10,26,27,
            11,28,12,
            11,27,28,
            12,29,13,
            12,28,29,
            13,30,14,
            13,29,30,
            14,31,15,
            14,30,31,
            15,24,8,
            15,31,24
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
#endif