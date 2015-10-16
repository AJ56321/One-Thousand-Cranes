using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

public class JumpController : MonoBehaviour {

	private float[] touchTime;
	private Vector2[] startPos;
	private Vector2[] endPos;
	private bool jump;
	private GameObject Player;
	private float maxForce;
	private bool grounded;
	// Use this for initialization
	void Start () {
		Player = GameObject.FindGameObjectWithTag("Player");

		touchTime = new float[2];
		touchTime[0] = 0;
		touchTime[1] = 0;
		startPos = new Vector2[2];
		startPos [0] = Vector2.zero;
		startPos [1] = Vector2.zero;
		endPos = new Vector2[2];
		endPos [0] = Vector2.zero;
		endPos [1] = Vector2.zero;
		grounded = true;
		jump = false;
		maxForce = 1000f;
	}
	
	// Update is called once per frame
	public void Execute () {
		//Discern which touch is which? Does it matter?
		//Internal variable for if the frog is connected to something
		for (int i = 0; i < Input.touchCount; i++)
		{
			if (i > 1)
				break;
			
			//Initial contact on phone
			if(Input.GetTouch(i).phase == TouchPhase.Began)
			{
				touchTime[i] += Input.GetTouch(i).deltaTime;
				startPos[i] = Input.GetTouch(i).position;
				continue;
			}
			
			//Contact breaks (How does it get through the loop then...?)
			if(Input.GetTouch(i).phase == TouchPhase.Ended)
			{
				if(touchTime[i] <= 0.2f && jump)
				{
					Vector2 vec = endPos[i] - startPos[i];
					vec = vec.normalized;

					Player.GetComponent<Rigidbody>().AddForce(new Vector3(maxForce * vec.x, maxForce * vec.y, 0));
					grounded = false;
					jump = false;
				}
				startPos[i] = Vector2.zero;
				endPos[i] = Vector2.zero;
				touchTime[i] = 0;
				//Tap to shoot prefab... if time is low enough between .Began and .Ended
			}

			else if (Input.GetTouch(i).phase == TouchPhase.Moved)
			{
				if (Input.GetTouch(i).deltaPosition.magnitude > 1 && touchTime[i] <= 0.2f && grounded)
				{
				 	jump = true;
				}
				else
					jump = false;

				endPos[i] = Input.GetTouch(i).position;
				touchTime[i] += Input.GetTouch(i).deltaTime;
			}
			
			//Contact is ongoing
			else if(Input.GetTouch(i).phase == TouchPhase.Stationary)
			{
				//Swipe to jump? To Disconnect tongue, horiz to swing
				touchTime[i] += Input.GetTouch(i).deltaTime;
			}
		}
		if (Input.GetButtonUp("Jump") && !Application.isMobilePlatform && grounded)
		{
			Player.GetComponent<Rigidbody>().AddForce(new Vector3(maxForce/2, maxForce/2, 0));
			grounded = false;
		}
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Ground")
		{
			RaycastHit hit;
			Debug.Log ("frog");
			if (Physics.Raycast(transform.position, Vector3.down, out hit, 1))
			{
				Debug.DrawLine(transform.position, hit.point);
				if (hit.collider.gameObject == other.gameObject)
					grounded = true;
			}
			
		}
	}
}

