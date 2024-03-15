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
                trianglePositions[i] = originPositions[i]; // Initialize synced positions
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
                    ChangeTriangleState(MovingToTarget);
                }
                break;
            case MovingToTarget:
                for (int i = 0; i < 3; i++)
                {
                    MoveToTarget(ref originPositions[i], targetPositions[i], $"Triangle{i}", i);
                }
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
                for (int i = 0; i < 3; i++)
                {
                    MoveToOrigin(ref originPositions[i], targetPositions[i], $"Triangle{i}", i);
                }
                if (Time.time - moveStartTime > travelTime)
                {
                    moveStartTime = Time.time;
                    ChangeTriangleState(WaitingAtOrigin);
                }
                break;
        }
    }

    void MoveToTarget(ref Vector3 originPosition, Vector3 targetPosition, string objectName, int index)
    {
        GameObject triangleObject = GameObject.Find(objectName);
        if (triangleObject != null)
        {
            float elapsed = Time.time - moveStartTime;
            float fraction = elapsed / travelTime; // Normalized time fraction
            trianglePositions[index] = Vector3.Lerp(originPosition, targetPosition, fraction);
            triangleObject.transform.position = trianglePositions[index];
        }
        else
        {
            Debug.LogError($"Triangle object '{objectName}' not found.");
        }
    }

    void MoveToOrigin(ref Vector3 originPosition, Vector3 targetPosition, string objectName, int index)
    {
        GameObject triangleObject = GameObject.Find(objectName);
        if (triangleObject != null)
        {
            float elapsed = Time.time - moveStartTime;
            float fraction = elapsed / travelTime; // Normalized time fraction
            trianglePositions[index] = Vector3.Lerp(targetPosition, originPosition, fraction);
            triangleObject.transform.position = trianglePositions[index];
        }
        else
        {
            Debug.LogError($"Triangle object '{objectName}' not found.");
        }
    }

    // Change the triangle state and request serialization
    void ChangeTriangleState(int newState)
    {
        currentState = newState;
        RequestSerialization();
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
                triangleObject.transform.position = trianglePositions[i];

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