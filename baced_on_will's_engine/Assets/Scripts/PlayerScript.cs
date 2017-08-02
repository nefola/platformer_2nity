﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public bool pushPressed;
    PhysicsScript physicsScript;
    public float speed;
    public float jumpVelocity;
    public int jumps;
    public int framesSinceLastJump;
	public int framesSinceLastDash;
	public bool amDashing = false;
	public int beenDashing;
    public int jumpCooldown = 9;
	public int dashCooldown = 60;
    public int maxJumps = 2;
    public float speedLimit = 1.5f;
    int facing;
    GraberScript graberScript;
    MovementControllerScript movementControllerScript;
    SpriteRenderer sprite;
    // Use this for initialization

    void Start() {
        physicsScript = GetComponent<PhysicsScript>();
        sprite = GetComponent<SpriteRenderer>();
        graberScript = GetComponent<GraberScript>();
        movementControllerScript = GetComponent<MovementControllerScript>();
        facing = 1;
    }

    // Update is called once per frame
    void Update() {
        sprite.flipX = (facing == -1);
        framesSinceLastJump++;      
        PushMovement();
//        Debug.Log(framesSinceLastJump);
        if (physicsScript.onGround)
        {
            jumps = maxJumps;
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
            physicsScript.velocity.x = physicsScript.velocity.x - 0.05f;
        }
            if (!pushPressed)
        {
            facing = -1;
        }
    }
    public void right()
    {
        //physicsScript.velocity.x = speed;
        if (physicsScript.velocity.x < speedLimit)
        {
            physicsScript.velocity.x = physicsScript.velocity.x + 0.05f;
        }
        if (!pushPressed)
        {
            facing = 1;
        }
    }
	public void dash()
	{
		Debug.Log("try dash");
		if (framesSinceLastDash > dashCooldown && physicsScript.onGround) {
			Debug.Log("dash!");
			physicsScript.velocity.x = 1.6f * facing;
			amDashing = true;
			framesSinceLastDash = 0;
		}
	}
	public void power()
	{
		
	}
    public void jump()
    {
      // if (physicsScript.onGround && framesSinceLastJump > jumpCooldown)
       // {
       //     physicsScript.velocity.y = jumpVelocity;
         //   framesSinceLastJump = 0;
           // jumps = jumps -1;
       // }
//        else
 if (jumps > 0 && framesSinceLastJump > jumpCooldown)
        {
            physicsScript.velocity.y = jumpVelocity;
            framesSinceLastJump = 0;
            jumps = jumps -1;
        }
    }
    public void down()
    {

    }
}
