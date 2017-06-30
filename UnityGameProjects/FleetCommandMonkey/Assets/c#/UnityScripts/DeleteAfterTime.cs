using UnityEngine;
using System.Collections;

public class DeleteAfterTime : MonoBehaviour {

    public float lifeTime = 1.0f;
	void Start ()
    {
        Invoke("DestroyObj", lifeTime);
	}

    void DestroyObj()
    {
        Destroy(gameObject);
    }
}
