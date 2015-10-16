using UnityEngine;
using System.Collections;

public class CraneCollect : MonoBehaviour {

    public int collected = 0;

	// Use this for initialization
	void Start () {
        //Get number of cranes from wherever we save it
    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Crane")
        {
            collected++;
            Destroy(other.gameObject);
            //fancy animation or particle effect
        }
    }

    public void SubtractCranes(int num)
    {
        collected = collected - num;
    }
}
