using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MovementControllerScript))]
[RequireComponent(typeof(RaycasterScript))]

public class GravityScript : MonoBehaviour {

    MovementControllerScript movementControllerScript;
    public Vector2 gravity;
    RaycasterScript raycasterScript;
  

	// Use this for initialization
	void Start () {
        movementControllerScript = GetComponent<MovementControllerScript>();
        raycasterScript = GetComponent<RaycasterScript>();
	}
	

    public void MovementUpdate()
    {
        float yVelocityZero = 0;
        if (movementControllerScript.thingImStandingOn != null)
        {
            yVelocityZero = movementControllerScript.thingImStandingOn.GetComponent<MovementControllerScript>().velocity.y;
        }

        movementControllerScript.isGrounded = (raycasterScript.collisionState.anyBottomHits() && !movementControllerScript.isJumping);// (movementControllerScript.velocity.y <= yVelocityZero);

        if ((movementControllerScript.isGrounded && movementControllerScript.velocity.y < 0))
        {
                      movementControllerScript.velocity.y = 0;
        }
        
        if    (raycasterScript.collisionState.anyTopHits() && movementControllerScript.velocity.y > 0)
        {

            foreach (GameObject go in raycasterScript.collisionState.topHits)
            {
                if (go != null)
                {
                    MovementControllerScript mcs = go.GetComponent<MovementControllerScript>();
                    //TODO IS THIS RIGHT?
//                    movementControllerScript.velocity.y = 0;
                    if (mcs != null)
                    {
                        if (movementControllerScript.velocity.y < mcs.velocity.y)
                        {
                            movementControllerScript.velocity.y = mcs.velocity.y;
                        }else
                        {
                            movementControllerScript.velocity.y = 0;
                        }
                    }
                    else
                    {
                        movementControllerScript.velocity.y = 0;
                    }
                }
            }
        }

        movementControllerScript.velocity += gravity;
    }
}