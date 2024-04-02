#if UNITY_EDITOR
using UnityEngine;

public static class TriMesh3RotGenerator
{
    public static Mesh Generate(float baseWidth, float xRotation, float yRotation, float zRotation, bool leftScreen, bool rightScreen)
    {
        Mesh mesh = new Mesh();

        float uv_x_0;
        float uv_x_1;
        float uv_x_2;
        float uv_y_0;
        float uv_y_1;
        float uv_y_2;

        float halfBaseWidth = baseWidth / 2f;
        float height = Mathf.Sqrt(3f) / 2f * baseWidth;

        // Calculate the centroid position
        Vector3 centroidPosition = new Vector3(0f, height / 3f, 0f);
        centroidPosition = Vector3.zero;

        // Define the vertices relative to the centroid position
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-halfBaseWidth, 0f, 0f) - centroidPosition,
            new Vector3(halfBaseWidth, 0f, 0f) - centroidPosition,
            new Vector3(0f, height, 0f) - centroidPosition
        };

        // Rotate the vertices
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = rotation * vertices[i];
        }

        int[] triangles = new int[]
        {
            0, 1, 2
        };

        if (leftScreen)
        {
            uv_x_0 = 0f;
            uv_x_1 = 0.5f;
            uv_x_2 = 1f;
            uv_y_0 = 1f;
            uv_y_1 = 0f;
            uv_y_2 = 1f;
        }
        else if (rightScreen)
        {
            uv_x_0 = 0.5f;
            uv_x_1 = 0f;
            uv_x_2 = 1f;
            uv_y_0 = 0f;
            uv_y_1 = 1f;
            uv_y_2 = 1f;
        }
        else
        {
            // middle
            uv_x_0 = 0f;
            uv_x_1 = 1f;
            uv_x_2 = 0.5f;
            uv_y_0 = 0f;
            uv_y_1 = 0f;
            uv_y_2 = 1f;
        }

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(uv_x_0, uv_y_0),
            new Vector2(uv_x_1, uv_y_1),
            new Vector2(uv_x_2, uv_y_2)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}
#endif