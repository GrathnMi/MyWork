using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manages the unity/gameengine aspects of the shipdata.
/// </summary>

public class ShipManager : MonoBehaviour
{
    //set in inspector
    public float __partSize = 3.1f;

    private Vector2 centureOfMass;
    private Rigidbody2D rb2D;
    private ShipData.Ship _ship;
    private int _thisShipHash;
    protected Dictionary<ShipData.Parts, GameObject> _partVisuels = new Dictionary<ShipData.Parts, GameObject>();
    
    private List<GameObject> _otherShips = new List<GameObject>();
    public List<GameObject> GetOtherShips { get { return _otherShips; } }

    //          Buffs and debuff stuff:
    //manages if there is any "buffs" or "debuffs"
    private List<ShipData.BuffDebuff> buffDebuff = new List<ShipData.BuffDebuff>();
    //Works as the final ship stats, basicly being BasicStats * buffDebuff = finalStats
    private Dictionary<ShipData.Parts.partTypes, float> _finalStats;
    private float buffdebuffLoopTime = 0.5f;



    // Unity Functions
    void Start () {
        _thisShipHash = gameObject.GetHashCode();
        InvokeRepeating("BuffDebuffLoop", 1, buffdebuffLoopTime);
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        InvokeRepeating("FireWeapons", 1, 1);
    }
	
	void Update () {
        
	}
    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(_partVisuels.Count.ToString());
        //TODO: optimize

