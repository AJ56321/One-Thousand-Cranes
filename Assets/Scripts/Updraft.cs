using UnityEngine;
using System.Collections;

public class Updraft : MonoBehaviour {

	public float windForce = 10;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			other.gameObject.GetComponent<Rigidbody>().AddForce (new Vector3(0,windForce,0));
		}
	}
	
}
