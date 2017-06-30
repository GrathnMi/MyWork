using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour {

    //test script.
    //set in inspector
    public GameObject sausages;
    public float damage;
    public ShipData.ProjectileInfo.projTypes projType;
    

	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnMissile", 1.0f, 3.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void SpawnMissile()
    {
        WeaponManager.instance.FireProjectile(gameObject, gameObject.GetHashCode(), new ShipData.ProjectileInfo(projType, damage));
    }

}
