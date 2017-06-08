using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManagerScript : MonoBehaviour {

    public List<MovementControllerScript> movementControllers;
    public GameObject platform;
    public int numberOfPasses;
    public bool somethingMoved;
    public int numberOfPlatformsToSpawn;
  
	// Use this for initialization
	void Start () {
        MovementControllerScript[] allMcs = GameObject.FindObjectsOfType<MovementControllerScript>();
        movementControllers = new List<MovementControllerScript>();
        movementControllers.AddRange( allMcs);
      for (int i = 0; i < numberOfPlatformsToSpawn; i++){
            GameObject.Instantiate(platform);
            MovingPlatformScript mps = platform.GetComponent<MovingPlatformScript>();
            platform.transform.position = new Vector3(Random.Range(-100,100), Random.Range(-100,100), 0);
            mps.waypoints[0] = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
            mps.waypoints[1] = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        foreach (MovementControllerScript movementController in movementControllers)
        {
            if (movementController.controllerType == MovementControllerScript.ControllerType.BOX)
            {
                movementController.GetComponent<GravityScript>().MovementUpdate();
            }
            if (movementController.controllerType == MovementControllerScript.ControllerType.PLAYER)
            {
                movementController.playerScript.MovementUpdate();
            }

            if (movementController.controllerType == MovementControllerScript.ControllerType.MOVING_PLATFORM)
            {
                movementController.movingPlatformScript.MovementUpdate();
            }
        }

        foreach (MovementControllerScript movementController in movementControllers)
        {
            if (movementController.GetComponent<PassengerHandlerScript>() != null)
            {
                PassengerHandlerScript phs = movementController.GetComponent<PassengerHandlerScript>();
                phs.HandlePassengersRecursive();
            }
        }

        foreach (MovementControllerScript movementController in movementControllers)
        {
            movementController.MovementSetup();
        }
        //THIS CAN BE DONE MORE EFFICIENTLY??
        for (int i = 0; i < numberOfPasses; i++)
        {
            somethingMoved = false;

            foreach (MovementControllerScript movementController in movementControllers)
            {
                movementController.BaseMoveUpdate();
            }

            foreach (MovementControllerScript movementController in movementControllers)
            {
                movementController.BaseMoveUpdate();
            }


            foreach (MovementControllerScript movementController in movementControllers)
            {
                movementController.MovementUpdate();
            }
            if (somethingMoved == false)
            {
                //if nothing got moved, we're done!
          //      i = numberOfPasses;
            }
        }

        foreach (MovementControllerScript movementController in movementControllers)
        {
            if (movementController.GetComponent<PassengerHandlerScript>() != null)
            {
                movementController.GetComponent<PassengerHandlerScript>().MovementUpdateCleanupPass();
                if (movementController.GetComponent<PusherScript>() != null)
                {
                    movementController.GetComponent<PusherScript>().PushCleanup();
                }

            }
        }

        foreach (MovementControllerScript movementController in movementControllers)
        {
            movementController.MovementCleanup();
        }
    }

    void AddMovementController(MovementControllerScript movementController)
    {
        if (!movementControllers.Contains(movementController))
        {
            movementControllers.Add(movementController);
        }
    }

    void RemoveMovementController(MovementControllerScript movementController)
    {
        movementControllers.Remove(movementController);
    }
}