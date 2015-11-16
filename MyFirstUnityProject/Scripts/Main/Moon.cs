using UnityEngine;
using System.Collections;

public class Moon : MonoBehaviour
{

	public float rotatespeed = 20f;
	public static float spinspeed = 10f;
	public static float spinspeedtoearth = 20f;
	public Transform thesun;
	public Transform theearth;
	private Ray ray;
	private RaycastHit hit;
   
		
	// Update is called once per frame
	void Update ()
	{
		//ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		//rotation
		transform.Rotate (Vector3.up * rotatespeed * Time.deltaTime);
		//if (!Physics.Raycast (ray, out hit, 100)) 
		{


			//revolution
			transform.RotateAround (Vector3.zero, Vector3.up, spinspeed * Time.deltaTime);

        
			transform.RotateAround (theearth.position, theearth.up, spinspeedtoearth * Time.deltaTime);
	
		}
	}
}
