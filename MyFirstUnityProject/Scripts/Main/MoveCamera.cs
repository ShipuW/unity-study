using UnityEngine;
using System.Collections;


public class MoveCamera : MonoBehaviour {
	private bool isSun=false;
	private bool isMercury=false;
	private bool isVenus=false;
	private bool isEarth=false;
	private bool isMoon=false;
	private bool isMars=false;
	private bool isJupiter=false;
	private bool isSaturn=false;
	private bool isUranus=false;
	private bool isNeptune=false;
	private bool entered = false;

	public Transform sun;
	public Transform mercury;
	public Transform venus;
	public Transform earth;
	public Transform moon;
	public Transform mars;
	public Transform jupiter;
	public Transform saturn;
	public Transform uranus;
	public Transform neptune;
	public GameObject enterButton;


	public float edgeBorder = 0.1f;
	public float horizontalSpeed = 360.0f;
	public float horizontalSpeedMultify = 0.1f;
	public float verticalSpeed = 120.0f;
	public float minVertical = 20.0f;
	public float maxVertical = 85.0f;
	public float cameraShiftHorizon = 2.0f;
	public float cameraShiftVertical = 2.0f;
	public bool  autoRotate = true;
	
	
	public float MouseWheelSensitivity = 10.0f;
	public float MouseZoomMin = 1.0f;
	public float MouseZoomMax = 7.0f;
	
	private float x = 0.0f;
	private float y = 0.0f;
	private float distance = 0.0f;
	private float verticalValue=0.0f;
	private float horizontalValue=0.0f;
	private float dt;
	
	//public GUISkin New_skin2;
	//public GUISkin New_skin3;
	
	//public Texture2D backMap;
	public GUIStyle   btnStyle1;
	public GUIStyle   btnStyle2;
	public GUIStyle   btnStyle3;
	public GUIStyle   btnStyle4;
	public GUIStyle   btnStyle5;
	
	private Vector3 frompos;
	private Quaternion fromrot;
	private Vector3 topos;
	private Quaternion torot;
	private bool movecamera = false;
	private float springiness = 4.0f;
	private float movecamerabegintime;
	private bool isenabled = false;
	private const float movelasttime = 2f;
	private bool leftMove;
	private bool rtMove;
	private bool upMove;
	private bool dnMove;

