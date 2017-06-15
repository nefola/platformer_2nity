using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableScript : MonoBehaviour {

    public float pushableLevel;
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
