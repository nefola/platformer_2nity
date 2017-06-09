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
    KeyCode leftKey2 = KeyCode.LeftArrow;
    KeyCode rightKey2 = KeyCode.RightArrow;
    KeyCode upKey2 = KeyCode.UpArrow;
    KeyCode downKey2 = KeyCode.DownArrow;
    KeyCode pushKey = KeyCode.LeftShift;

	// Use this for initialization
	void Start () {
        playerScript = GetComponent<PlayerScript>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        i++;
		if (Input.GetKey(leftKey) || Input.GetKey(leftKey2))
        {
            playerScript.leftPressed();
        }
        if (Input.GetKey(rightKey) || Input.GetKey(rightKey2))
        {
            playerScript.rightPressed();
        }
        if (Input.GetKey(upKey) || Input.GetKey(upKey2))
        {
            playerScript.upPressed();
        }
        if (Input.GetKey(downKey) || Input.GetKey(downKey2))
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