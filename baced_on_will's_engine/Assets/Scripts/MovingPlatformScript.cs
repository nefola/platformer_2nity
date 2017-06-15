using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour {

    public List<Vector2> waypoints;
    public Vector2 velocity;
    MovementControllerScript movementControllerScript;
    new BoxCollider2D collider;
    public float speed;
    int currentWaypointIndex;

	// Use this for initialization
	void Start () {
        collider = GetComponent<BoxCollider2D>();
        movementControllerScript = GetComponent<MovementControllerScript>();
        currentWaypointIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 delta = waypoints[currentWaypointIndex] - (Vector2) transform.position;
        if (delta.magnitude < speed)
        {
            velocity = delta;
            nextWaypoint();
        }else
        {
            velocity = delta.normalized * speed;
        }
        movementControllerScript.FindPassengers();
        movementControllerScript.Move(velocity);
    }

    void nextWaypoint()
    {
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Count)
        {
            currentWaypointIndex = 0;
        }
    }

   
}
