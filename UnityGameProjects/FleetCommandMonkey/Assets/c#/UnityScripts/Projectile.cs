using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private ShipControl2 targetSC;
    private int targetNID;
    
    
	
	//Unity functions
	void Update ()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, target.position, StartingVariables.projectileSpeed * Time.deltaTime);
        transform.LookAt(target);
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            OnTargetRemoved();
        }
    }
    

    //custom functions
    public void PooledObject_Setup(ShipControl2 _target)
    {
        target = _target.transform;
        targetSC = _target;
        targetNID = _target.shipNID;

        targetSC.CB_RegesterOnShipRemoval(OnTargetRemoved);
    }
    public void PooledObject_Remove()
    {
        gameObject.SetActive(false);
    }

    public void OnTargetRemoved()
    {

        targetSC.CB_UnregesterOnShipRemoval(OnTargetRemoved);
        PooledObject_Remove();
    }
    

}
