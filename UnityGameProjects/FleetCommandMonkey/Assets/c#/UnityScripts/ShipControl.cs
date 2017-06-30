using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ShipControl : NetworkBehaviour
{
    /* archived
    //a network position covers updating it to cliants
    [SyncVar]
    public string ownedByPlayerNID;

    [SyncVar(hook = "CliantCheckHP")]
    public int shipHP;

    [SyncVar(hook = "CliantCheckShield")]
    public int shipShieldHP;

    [SyncVar]
    public Vector3 shipSpeed;

    public string shipNID = "";


    //set in inspector
    public Renderer thisRenderer;
    public GameObject shipShieldObj;
    public GameObject ShipExplosionPrefab;


    //priv var
    private GameControl GC;
    private Quaternion rotateTowards;
    private ShipControl currentTargetSC;

    // UnityFunctions
    private void Awake ()
    {
        GC = GameControl.Instance;

        if (isServer)
        {
            CircleCollider2D circleCol2D = gameObject.GetComponent<CircleCollider2D>();
            circleCol2D.enabled = true;
            circleCol2D.radius = StartingVariables.shipDetectionRadius;
        }
    }
    private void OnEnable()
    {
        shipNID = GetComponent<NetworkIdentity>().netId.ToString();
        //GC.RegesterShip(shipNID, this);
        thisRenderer.material.color = GC.GetPlayerColour(ownedByPlayerNID);
        if (isServer)
        {
            InvokeRepeating("ThisShipAI", 0.25f, 0.25f);
        }
        Debug.Log("ping");

    }
    private void OnDisable()
    {
        GC.RemoveShip(GetComponent<NetworkIdentity>().netId.ToString(), transform.position, transform.rotation);
        Debug.Log("Unping");
    }

    void Update ()
    {
        if (!isServer)  //should only be on the server
        {
            return;
        }
        transform.Translate(shipSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTowards, StartingVariables.shipTurnSpeed);
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        //Has a target, no need to look for another
        if (currentTargetSC == null || !currentTargetSC.gameObject.activeSelf)
            currentTargetSC = null;
        if (currentTargetSC != null)
            return;
                
        //is it a ship?
        ShipControl otherShip = other.GetComponent<ShipControl>();
        if (otherShip == null) {
            Debug.LogError("Detecting a something that wasnt a ship");
            return;
        }
        //is it on my team??
        if (otherShip.ownedByPlayerNID != ownedByPlayerNID)
        {
            currentTargetSC = otherShip;
        }
    }

    //Custom functions
    //AI
    private void ThisShipAI()
    {
        //Debug.Log("I am Garry, the AI");
        if (currentTargetSC != null)                                    //do I have a target?
        {
            //InCombat
            //FireWeapon
            float delay = Vector3.Distance(transform.position, currentTargetSC.transform.position) / StartingVariables.projectileSpeed;
            GC.FireProjectile(this, currentTargetSC, StartingVariables.shipDamage, delay);
            
            //Movement
            Vector3 relPosition = currentTargetSC.transform.position - transform.position;
            relPosition.Normalize();
            float zAngle = Mathf.Atan2(relPosition.y, relPosition.x) * Mathf.Rad2Deg - 90.0f;
            rotateTowards = Quaternion.Euler(0.0f, 0.0f, zAngle);
        }
        else    //Out of Combat
        {
            //Debug.Log("No target");
            Vector3 relPosition = Vector3.zero - transform.position;
            relPosition.Normalize();
            float zAngle = Mathf.Atan2(relPosition.y, relPosition.x) * Mathf.Rad2Deg - 90.0f;
            rotateTowards = Quaternion.Euler(0.0f, 0.0f, zAngle);
        }
    }

    //Damage
    public void TakeDelayedDamage(int damage, float delay)
    {
        StartCoroutine(TakeDamageDelay(damage, delay));
    }
    private IEnumerator TakeDamageDelay(int damage, float delay)
    {
        //Debug.Log("delayed damage");
        yield return new WaitForSeconds(delay);
        //Debug.Log("dealing delayed damage");
        TakeDamage(damage);
    }
    private void TakeDamage(int damage)
    {
        //Shield Damage
        if (shipShieldHP > 0)                             //check the shield is active
        {
            shipShieldHP -= damage;                       //Deal Damage to shield
            //Debug.Log(shield.ToString());
            if (shipShieldHP <= 0)                        //is shield still active?
            {
                shipShieldObj.SetActive(false);
                StartCoroutine(ShieldActivaion(StartingVariables.shipShieldReactivationTime));
            }
            return;
        }
        //Ship HP Damage
        shipHP -= damage;
        if (shipHP <= 0)
            RemoveShip();
    }
    IEnumerator ShieldActivaion(float delay)
    {
        yield return new WaitForSeconds(delay);
        shipShieldHP = StartingVariables.shipShieldHP;
        shipShieldObj.SetActive(true);
    }
    private void CliantCheckHP(int shipHP)
    {
        if(shipHP <= 0.0f)
            Instantiate(ShipExplosionPrefab, transform.position, transform.rotation);
    }


    private void RemoveShip()
    {
        StopAllCoroutines();
        CancelInvoke("ThisShipAI");

        CustomSpawnManager.Instance.UnSpawnObject(gameObject);
        NetworkServer.UnSpawn(gameObject);
    }
    */
}
