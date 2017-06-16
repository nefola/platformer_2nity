using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public bool pushPressed;
    PhysicsScript physicsScript;
    public float speed;
    public float jumpVelocity;
    public int airJumps = 1;
    public int framesSinceLastJump;
    public int jumpCooldown;
    public int maxAirJumps = 2;
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
        physicsScript.velocity.x = -speed;
        if (!pushPressed)
        {
            facing = -1;
        }
    }
    public void right()
    {
        physicsScript.velocity.x = speed;
        if (!pushPressed)
        {
            facing = 1;
        }
    }
    public void up()
    {
        if (physicsScript.onGround && framesSinceLastJump > jumpCooldown)
        {
            physicsScript.velocity.y = jumpVelocity;
            framesSinceLastJump = 0;
            airJumps = maxAirJumps;
        }else if (airJumps > 0 && framesSinceLastJump > jumpCooldown)
        {
            physicsScript.velocity.y = jumpVelocity;
            framesSinceLastJump = 0;
            airJumps--;
        }
    }
    public void down()
    {

    }
}
