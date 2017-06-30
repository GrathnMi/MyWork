using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPartSpawner : MonoBehaviour {

    //set in inspector
    public GameObject __FloatingPartPrefab;
    private GameObject spawnedPart;

	// Unity functions
	void Start () {
        InvokeRepeating("SpawnFloatingPart", 1.0f, 5.0f);
	}
	
	void Update () {
		
	}

    //custom functions
    private void SpawnFloatingPart()
    {
        if(spawnedPart == null)
        {
            spawnedPart = (GameObject)Instantiate(__FloatingPartPrefab, transform.position, transform.rotation);

        }
    }
}
