using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {


    //Set in inspector
    public float __scrollSpeed = 5.0f;
    public float __scrollSnapSpeed = 5.0f;
    public float __startDistance = 30.0f;
    public float __minZoomDistance = 10.0f;
    public float __maxZoomDistance = 100.0f;


    public GameObject followThis;

    private float _atDistance;
    public float _desiredDistance;
    private float _zoomDist;

    // Unity Functions
    public static CameraControl instance;
    void Awake()
    {
        instance = this;
        _atDistance = __startDistance;
        _desiredDistance = __startDistance;
    }
    void Start () {
		
	}
	
	void Update () {
        if(followThis != null)
        {
            transform.position = new Vector2(followThis.transform.position.x, followThis.transform.position.y);
        }
        WheelZoom();
        transform.position = new Vector3(transform.position.x, transform.position.y, -_desiredDistance);
	}

    // custom functions
    private void WheelZoom()
    {
        _desiredDistance += Input.GetAxis("Mouse ScrollWheel") * __scrollSpeed;
        _desiredDistance = Mathf.Clamp(_desiredDistance, __minZoomDistance, __maxZoomDistance);
    }

}
