using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public bool pushPressed;
    PhysicsScript physicsScript;
    public float speed;
    public float jumpVelocity;
    public int jumps;
	public int flutters;
    public int framesSinceLastJump;
	public int framesSinceLastDash;
	public int framesframesSinceLastFlutter;
	public bool amFluttering = false;
	public bool amDashing = false;
	public bool attemptFluttering = false;
	public int beenDashing;
	public int beenFluttering;
    public int jumpCooldown = 9;
	public int dashCooldown = 60;
    public int maxJumps = 2;
	public int maxFlutters = 1;
    public float speedLimit = 1.5f;
    int facing;
    GraberScript graberScript;
    MovementControllerScript movementControllerScript;
    SpriteRenderer sprite;
	KeyboardInputScript inputScript;
    // Use this for initialization

    void Start() {
        physicsScript = GetComponent<PhysicsScript>();
        sprite = GetComponent<SpriteRenderer>();
        graberScript = GetComponent<GraberScript>();
        movementControllerScript = GetComponent<MovementControllerScript>();
		inputScript = GetComponent<KeyboardInputScript>();
        facing = 1;
    }

    // Update is called once per frame
    void Update() {
        sprite.flipX = (facing == -1); //very smart
        framesSinceLastJump++;      
        PushMovement();
//        Debug.Log(framesSinceLastJump);
        if (physicsScript.onGround)
        {
            jumps = maxJumps;
			flutters = maxFlutters;
        }
		if (amDashing == true) {
			physicsScript.affectedByFriction = false;
			physicsScript.affectedByGravity = false;
			beenDashing++;
			if (beenDashing > 10) {
				physicsScript.affectedByFriction = true;
				physicsScript.affectedByGravity = true;
				amDashing = false;
			}
		} else {
			framesSinceLastDash++;
		}
		flutter();
		Debug.Log (attemptFluttering);
		Debug.Log (amFluttering);
        //speed = speed * 0.0000000007f; //this creates friction
    }

    public void PushMovement()
    {
        if (pushPressed)
        {
            List<GameObject> thingsIAmFacing = graberScript.WhatAmITouacing(facing);
            graberScript.StartGrabing(thingsIAmFacing);
        }
        else
        {
            graberScript.StopGrabing();
        }

    }

    public void left()
    {
        //physicsScript.velocity.x = -speed;
        if (physicsScript.velocity.x > -speedLimit)
        {
            physicsScript.velocity.x = physicsScript.velocity.x - 0.025f;
        }
           
            facing = -1;
    }
    public void right()
    {
        //physicsScript.velocity.x = speed;
        if (physicsScript.velocity.x < speedLimit)
        {
            physicsScript.velocity.x = physicsScript.velocity.x + 0.025f;
        }
            facing = 1;
    }
	public void dash()
	{
		Debug.Log("try dash");
		if (framesSinceLastDash > dashCooldown && physicsScript.onGround) {
			Debug.Log("dash!");
			amDashing = true;
			physicsScript.velocity.x = 1.6f * facing;
			framesSinceLastDash = 0;
			beenDashing = 0;
		}
	}
	public void power()
	{
		
	}
	public void flutter()
	{
		if (physicsScript.onGround == false && attemptFluttering == true && flutters > 0) {
			amFluttering = true;
			flutters--;
		}
		if (amFluttering == true){
		physicsScript.affectedByGravity = false;
			beenFluttering++;
			physicsScript.velocity.y = 0;
			if (beenFluttering > 60 || attemptFluttering == false) {
				amFluttering = false;
				physicsScript.affectedByGravity = true;

		} 
	}
	}
    public void jump()
    {
 if (jumps > 0 && framesSinceLastJump > jumpCooldown)
        {
            physicsScript.velocity.y = jumpVelocity;
            framesSinceLastJump = 0;
            jumps = jumps -1;
        }
    }
    public void down()
    {
		physicsScript.velocity.y -= 0.03f;
    }
}
