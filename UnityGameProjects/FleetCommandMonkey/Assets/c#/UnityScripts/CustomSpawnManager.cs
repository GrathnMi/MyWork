using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class CustomSpawnManager : MonoBehaviour {

    public GameObject pool_ObjPrefab;
    public List<GameObject> pool_Ships = new List<GameObject>();
    
    public NetworkHash128 assetId { get; set; }

    public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetId);
    public delegate void UnSpawnDelegate(GameObject spawned);


    //Singleton
    public static CustomSpawnManager Instance;
    void Awake()
    { Instance = this; }
    // Use this for initialization
    void Start () {

        assetId = pool_ObjPrefab.GetComponent<NetworkIdentity>().assetId;
        ClientScene.RegisterSpawnHandler(assetId, SpawnObject, UnSpawnObject);
        
    }
    public GameObject GetFromPool(Vector3 position)
    {
        foreach (var obj in pool_Ships)
        {
            if (!obj.activeInHierarchy)
            {
                //Debug.Log("Activating object: " + obj.name);
                obj.transform.position = position;
                obj.SetActive(true);
                return obj;
            }
        }
        GameObject newShip = (GameObject)Instantiate(pool_ObjPrefab, position, Quaternion.identity);
        newShip.name = "PoolObject_Ship " + pool_Ships.Count.ToString();
        pool_Ships.Add(newShip);
        newShip.transform.SetParent(transform);
        return newShip;
    }
    public GameObject SpawnObject(Vector3 position, NetworkHash128 assetId)
    {
        return GetFromPool(position);
    }

    //cliant side?
    public void UnSpawnObject(GameObject spawned)
    {
        //Debug.Log("Re-pooling object " + spawned.name);
        spawned.GetComponent<ShipControl2>().OnUnspawn();

        GameFX.Instance.ShipExplosionVisuels(spawned.transform.position);

        spawned.SetActive(false);
    }
}
