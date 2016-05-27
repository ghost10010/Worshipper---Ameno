using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraWaypointController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> waypoints = new List<GameObject>();

    private int currentWaypointId = 0;
    private GameObject currentWaypoint;

    // Use this for initialization
    void Start()
    {
        SetNextWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToWaypoint();
        RotateToWaypoint();
    }
    public void MoveToWaypoint()
    {
        Vector3 currentPosition = transform.position;

        Vector3 newPosition = Vector3.Lerp(currentPosition, waypoints[currentWaypointId].transform.position, Time.deltaTime * 0.25f);
        
        transform.position = newPosition;

        if (Vector3.Distance(this.transform.position, waypoints[currentWaypointId].transform.position) < 5.0f)
        {
            SetNextWaypoint();
        }
    }
    public void RotateToWaypoint()
    {
        int nextWaypoint = GetNextWaypointId();
        int damping = 2;

        Vector3 lookPos = waypoints[nextWaypoint].transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        Quaternion position = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);

        transform.rotation = position;

        float angle = 10;
        if (Vector3.Angle(waypoints[nextWaypoint].transform.forward, transform.position - waypoints[nextWaypoint].transform.position) < angle)
        {
            //Debug.Log("Looking at");
        }
    }

    private void SetNextWaypoint()
    {
        currentWaypointId++;

        if (currentWaypointId >= waypoints.Count)
        {
            currentWaypointId = 0;
        }
    }
    public int GetNextWaypointId()
    {
        int nextId = currentWaypointId;
        nextId++;

        if (nextId >= waypoints.Count)
        {
            nextId = 0;
        }

        return nextId;
    }
}