using UnityEngine;
using System.Collections;

public class Earth : MonoBehaviour {

    public static float rotatespeed = 150f;
    public  float spinspeed = 30f;
    public Transform thesun;
	RaycastHit hit;
	Ray ray;
	
    
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		//rotation
		transform.Rotate (Vector3.up * rotatespeed * Time.deltaTime);
		//if (!Physics.Raycast (ray,out hit, 100))
		{

        
			//revolution
			transform.RotateAround (Vector3.zero, Vector3.up, spinspeed * Time.deltaTime);
		}
	}
}
