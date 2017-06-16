using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraberScript : MonoBehaviour {

	  public int GraberLevel;
    GameObject thingIAmGrabing;
    List<GameObject> thingsIAmFacing;
    MovementControllerScript movementControllerScript;

    private void Start()
    {
        movementControllerScript = GetComponent<MovementControllerScript>();
    }

    public List<GameObject> WhatAmITouacing(float direction)
    {
        List<GameObject> objects = new List<GameObject>();
        List<RaycastHit2D> hits =  movementControllerScript.HorizontalRaycastHits(direction * .001f);
        hits.AddRange(movementControllerScript.VerticalRaycastHits(-0.001f));
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

    public void StartGrabing(List<GameObject> objects)
    {
        //        StopGrabing();
        if (thingIAmGrabing == null)
        {
            foreach (GameObject go in objects)
            {
                if (go.GetComponent<GrabableScript>() != null)
                {
                    if (!movementControllerScript.attachedObjects.Contains(go))
                    {
                        thingIAmGrabing = go;
                        movementControllerScript.attachedObjects.Add(go);
                        go.GetComponent<PhysicsScript>().affectedByGravity = false;
                        go.transform.position = transform.position;
                        break;
                    }
                }
            }
        }
    }

    public void StopGrabing()
    {
        movementControllerScript.attachedObjects.Remove(thingIAmGrabing);
        if(thingIAmGrabing != null){
            thingIAmGrabing.GetComponent<PhysicsScript>().affectedByGravity = true;
            thingIAmGrabing.transform.Translate(new Vector2(0, 2));
        }
        thingIAmGrabing = null;
    }
}
