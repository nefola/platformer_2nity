using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KeyboardInputScript : MonoBehaviour {
    PlayerScript playerScript;

    int i = 0;

    KeyCode leftKey = KeyCode.A;
    KeyCode rightKey = KeyCode.D;
    KeyCode upKey = KeyCode.W;
    KeyCode downKey = KeyCode.S;
    KeyCode pushKey = KeyCode.LeftShift;

	// Use this for initialization
	void Start () {
        playerScript = GetComponent<PlayerScript>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        i++;
		if (Input.GetKey(leftKey))
        {
            playerScript.leftPressed();
        }
        if (Input.GetKey(rightKey))
        {
            playerScript.rightPressed();
        }
        if (Input.GetKey(upKey))
        {
            playerScript.upPressed();
        }
        if (Input.GetKey(downKey))
        {
            playerScript.downPressed();
        }

        playerScript.pushKeyPressed = Input.GetKey(pushKey);
        if (i == 25)
        {
            playerScript.downPressed();
//            playerScript.pushKeyPressed = true;

        }
    }
}