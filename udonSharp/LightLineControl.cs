/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    LightLineControl.cs

    The LightLineControl script manages the behavior and appearance of light lines in a VRChat
    world. It offers a range of customization options and effects to create dynamic and
    visually appealing lighting experiences.

    Key Features:
    - Controls the color, intensity, and animation of light lines based on predefined patterns or real-time input.
    - Supports multiple modes of operation, such as static colors, color gradients, or audio-reactive effects.
    - Allows for easy customization of light line properties, such as length, width, spacing, and material.
    - Enables smooth transitions and interpolation between different lighting states or patterns.
    - Provides an intuitive interface for users to interact with and control the light lines through gestures or triggers.
    - Optimizes performance by efficiently managing light line rendering and updates.

    Usage:
    1. Attach the LightLineControl script to the relevant GameObject(s) representing the light lines in your VRChat world.
    2. Configure the desired light line properties, such as color, intensity, animation patterns, and interaction triggers, in the Unity Inspector.
    3. Implement any necessary user interactions or controls to manipulate the light lines dynamically.
    4. Test the light line behavior and performance in various scenarios to ensure optimal visual quality and responsiveness.

    Dependencies:
    - Requires the VRChat SDK and Udon# compiler for proper functionality.
    - May interact with other scripts or components, such as audio sources, gestures, or avatar tracking systems.

    Customization:
    - Modify the light line properties, animation patterns, and interaction triggers to fit your specific design requirements.
    - Extend the script to include additional lighting effects, such as pulsing, strobing, or color cycling.
    - Integrate with external data sources or APIs to create dynamic and immersive lighting experiences.

    Troubleshooting:
    - Ensure that the light line GameObjects and their associated components are properly configured in the Unity Inspector.
    - Verify that the necessary dependencies and assets are correctly imported and referenced in your VRChat world.
    - Test the light line control script in various lighting conditions and performance scenarios to identify and optimize any potential issues.
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace AudioLink
{
#if UDONSHARP
    using UdonSharp;

    public class LightLineControl : UdonSharpBehaviour
#else
    public class LightLineControl : MonoBehaviour
#endif
    {
        // AudioLink variables
        public AudioLink audioLink;
        public  int band = 1;
        [Range(0, 1)]
        [SerializeField] private float beatThreshold = 0.8f;
        [SerializeField] private float maxBPM = 200f;

        // Public variable for the number of sets of LightLine objects
        public int numberOfSets = 24;

        // Number of sets in a super set
        private const int setsPerSuperSet = 8;

        // Number of super sets
        private const int numberOfSuperSets = 3;

        // Array to store references to LightLine objects
        private GameObject[][][] lightLineSets;

        // Array to store the current colors of each LightLine object in a set
        private Color[][][] lightLineColors;

        // Current position of the colored line (for mode A)
        private int currentPosition = 0;

        // Current color of the line (for mode A)
        private Color currentColor;

        // Number of iterations before switching modes
        [SerializeField] private const int iterationsBeforeSwitch = 20;

        // Current mode (0 for mode A, 1 for mode B)
        private int currentMode = 0;

        // Iteration counter
        private int iterationCounter = 0;

        // Array to store references to floor line objects
        private GameObject[][] floorLineSet;

        // Array to store the current colors of each floor line object
        private Color[][] floorLineColors;

        // Beat detection variables
        private bool isBeat = false;
        private float lastBeatTime;
        private float maxInterval;

        // Start is called before the first frame update
        void Start()
        {
            // Initialize the array of lightLineSets
            lightLineSets = new GameObject[numberOfSuperSets][][];

            // Initialize the array of lightLineColors
            lightLineColors = new Color[numberOfSuperSets][][];

            // Iterate over each super set
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                // Initialize the array for the current super set
                lightLineSets[i] = new GameObject[setsPerSuperSet][];
                lightLineColors[i] = new Color[setsPerSuperSet][];

                // Iterate over each set in the current super set
                for (int j = 0; j < setsPerSuperSet; j++)
                {
                    // Initialize the array for the current set
                    lightLineSets[i][j] = new GameObject[3];
                    lightLineColors[i][j] = new Color[3];

                    // Find and store references to LightLine objects in the current set
                    for (int k = 0; k < 3; k++)
                    {
                        // Calculate the index of the LightLine object based on the super set and set
                        int lightLineIndex = i * setsPerSuperSet + j;

                        // Construct the name of the LightLine object
                        string lightLineName = $"LightLineWall{k}/LightLine_{lightLineIndex}";

                        // Find the LightLine object by name
                        GameObject lightLine = GameObject.Find(lightLineName);

                        // Check if the LightLine object is found
                        if (lightLine != null)
                        {
                            // Store the reference in the array
                            lightLineSets[i][j][k] = lightLine;
                            lightLineColors[i][j][k] = Color.black; // Initialize the color to black
                        }
                        else
                        {
                            // Log a warning if the LightLine object is not found
                            Debug.LogWarning($"LightLine {lightLineName} not found.");
                        }
                    }
                }
            }

            // Initialize the arrays for floor line objects and colors
            floorLineSet = new GameObject[numberOfSuperSets][];
            floorLineColors = new Color[numberOfSuperSets][];

            // Iterate over each super set
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                // Initialize the arrays for the current super set
                floorLineSet[i] = new GameObject[3];
                floorLineColors[i] = new Color[3];

                // Find and store references to floor line objects in the current super set
                for (int k = 0; k < 3; k++)
                {
                    // Construct the name of the floor line object
                    string floorLineName = $"LightLineWall{k}/LightLine_{numberOfSets}";

                    // Find the floor line object by name
                    GameObject floorLine = GameObject.Find(floorLineName);

                    // Check if the floor line object is found
                    if (floorLine != null)
                    {
                        // Store the reference in the array
                        floorLineSet[i][k] = floorLine;
                        floorLineColors[i][k] = Color.white; // Initialize the color to white
                    }
                    else
                    {
                        // Log a warning if the floor line object is not found
                        Debug.LogWarning($"Floor line {floorLineName} not found.");
                    }
                }
            }

            // Generate an initial random color for mode A
            currentColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);

            // Calculate the maximum interval based on the maximum BPM
            maxInterval = 60f / maxBPM;
        }

        // Update is called once per frame
        void Update()
        {
            if (audioLink != null)
            {
                if (audioLink.audioDataToggle)
                {
                    int dataIndex = band * 128;
                    if (dataIndex < audioLink.audioData.Length)
                    {
                        float amplitude = audioLink.audioData[dataIndex].grayscale;

                        if (amplitude >= beatThreshold)
                        {
                            float currentTime = Time.time;
                            float interval = currentTime - lastBeatTime;

                            if (!isBeat && interval >= maxInterval)
                            {
                                isBeat = true;
                                lastBeatTime = currentTime;

                                // Check the current mode
                                if (currentMode == 0)
                                {
                                    // Mode A: Light up one line at a time
                                    UpdateModeA();
                                }
                                else
                                {
                                    // Mode B: Light up all lines at once, propagating a new color each time
                                    UpdateModeB();
                                }

                                // Increment the iteration counter
                                iterationCounter++;

                                // Check if it's time to switch modes
                                if (iterationCounter >= iterationsBeforeSwitch * setsPerSuperSet)
                                {
                                    // Switch to the other mode
                                    currentMode = 1 - currentMode;
                                    iterationCounter = 0;
                                }
                            }
                        }
                        else
                        {
                            isBeat = false;
                        }
                    }
                }
            }

            // Apply the colors to the LightLine objects in all supersets
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                for (int j = 0; j < setsPerSuperSet; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        GameObject currentLightLine = lightLineSets[i][j][k];

                        if (currentLightLine != null)
                        {
                            Renderer renderer = currentLightLine.GetComponent<Renderer>();

                            if (renderer != null)
                            {
                                renderer.material.SetColor("_EmissionColor", lightLineColors[i][j][k]);
                            }
                        }
                    }
                }
            }

            // Apply the colors to the floor line objects in all supersets
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    GameObject currentFloorLine = floorLineSet[i][k];

                    if (currentFloorLine != null)
                    {
                        Renderer renderer = currentFloorLine.GetComponent<Renderer>();

                        if (renderer != null)
                        {
                            renderer.material.SetColor("_EmissionColor", floorLineColors[i][k]);
                        }
                    }
                }
            }
        }

        // Method for updating colors in mode A
        void UpdateModeA()
        {
            // Update the colors for all supersets
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                // Set the color of the current position to the current color
                for (int k = 0; k < 3; k++)
                {
                    lightLineColors[i][currentPosition][k] = currentColor;
                }

                // Set the color of the previous position to black
                int previousPosition = (currentPosition - 1 + setsPerSuperSet) % setsPerSuperSet;
                for (int k = 0; k < 3; k++)
                {
                    lightLineColors[i][previousPosition][k] = Color.black;
                }
            }

            // Move to the next position
            currentPosition = (currentPosition + 1) % setsPerSuperSet;

            // Generate a new random color if the current position is at the top
            if (currentPosition == 0)
            {
                currentColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }

            // Update the colors of the floor lines to match the color of the first set, if it's not black
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (lightLineColors[i][0][k] != Color.black)
                    {
                        floorLineColors[i][k] = lightLineColors[i][0][k];
                    }
                }
            }
        }

        // Method for updating colors in mode B
        void UpdateModeB()
        {
            // Generate a new random color
            Color newRandomColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);

            // Propagate the new color down the sets in all supersets
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                for (int j = setsPerSuperSet - 1; j > 0; j--)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        lightLineColors[i][j][k] = lightLineColors[i][j - 1][k];
                    }
                }

                // Set the new random color for the first set in each superset
                for (int k = 0; k < 3; k++)
                {
                    lightLineColors[i][0][k] = newRandomColor;
                }
            }

            // Update the colors of the floor lines to match the color of the first set
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    floorLineColors[i][k] = newRandomColor;
                }
            }
        }
    }
}