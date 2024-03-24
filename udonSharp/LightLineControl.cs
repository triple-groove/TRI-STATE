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
using System.CodeDom;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace AudioLink
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class LightLineControl : UdonSharpBehaviour
    {
        public AudioLink audioLink;
        public int band = 0;

        [SerializeField, Range(0, 1)]
        private float beatThreshold = 0.8f;
        [SerializeField]
        private float maxBPM = 200f;
        [SerializeField]
        private const int setsPerSuperSet = 8;
        [SerializeField]
        private const int numberOfSuperSets = 3;
        [SerializeField]
        private const int iterationsBeforeSwitch = 2;

        private const int numberOfSides = 3;

        private int numberOfSets;

        // lightLineSets[numberOfSuperSets][setsPerSuperSet][numberOfSides]
        private GameObject[][][] lightLineSets;

        // Arrays to store the current colors of each LightLine object in a set
        private Color[] lightLineColors = new Color[setsPerSuperSet];

        // Current position of the colored line (for mode A)
        [UdonSynced]
        private int currentPosition = 0;

        // Current color of the line (for mode A)
        [UdonSynced]
        private Color currentColor;

        // Current mode (0 for mode A, 1 for mode B)
        [UdonSynced]
        private int currentMode = 0;

        // Iteration counter
        [UdonSynced]
        private int iterationCounter = 0;

        // Array to store references to floor line objects
        private GameObject[] floorLineSet;

        // Array to store the current colors of each floor line object
        [UdonSynced]
        private Color floorLineColor;

        // Beat detection variables
        private bool isBeat = false;
        private float lastBeatTime;
        private float maxInterval;

        // Start is called before the first frame update
        void Start()
        {
            numberOfSets = numberOfSuperSets * setsPerSuperSet;

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
                    lightLineSets[i][j] = new GameObject[numberOfSides];

                    // Find and store references to LightLine objects in the current set
                    for (int k = 0; k < numberOfSides; k++)
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
                            lightLineColors[j] = Color.black;
                        }
                        else
                        {
                            // Log a warning if the LightLine object is not found
                            Debug.LogWarning($"LightLine {lightLineName} not found.");
                        }
                    }
                }
            }

            floorLineSet = new GameObject[numberOfSides];

            for (int i = 0; i < numberOfSides; i++)
            {
                string floorLineName = $"LightLineWall{i}/LightLine_{numberOfSets}";

                GameObject floorLine = GameObject.Find(floorLineName);

                if (floorLine != null)
                {
                    floorLineSet[i] = floorLine;
                }
                else
                {
                    Debug.LogWarning($"Floor line {floorLineName} not found.");
                }
            }

            floorLineColor = Color.white;

            if (Networking.IsOwner(gameObject))
            {
                currentColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }

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

                                OnBeat();
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
                    for (int k = 0; k < numberOfSides; k++)
                    {
                        GameObject currentLightLine = lightLineSets[i][j][k];

                        if (currentLightLine != null)
                        {
                            Renderer renderer = currentLightLine.GetComponent<Renderer>();

                            if (renderer != null)
                            {
                                Color color = lightLineColors[j];
                                renderer.material.SetColor("_EmissionColor", color);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < numberOfSides; i++)
            {
                GameObject currentFloorLine = floorLineSet[i];

                if (currentFloorLine != null)
                {
                    Renderer renderer = currentFloorLine.GetComponent<Renderer>();

                    if (renderer != null)
                    {
                        renderer.material.SetColor("_EmissionColor", floorLineColor);
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

        void UpdateModeA()
        {
            lightLineColors[currentPosition]= currentColor;

            // Set the color of the previous position to black
            int previousPosition = (currentPosition - 1 + setsPerSuperSet) % setsPerSuperSet;
            lightLineColors[previousPosition] = Color.black;

            // Move to the next position
            currentPosition = (currentPosition + 1) % setsPerSuperSet;

            // Generate a new random color if the current position is at the top
            if (currentPosition == 0)
            {
                currentColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }

                for (int k = 0; k < numberOfSides; k++)
                {
                    Color color = lightLineColors[0];
                    if (color != Color.black)
                    {
                        floorLineColor = color;
                    }
                }
        }

        void UpdateModeB()
        {
            // Generate a new random color
            Color newRandomColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);

            // Propagate the new color down the sets in all supersets
            for (int j = setsPerSuperSet - 1; j > 0; j--)
            {
                Color previousColor = lightLineColors[j - 1];
                lightLineColors[j] = previousColor;
            }

            // Set the new random color for the first set in each superset
            lightLineColors[0] = newRandomColor;

            floorLineColor = newRandomColor;
        }
    }
}