using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameFX : MonoBehaviour {



    //set in inspector
    public GameObject ProjectilePrefab;
    public GameObject ShipExplosionPrefab;

    //Var
    private List<GameObject> projPoolList = new List<GameObject>();
    private List<GameObject> ShipExplosionPoolList = new List<GameObject>();


    private GameControl GC;

    //Singleton
    public static GameFX Instance;
    void Awake()
    { Instance = this; }

    void Start () {
        GC = GameControl.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    //custom functions
    public void ProjectileVisuels(ShipControl2 shipFired, ShipControl2 targetShip)
    {
        if (shipFired == null || targetShip == null)
        {
            Debug.Log("(cosmetic) Cant Fire as one of the ships is destroyed.");
            return;
        }

        GameObject newProj = ProjectilePool();
        newProj.transform.position = shipFired.transform.position;
        newProj.GetComponent<Projectile>().PooledObject_Setup(targetShip);
    }
    private GameObject ProjectilePool()
    {
        foreach (GameObject obj in projPoolList)
        {
            if (!obj.activeInHierarchy)
            {
                //Debug.Log("Activating object: " + obj.name);
                obj.SetActive(true);
                return obj;
            }
        }
        GameObject newProj = (GameObject)Instantiate(ProjectilePrefab);
        newProj.name = "ProjPoolObject " + projPoolList.Count.ToString();
        projPoolList.Add(newProj);

        //Set parant to keep the hierarchy clean.
        newProj.transform.SetParent(transform);

        return newProj;
    }
    public void ShipExplosionVisuels(Vector2 explosionPos)
    {

        GameObject newShipExplosion = ShipExplosionPool();
        newShipExplosion.transform.position = explosionPos;
        newShipExplosion.GetComponent<FX_ShipExplosion>().PooledObject_Setup(0.75f);
    }
    private GameObject ShipExplosionPool()
    {
        foreach (GameObject obj in ShipExplosionPoolList)
        {
            if (!obj.activeInHierarchy)
            {
                //Debug.Log("Activating object: " + obj.name);
                obj.SetActive(true);
                return obj;
            }
        }
        GameObject newShipExplosion = (GameObject)Instantiate(ShipExplosionPrefab);
        newShipExplosion.name = "ShipExplosionPoolObject " + ShipExplosionPoolList.Count.ToString();
        ShipExplosionPoolList.Add(newShipExplosion);

        //Set parant to keep the hierarchy clean.
        newShipExplosion.transform.SetParent(transform);

        return newShipExplosion;
    }
}
