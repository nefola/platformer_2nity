using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PusherScript : MonoBehaviour {
    public int pusherLevel;
    GameObject thingIAmPushing;
    List<GameObject> thingsIAmFacing;
    MovementControllerScript movementControllerScript;

    private void Start()
    {
        movementControllerScript = GetComponent<MovementControllerScript>();
    }

    public List<GameObject> WhatAmIFacing(float direction)
    {
        List<GameObject> objects = new List<GameObject>();
        List<RaycastHit2D> hits =  movementControllerScript.HorizontalRaycastHits(direction * .001f);
        foreach (RaycastHit2D hit in hits) {
            if (hit.transform != null)
            {
                if (!objects.Contains(hit.transform.gameObject))
                {
                    objects.Add(hit.transform.gameObject);
                }
            }
        }
        return objects;
    }

    public void StartPushing(List<GameObject> objects)
    {
        StopPushing();
        foreach (GameObject go in objects)
        {
            if (go.GetComponent<PushableScript>() != null)
            {
                if (!movementControllerScript.attachedObjects.Contains(go))
                {
                    thingIAmPushing = go;
                    movementControllerScript.attachedObjects.Add(go);
                }
            }
        }
    }

    public void StopPushing()
    {
        movementControllerScript.attachedObjects.Remove(thingIAmPushing);
        thingIAmPushing = null;
    }
}
 