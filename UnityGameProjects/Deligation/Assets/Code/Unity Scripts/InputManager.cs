using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {


    private ShipManager currentPlayerShipManager;

    // Unity functions
    //CrappySingleton
    public static InputManager instance;
    void Awake()
    {
        instance = this;
    }
	void Start () {
		
	}
	
	void Update () {

        //TODO: hovering over a UI should stop this.

        if (currentPlayerShipManager != null && Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.forward, 0);

            float dist;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out dist))
            {
                Vector3 point = ray.GetPoint(dist);
                Debug.DrawLine(Camera.main.transform.position, point);

                if(Vector3.Distance(currentPlayerShipManager.transform.position, point) < 15.0f)
                    currentPlayerShipManager.Movement(point);
            }

        }
	}

    //customfunctions
    public void NewPlayerShipSetup(ShipManager shipStartParts)
    {
        currentPlayerShipManager = shipStartParts;
    }
}
