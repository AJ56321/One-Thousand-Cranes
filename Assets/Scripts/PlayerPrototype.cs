using UnityEngine;
using System.Collections;

public enum State{
	Run,
	Jump,
	Fly,
};

public enum Facing {
	Left, 
	Right 
};

public class PlayerPrototype : MonoBehaviour
{
    //Prefab
    public GameObject tonguePrefab;

    //touch variables
    private float[] touchTime;
    private float clickTime = 0;

    //Run form variables
    private float currSpeed = 0;
    private float jumpTimer = 0.2f;
    private int grounded;
    private GameObject tongueAttachedObj;

    private Vector2[] recentTouch;

    //Jump Related
    private Vector2[] pos1;
    private Vector2[] pos2;
    private float maxForce;

    //Fly variables
    public float originalDrag = 0;
    public float maxDrag = 2;

    //Touch control variables
    public float deltaSum = 0;

    //Player and State machine / Generic
    private State state;
    private Facing facing;

    //Model objects / State changing variables
    public GameObject panda;
    public GameObject frog;
    public GameObject hummingbird;

    //This prevents the immediate motion of something on switching to the state
    private bool formSwitch;
    private bool transforming;

    //temporary
    private float velocity;
    private Vector3 tongueVelocity;
    private Vector3 rotationVelocity;


    // Use this for initialization
    void Start()
    {
        state = State.Run;
        hummingbird.SetActive(false);
        frog.SetActive(false);
        formSwitch = false;

        pos1 = new Vector2[Globals.NUMBER_OF_RECORDED_FRAMES];
        pos2 = new Vector2[Globals.NUMBER_OF_RECORDED_FRAMES];
        for (int j = 0; j < Globals.NUMBER_OF_RECORDED_FRAMES; j++)
        {
            pos1[j] = new Vector2(-1, -1);
            pos2[j] = new Vector2(-1, -1);
        }

        rotationVelocity = Vector3.zero;
        tongueVelocity = Vector3.zero;

        //set initial run variables
        touchTime = new float[2];
        touchTime[0] = 0;
        touchTime[1] = 0;
        facing = Facing.Right;
        grounded = 0;
        maxForce = 750f;
        tongueAttachedObj = null;
        //gameObject.GetComponent<Rigidbody>().drag = originalDrag;
        originalDrag = gameObject.GetComponent<Rigidbody>().drag;
        recentTouch = new Vector2[2];
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(tongueAttachedObj == null && gameObject.transform.eulerAngles.z != 0)
        {
            if (gameObject.transform.eulerAngles.z == 360f)
                gameObject.transform.eulerAngles = Vector3.zero;
            Vector3 target = Vector3.zero;
            if (gameObject.transform.eulerAngles.z - 180 > 0f)
                target = new Vector3(0f, 0f, 360f);
            gameObject.transform.eulerAngles = Vector3.SmoothDamp(gameObject.transform.eulerAngles, target, ref rotationVelocity, 0.3f);
        }
        if (grounded > 1)
            grounded = 1;
        if (Input.touchCount == 0)
        {
            if (state != State.Fly)
            {
                if (currSpeed > 0 && grounded > 0)
                    currSpeed -= Globals.PANDA_DELTA_SPD / 2;
                else if (currSpeed > 0 && !(grounded > 0))
                    currSpeed -= Globals.PANDA_DELTA_SPD / 30;
                else if (currSpeed < 0)
                    currSpeed = 0;

                if (facing == Facing.Right)
                    gameObject.transform.Translate(Vector3.right * currSpeed * Time.deltaTime);
                else
                    gameObject.transform.Translate(Vector3.left * currSpeed * Time.deltaTime);
            }
            else
            {
                if (currSpeed > Globals.BIRD_NORMAL_SPD && grounded == 0)
                    currSpeed -= Globals.BIRD_DELTA_SPD / 2;
                else if (currSpeed < Globals.BIRD_NORMAL_SPD && grounded == 0)
                    currSpeed += Globals.BIRD_DELTA_SPD / 2;

                if (facing == Facing.Right)
                    gameObject.transform.Translate(Vector3.right * currSpeed * Time.deltaTime);
                else
                    gameObject.transform.Translate(Vector3.left * currSpeed * Time.deltaTime);
            }
        }
        else
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (i > 1)
                    break;
                
                recentTouch[i] = Input.GetTouch(i).position;
                //Touch begins
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    touchTime[i] += Input.GetTouch(i).deltaTime;
                    for (int j = 0; j < Globals.NUMBER_OF_RECORDED_FRAMES; j++)
                    {
                        SetPosition(new Vector2(-1, -1), j, i);
                    }
                    continue;
                }

