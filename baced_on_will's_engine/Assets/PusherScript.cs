using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PusherScript : MonoBehaviour
{
    public bool amIPushing;
    public bool amIPulling;
    public int pusherLevel;
    MovementControllerScript movementControllerScript;
    RaycasterScript raycasterScript;


    // Use this for initialization
    void Start()
    {
        movementControllerScript = GetComponent<MovementControllerScript>();
        raycasterScript = GetComponent<RaycasterScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PushCleanup()
    {
        List<GameObject> thingsIAmHitting = new List<GameObject>();
        Vector2 amountLeftToMove = movementControllerScript.amountLeftToMove + movementControllerScript.baseAmountLeftToMove;
        if (Mathf.Abs(amountLeftToMove.x) + Mathf.Abs(amountLeftToMove.y) > .005)
        {
            //this checks against previous collision state because what if we just moved to touch this frame and didn't actually collide?
            // there is a frame where there can be a problem if the moving platform does its movement and doesn't find anything to push
            // then movement happens that brings them next to each other and then they can't move/push and this gets triggered.
            //maybe pushing should happen after all movement to fix the problem?  I doubt it though...
            if (amountLeftToMove.x < 0)
            {
                thingsIAmHitting.AddRange(raycasterScript.prevCollisionState.leftHits);
            }
            if (amountLeftToMove.x > 0)
            {
                thingsIAmHitting.AddRange(raycasterScript.prevCollisionState.rightHits);
            }

            if (amountLeftToMove.y > 0)
            {
                thingsIAmHitting.AddRange(raycasterScript.prevCollisionState.topHits);
            }

            if (amountLeftToMove.y < 0)
            {
                thingsIAmHitting.AddRange(raycasterScript.prevCollisionState.bottomHits);
            }

            int count = 0;

            List<GameObject> uniqueThings = thingsIAmHitting.Distinct<GameObject>().ToList<GameObject>();

            foreach (GameObject hitThing in uniqueThings)
            {
                if (hitThing != null)
                {
                    if (gameObject.GetComponent<MovingPlatformScript>() != null)
                    {
                        count++;
                        gameObject.GetComponent<MovingPlatformScript>().nextWaypoint();
                    }
                }
            }
        }
    }



    public void PushMovement()
    {
        if (amIPushing)
        {
            Vector2 pushDelta = new Vector2(movementControllerScript.velocity.x, 0);
            int cornerX = 0;
            if (pushDelta.x > 0)
            {
                cornerX = 1;
            }
            else if (pushDelta.x < 0)
            {
                cornerX = -1;
            }
            if (cornerX != 0)
            {
 
                List<GameObject> thingsToPush = new List<GameObject>();
                Vector2 rayOrigin = raycasterScript.raycastOrigins.getCorner(cornerX, -1);
                for (int i = 0; i < raycasterScript.horizontalRayCount; i++)
                {
                    rayOrigin += Vector2.up * (raycasterScript.horizontalRaySpacing * i);
                    RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, pushDelta, Mathf.Abs(pushDelta.x), raycasterScript.collisionLayer);

                    foreach (RaycastHit2D hit in hits)
                    {
                        if (!raycasterScript.gameObjectsIshouldIgnore.Contains(hit.transform.gameObject))
                        {
                            GameObject go = hit.transform.gameObject;
                            if (go != null)
                            {
                                PushableScript ps = go.GetComponent<PushableScript>();
                                if (ps != null)
                                {
                                    if (ps.pushableLevel < pusherLevel)
                                    {
                                        if (thingsToPush.Contains(go) == false)
                                        {
                                            thingsToPush.Add(go);
                                            pushDelta.x = Mathf.Sign(pushDelta.x) * (hit.distance-raycasterScript.skinWidth);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }




                foreach (GameObject pushee in thingsToPush)
                {
                    pushee.GetComponent<MovementControllerScript>().baseVelocity.x += (movementControllerScript.velocity.x - pushDelta.x);
                    if (Mathf.Sign(pushee.GetComponent<MovementControllerScript>().velocity.x) != Mathf.Sign(movementControllerScript.velocity.x))
                    {
                        pushee.GetComponent<MovementControllerScript>().velocity.x = 0;
                    }
                    pushee.GetComponent<PushableScript>().gettingPushed = true;
                    pushee.GetComponent<PushableScript>().pushVector = new Vector2(movementControllerScript.velocity.x, 0);
                }
            }
        }
    }
}
