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

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
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

    // Synced variables for triangle state and positions
    [UdonSynced]
    private int currentState = WaitingAtOrigin;
    [UdonSynced]
    private Vector3 triangle0Position;
    [UdonSynced]
    private Vector3 triangle1Position;
    [UdonSynced]
    private Vector3 triangle2Position;

    void Start()
    {
        // Capture the starting positions as the origin for each object
        for (int i = 0; i < 3; i++)
        {
            GameObject triangleObject = GameObject.Find($"Triangle{i}");
            if (triangleObject != null)
            {
                originPositions[i] = triangleObject.transform.position;
                switch (i)
                {
                    case 0:
                        triangle0Position = originPositions[i];
                        break;
                    case 1:
                        triangle1Position = originPositions[i];
                        break;
                    case 2:
                        triangle2Position = originPositions[i];
                        break;
                }
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

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case WaitingAtOrigin:
                if (Time.time - moveStartTime > waitTime)
                {
                    moveStartTime = Time.time;
                    ChangeTriangleState(MovingToTarget);
                }
                break;
            case MovingToTarget:
                MoveToTarget(ref originPositions[0], targetPositions[0], "Triangle0", ref triangle0Position);
                MoveToTarget(ref originPositions[1], targetPositions[1], "Triangle1", ref triangle1Position);
                MoveToTarget(ref originPositions[2], targetPositions[2], "Triangle2", ref triangle2Position);
                if (Time.time - moveStartTime > travelTime)
                {
                    moveStartTime = Time.time;
                    ChangeTriangleState(WaitingAtTarget);
                }
                break;
            case WaitingAtTarget:
                if (Time.time - moveStartTime > waitTime)
                {
                    moveStartTime = Time.time;
                    ChangeTriangleState(MovingBackToOrigin);
                }
                break;
            case MovingBackToOrigin:
                MoveToOrigin(ref originPositions[0], targetPositions[0], "Triangle0", ref triangle0Position);
                MoveToOrigin(ref originPositions[1], targetPositions[1], "Triangle1", ref triangle1Position);
                MoveToOrigin(ref originPositions[2], targetPositions[2], "Triangle2", ref triangle2Position);
                if (Time.time - moveStartTime > travelTime)
                {
                    moveStartTime = Time.time;
                    ChangeTriangleState(WaitingAtOrigin);
                }
                break;
        }
    }

    void MoveToTarget(ref Vector3 originPosition, Vector3 targetPosition, string objectName, ref Vector3 trianglePosition)
    {
        GameObject triangleObject = GameObject.Find(objectName);
        if (triangleObject != null)
        {
            float elapsed = Time.time - moveStartTime;
            float fraction = elapsed / travelTime; // Normalized time fraction
            trianglePosition = Vector3.Lerp(originPosition, targetPosition, fraction);
            triangleObject.transform.position = trianglePosition;
        }
        else
        {
            Debug.LogError($"Triangle object '{objectName}' not found.");
        }
    }

    void MoveToOrigin(ref Vector3 originPosition, Vector3 targetPosition, string objectName, ref Vector3 trianglePosition)
    {
        GameObject triangleObject = GameObject.Find(objectName);
        if (triangleObject != null)
        {
            float elapsed = Time.time - moveStartTime;
            float fraction = elapsed / travelTime; // Normalized time fraction
            trianglePosition = Vector3.Lerp(targetPosition, originPosition, fraction);
            triangleObject.transform.position = trianglePosition;
        }
        else
        {
            Debug.LogError($"Triangle object '{objectName}' not found.");
        }
    }

    // Custom network event for synchronizing the triangle state and positions
    public void OnTriangleSync()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject triangleObject = GameObject.Find($"Triangle{i}");
            if (triangleObject != null)
            {
                // Update the triangle position based on the synchronized data
                switch (i)
                {
                    case 0:
                        triangleObject.transform.position = triangle0Position;
                        break;
                    case 1:
                        triangleObject.transform.position = triangle1Position;
                        break;
                    case 2:
                        triangleObject.transform.position = triangle2Position;
                        break;
                }

                // Handle the synchronized state
                switch (currentState)
                {
                    case WaitingAtOrigin:
                        moveStartTime = Time.time;
                        break;
                    case MovingToTarget:
                        // Start moving to the target position
                        moveStartTime = Time.time;
                        break;
                    case WaitingAtTarget:
                        moveStartTime = Time.time;
                        break;
                    case MovingBackToOrigin:
                        // Start moving back to the origin position
                        moveStartTime = Time.time;
                        break;
                }
            }
            else
            {
                Debug.LogError($"Triangle{i} not found.");
            }
        }
    }

    // Change the triangle state and send a custom network event for synchronization
    void ChangeTriangleState(int newState)
    {
        currentState = newState;
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(OnTriangleSync));
    }

    // Called when the script receives synchronized data
    public override void OnDeserialization()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject triangleObject = GameObject.Find($"Triangle{i}");
            if (triangleObject != null)
            {
                // Update the triangle position based on the synchronized data
                switch (i)
                {
                    case 0:
                        triangleObject.transform.position = triangle0Position;
                        break;
                    case 1:
                        triangleObject.transform.position = triangle1Position;
                        break;
                    case 2:
                        triangleObject.transform.position = triangle2Position;
                        break;
                }

                // Handle the synchronized state
                switch (currentState)
                {
                    case WaitingAtOrigin:
                        // No action needed, already waiting at the origin
                        break;
                    case MovingToTarget:
                        // Start moving to the target position
                        moveStartTime = Time.time;
                        break;
                    case WaitingAtTarget:
                        // No action needed, already waiting at the target
                        break;
                    case MovingBackToOrigin:
                        // Start moving back to the origin position
                        moveStartTime = Time.time;
                        break;
                }
            }
            else
            {
                Debug.LogError($"Triangle{i} not found.");
            }
        }
    }
}