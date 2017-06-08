using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableScript : MonoBehaviour {
    public int pushableLevel;
    public bool gettingPushed;
    public bool gettingPulled;
    public Vector2 pushVector;
    public Vector2 pullVector;
    MovementControllerScript movementController;
	// Use this for initialization
	void Start () {
        pushVector = new Vector2(0, 0);
        movementController = GetComponent<MovementControllerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void LateUpdate()
    {
        if (gettingPushed)
        {
            gettingPushed = false;
            if (pushVector.x != 0)
            {

                Debug.Log("movementController.totalBaseMoveThisFrame.x = " + movementController.totalBaseMoveThisFrame.x);
                Debug.Log("movementController.totalMoveThisFrame.x = " + movementController.totalMovedThisFrame.x);
                if (Mathf.Sign(movementController.totalBaseMoveThisFrame.x + movementController.totalMovedThisFrame.x) == Mathf.Sign(pushVector.x))
                {
                    pushVector.x = -1 * movementController.totalBaseMoveThisFrame.x;
                    movementController.Move(pushVector);
                }
            }
        }else
        {
            if (gettingPulled)
            {
                movementController.Move(pullVector);
            }
        }
        pullVector = new Vector2(0, 0);
        pushVector = new Vector2(0, 0);
    }
}
