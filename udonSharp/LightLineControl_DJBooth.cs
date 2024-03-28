/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    LightLineControl_DJBooth.cs

    The LightLineControl_DJBooth script manages the behavior and appearance of layers
    of DJ Booth.
*/
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace AudioLink
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class LightLineControl_DJBooth : UdonSharpBehaviour
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
        private const int numberOfSuperSets = 2;
        [SerializeField]
        private const int iterationsBeforeSwitch = 8;

        // layerSets[numberOfSuperSets][setsPerSuperSet]
        private GameObject[][] layerSets;

        // Arrays to store the current colors of each Layer object in a set
        private Color[] layerColors = new Color[setsPerSuperSet];

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

        // Beat detection variables
        private bool isBeat = false;
        private float lastBeatTime;
        private float maxInterval;

        // Start is called before the first frame update
        void Start()
        {
            // Initialize the array of layerSets
            layerSets = new GameObject[numberOfSuperSets][];

            // Iterate over each super set
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                // Initialize the array for the current super set
                layerSets[i] = new GameObject[setsPerSuperSet];

                // Iterate over each set in the current super set
                for (int j = 0; j < setsPerSuperSet; j++)
                {
                    // Calculate the index of the Layer object based on the super set and set
                    int layerIndex = i * setsPerSuperSet + j;

                    // Construct the name of the Layer object
                    string layerName = $"DJBooth/Layer_{layerIndex}";

                    // Find the Layer object by name
                    GameObject layer = GameObject.Find(layerName);

                    // Check if the Layer object is found
                    if (layer != null)
                    {
                        // Store the reference in the array
                        layerSets[i][j] = layer;

                        // Initialize the color to black
                        layerColors[j] = Color.black;
                    }
                    else
                    {
                        // Log a warning if the Layer object is not found
                        Debug.LogWarning($"Layer {layerName} not found.");
                    }
                }
            }

            if (Networking.IsMaster)
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

            ApplyColors();
        }

        void ApplyColors()
        {
            for (int i = 0; i < numberOfSuperSets; i++)
            {
                for (int j = 0; j < setsPerSuperSet; j++)
                {
                    GameObject currentLayer = layerSets[i][j];

                    if (currentLayer != null)
                    {
                        Renderer renderer = currentLayer.GetComponent<Renderer>();

                        if (renderer != null)
                        {
                            Color color = layerColors[j];
                            renderer.material.SetColor("_EmissionColor", color);
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

            if (Networking.IsMaster)
            {
                RequestSerialization();
            }
        }

        void UpdateModeA()
        {
            layerColors[currentPosition]= currentColor;

            // Set the color of the previous position to black
            int previousPosition = (currentPosition - 1 + setsPerSuperSet) % setsPerSuperSet;
            layerColors[previousPosition] = Color.black;

            // Move to the next position
            currentPosition = (currentPosition + 1) % setsPerSuperSet;

            // Master sends the new color for next time
            // Generate a new random color if the current position is at the top
            if (currentPosition == 0 && Networking.IsMaster)
            {
                currentColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }
        }

        void UpdateModeB()
        {
            // Propagate the new color down the sets in all supersets
            for (int j = setsPerSuperSet - 1; j > 0; j--)
            {
                Color previousColor = layerColors[j - 1];
                layerColors[j] = previousColor;
            }

            // Set the new random color for the first set in each superset
            layerColors[0] = currentColor;

            // Master sends the new color for next time
            if (Networking.IsMaster)
            {
                currentColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }
        }
    }
}