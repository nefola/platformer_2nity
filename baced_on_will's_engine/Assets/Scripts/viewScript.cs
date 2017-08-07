using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	// Use this for initialization

	void Start() {

		playerScript = player.GetComponent<PlayerScript>();
		physicsScript = player.GetComponent<PhysicsScript>();
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
			currentViewSize = restingViewSize * (vel + 1);
			camera.orthographicSize = currentViewSize;
		
		}
        

	}
}
