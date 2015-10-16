using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    
    private GameObject Player;
    private Vector3 velocity = Vector3.zero;
    
	// Use this for initialization
	void Start ()
    {
		Player = GameObject.FindGameObjectWithTag ("Player");
		this.transform.position = new Vector3(Player.transform.position.x + Globals.CAMERA_OFFSET.x, Player.transform.position.y + Globals.CAMERA_OFFSET.y, Globals.CAMERA_OFFSET.z);
	}
	

	// Update is called once per frame
	void LateUpdate ()
    {
		Vector3 targetPosition = new Vector3(Player.transform.position.x + Globals.CAMERA_OFFSET.x, Player.transform.position.y + Globals.CAMERA_OFFSET.y, Globals.CAMERA_OFFSET.z);
		Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, targetPosition, ref velocity, Globals.CAMERA_SMOOTH_TIME);

	}
}
