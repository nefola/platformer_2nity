using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableScript : MonoBehaviour {

    public float grabableLevel;
    MovementControllerScript movementControllerScript;
    public void Start()
    {
        movementControllerScript = GetComponent<MovementControllerScript>();
    }

    public void Update()
    {
        movementControllerScript.FindPassengers();
    }
}
