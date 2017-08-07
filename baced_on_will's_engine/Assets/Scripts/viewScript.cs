using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class viewScript : MonoBehaviour {

	public int restingViewSize = 5;
	public float currentViewSize = 0;
	public bool lockCamera = false;
	float vel;
	PhysicsScript physicsScript;
	PlayerScript playerScript;
	public GameObject player;
    public GameObject menu;
    public Camera camera;
    public GameObject textGameObject;
    Text text;
	// Use this for initialization

	void Start() {

		playerScript = player.GetComponent<PlayerScript>();
		physicsScript = player.GetComponent<PhysicsScript>();
        text = textGameObject.GetComponent<Text>();
		camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update() {
		vel = Mathf.Abs (physicsScript.velocity.x);
		if (playerScript.amDashing == true) {
			lockCamera = true;
		} else 
		{
			lockCamera = false;
		}
		if (lockCamera == false) {
			currentViewSize = restingViewSize * (vel * 1.5f + 1f);
			camera.orthographicSize = currentViewSize;
		
		}
        text.text = (playerScript.health + "/" + playerScript.maxHealth + "\n" + playerScript.power + "/" + playerScript.maxPower);

	}
}
