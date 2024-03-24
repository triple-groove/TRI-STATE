/*
    ████████╗██████╗ ██╗      ███████╗████████╗ █████╗ ████████╗███████╗
    ╚══██╔══╝██╔══██╗██║      ██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██╔════╝
       ██║   ██████╔╝██║█████╗███████╗   ██║   ███████║   ██║   █████╗  
       ██║   ██╔══██╗██║╚════╝╚════██║   ██║   ██╔══██║   ██║   ██╔══╝  
       ██║   ██║  ██║██║      ███████║   ██║   ██║  ██║   ██║   ███████╗
       ╚═╝   ╚═╝  ╚═╝╚═╝      ╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝   ╚══════╝
    triple_groove - meow btw...if you even care

    TrianglePositionStateMachine.cs

    The TrianglePositionStateMachine script controls the position triangular walls.
    This makes the walls of TRI-STATE breath.
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
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
    private float syncMoveStartTime;

    // Capture the starting positions as the origin for each object
    private Vector3[] originPositions = new Vector3[3];

    // Capture the starting positions of the reference objects
    private Vector3[] targetPositions = new Vector3[3];

    // State representation using integer constants
    private const int WaitingAtOrigin = 0;
    private const int MovingToTarget = 1;
    private const int WaitingAtTarget = 2;
    private const int MovingBackToOrigin = 3;

    // Synced variables for triangle state and positions
    [UdonSynced]
    private int currentState = WaitingAtOrigin;
    private int syncCurrentState = WaitingAtOrigin;

    private Vector3[] trianglePositions = new Vector3[3];

    void Start()
    {
        // Capture the starting positions as the origin for each object
        for (int i = 0; i < 3; i++)
        {
            GameObject triangleObject = GameObject.Find($"Triangle{i}");
            if (triangleObject != null)
            {
                originPositions[i] = triangleObject.transform.position;
                trianglePositions[i] = originPositions[i];
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

        if (Networking.IsMaster)
        {
            elapsedTime = Time.realtimeSinceStartup - moveStartTime;

            switch (currentState)
            {
                case WaitingAtOrigin:
                    if (elapsedTime > waitTime)
                    {
                        ChangeTriangleState(MovingToTarget);
                    }
                    break;
                case MovingToTarget:
                    for (int i = 0; i < 3; i++)
                    {
                        MoveToTarget(ref originPositions[i], targetPositions[i], $"Triangle{i}", elapsedTime, i);
                    }
                    if (elapsedTime > travelTime)
                    {
                        ChangeTriangleState(WaitingAtTarget);
                    }
                    break;
                case WaitingAtTarget:
                    if (elapsedTime > waitTime)
                    {
                        ChangeTriangleState(MovingBackToOrigin);
                    }
                    break;
                case MovingBackToOrigin:
                    for (int i = 0; i < 3; i++)
                    {
                        MoveToOrigin(ref originPositions[i], targetPositions[i], $"Triangle{i}", elapsedTime, i);
                    }
                    if (elapsedTime > travelTime)
                    {
                        ChangeTriangleState(WaitingAtOrigin);
                    }
                    break;
            }
        }
        else
        {
            // Special handling to offset the delay of sending networked variable
            // We will run our own position calculation here, but the state change will be late,
            // so we predict the state change and change behavior with a super state.
            elapsedTime = Time.realtimeSinceStartup - syncMoveStartTime;

            switch (syncCurrentState)
            {
                case WaitingAtOrigin:
                    if (elapsedTime > waitTime)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            MoveToTarget(ref originPositions[i], targetPositions[i], $"Triangle{i}", elapsedTime - waitTime, i);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            GameObject triangleObject = GameObject.Find($"Triangle{i}");
                            if (triangleObject != null)
                            {
                                triangleObject.transform.position = originPositions[i];
                            }
                        }
                    }
                    break;
                case MovingToTarget:
                    if (elapsedTime > travelTime)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            GameObject triangleObject = GameObject.Find($"Triangle{i}");
                            if (triangleObject != null)
                            {
                                triangleObject.transform.position = targetPositions[i];
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            MoveToTarget(ref originPositions[i], targetPositions[i], $"Triangle{i}", elapsedTime, i);
                        }
                    }
                    break;
                case WaitingAtTarget:
                    if (elapsedTime > waitTime)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            MoveToOrigin(ref originPositions[i], targetPositions[i], $"Triangle{i}", elapsedTime - waitTime, i);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            GameObject triangleObject = GameObject.Find($"Triangle{i}");
                            if (triangleObject != null)
                            {
                                triangleObject.transform.position = targetPositions[i];
                            }
                        }
                    }
                    break;
                case MovingBackToOrigin:
                    if (elapsedTime > travelTime)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            GameObject triangleObject = GameObject.Find($"Triangle{i}");
                            if (triangleObject != null)
                            {
                                triangleObject.transform.position = originPositions[i];
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            MoveToOrigin(ref originPositions[i], targetPositions[i], $"Triangle{i}", elapsedTime, i);
                        }
                    }
                    break;
            }
        }
    }

    void MoveToTarget(ref Vector3 originPosition, Vector3 targetPosition, string objectName, float elapsedTime, int index)
    {
        GameObject triangleObject = GameObject.Find(objectName);
        if (triangleObject != null)
        {
            float fraction = elapsedTime / travelTime; // Normalized time fraction
            Vector3 newPosition = Vector3.Lerp(originPosition, targetPosition, fraction);
            trianglePositions[index] = newPosition;
            triangleObject.transform.position = newPosition;
        }
        else
        {
            Debug.LogError($"Triangle object '{objectName}' not found.");
        }
    }

    void MoveToOrigin(ref Vector3 originPosition, Vector3 targetPosition, string objectName, float elapsedTime, int index)
    {
        GameObject triangleObject = GameObject.Find(objectName);
        if (triangleObject != null)
        {
            float fraction = elapsedTime / travelTime; // Normalized time fraction
            Vector3 newPosition = Vector3.Lerp(targetPosition, originPosition, fraction);
            trianglePositions[index] = newPosition;
            triangleObject.transform.position = newPosition;
        }
        else
        {
            Debug.LogError($"Triangle object '{objectName}' not found.");
        }
    }

    // Called when the script receives synchronized data
    public override void OnDeserialization(VRC.Udon.Common.DeserializationResult deserializationResult)
    {
        // We must protect since we do not know what has been deserialized!
        if (syncCurrentState != currentState)
        {
            syncMoveStartTime = deserializationResult.sendTime;
            syncCurrentState = currentState;
        }
    }

    // Change the triangle state and send a custom network event for synchronization
    void ChangeTriangleState(int newState)
    {
        currentState = newState;
        moveStartTime = Time.realtimeSinceStartup;

        RequestSerialization();
    }
}