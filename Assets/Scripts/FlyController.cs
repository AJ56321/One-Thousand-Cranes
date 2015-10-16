using UnityEngine;
using System.Collections;

public class FlyController : MonoBehaviour {

    //Fly variables
    public float originalDrag = 0;
    public float maxDrag = 2;
    public bool grounded = true;

    //Editor control variables
    public Vector3 prevMouse = Vector3.zero;

	//Touch control variables
	public Vector3 prevTouch = Vector2.zero;
    public float deltaSum = 0;

    private GameObject Player;

	// Use this for initialization
	void Start () {
        Player = GameObject.FindGameObjectWithTag("Player");
        Player.GetComponent<Rigidbody>().drag = originalDrag;
	}
	
	// Update is called once per frame
	void Update () {
        //Editor Controls
        if(Input.GetButtonDown("Jump") && !Application.isMobilePlatform)
        {
            grounded = false;
            Player.GetComponent<Rigidbody>().AddForce(new Vector3(10,15,0), ForceMode.Impulse);
        }


        if(Input.GetMouseButton(0) && !Application.isMobilePlatform)
        {
            Vector3 mousePos = Input.mousePosition;

            if(prevMouse == Vector3.zero)
                prevMouse = mousePos;
 
            Player.GetComponent<Rigidbody>().drag += (mousePos.y - prevMouse.y)/(Screen.height/(maxDrag * 2));

            if (Player.GetComponent<Rigidbody>().drag > maxDrag)
                Player.GetComponent<Rigidbody>().drag = maxDrag;
            
            if (Player.GetComponent<Rigidbody>().drag < -0.1f)
            {
                Player.GetComponent<Rigidbody>().drag = -0.1f;
            }
                
            if(Player.GetComponent<Rigidbody>().drag <= -0.1f)
            {
                Player.GetComponent<Rigidbody>().AddForce(new Vector3(20, -5, 0) * Time.deltaTime, ForceMode.Impulse);
            } 

            prevMouse = mousePos;
        }

		//Touch controls
		if(Input.touchCount > 0 && Application.isMobilePlatform)
		{
			if(Input.GetTouch (0).phase == TouchPhase.Began)
			{
				if(prevTouch == Vector3.zero)
					prevTouch = Input.GetTouch(0).position;
			}

			if(Input.GetTouch (0).phase == TouchPhase.Moved)
			{
				Player.GetComponent<Rigidbody>().drag += (Input.GetTouch (0).position.y - prevTouch.y)/(Screen.height/(maxDrag * 2));
				
				if (Player.GetComponent<Rigidbody>().drag > maxDrag)
					Player.GetComponent<Rigidbody>().drag = maxDrag;
				
				if (Player.GetComponent<Rigidbody>().drag < -0.1f)
				{
					Player.GetComponent<Rigidbody>().drag = -0.1f;
				}
				
				if(Player.GetComponent<Rigidbody>().drag <= -0.1f)
				{
					Player.GetComponent<Rigidbody>().AddForce(new Vector3(20, -5, 0) * Time.deltaTime, ForceMode.Impulse);
				}

                deltaSum += Vector2.Distance(prevTouch,Input.GetTouch(0).position); 
			}

			if(Input.GetTouch (0).phase == TouchPhase.Ended)
			{
                if(deltaSum < 10f && grounded)
                {
                    grounded = false;
                    Player.GetComponent<Rigidbody>().AddForce(new Vector3(10, 15, 0), ForceMode.Impulse);
                    deltaSum = 0;
                }

            }

			prevTouch = Input.GetTouch (0).position;
		}

        if (grounded)
        {
            Player.GetComponent<Rigidbody>().drag = originalDrag;
            deltaSum = 0;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }
	
}

