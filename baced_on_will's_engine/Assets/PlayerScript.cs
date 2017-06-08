using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityScript))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(GravityScript))]
[RequireComponent(typeof(KeyboardInputScript))]
[RequireComponent(typeof(MovementControllerScript))]
[RequireComponent(typeof(RaycasterScript))]
[RequireComponent(typeof(PassengerHandlerScript))]

public class PlayerScript : MonoBehaviour
{


    new BoxCollider2D collider;
    GravityScript gravityScript;
    KeyboardInputScript keyboardInputScript;
    MovementControllerScript movementControllerScript;
    RaycasterScript raycasterScript;
    PassengerHandlerScript passengerHandlerScript;
    GrabHangAndClimbScript grabHangAndClimbsScript;

    public bool pushKeyPressed = false;
    public float horizontalAccelerationTimeAirborn = .2f;
    public float horizontalAccelerationTimeGrounded = .1f;


    public float jumpHeight;
    public float framesToJumpApex;
    public float horizontalFriction;
    public float horizontalSpeed;
    Vector2 targetVelocity;
    float velocityXSmoothing;
    PusherScript pusherScript;
    float jumpVelocity;

    // Use this for initialization
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        gravityScript = GetComponent<GravityScript>();
        keyboardInputScript = GetComponent<KeyboardInputScript>();
        movementControllerScript = GetComponent<MovementControllerScript>();
        raycasterScript = GetComponent<RaycasterScript>();
        passengerHandlerScript = GetComponent<PassengerHandlerScript>();
        pusherScript = GetComponent<PusherScript>();

        grabHangAndClimbsScript = GetComponent<GrabHangAndClimbScript>();

        setGravity();
    }

    void setGravity()
    {
        float g = -2 * jumpHeight / (framesToJumpApex * framesToJumpApex);
        gravityScript.gravity = new Vector2(0, g);
        jumpVelocity = Mathf.Abs(g * framesToJumpApex);
    }



    public void MovementUpdate()
    {

        if (movementControllerScript.isGrounded)
        {
            targetVelocity.x *= horizontalFriction;
        }

        movementControllerScript.velocity.x = Mathf.SmoothDamp(movementControllerScript.velocity.x, targetVelocity.x, ref velocityXSmoothing,
        movementControllerScript.isGrounded ? horizontalAccelerationTimeGrounded : horizontalAccelerationTimeAirborn);

        gravityScript.MovementUpdate();

        if (movementControllerScript.isGrounded)
        {
            targetVelocity = new Vector2(0, 0);
            if (pusherScript != null)
            {
                GetComponent<PusherScript>().amIPushing = pushKeyPressed;
                GetComponent<PullerScript>().amIPulling = pushKeyPressed;

                GetComponent<PusherScript>().PushMovement();
                GetComponent<PullerScript>().PullMovement();

            }
        }
        else
        {
            grabHangAndClimbsScript.MovementUpdate();
            if (grabHangAndClimbsScript.hangingState != GrabHangAndClimbScript.HangingState.NONE)
            {
                targetVelocity = new Vector2(0, 0);
            }
        }
    }

    public void leftPressed()
    {
        if (grabHangAndClimbsScript.hangingState == GrabHangAndClimbScript.HangingState.NONE)
        {
            targetVelocity.x = -horizontalSpeed;
        }
    }

    public void rightPressed()
    {
        if (grabHangAndClimbsScript.hangingState == GrabHangAndClimbScript.HangingState.NONE)
        {
            targetVelocity.x = horizontalSpeed;
        }
    }

    public void upPressed()
    {
        if (grabHangAndClimbsScript.hangingState == GrabHangAndClimbScript.HangingState.NONE)
        {

            if (movementControllerScript.isGrounded && movementControllerScript.isJumping == false)
            {
                movementControllerScript.velocity.y = jumpVelocity;
                movementControllerScript.isJumping = true;
                movementControllerScript.isGrounded = false;


                if (movementControllerScript.thingImStandingOn != null)
                {
                    movementControllerScript.velocity += movementControllerScript.thingImStandingOn.GetComponent<MovementControllerScript>().velocity;
                    targetVelocity.x = movementControllerScript.velocity.x;
                }
            }
        }
        else
        {
            grabHangAndClimbsScript.startClimbUp();
        }
    }

    public void downPressed()
    {
        if (grabHangAndClimbsScript.hangingState != GrabHangAndClimbScript.HangingState.NONE)
        {
            grabHangAndClimbsScript.drop();
        }else
        {
            grabHangAndClimbsScript.ClimbDown();
        }
        
    }
}
