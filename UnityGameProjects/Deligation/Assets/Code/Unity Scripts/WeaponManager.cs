using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

    //set in inspector
    public GameObject __projectilePrefab;


    public static WeaponManager instance;
    void Awake()
    {
        instance = this;
    }
    // Unity funtions
    void Start () {
		
	}
	
	void Update () {
		
	}


    //custom
    public void FireProjectile(GameObject firedFrom, int firedfromIdentifyer, ShipData.ProjectileInfo proj, GameObject target = null)
    {
        GameObject newObj = Instantiate(__projectilePrefab, firedFrom.transform.position, firedFrom.transform.rotation);
        newObj.GetComponent<Projectile>().Setup(firedfromIdentifyer, proj, target);
    }


}
