using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipNPC : MonoBehaviour {

    private ShipManager ship;
    private Vector2 startPos;
    private List<GameObject> _otherShips;

	// Unity fucntions
	void Start () {
        ship = GetComponent<ShipManager>();
        startPos = new Vector2( transform.position.x, transform.position.y - 2);
        _otherShips = ship.GetOtherShips;
	}
	
	void Update () {

        //Hunt other ship
        if (_otherShips.Count > 0)
        {
            ship.Movement(_otherShips[0].transform.position);

        }
        else  //no other ships
        {
            ship.Movement(startPos);
        }
        


        





	}


}
