using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabHangAndClimbScript : MonoBehaviour {

    public HangingState hangingState;
    RaycasterScript raycasterScript;
    MovementControllerScript movementControllerScript;
    public float grabPointY;
    public float distanceAboveGrabPoint;
    int dropFrames = 0;
    int climbFrames = 0;
    public enum HangingState
    {
        HANGING,
        CLIMBING_UP,
        CLIMBING_DOWN,
        DROPPING,
        NONE
    }

    public void Start()
    {
        hangingState = HangingState.NONE;
        raycasterScript = GetComponent<RaycasterScript>();
        movementControllerScript = GetComponent<MovementControllerScript>();
    }

    public void MovementUpdate()
    {
        
        DetermineHangingState();
        if (hangingState == HangingState.HANGING)
        {
            movementControllerScript.velocity.y = 0;
        }else if (hangingState == HangingState.CLIMBING_UP)
        {
            climbUp();
        }
    }

    public void startClimbUp()
    {
        hangingState = HangingState.CLIMBING_UP;
    }

    public void ClimbDown()
    {

    }

    void climbUp()
    {
        climbFrames++;
        Vector2 facing = new Vector2(movementControllerScript.facing*.1f, 0);
        raycasterScript.HorizontalCollisions(ref facing);
        GameObject[] hits = null;
        bool anyHits = true;

        if (facing.x > 0)
        {
            hits = raycasterScript.collisionState.rightHits;
            anyHits = raycasterScript.collisionState.anyRightHits();
            Debug.Log("anyHits = " + anyHits);
        }else if (facing.x < 0)
        {
            hits = raycasterScript.collisionState.leftHits;
            anyHits = raycasterScript.collisionState.anyLeftHits();
        }

        if (anyHits)
        {
            Debug.Log("hits");
            movementControllerScript.velocity.y = .1f;
        }else
        {
            movementControllerScript.velocity.y = 0;
            movementControllerScript.velocity.x = facing.x;
            hangingState = HangingState.NONE;
        }




    }

    void DetermineHangingState()
    {
        if (hangingState == HangingState.NONE)
        {
            ShouldIGrab();
        }
        else if (hangingState == HangingState.DROPPING)
            {
                dropFrames++;
                if (dropFrames > 5)
                {
                    hangingState = HangingState.NONE;
                }
            }
        else if(hangingState == HangingState.HANGING)
        {
            movementControllerScript.velocity = new Vector2(0, 0);
        }
    }

    bool ShouldIGrab()
    {   if (movementControllerScript.velocity.y < 0)
        {
            Debug.Log("checking");
            Vector2 origin;

            if (movementControllerScript.facing == -1)
            {
                origin = raycasterScript.raycastOrigins.bottomLeft;
            }
            else
            {
                origin = raycasterScript.raycastOrigins.bottomRight;
            }

            origin += new Vector2(0, grabPointY);

            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, new Vector2(movementControllerScript.facing, 0),
            raycasterScript.skinWidth + .01f, raycasterScript.collisionLayer);

            Debug.DrawRay(origin, new Vector2(movementControllerScript.facing, 0), Color.blue) ;

            RaycastHit2D grabPointHit = new RaycastHit2D();
            grabPointHit.distance = Mathf.Infinity;

            foreach (RaycastHit2D checkHit in hits)
            {
                if (!raycasterScript.gameObjectsIshouldIgnore.Contains(checkHit.transform.gameObject))
                {
                    if (checkHit.distance < grabPointHit.distance)
                    {
                        grabPointHit = checkHit;
                    }
                }
            }

            if (grabPointHit.transform != null)
            {
                Debug.Log("grabHit is good");
                origin += new Vector2(0, distanceAboveGrabPoint);

                hits = Physics2D.RaycastAll(origin, new Vector2(movementControllerScript.facing, 0),
                raycasterScript.skinWidth + .001f, raycasterScript.collisionLayer);
                Debug.DrawRay(origin, new Vector2(movementControllerScript.facing, 0), Color.yellow);

                RaycastHit2D hitAbove = new RaycastHit2D();
                hitAbove.distance = Mathf.Infinity;
                bool canIGrab = true;
                foreach (RaycastHit2D checkHit in hits)
                {
                    if (!raycasterScript.gameObjectsIshouldIgnore.Contains(checkHit.transform.gameObject))
                    {
                        Debug.Log("hit = " + checkHit.transform.gameObject);
                        canIGrab = false;
                        break;
                    }
                }

                if (canIGrab)
                {
                    hangingState = HangingState.HANGING;
                    movementControllerScript.velocity.y = 0;
                    movementControllerScript.velocity.x = 0;
                    movementControllerScript.amountLeftToMove = new Vector2(0, 0);
                    return true;
                }
            }
        }
        hangingState = HangingState.NONE;
        return false;
    }

    public void drop()
    {
        hangingState = HangingState.DROPPING;
        dropFrames = 0;
    }


}
