using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_ShipExplosion : MonoBehaviour {


    public void PooledObject_Setup(float delay)
    {

        Invoke("PooledObject_Remove", delay);
        

    }

    public void PooledObject_Remove()
    {
        gameObject.transform.gameObject.SetActive(false);
        
    }





}
