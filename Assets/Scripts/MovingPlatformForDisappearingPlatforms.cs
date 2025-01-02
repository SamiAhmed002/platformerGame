using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformForDisappearingPlatforms : MonoBehaviour
{
    [SerializeField]
    private WaypointPath _waypointPath;

    [SerializeField]
    private float _speed;

    private int _targetWaypointIndex;
    private Transform _previousWaypoint;
    private Transform _targetWaypoint;

    private float _timeToWaypoint;
    private float _elapsedTime;

    private bool _isActive = true;

    void Start()
    {
        TargetNextWaypoint();
    }

    // DOES WORK
    void FixedUpdate()
    {
        if (!_isActive) return; // Skip movement when inactive

        // Get the global timer from the manager
        TimedPlatformManagerForMovingPlatforms manager = FindObjectOfType<TimedPlatformManagerForMovingPlatforms>();
        if (manager == null) return;

        float globalTimer = manager.globalTimer;

        // Calculate progress based on global timer and cycle duration
        float cycleProgress = (globalTimer % manager.cycleDuration) / manager.cycleDuration;

        // Get the total path length
        float totalPathLength = GetTotalPathLength();

        // Determine the distance traveled along the path
        float distanceTraveled = cycleProgress * totalPathLength;

        // Update position and rotation based on the distance traveled
        UpdatePlatformPosition(distanceTraveled);

        //Debug.Log($"MovingPlatformForDisappearingPlatforms Speed: {distanceTraveled / manager.cycleDuration}");
    }

    //DOES NOT WORK
    //void FixedUpdate()
    //{
    //    // Ensure the platform's active state aligns with the global timer
    //    CheckDisappearingState();

    //    if (!_isActive) return; // Skip movement if the platform is inactive

    //    // Get the global timer and sync progress
    //    TimedPlatformManagerForMovingPlatforms manager = FindObjectOfType<TimedPlatformManagerForMovingPlatforms>();
    //    if (manager == null) return;

    //    float globalTimer = manager.globalTimer;

    //    // Calculate the global cycle progress (0 to 1)
    //    float cycleProgress = (globalTimer % manager.cycleDuration) / manager.cycleDuration;

    //    // Convert global cycle progress to a position along the path
    //    UpdatePlatformPosition(cycleProgress);
    //}

    //DOES WORK
    private void TargetNextWaypoint()
    {
        _previousWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);
        _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
        _targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);

        // Calculate time to the next waypoint based on distance and speed
        float distanceToWaypoint = Vector3.Distance(_previousWaypoint.position, _targetWaypoint.position);
        _timeToWaypoint = distanceToWaypoint / _speed;
    }

    //DOES NOT WORK
    //private void TargetNextWaypoint()
    //{
    //    _previousWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);
    //    _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
    //    _targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex);

    //    _elapsedTime = 0;

    //    float distanceToWaypoint = Vector3.Distance(_previousWaypoint.position, _targetWaypoint.position);
    //    _timeToWaypoint = distanceToWaypoint / _speed;
    //}

    //DOES NOT WORK
    //private void UpdatePlatformPosition(float cycleProgress)
    //{
    //    float totalPathLength = GetTotalPathLength(); // Total length of the path
    //    float distanceTraveled = cycleProgress * totalPathLength; // Distance along the path based on progress

    //    // Traverse waypoints to find the current segment
    //    float accumulatedLength = 0f;

    //    for (int i = 0; i < _waypointPath.transform.childCount; i++)
    //    {
    //        int nextIndex = (i + 1) % _waypointPath.transform.childCount; // Loop back to start
    //        Transform startWaypoint = _waypointPath.GetWaypoint(i);
    //        Transform endWaypoint = _waypointPath.GetWaypoint(nextIndex);

    //        float segmentLength = Vector3.Distance(startWaypoint.position, endWaypoint.position);

    //        if (accumulatedLength + segmentLength >= distanceTraveled)
    //        {
    //            // Calculate progress within the current segment
    //            float segmentProgress = (distanceTraveled - accumulatedLength) / segmentLength;

    //            // Update position and rotation based on segment progress
    //            transform.position = Vector3.Lerp(startWaypoint.position, endWaypoint.position, segmentProgress);
    //            transform.rotation = Quaternion.Lerp(startWaypoint.rotation, endWaypoint.rotation, segmentProgress);

    //            return;
    //        }

    //        accumulatedLength += segmentLength;
    //    }
    //}

    //DOES WORK
    private void UpdatePlatformPosition(float distanceTraveled)
    {
        // Traverse waypoints to find the current segment
        float totalPathLength = GetTotalPathLength();
        float segmentLength = 0f;
        float accumulatedLength = 0f;

        for (int i = 0; i < _waypointPath.transform.childCount; i++)
        {
            int nextIndex = (i + 1) % _waypointPath.transform.childCount;
            Transform startWaypoint = _waypointPath.GetWaypoint(i);
            Transform endWaypoint = _waypointPath.GetWaypoint(nextIndex);

            segmentLength = Vector3.Distance(startWaypoint.position, endWaypoint.position);
            if (accumulatedLength + segmentLength >= distanceTraveled)
            {
                // Calculate progress within the current segment
                float segmentProgress = (distanceTraveled - accumulatedLength) / segmentLength;

                // Update position and rotation
                transform.position = Vector3.Lerp(startWaypoint.position, endWaypoint.position, segmentProgress);
                transform.rotation = Quaternion.Lerp(startWaypoint.rotation, endWaypoint.rotation, segmentProgress);
                return;
            }
            accumulatedLength += segmentLength;
        }
    }

    public float GetTotalPathLength()
    {
        float totalLength = 0f;

        int waypointCount = _waypointPath.transform.childCount;
        for (int i = 0; i < waypointCount; i++)
        {
            int nextIndex = (i + 1) % waypointCount;
            Transform currentWaypoint = _waypointPath.GetWaypoint(i);
            Transform nextWaypoint = _waypointPath.GetWaypoint(nextIndex);
            totalLength += Vector3.Distance(currentWaypoint.position, nextWaypoint.position);
        }

        return totalLength;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public void SetActiveState(bool isActive)
    {
        _isActive = isActive;

        if (!isActive)
        {
            // Detach the player to ensure they fall when the platform disappears
            DetachPlayer();

            // Check and log collider status before deactivation
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                //Debug.Log($"Platform {gameObject.name} collider status before deactivation: {collider.enabled}");
            }

            // Completely deactivate the platform
            gameObject.SetActive(false);

            // Verify that the platform is deactivated
            //Debug.Log($"Platform {gameObject.name} is now deactivated.");
        }
        else
        {
            // Reactivate the platform and resume its functionality
            gameObject.SetActive(true);

            // Reset the platform's renderer and colliders (in case it's needed)
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = true;
            }

            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = true;
            }

            // Check and log collider status after reactivation
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                //Debug.Log($"Platform {gameObject.name} collider status after reactivation: {collider.enabled}");
            }

            //Debug.Log($"Platform {gameObject.name} reactivated and is now visible.");
        }

        //Debug.Log($"Platform {gameObject.name} visibility set to {isActive}");
    }

    private void DetachPlayer()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Player")) // Ensure it detaches the player only
            {
                child.SetParent(null); // Detach the player
                //Debug.Log($"Player detached from platform {gameObject.name}");
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform); // Make the player a child of the platform
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null); // Remove the player from the platform
        }
    }

    private void CheckDisappearingState()
    {
        TimedPlatformManagerForMovingPlatforms manager = FindObjectOfType<TimedPlatformManagerForMovingPlatforms>();
        if (manager == null) return;

        float globalTimer = manager.globalTimer;

        // Example: Make platforms active for half the cycle duration
        bool shouldBeActive = (globalTimer % manager.cycleDuration) < (manager.cycleDuration / 2);
        SetActiveState(shouldBeActive);
    }
}



