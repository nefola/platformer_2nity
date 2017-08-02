using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class viewScript : MonoBehaviour {

	public int restingViewSize = 5;
	public float currentViewSize = 0;
	float vel;
	PhysicsScript physicsScript;
	public GameObject player;
	public Camera camera;
	// Use this for initialization

	void Start() {

		physicsScript = player.GetComponent<PhysicsScript>();
		camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update() {
		vel = Mathf.Abs (physicsScript.velocity.x);

		currentViewSize = restingViewSize * (vel + 1);
		camera.orthographicSize = currentViewSize;
		
	}
}
