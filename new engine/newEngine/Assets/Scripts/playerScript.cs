using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (controller2D))]
public class playerScript : MonoBehaviour {

    controller2D controller;
	
	void Start () {

        controller = GetComponent<controller2D>();
	}

}
