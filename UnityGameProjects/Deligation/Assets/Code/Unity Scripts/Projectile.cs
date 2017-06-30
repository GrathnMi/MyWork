using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manages the visuels and unity aspects of the projectileinfo class.
/// can only hit "ship" layer.
/// </summary>


public class Projectile : MonoBehaviour {

    public Vector3 speed;
    public float totalLifeTime = 10;
    private float timeAlive;

    private ShipData.ProjectileInfo _projInfo;
    public ShipData.ProjectileInfo GetProjInfo { get { return _projInfo; } }
    public int firedFrom { get; protected set;  }

    private GameObject _target;
    

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (_projInfo != null)
        {
            timeAlive += Time.deltaTime;
            if (timeAlive > totalLifeTime)
                Destroy(gameObject);

            transform.Translate(speed * Time.deltaTime);
            if (_target != null)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_target.transform.position), 10.0f * Time.deltaTime);
            }
        }

	}

    void OnTriggerEnter2D(Collider2D other)
    {

    }

    //custom func
    public void Setup(int firedFrom, ShipData.ProjectileInfo projInfo, GameObject target )
    {

        _projInfo = projInfo;
        _target = target;
        this.firedFrom = firedFrom;
        
        //projectile visuels
        switch (_projInfo.GetProjType)
        {
            case ShipData.ProjectileInfo.projTypes.missile:
                break;
            case ShipData.ProjectileInfo.projTypes.laser:
                break;
            case ShipData.ProjectileInfo.projTypes.plasma:
                break;
            default:
                break;
        }
    }
}