	// Use this for initialization
	void Start () {
		MoveCamera0 ();
		enterButton.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

	}
	void LateUpdate()
	{
		//onClick ();
		//float dt = Time.deltaTime;
		dt = Time.deltaTime;
		if (isEarth) {
			topos = new Vector3 (34f, 0.65f, 39.33f);
		}
//		if (movecamera) {
//			float ratio;
//			ratio = (Time.time - movecamerabegintime) / movelasttime;
//			if (ratio > 1)
//				ratio = 1f;
//			//transform.position = Vector3.Lerp(transform.position, topos, Time.deltaTime * springiness);
//			transform.position = Vector3.Lerp (frompos, topos, ratio);
//			// transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, Time.time * speed);
//			transform.rotation = Quaternion.Lerp (fromrot, torot, ratio);
//			
//			x = transform.eulerAngles.y;
//			y = transform.eulerAngles.x;
//			distance = (transform.position - sun.position).magnitude;
//			
//			//print((transform.position - topos).magnitude);
//			
//			//if ((transform.position - topos).magnitude < 0.001f)
//			if (Time.time - movecamerabegintime > movelasttime + 0.5f) {
//				x = transform.eulerAngles.y;
//				y = transform.eulerAngles.x;
//				distance = (transform.position - sun.position).magnitude;
//				horizontalValue = 0f;
//				verticalValue = 0f;
//				
//				//transform.position = topos;
//				//transform.rotation = torot;
//				movecamera = false;
//				isenabled = true;
//				// print(movecamera);
//				
//			}
//			
//		}
		if (!movecamera) {
			if (isSun) {
				transform.position = sun.position + new Vector3 (1, 20, 1);
				transform.LookAt (sun);
				if(entered){
					Application.LoadLevel(2);
				}
			}
				else if (isEarth) {
					transform.position = earth.position + new Vector3 (1, 1, 1);
					transform.LookAt (earth);
				earth.Rotate (Vector3.up * 0.1f * Time.deltaTime, Space.Self);
				earth.RotateAround (new Vector3 (0, 1f, 0), 0.01f * Time.deltaTime);
				} 
				else if (isMercury) {
					transform.position = mercury.position + new Vector3 (1, 1, 1);
					transform.LookAt (mercury);
					mercury.Rotate (Vector3.up * 0.01f * Time.deltaTime, Space.Self);
					mercury.RotateAround (new Vector3 (0, 1f, 0), 0.001f * Time.deltaTime);
				}
				else if (isVenus) {
					transform.position = venus.position + new Vector3 (1, 1, 1);
					transform.LookAt (venus);
					venus.Rotate (Vector3.up * 0.1f * Time.deltaTime, Space.Self);
					venus.RotateAround (new Vector3 (0, 1f, 0), 0.01f * Time.deltaTime);
				}
				else if (isMoon) {
					transform.position = moon.position + new Vector3 (0.2f, 0.2f, 0.2f);
					transform.LookAt (moon);
					moon.Rotate (Vector3.up * 0.1f * Time.deltaTime, Space.Self);
					moon.RotateAround (new Vector3 (0, 1f, 0), 0.01f * Time.deltaTime);
				}
				else if (isMars) {
					transform.position = mars.position + new Vector3 (0.5f, 0.5f, 0.5f);
					transform.LookAt (mars);
				}
				else if (isJupiter) {
					transform.position = jupiter.position + new Vector3 (2, 2, 2);
					transform.LookAt (jupiter);
				}
				else if (isSaturn) {
					transform.position = saturn.position + new Vector3 (1, 1, 1);
					transform.LookAt (saturn);
				}
				else if (isUranus) {
					transform.position = uranus.position + new Vector3 (1, 1, 1);
					transform.LookAt (uranus);
				} else if (isNeptune) {
					transform.position = neptune.position + new Vector3 (1, 1, 1);
					transform.LookAt (neptune);
				} else {
					Quaternion rotation = Quaternion.Euler (y, x, 0);
				
					transform.position = rotation * new Vector3 (horizontalValue, verticalValue, -distance) + sun.position;
				
					transform.rotation = rotation;
					//onClick();
				}
			}

		}

	public void MoveToSun(){
		movecamera = !movecamera;
		isSun =!isSun;
		Application.LoadLevelAsync ("Sun");
	}

	public void MoveToMercury(){
		movecamera = !movecamera;
		isMercury =!isMercury;
	}
	public void MoveToVenus(){
		movecamera = !movecamera;
		isVenus =!isVenus;
	}
	public void MoveToEarth(){
		movecamera = !movecamera;
		isEarth =!isEarth;
		Application.LoadLevelAsync ("Earth");
	}
	public void MoveToMoon(){
		movecamera = !movecamera;
		isMoon =!isMoon;
		Application.LoadLevelAsync ("Moon");
	}
	public void MoveToMars(){
		movecamera = !movecamera;
		isMars =!isMars;
	}
	public void MoveToJupiter(){
		movecamera = !movecamera;
		isJupiter =!isJupiter;
	}
	public void MoveToSaturn(){
		movecamera = !movecamera;
		isSaturn =!isSaturn;
	}
	public void MoveToUranus(){
		movecamera = !movecamera;
		isUranus =!isUranus;
	}
	public void MoveToNeptune(){
		movecamera = !movecamera;
		isNeptune =!isNeptune;
	}

	public void backStartScene(){
		Application.LoadLevelAsync ("Start");
	}

	public void isEnter(){
		entered = !entered;
	}

	void MoveCamera0()
	{
		
		topos = new Vector3(27f, 5f, 39.33f);
		torot = new Quaternion(0.0077f, -0.9548f, 0.0048f, 0.2960f);
		frompos = transform.position;
		fromrot = transform.rotation;
		movecamerabegintime = Time.time;
		movecamera = true;
	}
}
