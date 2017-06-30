using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconectedPart : MonoBehaviour {


    private ShipData.Parts part;




	// Unity functions
	void Awake ()
    {
        part = new ShipData.Parts(10, ShipData.Parts.RandomPartType(), 10, 100);
	}
	
	void Update ()
    {
		
	}

    //Custom Functions

    public ShipData.Parts GetPart()
    {
        return part;
    }
}
