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
    in a VRChat world. It utilizes a state machine approach to manage the elevator's based on a timer.
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
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

    // Synced variables for elevator state and position
    [UdonSynced]
    private int currentState = WaitingAtBottom;
    [UdonSynced]
    private Vector3 elevatorPosition;

    void Start()
    {
        // Capture the starting position as the bottom position
        GameObject elevatorObject = GameObject.Find("CircleElevator");
        if (elevatorObject != null)
        {
            bottomPosition = elevatorObject.transform.position;
            elevatorPosition = bottomPosition; // Initialize synced position
        }
        else
        {
            Debug.LogError("CircleElevator not found.");
        }

        // Set initial moveStartTime
        moveStartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case WaitingAtBottom:
                if (Time.time - moveStartTime > waitTime)
                {
                    moveStartTime = Time.time;
                    ChangeElevatorState(MovingToTop);
                }
                break;
            case MovingToTop:
                MoveToHeight(bottomPosition.y, targetHeight);
                if (Time.time - moveStartTime > travelTime)
                {
                    moveStartTime = Time.time;
                    ChangeElevatorState(WaitingAtTop);
                }
                break;
            case WaitingAtTop:
                if (Time.time - moveStartTime > waitTime)
                {
                    moveStartTime = Time.time;
                    ChangeElevatorState(MovingToBottom);
                }
                break;
            case MovingToBottom:
                MoveToHeight(targetHeight, bottomPosition.y);
                if (Time.time - moveStartTime > travelTime)
                {
                    moveStartTime = Time.time;
                    ChangeElevatorState(WaitingAtBottom);
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
            elevatorPosition = new Vector3(elevatorObject.transform.position.x, currentHeight, elevatorObject.transform.position.z);
            elevatorObject.transform.position = elevatorPosition;
        }
        else
        {
            Debug.LogError("CircleElevator not found.");
        }
    }

    // Custom network event for synchronizing the elevator state and position
    public void OnElevatorSync()
    {
        GameObject elevatorObject = GameObject.Find("CircleElevator");
        if (elevatorObject != null)
        {
            // Update the elevator position based on the synchronized data
            elevatorObject.transform.position = elevatorPosition;

            // Handle the synchronized state
            switch (currentState)
            {
                case WaitingAtBottom:
                    // No action needed, already waiting at the bottom
                    break;
                case MovingToTop:
                    // Start moving to the top position
                    moveStartTime = Time.time;
                    break;
                case WaitingAtTop:
                    // No action needed, already waiting at the top
                    break;
                case MovingToBottom:
                    // Start moving to the bottom position
                    moveStartTime = Time.time;
                    break;
            }
        }
        else
        {
            Debug.LogError("CircleElevator not found.");
        }
    }

    // Change the elevator state and send a custom network event for synchronization
    void ChangeElevatorState(int newState)
    {
        currentState = newState;
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(OnElevatorSync));
    }


    // Called when the script receives synchronized data
    public override void OnDeserialization()
    {
        GameObject elevatorObject = GameObject.Find("CircleElevator");
        if (elevatorObject != null)
        {
            // Update the elevator position based on the synchronized data
            elevatorObject.transform.position = elevatorPosition;

            // Handle the synchronized state
            switch (currentState)
            {
                case WaitingAtBottom:
                    // No action needed, already waiting at the bottom
                    break;
                case MovingToTop:
                    // Start moving to the top position
                    moveStartTime = Time.time;
                    break;
                case WaitingAtTop:
                    // No action needed, already waiting at the top
                    break;
                case MovingToBottom:
                    // Start moving to the bottom position
                    moveStartTime = Time.time;
                    break;
            }
        }
        else
        {
            Debug.LogError("CircleElevator not found.");
        }
    }
}