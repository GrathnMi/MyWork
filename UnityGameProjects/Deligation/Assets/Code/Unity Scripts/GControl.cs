using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GControl : MonoBehaviour {

    //Set in Inspector
    public GameObject shipPrefab;
    public GameObject partPrefab;
    public GameObject partSpawner;

    private GameObject currentPlayerShip;


    //Crappy singleton
    public static GControl instance;
    void Awake() { instance = this; }
	
	// Unity functions
	void Update () {
		
	}





    public void CreateTestShip(bool isPlayer)
    {
        if (isPlayer == true && currentPlayerShip != null)
        {
            Destroy(currentPlayerShip);
        }

        GameObject currentShip = (GameObject)Instantiate(shipPrefab, Vector3.zero, Quaternion.identity);

        if (isPlayer)
        {
            currentPlayerShip = currentShip;
        }
        else
        {
            currentShip.transform.position = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
            currentShip.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }
        
        ShipManager newShipManager = currentShip.GetComponent<ShipManager>();
        
        List<ShipData.Parts> newShipParts = new List<ShipData.Parts>();

        //add a shield
        ShipData.Parts shield = new ShipData.Parts(10, ShipData.Parts.partTypes.shield, 10, 100);
        shield.AssignPosition(new ShipData.intPos2(0, 0));
        newShipParts.Add(shield);

        //add an engine (south)
        ShipData.Parts engine = new ShipData.Parts(10, ShipData.Parts.partTypes.engine, 10, 100);
        engine.AssignPosition(new ShipData.intPos2(0, -1));
        newShipParts.Add(engine);

        //add a gun (north)
        ShipData.Parts weapon = new ShipData.Parts(10, ShipData.Parts.partTypes.weapon, 10, 100);
        weapon.AssignPosition(new ShipData.intPos2(0, 1));
        newShipParts.Add(weapon);
        
        newShipManager.SetupShip(isPlayer, newShipParts);
    }
    public void TestDamage()
    {
        if(currentPlayerShip != null)
            currentPlayerShip.GetComponent<ShipManager>().TakeDamage(5, Vector3.zero);
    }
    public void TestBuff()
    {
        if (currentPlayerShip != null)
        {
            currentPlayerShip.GetComponent<ShipManager>().AddBuffDebuff(
                new ShipData.BuffDebuff(ShipData.Parts.partTypes.engine, 6.0f, 2.0f, 10.0f, "test engine buff" ));
        }
            
    }
    public void TestAddPart()
    {
        if (currentPlayerShip != null)
        {
            currentPlayerShip.GetComponent<ShipManager>().AddParts(new ShipData.Parts(10, ShipData.Parts.RandomPartType(), 6, 100));
        }

    }
    public void TestAddPartSpawener()
    {
        Instantiate(
            partSpawner, 
            new Vector2(Random.Range(-100, 100), Random.Range(-100, 100)), 
            Quaternion.Euler( 0 , 0 , Random.Range(0,360) )
            );
    }
}
