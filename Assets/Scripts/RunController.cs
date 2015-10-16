using UnityEngine;
using System.Collections;


public class RunController : MonoBehaviour
{

    //touch variables
    public float[] touchTime;

    //click variables (only for testing)
    public float clickTime = 0;

    //Run form variables
    public float maxSpeed = 5;
    public float deltaSpeed = 1;
    public float currSpeed = 0;
    public float hopPower = 300;
    public bool grounded = true;
    public bool hop = false;
    public enum Facing { Left, Right, }
    public Facing facing;

    private GameObject Player;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        //set initial run variables
        touchTime = new float[2];
        touchTime[0] = 0;
        touchTime[1] = 0;
        facing = Facing.Right;
    }


    // Update is called once per frame
    public void Execute()
    {
        //Touch controls
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (i > 1)
                break;

            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                touchTime[i] += Input.GetTouch(i).deltaTime;
                continue;
            }

            if (Input.GetTouch(i).phase == TouchPhase.Ended)
            {
                if (touchTime[i] <= 0.2f && hop)
                {
                    if (grounded)
                    {
                        Player.GetComponent<Rigidbody>().AddForce(new Vector3(0, hopPower, 0));
                        grounded = false;
                        hop = false;
                    }
                }

                touchTime[i] = 0;
            }

            else if (Input.GetTouch(i).phase == TouchPhase.Moved)
            {
                if (Input.GetTouch(i).deltaPosition.y > 1 && touchTime[i] <= 0.2f)
                    hop = true;
                else
                    hop = false;

                touchTime[i] += Input.GetTouch(i).deltaTime;
            }

            else if (Input.GetTouch(i).phase == TouchPhase.Stationary && i == 0)
            {
                if (currSpeed < maxSpeed && grounded)
                    currSpeed += deltaSpeed;
                else if (currSpeed < 0 && !grounded)
                    currSpeed -= deltaSpeed / 30;
                else if (currSpeed > maxSpeed)
                    currSpeed = maxSpeed;
                else if (currSpeed < 0)
                    currSpeed = 0;

                Vector3 touchPos = Input.GetTouch(i).position;

                touchPos.z = 10;

                if (Camera.main.ScreenToWorldPoint(touchPos).x < Player.transform.position.x)
                {
                    Player.transform.Translate(Vector3.left * currSpeed * Time.deltaTime);

                    facing = Facing.Left;
                }
                else
                {
                    Player.transform.Translate(Vector3.right * currSpeed * Time.deltaTime);

                    facing = Facing.Right;
                }

                touchTime[i] += Input.GetTouch(i).deltaTime;
            }
        }

        if (Input.touchCount == 0)
        {
            if (currSpeed > 0 && grounded)
                currSpeed -= deltaSpeed / 2;
            else if (currSpeed > 0 && !grounded)
                currSpeed -= deltaSpeed / 30;
            else if (currSpeed < 0)
                currSpeed = 0;

            if (facing == Facing.Right)
                Player.transform.Translate(Vector3.right * currSpeed * Time.deltaTime);
            else
                Player.transform.Translate(Vector3.left * currSpeed * Time.deltaTime);
        }

        //Write some code for if run into something make it slow down?



        //Editor Controls
        if (Input.GetButtonUp("Jump") && !Application.isMobilePlatform && grounded)
        {
            Debug.Log("Click Time: " + clickTime);

            //if (clickTime <= 0.2f)
            //{
                Player.GetComponent<Rigidbody>().AddForce(new Vector3(0, hopPower, 0));
                grounded = false;
            //}

           clickTime = 0;
        }
        else if (Input.GetMouseButton(0) && !Application.isMobilePlatform && grounded)
        {
            if (clickTime > 0.2f)
            {
                if (currSpeed < maxSpeed)
                    currSpeed += deltaSpeed;
                else if (currSpeed > maxSpeed)
                    currSpeed = maxSpeed;

                Vector3 mousePos = Input.mousePosition;

                mousePos.z = 10;

                //Debug.Log("mouse x: " + Camera.main.ScreenToWorldPoint(mousePos) + ", player x: " + Player.transform.position.x);
                if (Camera.main.ScreenToWorldPoint(mousePos).x < Player.transform.position.x)
                {

                    Player.transform.Translate(Vector3.left * currSpeed * Time.deltaTime);
                    facing = Facing.Left;
                }
                else
                {
                    Player.transform.Translate(Vector3.right * currSpeed * Time.deltaTime);
                    facing = Facing.Right;
                }
            }

            clickTime += Time.deltaTime;
        }
		else
		{
			if (currSpeed > 0 && grounded)
				currSpeed -= deltaSpeed / 2;
			else if (currSpeed > 0 && !grounded)
				currSpeed -= deltaSpeed / 30;
			else if (currSpeed < 0)
				currSpeed = 0;
			
			clickTime = 0;
	}

    
    }


    /*void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            RaycastHit hit;
			Debug.Log ("here");
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1))
            {
                Debug.DrawLine(transform.position, hit.point);
                if (hit.collider.gameObject == other.gameObject)
                    grounded = true;
            }

        }
    }*/
}
