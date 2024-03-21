/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    LightLineControl.cs

    The LightLineControl script manages the behavior and appearance of light lines.
*/
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace AudioLink
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class LightLineControl : UdonSharpBehaviour
    {
        // AudioLink variables
        public AudioLink audioLink;
        public int band = 0;
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

        // Arrays to store the current colors of each LightLine object in a set
        [UdonSynced]
        private Color lightLineColors_0;
        [UdonSynced]
        private Color lightLineColors_1;
        [UdonSynced]
        private Color lightLineColors_2;
        [UdonSynced]
        private Color lightLineColors_3;
        [UdonSynced]
        private Color lightLineColors_4;
        [UdonSynced]
        private Color lightLineColors_5;
        [UdonSynced]
        private Color lightLineColors_6;
        [UdonSynced]
        private Color lightLineColors_7;

        // Current position of the colored line (for mode A)
        [UdonSynced]
        private int currentPosition = 0;

        // Current color of the line (for mode A)
        [UdonSynced]
        private Color currentColor;

        // Number of iterations before switching modes
        [SerializeField] private const int iterationsBeforeSwitch = 20;

        // Current mode (0 for mode A, 1 for mode B)
        [UdonSynced]
        private int currentMode = 0;

        // Iteration counter
        [UdonSynced]
        private int iterationCounter = 0;

        // Array to store references to floor line objects
        private GameObject[][] floorLineSet;

        // Beat detection variables
        private bool isBeat = false;
        private float lastBeatTime;
        private float maxInterval;

        // Start is called before the first frame update
        void Start()
        {
            // Initialize the array of lightLineSets
            lightLineSets = new GameObject[numberOfSuperSets][][];

            // Iterate over each super set
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                // Initialize the array for the current super set
                lightLineSets[i] = new GameObject[setsPerSuperSet][];

                // Iterate over each set in the current super set
                for (int j = 0; j < setsPerSuperSet; j++)
                {
                    // Initialize the array for the current set
                    lightLineSets[i][j] = new GameObject[3];

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

                            // Initialize the color to black
                            SetColorArray(j, Color.black);
                        }
                        else
                        {
                            // Log a warning if the LightLine object is not found
                            Debug.LogWarning($"LightLine {lightLineName} not found.");
                        }
                    }
                }
            }

            // Initialize the arrays for floor line objects
            floorLineSet = new GameObject[numberOfSuperSets][];

            // Iterate over each super set
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                // Initialize the arrays for the current super set
                floorLineSet[i] = new GameObject[3];

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
                    }
                    else
                    {
                        // Log a warning if the floor line object is not found
                        Debug.LogWarning($"Floor line {floorLineName} not found.");
                    }
                }
            }

            // Generate an initial random color for mode A
            if (Networking.IsOwner(gameObject))
            {
                currentColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }

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

                                // Send a custom network event to trigger the beat for all players
                                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(OnBeat));
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
                                Color color = GetColorArray(j);
                                renderer.material.SetColor("_EmissionColor", color);
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
                            renderer.material.SetColor("_EmissionColor", currentColor);
                        }
                    }
                }
            }
        }

        // Custom network event for handling the beat
        public void OnBeat()
        {
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

        // Method for updating colors in mode A
        void UpdateModeA()
        {
            // Update the colors for all supersets
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                // Set the color of the current position to the current color
                SetColorArray(currentPosition, currentColor);

                // Set the color of the previous position to black
                int previousPosition = (currentPosition - 1 + setsPerSuperSet) % setsPerSuperSet;
                SetColorArray(previousPosition, Color.black);
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
                    Color color = GetColorArray(0);
                    if (color != Color.black)
                    {
                        currentColor = color;
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
                    Color previousColor = GetColorArray(j - 1);
                    SetColorArray(j, previousColor);
                }

                // Set the new random color for the first set in each superset
                SetColorArray(0, newRandomColor);
            }

            // Update the colors of the floor lines to match the color of the first set
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    currentColor = newRandomColor;
                }
            }
        }

        // Helper method to get the appropriate color array based on superset and wall indices
        private Color GetColorArray(int superSetIndex)
        {
            switch (superSetIndex)
            {
                case 0:
                    return lightLineColors_0;
                case 1:
                    return lightLineColors_1;
                case 2:
                    return lightLineColors_2;
                case 3:
                    return lightLineColors_3;
                case 4:
                    return lightLineColors_4;
                case 5:
                    return lightLineColors_5;
                case 6:
                    return lightLineColors_6;
                case 7:
                    return lightLineColors_7;
                default:
                    return Color.black;
            }
        }

        // Helper method to set the appropriate color array based on superset and wall indices
        void SetColorArray(int superSetIndex, Color color)
        {
            switch (superSetIndex)
            {
                case 0:
                    lightLineColors_0 = color;
                    break;
                case 1:
                    lightLineColors_1 = color;
                    break;
                case 2:
                    lightLineColors_2 = color;
                    break;
                case 3:
                    lightLineColors_3 = color;
                    break;
                case 4:
                    lightLineColors_4 = color;
                    break;
                case 5:
                    lightLineColors_5 = color;
                    break;
                case 6:
                    lightLineColors_6 = color;
                    break;
                case 7:
                    lightLineColors_7 = color;
                    break;
            }
        }
    }
}