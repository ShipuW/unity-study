using UnityEngine;
using System.Collections;

public class SelfRotate : MonoBehaviour {

	public float rotatespeed = 10f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
		transform.Rotate(Vector3.up * rotatespeed * Time.deltaTime, Space.Self);
	
	}
}
