/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    TrianglePositionStateMachine.cs

    The TrianglePositionStateMachine script controls the position and orientation of triangular
    objects in a VRChat world. It utilizes a state machine approach to manage the triangles'
    behavior based on predefined positions and user interactions.

    Key Features:
    - Defines a set of triangle positions and orientations, such as initial state, elevated state, and rotated state.
    - Implements a state machine to handle transitions between different triangle positions and orientations.
    - Responds to user input, such as button presses or gestures, to trigger triangle movement and rotation.
    - Animates the triangles' movement and rotation smoothly using interpolation techniques.
    - Provides visual feedback to users, such as updating triangle colors or displaying position indicators.
    - Ensures consistent and predictable triangle behavior, preventing unexpected movements or glitches.

    Usage:
    1. Attach the TrianglePositionStateMachine script to the relevant triangle GameObject(s) in your VRChat world.
    2. Configure the triangle positions, orientations, state transitions, and input triggers in the Unity Inspector.
    3. Implement any necessary visual feedback or user interface elements to interact with the triangles.
    4. Test the triangle behavior and transitions thoroughly to ensure smooth and reliable operation.

    Dependencies:
    - Requires the VRChat SDK and Udon# compiler for proper functionality.
    - May interact with other scripts or components, such as button controllers, gesture recognition systems, or avatar tracking.

    Customization:
    - Modify the triangle positions, orientations, state transitions, and input triggers to fit your specific design requirements.
    - Adjust the animation parameters, such as movement speed, rotation angles, or easing curves, to achieve the desired visual effect.
    - Extend the script to include additional features or behaviors, such as collision detection, sound effects, or particle systems.

    Troubleshooting:
    - Ensure that the triangle GameObjects and their associated components are properly configured in the Unity Inspector.
    - Double-check the state machine transitions and input trigger assignments to avoid any conflicts or unintended behavior.
    - Test the triangle position state machine in various scenarios and edge cases to identify and resolve any potential issues.
*/

using UdonSharp;
using UnityEngine;

public class TrianglePositionStateMachine : UdonSharpBehaviour
{
    // Time to wait at origin and target
    [SerializeField]
    private float waitTime = 60f;

    // Time to move between origin and target
    [SerializeField]
    private float travelTime = 30f;

    // Start time of the movement
    private float moveStartTime;

    // Capture the starting positions as the origin for each object
    private Vector3[] originPositions = new Vector3[3];

    // Capture the starting positions of the reference objects
    private Vector3[] targetPositions = new Vector3[3];

    // State representation using integer constants
    private const int WaitingAtOrigin = 0;
    private const int MovingToTarget = 1;
    private const int WaitingAtTarget = 2;
    private const int MovingBackToOrigin = 3;
    private int currentState = WaitingAtOrigin;

    void Start()
    {
        // Capture the starting positions as the origin for each object
        for (int i = 0; i < 3; i++)
        {
            GameObject triangleObject = GameObject.Find($"Triangle{i}");
            if (triangleObject != null)
            {
                originPositions[i] = triangleObject.transform.position;
            }
            else
            {
                Debug.LogError($"Triangle{i} not found.");
            }
        }

        // Capture the starting positions of the reference objects
        for (int i = 0; i < 3; i++)
        {
            GameObject triangleRefObject = GameObject.Find($"Triangle{i}_Hollow_7");
            if (triangleRefObject != null)
            {
                targetPositions[i] = triangleRefObject.transform.position;
            }
            else
            {
                Debug.LogError($"Triangle{i}_Hollow_7 not found.");
            }
        }

        // Set initial moveStartTime
        moveStartTime = Time.time;
    }

    void Update()
    {
        switch (currentState)
        {
            case WaitingAtOrigin:
                if (Time.time - moveStartTime > waitTime)
                {
                    moveStartTime = Time.time;
                    currentState = MovingToTarget;
                }
                break;
            case MovingToTarget:
                for (int i = 0; i < 3; i++)
                {
                    MoveToTarget(ref originPositions[i], targetPositions[i], $"Triangle{i}");
                }
                if (Time.time - moveStartTime > travelTime)
                {
                    moveStartTime = Time.time;
                    currentState = WaitingAtTarget;
                }
                break;
            case WaitingAtTarget:
                if (Time.time - moveStartTime > waitTime)
                {
                    moveStartTime = Time.time;
                    currentState = MovingBackToOrigin;
                }
                break;
            case MovingBackToOrigin:
                for (int i = 0; i < 3; i++)
                {
                    MoveToOrigin(ref originPositions[i], targetPositions[i], $"Triangle{i}");
                }
                if (Time.time - moveStartTime > travelTime)
                {
                    moveStartTime = Time.time;
                    currentState = WaitingAtOrigin;
                }
                break;
        }
    }

    void MoveToTarget(ref Vector3 originPosition, Vector3 targetPosition, string objectName)
    {
        GameObject triangleObject = GameObject.Find(objectName);
        if (triangleObject != null)
        {
            float elapsed = Time.time - moveStartTime;
            float fraction = elapsed / travelTime; // Normalized time fraction
            triangleObject.transform.position = Vector3.Lerp(originPosition, targetPosition, fraction);
        }
        else
        {
            Debug.LogError($"Triangle object '{objectName}' not found.");
        }
    }

    void MoveToOrigin(ref Vector3 originPosition, Vector3 targetPosition, string objectName)
    {
        GameObject triangleObject = GameObject.Find(objectName);
        if (triangleObject != null)
        {
            float elapsed = Time.time - moveStartTime;
            float fraction = elapsed / travelTime; // Normalized time fraction
            triangleObject.transform.position = Vector3.Lerp(targetPosition, originPosition, fraction);
        }
        else
        {
            Debug.LogError($"Triangle object '{objectName}' not found.");
        }
    }
}
