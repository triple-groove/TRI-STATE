#if UNITY_EDITOR
using UnityEngine;

public static class TriMeshGenerator
{
    public static Mesh Generate(float baseWidth)
    {
        Mesh mesh = new Mesh();

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

        int[] triangles = new int[]
        {
            0, 1, 2
        };

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0.5f, 1f)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}
#endif