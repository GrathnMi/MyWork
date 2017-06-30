using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameControl : NetworkBehaviour
{
    //Set in inspector
    public GameObject ProjectilePrefab;
    public GameObject shipPrefab;
    public GameObject EnemyAIPrefab;
    public GameObject ShipExplosionPrefab;
    public GameObject victoryUI;
    public Text victoryText;

    //var
    public Dictionary<int, ShipControl2> allActiveShips = new Dictionary<int, ShipControl2>();
    public Dictionary<string, int> destroyedShips = new Dictionary<string, int>();
    private int shipIdentifyer = 1;
    private GameFX GFX;
    private bool gameOver = false;

    // Unity Functions
    //Singleton
    public static GameControl Instance;
	void Awake ()
    {   Instance = this;    }

    void Start()
    {
        GFX = GameFX.Instance;
    }
    
	void Update ()
    {
        if (Input.GetKeyDown("space"))
        {
            GameObject sausages = (GameObject)Instantiate(EnemyAIPrefab);
            NetworkServer.Spawn(sausages);
        }
    }

    // Custom Functions
    public void RegesterShip(int shipNID, ShipControl2 shipCtrl)
    {
        allActiveShips.Add(shipNID, shipCtrl);
        //Debug.Log("Regestoring ship: " + shipNID );
    }
    public void UnregesterShipNID(int shipNID)
    {
        allActiveShips.Remove(shipNID);
    }
    public ShipControl2 GetShipCtrl(int shipNID)
    {
        //Debug.Log("Looking up: " + shipNID.ToString());
        ShipControl2 sausages = null;
        allActiveShips.TryGetValue(shipNID, out sausages);

        //if (sausages == null)
        //    Debug.Log("Missing Ship; cant find ship with ID; " + shipNID.ToString() + ". Total IDs in Dict: " + allActiveShips.Count.ToString());

        return sausages;
    }
    
    [Command]
    public void CmdCreateShip(Vector3 pos, float angle, string playerNID, Color colour)
    {
        //NewShip
        GameObject newShipObj = CustomSpawnManager.Instance.GetFromPool(pos);
        newShipObj.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        ShipControl2 ship = newShipObj.GetComponent<ShipControl2>();
        ship.cliantNID = playerNID;
        ship.shipNID = ++shipIdentifyer;
        ship.hitPoints = StartingVariables.shipHP;
        ship.shieldHitPoints = StartingVariables.shipShieldHP;
        ship.shipSpeed = new Vector3(0, StartingVariables.shipSpeed, 0);
        ship.colour = colour;

        ship.StartAI();
        NetworkServer.Spawn(newShipObj, CustomSpawnManager.Instance.assetId);
    }
    public void FireProjectile(ShipControl2 shipThatFiredSC, ShipControl2 targetShipSC, int damage, float delay)
    {
        if (!targetShipSC.gameObject.activeSelf)
            return;
        //Create Visuels -- cliants
        RpcProjectileVisuels(shipThatFiredSC.shipNID, targetShipSC.shipNID);

        //Queue up Damage -- damage
        targetShipSC.TakeDelayedDamage(damage, delay);

    }
    [ClientRpc(channel = 2)]
    private void RpcProjectileVisuels(int shipThatFiredNID, int targetShipNID)
    {
        ShipControl2 shipFired = GetShipCtrl(shipThatFiredNID);
        ShipControl2 targetShip = GetShipCtrl(targetShipNID);

        GFX.ProjectileVisuels(shipFired, targetShip);
    }


    //Win Lose
    public void DestroyedShipCounter(string cliantNID)
    {
        if (destroyedShips.ContainsKey(cliantNID) == false)
        {
            destroyedShips.Add(cliantNID, 1);

            //Debug.Log("New cliant ship destroyed: " + cliantNID.ToString());
        }
        else
        {
            int i;
            destroyedShips.TryGetValue(cliantNID, out i);
            i++;    //no idea why i need to do this on another line..
            destroyedShips[cliantNID] = i;

            //Debug.Log("Cliant:   " + cliantNID.ToString() + " ship was destroyed. " + destroyedShips[cliantNID].ToString() + " total destroyed.");

            if (isServer && i > 3 && gameOver == false)
            {
                Debug.Log("ending game");
                gameOver = true;
                CmdServerGameOver(cliantNID);
            }
        }
    }

    [Server]
    private void CmdServerGameOver(string victorusPlayerNID)
    {



        RpcPlayerWon(victorusPlayerNID);
        StartCoroutine(FinishGame(5.0f));
        
        //endgame();
    }

    private IEnumerator FinishGame(float delay)
    {
        Debug.Log("shutting down server");
        yield return new WaitForSeconds(delay);

        endgame();

    }

    private void endgame()
    {
        GameObject obj = GameObject.Find("ServerObj");
        if (obj == null)
        {
            Debug.LogError("now is the time to panic!");
        }
        obj.GetComponent<NetworkManager>().StopHost();
    }
    
    [Client]
    private void RpcPlayerWon(string victorusPlayerNID)
    {
        victoryUI.SetActive(true);
        victoryText.text = victorusPlayerNID + " Won!";
    }


}