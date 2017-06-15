using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysicsScript : MonoBehaviour {

    MovementControllerScript movementControllerScript;

    //this is the velocity of the object -- it persists and is modified from frame to frame
    public Vector2 velocity;
    public bool onGround = false;
    public bool affectedByGravity = true;
    public float gravity;
    public float groundFriction;
    public float airFriction;

    //this is the velocity of the object's frame of reference (moving platform, etc) -- it is new every frame
    public Vector2 baseVelocity;

	// Use this for initialization
	void Start () {
        movementControllerScript = GetComponent<MovementControllerScript>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (affectedByGravity)
        {
            velocity.y -= gravity;
        }
        movementControllerScript.Move(velocity);
        onGround = checkIfGrounded();
        if (onGround)
        {
            velocity.y = 0;
            velocity.x *= (1-groundFriction);
        }
        else
        {
            velocity.x *= (1-airFriction);
        }
    }

    bool checkIfGrounded()
    {
        foreach (RaycastHit2D hit in movementControllerScript.collisionState.bottomHits)
        {
            if (hit.transform != null)
            {
                return true;
            }
        }
        return false;
    }
}
