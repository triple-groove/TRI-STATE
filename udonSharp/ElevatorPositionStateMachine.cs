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
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ElevatorPositionStateMachine : UdonSharpBehaviour
{
    // Time to wait at the top and bottom positions
    [SerializeField]
    private float waitTime = 60f;

    // Time to move between top and bottom positions
    [SerializeField]
    private float travelTime = 5f;

    // Start time of the movement (MASTER)
    private float moveStartTime;

    // Start time of the movement (REMOTE non-MASTER)
    private float syncMoveStartTime;

    private Vector3 topPosition;
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

    private int syncCurrentState = WaitingAtBottom;

    private Vector3 elevatorPosition;

    void Start()
    {
        GameObject elevatorObject = GameObject.Find("CircleElevator");

        topPosition = new Vector3(elevatorObject.transform.position.x, targetHeight, elevatorObject.transform.position.z);
        bottomPosition = elevatorObject.transform.position;
        elevatorPosition = bottomPosition; // Initialize synced position

        // Set initial moveStartTime
        moveStartTime = Time.realtimeSinceStartup;
    }

    // Prevents non-master players from taking ownership
    public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
    {
        return requestedOwner.isMaster;
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime;

        GameObject elevatorObject = GameObject.Find("CircleElevator");
        if (elevatorObject != null)
        {
            if (Networking.IsMaster)
            {
                elapsedTime = Time.realtimeSinceStartup - moveStartTime;

                switch (currentState)
                {
                    case WaitingAtBottom:
                        if (elapsedTime > waitTime)
                        {
                            ChangeElevatorState(MovingToTop);
                        }
                        break;
                    case MovingToTop:
                        MoveToHeight(bottomPosition.y, targetHeight, elapsedTime);
                        if (elapsedTime > travelTime)
                        {
                            ChangeElevatorState(WaitingAtTop);
                        }
                        break;
                    case WaitingAtTop:
                        if (elapsedTime > waitTime)
                        {
                            ChangeElevatorState(MovingToBottom);
                        }
                        break;
                    case MovingToBottom:
                        MoveToHeight(targetHeight, bottomPosition.y, elapsedTime);
                        if (elapsedTime > travelTime)
                        {
                            ChangeElevatorState(WaitingAtBottom);
                        }
                        break;
                }
            }
            else
            {
                // Special handling to offset the delay of sending networked variable
                // We will run out own position calc here but the state change will be late
                // so we predict the state change and change behaviour with a super state.
                // Handling on time for wait state's super state.
                elapsedTime = Time.realtimeSinceStartup - syncMoveStartTime;

                switch (syncCurrentState)
                {
                    case WaitingAtBottom:
                        if (elapsedTime > waitTime)
                        {
                            MoveToHeight(bottomPosition.y, targetHeight, elapsedTime-waitTime);
                        }
                        else
                        {
                            elevatorObject.transform.position = bottomPosition;
                        }
                        break;
                    case MovingToTop:
                        if (elapsedTime > travelTime)
                        {
                            elevatorObject.transform.position = topPosition;
                        }
                        else
                        {
                            MoveToHeight(bottomPosition.y, targetHeight, elapsedTime);
                        }
                        break;
                    case WaitingAtTop:
                        if (elapsedTime > waitTime)
                        {
                            MoveToHeight(targetHeight, bottomPosition.y, elapsedTime-waitTime);
                        }
                        else
                        {
                            elevatorObject.transform.position = topPosition;
                        }
                        break;
                    case MovingToBottom:
                        if (elapsedTime > travelTime)
                        {
                            elevatorObject.transform.position = bottomPosition;
                        }
                        else
                        {
                            MoveToHeight(targetHeight, bottomPosition.y, elapsedTime);
                        }
                        break;
                }
            }
        }
    }

    void MoveToHeight(float startHeight, float targetHeight, float elapsedTime)
    {
        GameObject elevatorObject = GameObject.Find("CircleElevator");
        if (elevatorObject != null)
        {
            float fraction = elapsedTime / travelTime; // Normalized time fraction
            float currentHeight = Mathf.Lerp(startHeight, targetHeight, fraction);
            elevatorPosition = new Vector3(elevatorObject.transform.position.x, currentHeight, elevatorObject.transform.position.z);
            elevatorObject.transform.position = elevatorPosition;
        }
        else
        {
            Debug.LogError("CircleElevator not found.");
        }
    }

    // Called when the script receives synchronized data
    public override void OnDeserialization(VRC.Udon.Common.DeserializationResult deserializationResult)
    {
        // We must protecc, since we do not know what has been deserialized!
        if (syncCurrentState != currentState)
        {
            syncMoveStartTime = deserializationResult.sendTime;
            syncCurrentState = currentState;
        }
    }

    // Change the elevator state and send a custom network event for synchronization
    void ChangeElevatorState(int newState)
    {
        currentState = newState;
        moveStartTime = Time.realtimeSinceStartup;
        
        RequestSerialization();
    }
}