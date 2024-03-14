/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care
    
    LightWallGenerator.cs

    The LightWallGenerator script generates the light walls of the TRI-STATE structure based
    on the specified parameters. It provides the following functionality:

    - Generates a series of light walls with customizable length, height, thickness,
      spacing, count, and material.
    - Creates a series of cubes with the specified dimensions and positions them vertically
      to form the light walls.
    - Applies the specified material to each cube in the light walls.
    - Generates a GameObject representing the entire set of light walls.

    The generated light walls are used to enhance the visual appearance and atmosphere of
    the TRI-STATE structure. They add dynamic lighting and create an immersive environment
    within the structure.

    The LightWallGenerator script encapsulates the logic for creating the light walls,
    making it reusable and modular within the TRI-STATE project. It abstracts the complexity
    of generating and positioning multiple cubes to form the light walls, providing a
    high-level interface for customizing and generating the light walls.

    By leveraging the specified parameters, the script allows for flexible customization of
    the light walls' dimensions, spacing, count, and material. This enables easy
    experimentation and fine-tuning of the light walls to achieve the desired visual effect.

    The resulting light walls are optimized for rendering and can be seamlessly integrated
    into the TRI-STATE structure, enhancing its overall visual appeal and immersion.

    triple_groove - meow btw...if you even care
*/

#if UNITY_EDITOR
using UnityEngine;

public class LightWallGenerator : MonoBehaviour
{
    public static GameObject Generate(float length, float height, float thickness, float spacing, int count, Material material)
    {
        // Create a parent object to hold all LightLine_ objects
        GameObject parentObject = new GameObject("LightLineWall");

        for (int i = 0; i < count; i++)
        {
            // Create a cube GameObject
            GameObject cubeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Adjust the size of the cube
            cubeObject.transform.localScale = new Vector3(length, height, thickness);

            // Remove the collider (if not needed)
            DestroyImmediate(cubeObject.GetComponent<BoxCollider>());

            // Add a MeshRenderer if not already present, but skip top 0
            MeshRenderer meshRenderer = cubeObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = cubeObject.AddComponent<MeshRenderer>();

            // Assign the selected material to the cube
            if (material != null)
            {
                meshRenderer.sharedMaterial = material;
            }
            else
            {
                meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
            }

            // Position the cube
            cubeObject.transform.position = Vector3.up * spacing * (count - i - 1); // Decrementing spacing

            // Set the name of the cubeObject
            cubeObject.name = "LightLine_" + i;

            // Set the parent of the cubeObject to the parentObject
            cubeObject.transform.parent = parentObject.transform;
        }

        return parentObject;
    }
}
#endif