using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RaycasterScript))]
public class PassengerHandlerScript : MonoBehaviour {

    RaycasterScript raycasterScript;
    MovementControllerScript movementControllerScript;
    public List<GameObject> passengers;

	// Use this for initialization
	void Start () {
        raycasterScript = GetComponent<RaycasterScript>();
        movementControllerScript = GetComponent<MovementControllerScript>();
	}

    public void LateUpdate()
    {
        passengers = null;
    }

    public void MovementUpdateCleanupPass()
    {

        Vector2 amountLeftToMove = movementControllerScript.amountLeftToMove + movementControllerScript.baseAmountLeftToMove;
        passengers = FindPassengers();

        if (Mathf.Abs(amountLeftToMove.x) + Mathf.Abs(amountLeftToMove.y) > .005)
        {

            foreach (GameObject passenger in passengers)
            {
                passenger.GetComponent<MovementControllerScript>().Move(new Vector2(amountLeftToMove.x * -1, -1));
            }
        }
    }

    public List<GameObject> FindAllPassengersRecursive()
    {
        List<GameObject> myPassengers = FindPassengers();
        List<GameObject> passengersRecursive = new List<GameObject>();
        passengersRecursive.AddRange(myPassengers);
        foreach (GameObject passenger in myPassengers)
        {
            PassengerHandlerScript phs = passenger.GetComponent<PassengerHandlerScript>();
            if (phs != null)
            {
                passengersRecursive.AddRange(phs.FindAllPassengersRecursive());
            }
        }
        return passengersRecursive;
    }

    public void HandlePassengersRecursive()
    {
        passengers = FindAllPassengersRecursive();
            foreach (GameObject passenger in passengers)
            {
                MovementControllerScript mcs = passenger.GetComponent<MovementControllerScript>();
                mcs.baseVelocity.x += movementControllerScript.velocity.x;

         
                if (movementControllerScript.isJumping){
                    mcs.velocity.y = movementControllerScript.velocity.y;
                    mcs.isJumping = true;
                }else
                {
                    mcs.baseVelocity.y = movementControllerScript.velocity.y;
                }

            if (!mcs.isJumping)
                {
                    mcs.isGrounded = true;
                    mcs.velocity.y = 0;
                }
            }
    }
    


    public List<GameObject> FindPassengers() {
        List<GameObject> passengersTmp = new List<GameObject>();
        Vector2 up = new Vector2(0, raycasterScript.skinWidth + Mathf.Abs(movementControllerScript.velocity.y)  );
        raycasterScript.VerticalCollisions(ref up, true);
        for (int i = 0; i < raycasterScript.collisionState.topHits.Length; i++)
        {
            GameObject go = raycasterScript.collisionState.topHits[i];
            if (go != null)
            {
                GravityScript gs = go.GetComponent<GravityScript>();
                if (gs != null) { 

                MovementControllerScript mcs = go.GetComponent<MovementControllerScript>();
                    if (mcs)
                    {

                        if (mcs.isJumping == false)
                        {
                            raycasterScript.collisionState.topHits[i] = null;
                            mcs.thingImStandingOn = gameObject;
                            if (!passengersTmp.Contains(go))
                            {
                                passengersTmp.Add(go);

                                //this will keed me anchored to the platform I'm on
                                if (movementControllerScript.velocity.y < 0)
                                {
                                    mcs.Move(Vector2.down);
                                }
                            }
                        }
                    }
                }
            }
        }
    //    passengers = passengersTmp;
        return passengersTmp;
    }
}
