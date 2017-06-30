using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CliantControl : NetworkBehaviour
{
    //Input
    private Vector3 pressDown;
    private Vector3 pressRelease;
    private Vector3 pressDist;

    //colour
    public Color playerColour = Color.black;
    //[SyncVar]
    private float r;
    //[SyncVar(hook = "UpdatePlayerColour")]
    private float g;
    //[SyncVar(hook = "UpdatePlayerColour")]
    private float b;

    public Color shipColour; //fortesting
    private string netID;


    //GUI stuff
    private GameControl GC;

    // UnityFunctions
    void Start()
    {
        GC = GameControl.Instance;
        netID = GetComponent<NetworkIdentity>().netId.ToString();

        if (isLocalPlayer)
        {
            r = Random.value;
            g = Random.value;
            b = Random.value;
            playerColour = new Color(r, g, b);
        }
    }
    void Update()
    {
        if (!isLocalPlayer)
            return;

        PlaceShip();
    }
    // CustomFunctions
    private void PlaceShip()
    {
        if (Input.GetButtonDown("Press"))
        {
            pressDown = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pressDown = new Vector3(pressDown.x, pressDown.y, 0.0f);
        }
        if (Input.GetButtonUp("Press"))
        {
            pressRelease = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pressRelease = new Vector3(pressRelease.x, pressRelease.y, 0.0f);

            //Colour
            shipColour = new Color(r, g, b);

            //Rotation
            pressDist = pressRelease - pressDown;
            pressDist.Normalize();
            float zAngle = Mathf.Atan2(pressDist.y, pressDist.x) * Mathf.Rad2Deg - 90.0f;



            //Assign
            CmdCreateShipOnServer(pressDown, zAngle, GetComponent<NetworkIdentity>().netId.ToString(), playerColour );

        }
    }
    [Command]
    private void CmdCreateShipOnServer(Vector3 pos, float angle, string playerNID, Color Colour)
    {
        GC.CmdCreateShip(pos, angle, playerNID, Colour);
    }
}