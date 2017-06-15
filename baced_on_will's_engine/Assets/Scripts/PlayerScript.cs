using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public bool pushPressed;
    PhysicsScript physicsScript;
    public float speed;
    public float jumpVelocity;
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
        if (physicsScript.onGround)
        {
            physicsScript.velocity.y = jumpVelocity;
        }
    }
    public void down()
    {

    }
}