                //Touch is removed
                if (Input.GetTouch(i).phase == TouchPhase.Ended)
                {
                    //Get the theoretical swipe distance
                    float dragDistance = 0f;
                    float angleRads = 0f;
                    int timeIndex = (int)(touchTime[i] / (Globals.FIXED_SEC_PER_FRAME));
                    if (timeIndex > Globals.NUMBER_OF_RECORDED_FRAMES - 1)
                        timeIndex = Globals.NUMBER_OF_RECORDED_FRAMES - 1;
                    Vector2 vec = new Vector2(0, 0);

                    if (GetPosition(Globals.NUMBER_OF_RECORDED_FRAMES - 1 - timeIndex, i).x >= 0)
                    {
                        dragDistance = Vector2.Distance(GetPosition(Globals.NUMBER_OF_RECORDED_FRAMES - 1 - timeIndex, i), recentTouch[i]);
                        vec = recentTouch[i] - GetPosition(Globals.NUMBER_OF_RECORDED_FRAMES - 1 - timeIndex, i);
                        vec = vec.normalized;
                        angleRads = Mathf.Atan2(vec.y, vec.x);
                    }
                    //Swipe?
                    Debug.Log("Drag Distance: " + dragDistance);
                    Debug.Log("Req Swipe Distance: " + Globals.DISTANCE_FOR_SWIPE);
                    if (dragDistance > Globals.DISTANCE_FOR_SWIPE)
                    {
                        float degrees = angleRads * Mathf.Rad2Deg;
                        Debug.Log(degrees);
                        //Frog
                        if (degrees > 20f && degrees < 160f)
                        {
                            //If we can, Jump!
                            Debug.Log("Grounded: " + grounded);
                            if (grounded > 0)
                            {
                                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(gameObject.GetComponent<Rigidbody>().velocity.x, 0, 0);
                                gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(maxForce * vec.x, maxForce * vec.y, 0));
                                StartCoroutine("ChangeForm", frog);
                            }
                        }
                        //Bird
                        /*else if (degrees < -20f && degrees > -160f)
                        {
                            StartCoroutine("ChangeForm", hummingbird);
                        }
                        //Panda (right or left swipe)
                        else
                        {
                            StartCoroutine("ChangeForm", panda);
                        }*/
                    }

