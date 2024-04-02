#if UNITY_EDITOR
using UnityEngine;

public static class TriMeshOutline3RotGenerator
{
    public static Mesh Generate(float baseWidth, float thickness, float xRotation, float yRotation, float zRotation)
    {
        Mesh mesh = new Mesh();

        float halfBaseWidth = baseWidth / 2f;
        float height = Mathf.Sqrt(3f) / 2f * baseWidth;

        // Calculate the adjusted base width and height based on the thickness
        float adjustedBaseWidth = baseWidth + thickness * 2f * Mathf.Sqrt(3f);
        float adjustedHeight = height + thickness * Mathf.Sqrt(3f);

        // Calculate the positions of the outer vertices
        // float outerBottomY = -thickness * Mathf.Sqrt(3f) / 3f;
        float outerBottomY = -thickness;
        Vector3 outerBottomLeft = new Vector3(-adjustedBaseWidth / 2f, outerBottomY, 0f);
        Vector3 outerBottomRight = new Vector3(adjustedBaseWidth / 2f, outerBottomY, 0f);
        Vector3 outerTop = new Vector3(0f, adjustedHeight, 0f);

        // Calculate the positions of the inner vertices
        Vector3 innerBottomLeft = new Vector3(-halfBaseWidth, 0f, 0f);
        Vector3 innerBottomRight = new Vector3(halfBaseWidth, 0f, 0f);
        Vector3 innerTop = new Vector3(0f, height, 0f);

        // Calculate the centroid position
        Vector3 centroidPosition = new Vector3(0f, height / 3f, 0f);
        
        centroidPosition = Vector3.zero;

        // Adjust the vertex positions relative to the centroid
        outerBottomLeft -= centroidPosition;
        outerBottomRight -= centroidPosition;
        outerTop -= centroidPosition;
        innerBottomLeft -= centroidPosition;
        innerBottomRight -= centroidPosition;
        innerTop -= centroidPosition;

        Vector3[] vertices = new Vector3[]
        {
            outerBottomLeft,
            outerBottomRight,
            outerTop,
            innerBottomLeft,
            innerBottomRight,
            innerTop
        };

        // Rotate the vertices
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = rotation * vertices[i];
        }

        int[] triangles = new int[]
        {
            0, 1, 4,
            0, 4, 3,
            1, 2, 5,
            1, 5, 4,
            2, 0, 3,
            2, 3, 5
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
#endif