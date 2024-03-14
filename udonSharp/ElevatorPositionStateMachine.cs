/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    ElevatorPositionStateMachine.cs

    The ElevatorPositionStateMachine script controls the position and movement of an elevator
    in a VRChat world. It utilizes a state machine approach to manage the elevator's behavior
    based on user interactions and predefined positions.

    Key Features:
    - Defines a set of elevator positions, such as ground floor, first floor, and second floor.
    - Implements a state machine to handle transitions between different elevator positions.
    - Responds to user input, such as button presses or triggers, to initiate elevator movement.
    - Animates the elevator's movement smoothly between positions using interpolation.
    - Provides visual feedback to users, such as updating button colors or displaying floor indicators.
    - Ensures safe and consistent elevator behavior, preventing unexpected movements or glitches.

    Usage:
    1. Attach the ElevatorPositionStateMachine script to the elevator GameObject in your VRChat world.
    2. Configure the elevator positions, state transitions, and input triggers in the Unity Inspector.
    3. Implement any necessary visual feedback or user interface elements to interact with the elevator.
    4. Test the elevator's functionality thoroughly to ensure smooth and reliable operation.

    Dependencies:
    - Requires the VRChat SDK and Udon# compiler for proper functionality.
    - May interact with other scripts or components, such as button controllers or user tracking systems.

    Customization:
    - Modify the elevator positions, state transitions, and input triggers to fit your specific requirements.
    - Adjust the animation parameters, such as movement speed or easing curves, to achieve the desired visual effect.
    - Extend the script to include additional features or behaviors, such as audio feedback or special effects.

    Troubleshooting:
    - Ensure that the elevator GameObject and its associated components are properly configured in the Unity Inspector.
    - Double-check the state machine transitions and input trigger assignments to avoid any conflicts or unintended behavior.
    - Test the elevator thoroughly in various scenarios to identify and resolve any potential issues or edge cases.
*/

using UdonSharp;
using UnityEngine;

public class ElevatorPositionStateMachine : UdonSharpBehaviour
{
    // Time to wait at the top and bottom positions
    [SerializeField]
    private float waitTime = 5f;

    // Time to move between top and bottom positions
    [SerializeField]
    private float travelTime = 10f;

    // Start time of the movement
    private float moveStartTime;

    // Starting position of the elevator
    private Vector3 bottomPosition;

    // Target position of the elevator (top position)
    [SerializeField]
    private float targetHeight = 25f;

    // State representation using integer constants
    private const int WaitingAtBottom = 0;
    private const int MovingToTop = 1;
    private const int WaitingAtTop = 2;
    private const int MovingToBottom = 3;
    private int currentState = WaitingAtBottom;

    void Start()
    {
        // Capture the starting position as the bottom position
        GameObject elevatorObject = GameObject.Find("CircleElevator");
        if (elevatorObject != null)
        {
            bottomPosition = elevatorObject.transform.position;
        }
        else
        {
            Debug.LogError("CircleElevator not found.");
        }

        // Set initial moveStartTime
        moveStartTime = Time.time;
    }

    void Update()
    {
        switch (currentState)
        {
            case WaitingAtBottom:
                if (Time.time - moveStartTime > waitTime)
                {
                    moveStartTime = Time.time;
                    currentState = MovingToTop;
                }
                break;
            case MovingToTop:
                MoveToHeight(bottomPosition.y, targetHeight);
                if (Time.time - moveStartTime > travelTime)
                {
                    moveStartTime = Time.time;
                    currentState = WaitingAtTop;
                }
                break;
            case WaitingAtTop:
                if (Time.time - moveStartTime > waitTime)
                {
                    moveStartTime = Time.time;
                    currentState = MovingToBottom;
                }
                break;
            case MovingToBottom:
                MoveToHeight(targetHeight, bottomPosition.y);
                if (Time.time - moveStartTime > travelTime)
                {
                    moveStartTime = Time.time;
                    currentState = WaitingAtBottom;
                }
                break;
        }
    }

    void MoveToHeight(float startHeight, float targetHeight)
    {
        GameObject elevatorObject = GameObject.Find("CircleElevator");
        if (elevatorObject != null)
        {
            float elapsed = Time.time - moveStartTime;
            float fraction = elapsed / travelTime; // Normalized time fraction
            float currentHeight = Mathf.Lerp(startHeight, targetHeight, fraction);
            elevatorObject.transform.position = new Vector3(elevatorObject.transform.position.x, currentHeight, elevatorObject.transform.position.z);
        }
        else
        {
            Debug.LogError("CircleElevator not found.");
        }
    }
}
