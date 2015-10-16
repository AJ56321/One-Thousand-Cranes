using UnityEngine;
using System.Collections;

public class SmoothDampToObj : MonoBehaviour {

    public GameObject dest;
    private Vector3 vel = Vector3.zero;
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(gameObject.transform.position, dest.transform.position) > 0.02f)
            gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position, dest.transform.position, ref vel, Globals.TONGUE_PULL_SMOOTH_TIME);
            
	}
}
