using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class ShipControl2 : NetworkBehaviour {


    //a network position covers updating it to cliants
    [SyncVar(hook = "CliantOnCliantNIDChange")]
    public string cliantNID;
    [SyncVar(hook = "CliantOnShipNIDChange")]
    public int shipNID;
    [SyncVar(hook = "CliantOnColourChange")]
    public Color colour;
    [SyncVar(hook = "CliantOnHitPountChange")]
    public int hitPoints;
    [SyncVar(hook = "CliantOnShieldChange")]
    public int shieldHitPoints;
    [SyncVar]
    public Vector3 shipSpeed;

    //set in inspector
    public Renderer spriteRenderer;
    public GameObject shipShieldObj;
    
    

    //priv var
    private GameControl GC;
    private Quaternion rotateTowards;
    private bool hasTarget = false;
    private int targetNID = 0;
    private ShipControl2 currentTargetSC;
    private float weaponCooldown;
    private Action OnShipRemoval;

    void Awake()
    {
        GC = GameControl.Instance;
        if (GC == null)
            Debug.LogError("No gamecontrol! how can this happen?!");
    }

    void Update ()
    {
        if (!isServer)  //should only be on the server
            return;

        //"shipSpeed" and "rotateTowards" are both controlled by a the AI 4 times a second. Update impliments and smooths.
        transform.Translate(shipSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTowards, StartingVariables.shipTurnSpeed);
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        //Has a target, no need to look for another
        if (hasTarget)
        {
            //removing a target is handled in the AI
            return;
        }

        //No target; look for ship.
        //is "other" a ship?
        ShipControl2 otherShip = other.GetComponent<ShipControl2>();
        if (otherShip == null)
        {
            Debug.LogError("Detecting a something that wasnt a ship");
            return;
        }
        //is it on my team??
        if (otherShip.cliantNID != cliantNID)
        {
            currentTargetSC = otherShip;
            targetNID = otherShip.shipNID;
            hasTarget = true;
            Debug.DrawLine(transform.position, otherShip.transform.position, Color.green);
        }
    }

    //custom functions
    //Server functions
    [Server]
    public void StartAI()
    {
        //reset SERVER values here
        CircleCollider2D circleCol2D = gameObject.GetComponent<CircleCollider2D>();
        circleCol2D.enabled = true;
        circleCol2D.radius = StartingVariables.shipDetectionRadius;
        weaponCooldown = StartingVariables.weaponFireRate;
        DeselectTarget();

        InvokeRepeating("ThisShipAI", StartingVariables.aiUpdateTicks, StartingVariables.aiUpdateTicks);
    }
    //used by the shipPool, called from the CustomSpawnManager.
    public void OnUnspawn()
    {
        //Reset cliant values here
        shipShieldObj.SetActive(true);
        GC.UnregesterShipNID(shipNID);
        GC.DestroyedShipCounter(cliantNID);
        
    }
    //invoked from "StartAI", Cancelled from "RemoveShip".
    [Server]
    private void ThisShipAI()
    {
        weaponCooldown -= StartingVariables.aiUpdateTicks;
        
        if (hasTarget)                                    //do I have a target?
        {
            Vector3 relPosition = currentTargetSC.transform.position - transform.position;

            //has this target been destroyed? (returned to pool)
            if (!currentTargetSC.gameObject.activeSelf || targetNID != currentTargetSC.shipNID)
            {
                DeselectTarget();
                return;
            }
            

            //InCombat
            //FireWeapon
            if (weaponCooldown <= 0.0f)
            {

                float delay = Vector3.Distance(transform.position, currentTargetSC.transform.position) / StartingVariables.projectileSpeed;

                //is the target to far away?
                if (delay > StartingVariables.projectileTravelTime)
                {
                    DeselectTarget();
                    return;
                }
                
                //Debug.Log(Vector3.Angle(relPosition, transform.up).ToString());

                //is the target in weapon fire arc?
                if(Vector3.Angle(relPosition, transform.up) < StartingVariables.weaponFireArc)
                {

                    Debug.DrawLine(transform.position, currentTargetSC.transform.position, Color.green);


                    GC.FireProjectile(this, currentTargetSC, StartingVariables.shipDamage, delay);

                    weaponCooldown = StartingVariables.weaponFireRate;

                }
            }

            

            //Movement
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
    [Server]
    private void DeselectTarget()
    {
        currentTargetSC = null;
        targetNID = 0;
        hasTarget = false;

        Debug.DrawLine(transform.position, Vector3.zero, Color.red);
    }
    //Damage
    [Server]
    public void TakeDelayedDamage(int damage, float delay)
    {
        StartCoroutine(TakeDamageDelay(damage, delay));
    }
    [Server]
    private IEnumerator TakeDamageDelay(int damage, float delay)
    {
        //Debug.Log("delayed damage");
        yield return new WaitForSeconds(delay);
        //Debug.Log("dealing delayed damage");
        TakeDamage(damage);
    }
    [Server]
    private void TakeDamage(int damage)
    {
        //Shield Damage
        if (shieldHitPoints > 0)                             //check the shield is active
        {
            shieldHitPoints -= damage;                       //Deal Damage to shield
            //Debug.Log(shield.ToString());
            if (shieldHitPoints <= 0)                        //is shield still active?
            {
                shipShieldObj.SetActive(false);
                StartCoroutine(ShieldActivaion(StartingVariables.shipShieldReactivationTime));
            }
            return;
        }
        //Ship HP Damage
        hitPoints -= damage;
        if (hitPoints <= 0)
            RemoveShip();
    }
    [Server]
    private IEnumerator ShieldActivaion(float delay)
    {
        yield return new WaitForSeconds(delay);

        if(isServer)
            shieldHitPoints = StartingVariables.shipShieldHP;

        shipShieldObj.SetActive(true);
    }
    [Server]
    private void RemoveShip()
    {
        StopAllCoroutines();
        CancelInvoke("ThisShipAI");

        if(OnShipRemoval != null)
            OnShipRemoval();

        CustomSpawnManager.Instance.UnSpawnObject(gameObject);
        NetworkServer.UnSpawn(gameObject);
    }
    //cliant functions (hooks)
    private void CliantOnHitPountChange(int shipHP)
    {
        hitPoints = shipHP;
    }
    private void CliantOnShieldChange(int shieldHP)
    {
        shieldHitPoints = shieldHP;
        if (shieldHitPoints <= 0)                        //is shield still active?
        {
            shipShieldObj.SetActive(false);
            StartCoroutine(ShieldActivaion(StartingVariables.shipShieldReactivationTime));
        }
    }
    private void CliantOnCliantNIDChange(string _cliantNID)
    {
        cliantNID = _cliantNID;
    }
    private void CliantOnShipNIDChange(int _shipNID)
    {
        //unregester last shipNID
        //Debug.Log("Unregerstring: " + shipNID.ToString());
        GC.UnregesterShipNID(shipNID);

        //Assign new shipNID
        shipNID = _shipNID;

        //Reregester new shipNID
        //Debug.Log("Regerstring: " + shipNID.ToString());
        GC.RegesterShip(shipNID, this);
    }
    private void CliantOnColourChange(Color _colour)
    {
        colour = _colour;
        spriteRenderer.material.color = colour;
    }
    

    //Callbacks
    public void CB_RegesterOnShipRemoval(Action callback)
    {
        OnShipRemoval += callback;

    }
    public void CB_UnregesterOnShipRemoval(Action callback)
    {
        OnShipRemoval -= callback;
    }




}
