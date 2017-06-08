using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullerScript : MonoBehaviour {
    public bool amIPulling;
    public int pullerLevel;
    MovementControllerScript movementControllerScript;
    RaycasterScript raycasterScript;


    // Use this for initialization
    void Start()
    {
        movementControllerScript = GetComponent<MovementControllerScript>();
        raycasterScript = GetComponent<RaycasterScript>();
    }

    public void PullMovement()
    {
        if (amIPulling)
        {
            Debug.Log("am pulling");
            Vector2 pullDelta = new Vector2(movementControllerScript.velocity.x, 0);
            int cornerX = 0;
            if (pullDelta.x > 0)
            {
                cornerX = -1;
            }
            else if (pullDelta.x < 0)
            {
                cornerX = 1;
            }
            if (cornerX != 0)
            {
                List<GameObject> thingsToPull = new List<GameObject>();
                Vector2 rayOrigin = raycasterScript.raycastOrigins.getCorner(cornerX, -1);
                for (int i = 0; i < raycasterScript.horizontalRayCount; i++)
                {
                    rayOrigin += Vector2.up * (raycasterScript.horizontalRaySpacing * i);
                    RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, pullDelta*-1, Mathf.Abs(raycasterScript.skinWidth*2), raycasterScript.collisionLayer);

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
                                    if (ps.pushableLevel < pullerLevel)
                                    {
                                        if (thingsToPull.Contains(go) == false)
                                        {
                                            thingsToPull.Add(go);
                            
                                        }
                                    }
                                }
                            }
                        }
                    }
                }




                foreach (GameObject pullee in thingsToPull)
                {
                    Debug.Log("thingsToPull = " + pullee);
                    pullee.GetComponent<MovementControllerScript>().baseVelocity.x += (movementControllerScript.velocity.x*2);// + pullDelta.x);
                    if (Mathf.Sign(pullee.GetComponent<MovementControllerScript>().velocity.x) != Mathf.Sign(movementControllerScript.velocity.x))
                    {
                        pullee.GetComponent<MovementControllerScript>().velocity.x = 0;
                    }
                    pullee.GetComponent<PushableScript>().gettingPushed = false;
                    pullee.GetComponent<PushableScript>().gettingPulled = true;
                   pullee.GetComponent<PushableScript>().pullVector = new Vector2(movementControllerScript.velocity.x, 0);
                }
            }
        }
    }
}
