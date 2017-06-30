using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRandomlook : MonoBehaviour {




	// Unity Functions
	void Start () {
        transform.rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
	}
	
	
	void Update () {
		
	}

    //Custom Functions
}
