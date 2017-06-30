using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyPlayerAI : NetworkBehaviour {

    private GameControl GC;
    
    public string aiCliantID = "";
    public Color playerColour = Color.black;
    //[SyncVar(hook = "UpdateAIPlayerColour")]
    private float r;
    //[SyncVar(hook = "UpdateAIPlayerColour")]
    private float g;
    //[SyncVar(hook = "UpdateAIPlayerColour")]
    private float b;

    public Vector3 startingPos = Vector3.zero;

	// UnityFunctions
	void Start ()
    {
        GC = GameControl.Instance;
        aiCliantID = "AI" + GetComponent<NetworkIdentity>().netId.ToString();

        if (isServer)
        {
            r = Random.value;
            g = Random.value;
            b = Random.value;
            playerColour = new Color(r, g, b);

            startingPos = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), 0.0f);

            InvokeRepeating("CreateAIShip", 1.0f, 5.0f);
        }
    }
    
    // Customfunctions
    private void CreateAIShip()
    {


        Vector3 pressDist = Vector3.zero;
        pressDist = Vector3.zero - startingPos;
        pressDist.Normalize();
        float zAngle = Mathf.Atan2(pressDist.y, pressDist.x) * Mathf.Rad2Deg - 90.0f;

        CmdCreateShipOnServer(startingPos, zAngle, aiCliantID, playerColour);
    }

    [Command]
    private void CmdCreateShipOnServer(Vector3 pos, float angle, string aiPlayerNID, Color colour)
    {
        GC.CmdCreateShip(pos, angle, aiPlayerNID, colour);
    }
    
}