                    //Not a swipe. See if it was a tap
                    else if (touchTime[i] < Globals.TAP_TIME_MAX)
                    {
                        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(recentTouch[i].x, recentTouch[i].y, Camera.main.nearClipPlane - Globals.CAMERA_OFFSET.z));
                        GameObject[] objs = GameObject.FindGameObjectsWithTag("Tongueable");
                        foreach (GameObject obj in objs)
                        {
                            if (obj.GetComponent<Collider>().bounds.Contains(worldPos))
                            {
                                //We have tapped on an appropriate tongue swing object
                                if (tongueAttachedObj == null)
                                {
                                    Vector2 diff = new Vector2(obj.transform.position.x - gameObject.transform.position.x, obj.transform.position.y - gameObject.transform.position.y);
                                    tongueAttachedObj = Instantiate(tonguePrefab, new Vector3((gameObject.transform.position.x + Mathf.Cos(Mathf.Atan2(diff.y,diff.x))*Globals.TONGUE_DIST), (gameObject.transform.position.y + Mathf.Sin(Mathf.Atan2(diff.y,diff.x))* Globals.TONGUE_DIST), 0f), Quaternion.identity) as GameObject;
                                    SpringJoint joint = this.gameObject.AddComponent<SpringJoint>();
                                    joint.connectedBody = tongueAttachedObj.GetComponent<Rigidbody>();

                                    tongueAttachedObj.GetComponent<SmoothDampToObj>().dest = obj;
                                    joint.spring = Globals.TONGUE_SPRING_STRENGTH;
                                    joint.damper = Globals.TONGUE_SPRING_DAMPER;
                                    joint.maxDistance = 0.1f;
                                    joint.minDistance = 0f;
                                    gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
                                    StartCoroutine("TongueConnect", obj);
                                    panda.SetActive(false);
                                    hummingbird.SetActive(false);
                                    frog.SetActive(true);
                                    state = State.Jump;
                                }
                                else
                                {
                                    Destroy(this.GetComponent<SpringJoint>());
                                    Destroy(tongueAttachedObj);
                                    tongueAttachedObj = null;
                                    gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;
                                }
                            }
                        }
                    }

                    touchTime[i] = 0;
                }

                //Touch moves
                if (Input.GetTouch(i).phase == TouchPhase.Moved)
                {
                    touchTime[i] += Input.GetTouch(i).deltaTime;
                    Vector3 touchPos = recentTouch[i];
                    touchPos.z = Camera.main.nearClipPlane - Globals.CAMERA_OFFSET.z;

                    //grounded so run
                    if (i == 0 && grounded > 0 && touchTime[i] > Globals.MOVE_DELAY)
                    {
                        if (state != State.Run)
                            StartCoroutine("ChangeForm", panda);
                        /*if (currSpeed < Globals.PANDA_MAX_SPD)
                            currSpeed += Globals.PANDA_DELTA_SPD;
                        else if (currSpeed < 0)
                            currSpeed -= Globals.PANDA_DELTA_SPD / 30;
                        else if (currSpeed > Globals.PANDA_MAX_SPD)
                            currSpeed = Globals.PANDA_MAX_SPD;
                        else if (currSpeed < 0)
                            currSpeed = 0;*/

                        if (Camera.main.ScreenToWorldPoint(touchPos).x < gameObject.transform.position.x)
                        {
                            //gameObject.transform.Translate(Vector3.left * currSpeed * Time.deltaTime);
                            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(-1 * Globals.PANDA_FORCE_CONSTANT, 0, 0));
                            facing = Facing.Left;
                        }
                        else
                        {
                            //gameObject.transform.Translate(Vector3.right * currSpeed * Time.deltaTime);
                            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(Globals.PANDA_FORCE_CONSTANT, 0, 0));
                            facing = Facing.Right;
                        }
                    }
                    else if (i == 0 && grounded == 0 && touchTime[i] > Globals.MOVE_DELAY)
                    {
                        if (state != State.Fly)
                            StartCoroutine("ChangeForm", hummingbird);

                        /*if (Camera.main.ScreenToWorldPoint(touchPos).x < gameObject.transform.position.x)
                        {
                            if (currSpeed > Globals.BIRD_MIN_SPD)
                                currSpeed -= Globals.BIRD_DELTA_SPD;
                            else if (currSpeed < Globals.BIRD_MIN_SPD)
                                currSpeed = Globals.BIRD_MAX_SPD;
                        }
                        else
                        {
                            if (currSpeed < Globals.BIRD_MAX_SPD)
                                currSpeed += Globals.BIRD_DELTA_SPD;
                            else if (currSpeed > Globals.BIRD_MAX_SPD)
                                currSpeed = Globals.BIRD_MAX_SPD;
                        }
                        if(facing == Facing.Right)
                            gameObject.transform.Translate(Vector3.right * currSpeed * Time.deltaTime);
                        else
                            gameObject.transform.Translate(Vector3.left * currSpeed * Time.deltaTime);*/

                        if (facing == Facing.Right)
                            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right * Globals.BIRD_FORCE_CONSTANT + Vector3.down * Globals.BIRD_FORCE_CONSTANT / 2);
                        else
                            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.left * Globals.BIRD_FORCE_CONSTANT + Vector3.down * Globals.BIRD_FORCE_CONSTANT / 2);


                        /*gameObject.GetComponent<Rigidbody>().drag += (Input.GetTouch(0).position.y - GetPosition(Globals.NUMBER_OF_RECORDED_FRAMES - 1, i).y) / (Screen.height / (maxDrag * 2));

                        if (gameObject.GetComponent<Rigidbody>().drag > maxDrag)
                            gameObject.GetComponent<Rigidbody>().drag = maxDrag;

                        if (gameObject.GetComponent<Rigidbody>().drag < -0.1f)
                        {
                            gameObject.GetComponent<Rigidbody>().drag = -0.1f;
                        }

                        if (gameObject.GetComponent<Rigidbody>().drag <= -0.1f)
                        {
                            if(facing == Facing.Right)
                                gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(100, -5, 0) * Time.deltaTime, ForceMode.Impulse);
                            else
                                gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(-100, -5, 0) * Time.deltaTime, ForceMode.Impulse);
                        }

                        deltaSum += Vector2.Distance(GetPosition(Globals.NUMBER_OF_RECORDED_FRAMES-1, i), recentTouch[i]);*/


                    }
                }

                //Touch does not move
                if (Input.GetTouch(i).phase == TouchPhase.Stationary)
                {
                    touchTime[i] += Input.GetTouch(i).deltaTime;
                    Vector3 touchPos = recentTouch[i];
                    touchPos.z = Camera.main.nearClipPlane - Globals.CAMERA_OFFSET.z;

                    //grounded so run
                    if (i == 0 && grounded > 0 && touchTime[i] > Globals.MOVE_DELAY)
                    {
                        if (state != State.Run)
                            StartCoroutine("ChangeForm", panda);
                        /*if (currSpeed < Globals.PANDA_MAX_SPD)
                            currSpeed += Globals.PANDA_DELTA_SPD;
                        else if (currSpeed < 0)
                            currSpeed -= Globals.PANDA_DELTA_SPD / 30;
                        else if (currSpeed > Globals.PANDA_MAX_SPD)
                            currSpeed = Globals.PANDA_MAX_SPD;
                        else if (currSpeed < 0)
                            currSpeed = 0;*/

                        if (Camera.main.ScreenToWorldPoint(touchPos).x < gameObject.transform.position.x)
                        {
                            //gameObject.transform.Translate(Vector3.left * currSpeed * Time.deltaTime);
                            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(-1 * Globals.PANDA_FORCE_CONSTANT, 0, 0));
                            facing = Facing.Left;
                        }
                        else
                        {
                            //gameObject.transform.Translate(Vector3.right * currSpeed * Time.deltaTime);
                            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(Globals.PANDA_FORCE_CONSTANT, 0, 0));
                            facing = Facing.Right;
                        }
                    }
                    else if (i == 0 && grounded == 0 && touchTime[i] > Globals.MOVE_DELAY)
                    {
                        if (state != State.Fly)
                            StartCoroutine("ChangeForm", hummingbird);
                        bool drag = false;
                        if (Camera.main.ScreenToWorldPoint(touchPos).x < gameObject.transform.position.x)
                        {
                            /*if (currSpeed > Globals.BIRD_MIN_SPD)
                                currSpeed -= Globals.BIRD_DELTA_SPD;
                            else if (currSpeed < Globals.BIRD_MIN_SPD)
                                currSpeed = Globals.BIRD_MAX_SPD;*/
                            drag = true;
                        }
                        /*else
                        {
                            if (currSpeed < Globals.BIRD_MAX_SPD)
                                currSpeed += Globals.BIRD_DELTA_SPD;
                            else if (currSpeed > Globals.BIRD_MAX_SPD)
                                currSpeed = Globals.BIRD_MAX_SPD;
                        }*/

                        if (facing == Facing.Right)
                        {
                            if (drag)
                            {
                                //gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right * Globals.BIRD_DRAG_CONSTANT);
                                gameObject.GetComponent<Rigidbody>().drag = Globals.BIRD_DRAG_CONSTANT;
                            }
                            else
                            {
                                gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right * Globals.BIRD_FORCE_CONSTANT + Vector3.down * Globals.BIRD_FORCE_CONSTANT / 2);
                                gameObject.GetComponent<Rigidbody>().drag = originalDrag;
                            }
                            //gameObject.transform.Translate(Vector3.right * currSpeed * Time.deltaTime);
                        }
                        else
                        {
                            if (drag)
                            {
                                //gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right * Globals.BIRD_DRAG_CONSTANT);
                                gameObject.GetComponent<Rigidbody>().drag = Globals.BIRD_DRAG_CONSTANT;
                            }
                            else
                            {
                                gameObject.GetComponent<Rigidbody>().AddForce(Vector3.left * Globals.BIRD_FORCE_CONSTANT + Vector3.down * Globals.BIRD_FORCE_CONSTANT / 2);
                                gameObject.GetComponent<Rigidbody>().drag = originalDrag;
                            }
                                //gameObject.transform.Translate(Vector3.left * currSpeed * Time.deltaTime);
                        }
                    }
                }

                IteratePositions(recentTouch[i], i);

            }
        }
    }

    private void IteratePositions(Vector2 currentPos, int touch)
    {
        for (int i = 0; i < Globals.NUMBER_OF_RECORDED_FRAMES - 1; i++)
        {
            SetPosition(GetPosition(i + 1, touch), i, touch);
        }
        SetPosition(currentPos, Globals.NUMBER_OF_RECORDED_FRAMES - 1, touch);
    }

    private Vector2 GetPosition(int frame, int touch)
    {
        if (touch == 0)
            return pos1[frame];
        else
            return pos2[frame];
    }

    private void SetPosition(Vector2 pos, int frame, int touch)
    {
        if (touch == 0)
            pos1[frame] = pos;
        else
            pos2[frame] = pos;
    }

    public IEnumerator ChangeForm(GameObject form)
    {
        if (!transforming && tongueAttachedObj == null)
        {
            transforming = true;

            //current animation state goes to transform_end state

            //yield return new WaitForSeconds(0f);//length of transform_end);

            if(form == panda)
            {
                frog.SetActive(false);
                hummingbird.SetActive(false);
                state = State.Run;
                gameObject.GetComponent<Rigidbody>().drag = originalDrag;
            }
            else if (form == frog)
            {
                panda.SetActive(false);
                hummingbird.SetActive(false);
                state = State.Jump;
                gameObject.GetComponent<Rigidbody>().drag = originalDrag;
            }
            else if (form == hummingbird)
            {
                frog.SetActive(false);
                panda.SetActive(false);
                state = State.Fly;
            }
            form.SetActive(true);

            //start transform_begin animation state

            yield return new WaitForSeconds(0.1f);//length of transform_begin);

            transforming = false;
        }
    }

    public IEnumerator TongueConnect(GameObject joint)
    {
        while(tongueAttachedObj != null)
        {
            Vector2 diff = new Vector2(joint.transform.position.x - gameObject.transform.position.x, joint.transform.position.y - gameObject.transform.position.y);
            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            gameObject.transform.eulerAngles = new Vector3(0f, 0f, angle);
            yield return null;
        }
    }

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Ground" || other.gameObject.tag == "MovingPlatform")
        {
            if (state != State.Run)
                StartCoroutine("ChangeForm", panda);
            gameObject.GetComponent<Rigidbody>().drag = originalDrag;
            deltaSum = 0;
            grounded++;
            if(other.gameObject.tag == "MovingPlatform")
            {
                gameObject.transform.parent = other.gameObject.transform;
            }
		}
        if(other.gameObject.tag == "Water")
        {
            Application.LoadLevel(Application.loadedLevel);
        }
	}

	void OnTriggerStay(Collider other)
	{
		if(grounded == 0 && (other.gameObject.tag == "Ground" || other.gameObject.tag == "MovingPlatform"))
			grounded = 1;
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Ground" || other.gameObject.tag == "MovingPlatform")
        {
            grounded--;
            if(other.gameObject.tag == "MovingPlatform")
            {
                gameObject.transform.parent = null;
            }
        }
	}
}