using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

    public GameObject[] Points;
    public int movingTowards = 0;

    public float speed = 1;

    private Vector3[] Points2;

	// Use this for initialization
	void Start () {
        Points2 = new Vector3[Points.Length];
        for(int i = 0; i < Points.Length; i++)
        {
            Points2[i] = Points[i].transform.position;
            Destroy(Points[i]);
        }

	}
	
	// Update is called once per frame
	void Update () {
        if(Vector3.Distance(transform.position,Points2[movingTowards]) < 0.5f)
        {
            if(movingTowards + 1 == Points2.Length)
            {
                movingTowards = 0;
            }
            else
            {
                movingTowards++;
            }
        }

        Vector3 direction;

        direction = Points2[movingTowards] - transform.position;
        direction.Normalize();

        transform.Translate(direction * speed * Time.deltaTime);
	
	}
}