        //OtherShip?
        if (other.CompareTag("ShipDetector"))
        {
            //Debug.Log("other ship detected");
            _otherShips.Add(other.gameObject);
            return;
        }
        //Projectile?
        Projectile otherProj = other.GetComponent<Projectile>();
        if (otherProj != null && otherProj.firedFrom != _thisShipHash)        
        {
            //WE HAVE BEEN HIT!

            //find closest object to hitpoint
            float dist = Mathf.Infinity;
            ShipData.Parts part = null;
            foreach (ShipData.Parts aPart in _partVisuels.Keys)
            {
                float tempdist = Vector2.Distance(other.transform.position, _partVisuels[aPart].transform.position);
                if (tempdist < dist)
                {
                    dist = tempdist;
                    part = aPart;
                }
            }

            Destroy(other.gameObject);

            _ship.TakeDamage(other.GetComponent<Projectile>().GetProjInfo.Damage, new ShipData.intPos2(part.Column, part.Row));
        }
        //Floating Ship Part?
        DisconectedPart otherPart = other.GetComponent<DisconectedPart>();
        if (otherPart != null)
        {
            //We Found a free Part!

            ShipData.Parts newPart = otherPart.GetPart();
            if (_partVisuels.ContainsKey(newPart) == false)
            {
                AddParts(newPart);
            }

            //TODO add funtionality to assign the position of the object; insted of randomly.
            
            //Destroying 
            Destroy(other.gameObject);
        }

        
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ShipDetector"))
        {
            //Debug.Log("other ship left range");
            _otherShips.Remove(other.gameObject);
            return;
        }
    }


    // Custom Functions
    public void SetupShip(bool isPlayer, List<ShipData.Parts> shipStartParts)
    {
        _ship = new ShipData.Ship(OnShipStatChange, OnShipDestroyed, CreatePartVisuel, RemovePartVisuels);
        _ship.AddParts(shipStartParts);
        
        if (isPlayer)
        {
            InputManager.instance.NewPlayerShipSetup(this);
            CameraControl.instance.followThis = gameObject;
            gameObject.GetComponent<ShipNPC>().enabled = false;
        }
    }

    //Damage
    public void FireWeapons()
    {
        if (_otherShips.Count == 0)
            return;
        
        List<ShipData.Parts> wep = _ship.GetShipWeapons;
        if (wep.Count == 0)
            return;

        foreach (var part in wep)
        {
            WeaponManager.instance.FireProjectile(
                _partVisuels[part],
                _thisShipHash, 
                new ShipData.ProjectileInfo(
                    ShipData.ProjectileInfo.projTypes.missile, 
                    1*_finalStats[ShipData.Parts.partTypes.weapon])
                    );
        }

    }
    public void TakeDamage(float damageDelt, Vector3 collisionPosition)
    {
        //Debug.Log("taking damage: " + damageDelt.ToString() + " at " + hitPos.ToString());
        _ship.TakeDamage(damageDelt, new ShipData.intPos2(0,0));
    }
    protected void OnShipDestroyed(object o, EventArgs e)
    {
        //Debug.Log("ship destroyed");
        Destroy(gameObject);
    }

    //Parts 
    public void AddParts(ShipData.Parts newParts)
    {
        _ship.AddParts(newParts);
    }
    protected void CreatePartVisuel(ShipData.Parts part )
    {
        GameObject partObj = (GameObject)Instantiate(
            GControl.instance.partPrefab, 
            Vector3.zero, 
            transform.rotation);

        partObj.transform.SetParent(transform);
        partObj.transform.localPosition = PartColRowToLocalSpacePosition(part.Column, part.Row);
        partObj.GetComponentInChildren<TextMesh>().text = "C: " + part.Column.ToString() + "\nR: " + part.Row.ToString();
        partObj.name = "PartObj: " + part.GetPartType.ToString() + ". C: " + part.Column.ToString() + ". R: " + part.Row.ToString();
        switch (part.GetPartType)
        {
            case (ShipData.Parts.partTypes.engine):
                partObj.GetComponentInChildren<Renderer>().material.color = Color.green;
                break;
            case (ShipData.Parts.partTypes.armour):
                partObj.GetComponentInChildren<Renderer>().material.color = Color.yellow;
                break;
            case (ShipData.Parts.partTypes.weapon):
                partObj.GetComponentInChildren<Renderer>().material.color = Color.red;
                break;
            case (ShipData.Parts.partTypes.shield):
                partObj.GetComponentInChildren<Renderer>().material.color = Color.blue;
                break;
            default:
                partObj.GetComponentInChildren<Renderer>().material.color = Color.white;
                break;
        }
        
        _partVisuels.Add(part, partObj);
    }
    protected void RemovePartVisuels(ShipData.Parts part)
    {
        Destroy(_partVisuels[part]);
        _partVisuels.Remove(part);
    }
    private static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;
    private Vector3 PartColRowToLocalSpacePosition(int column, int row)
    {
        //TODO: works but sort this shit out; variables that dont need to be here.

        //size of the hex
        float diameterLongest = __partSize * 2;
        float diameterShortest = WIDTH_MULTIPLIER * diameterLongest;

        //how much things need to change in each direction
        float horiz = diameterLongest * 0.75f;
        float vert = diameterShortest;

        //First a square/offset grid is created:
        //  X: horiz * column
        //  Y: vert * row
        //This is then multiplyed by the offset:
        //  X: 0
        //  Y: diameterShortest/2 * column

        Vector3 worldpos = new Vector2( horiz * column, -(vert * row) + (-vert / 2  * column));
        
        return worldpos;
    }

    //Movement
    public void Movement(Vector3 TowardsPointer)
    {
        Vector3 relPosition = TowardsPointer - transform.position;
        relPosition.Normalize();
        float zRelAngle = Vector3.Angle(relPosition, transform.up);

        //As the zangle increases (so the towards pointer becomes more to the left or right) 
        //the engine power moves away from moving the ship and more into turning the ship.


        float shipSpeed = (_finalStats[ShipData.Parts.partTypes.engine]/_ship.GetPartCount) + 10;
        float thrustPower;
        float rotationPower;

        if (zRelAngle < 90)
        {
            thrustPower = 1;
            rotationPower = 0.5f;
        }
        else
        {
            thrustPower = 0.1f;
            rotationPower = 2;
        }

        //movement
        transform.Translate(new Vector3(0, (shipSpeed * thrustPower) * Time.deltaTime, 0));

        //Rotation
        float zAngle = Mathf.Atan2(relPosition.y, relPosition.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotateTowards = Quaternion.Euler(0.0f, 0.0f, zAngle);
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTowards, (shipSpeed * rotationPower) * Time.deltaTime);


        //Debug.Log("Angle: " + (zAngle + 90f).ToString() + ".    RelAngle: " + zRelAngle.ToString() +".    Thrust: " + thrustPower.ToString() + ".    Rotation: " + rotationPower.ToString());
        /*
        //movement
        float distance = Vector3.Distance(transform.position, TowardsPointer);

        transform.Translate(new Vector3(0, (shipSpeed * distance) * Time.deltaTime, 0));


        //Rotation
        relPosition.Normalize();
        float zAngle = Mathf.Atan2(relPosition.y, relPosition.x) * Mathf.Rad2Deg - 90.0f;
        Quaternion rotateTowards = Quaternion.Euler(0.0f, 0.0f, zAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTowards, shipSpeed * Time.deltaTime);
        */
    }
    private void MoveCentre()
    {
        centureOfMass = gameObject.GetComponent<Rigidbody2D>().centerOfMass;

    }

    //buffs and debuffs
    public void AddBuffDebuff(ShipData.BuffDebuff newBuffDebuff)
    {
        buffDebuff.Add(newBuffDebuff);
        Recalculate_finalStats();
    }
    private void RemoveBuffDebuff(ShipData.BuffDebuff existingBuffDebuff)
    {
        if (buffDebuff.Contains(existingBuffDebuff) == false)
        {
            Debug.LogError("Tried to remove a BuffDebuff that no longer exists");
            return;
        }
        Debug.Log("Buff/Debuff expired: " + existingBuffDebuff.Name);
        buffDebuff.Remove(existingBuffDebuff);
        Recalculate_finalStats();
    }
    //Loops though all the buffs and removes time from there duration
    private void BuffDebuffLoop()
    {
        for (int i = 0; i < buffDebuff.Count; i++)
        {
            //Debug.Log("timer: " + buffDebuff[i].Duration.ToString());
            buffDebuff[i].Duration -= buffdebuffLoopTime;
            if (buffDebuff[i].Duration < 0)
            {
                RemoveBuffDebuff(buffDebuff[i]);
                i--;
            }
        }
    }
    //this funcion is called by:
    //  "Recalculate_finalStats"          callback from _ship (base stats change)
    //  AddBuffDebuff()          
    //  RemoveBuffDebuff()        
    protected void OnShipStatChange(object s, EventArgs e)
    {
        Recalculate_finalStats();
    }
    private void Recalculate_finalStats()
    {
        //Debug.Log("calculating final stats");
        //Wipe old stats.
        _finalStats = new Dictionary<ShipData.Parts.partTypes, float>();

        //Creates an entry for each stat type and gets the "Ship Stats" from _ship
        foreach (ShipData.Parts.partTypes partType in Enum.GetValues(typeof(ShipData.Parts.partTypes)))
        {
            _finalStats.Add(partType, _ship.GetStatOfType(partType));

            //Debug.Log(partType.ToString() + " is now: " + _ship.GetStatOfType(partType).ToString());


            //loops though each buff adding any amount to the stat and then multiplys.
            //      (Ship Stats + buff Stats)*Buffmultiplyer = final stat
            //      E.g: ship engine stat = 6, stats from buffs = 10, multiplyer = 1.5
            //      therefor 6 + 10 * 1.5 = 24 engine power final stat
            if (buffDebuff != null && buffDebuff.Count > 0)
            {
                float statMultiplyer = 1;
                //Adds a buff to the base stat
                foreach (var buffDebuff in buffDebuff)
                {
                    if (buffDebuff.AffectedStat == partType)
                    {
                        _finalStats[partType] += buffDebuff.AddAmount;
                        //TODO this is very simple, can be moved to another function to calculate multiple multiplyers.
                        statMultiplyer += buffDebuff.Multiplyer - 1.0f;
                    }
                }
                //Finaly multiplys the stat by any multiplyer buffs.
                _finalStats[partType] *= statMultiplyer;
                //Debug.Log(partType.ToString() + ", After buffs, is now: " + _finalStats[partType].ToString());
            }
        }


        

    }
}
