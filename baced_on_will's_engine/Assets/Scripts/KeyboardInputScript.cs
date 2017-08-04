using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputScript : MonoBehaviour {

    public List<KeyCode> jumpKeys;
    public List<KeyCode> downKeys;
	public List<KeyCode> flutterKeys;
    public List<KeyCode> leftKeys;
    public List<KeyCode> rightKeys;
    public List<KeyCode> pushKeys;
	public List<KeyCode> dashKeys;
	public List<KeyCode> powerKeys;

    PlayerScript playerScript;

    // Use this for initialization
    void Start () {
        playerScript = GetComponent<PlayerScript>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		foreach (KeyCode code in jumpKeys)
        {
            if (Input.GetKey(code))
            {
                playerScript.jump();
                break;
            }
        }
        playerScript.flutteringHeld = false;
        foreach (KeyCode code in flutterKeys)
        {
            if (Input.GetKey(code) ) {
                playerScript.flutteringHeld = true;
                break;
            }
        }
		foreach (KeyCode code in dashKeys)
		{
			if (Input.GetKey(code))
			{
				playerScript.dash();
				break;
			}
		}
		foreach (KeyCode code in powerKeys)
		{
			if (Input.GetKey(code))
			{
				playerScript.power();
				break;
			}
		}
        foreach (KeyCode code in leftKeys)
        {
            if (Input.GetKey(code))
            {
                playerScript.left();
                break;
            }
        }
        foreach (KeyCode code in downKeys)
        {
            if (Input.GetKey(code))
            {
                playerScript.down();
                break;
            }
        }
        foreach (KeyCode code in rightKeys)
        {
            if (Input.GetKey(code))
            {
                playerScript.right();
                break;
            }
        }
        playerScript.pushPressed = false;
        foreach (KeyCode code in pushKeys)
        {
            if (Input.GetKey(code))
            {
                playerScript.pushPressed = true;
                break;
            }
        }
	}
}
