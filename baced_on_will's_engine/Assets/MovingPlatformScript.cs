using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PassengerHandlerScript))]
[RequireComponent(typeof(RaycasterScript))]
[RequireComponent(typeof(MovementControllerScript))]
[RequireComponent(typeof(BoxCollider2D))]


//TODO WHY DO WE PUSH BOXES WHEN WE'RE WALKING ON MOVING PLATFORMS?

public class MovingPlatformScript : MonoBehaviour {
    PassengerHandlerScript passengerHandlerScript;
    RaycasterScript raycasterScript;
    MovementControllerScript movementControllerScript;
    new BoxCollider2D collider;
    public List<Vector2> waypoints;
    int currentWaypointIndex = 1;
    public float speed;

    // Use this for initialization
    void Start () {
        passengerHandlerScript = GetComponent<PassengerHandlerScript>();
        raycasterScript = GetComponent<RaycasterScript>();
        movementControllerScript = GetComponent<MovementControllerScript>();
        collider = GetComponent<BoxCollider2D>();		
	}

    public void nextWaypoint()
    {
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Count)
        {
            currentWaypointIndex = 0;
        }
    }


    public void MovementUpdate()
    {
        Vector2 dif = (waypoints[currentWaypointIndex] - (Vector2)transform.position);
        Vector2 direction = dif.normalized;
        float distance = Vector2.Distance(transform.position, waypoints[currentWaypointIndex]);
        movementControllerScript.velocity = direction * speed;

        if (distance <= speed)
        {
            movementControllerScript.velocity = dif;
            nextWaypoint();
         }

        GetComponent<PusherScript>().PushMovement();
    }
}