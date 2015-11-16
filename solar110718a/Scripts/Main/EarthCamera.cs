using UnityEngine;
using System.Collections;

public class EarthCamera : MonoBehaviour {
	public GameObject character;
	private Vector3 positionVector;
	private Vector3 lookVector;
	SmoothFollowerObj posFollow;
	SmoothFollowerObj lookFollow;
	private Vector3 lastVelocityDir;
	private Vector3 lastPos;
	// Use this for initialization
	void Start () {
		positionVector = new Vector3 (0, 2, 4);
		lookVector = new Vector3 (0, 0, 1.5f);
		posFollow = new SmoothFollowerObj (0.5f, 0.5f);
		lookFollow = new SmoothFollowerObj (0.1f, 0.0f);
		posFollow.Update (transform.position, 0, true);
		lookFollow.Update (character.transform.position, 0, true);
		lastVelocityDir = character.transform.forward;
		lastPos = character.transform.position;


	}
	
	// Update is called once per frame
	void LateUpdate () {
		lastVelocityDir += (character.transform.position - lastPos) * 8;
		lastPos = character.transform.position;
		lastVelocityDir += character.transform.forward * Time.deltaTime;
		lastVelocityDir = lastVelocityDir.normalized;
		Vector3 horizontal = transform.position - character.transform.position;
		Vector3 horizontal2 = horizontal;
		Vector3 vertical = character.transform.up;
		Vector3.OrthoNormalize (ref vertical, ref horizontal2);
		if (horizontal.sqrMagnitude > horizontal2.sqrMagnitude) {
			horizontal = horizontal2;
		}
		transform.position = posFollow.Update (
			character.transform.position + horizontal * Mathf.Abs (positionVector.z) + vertical * positionVector.y,
			Time.deltaTime);

		horizontal = lastVelocityDir;
		Vector3 look = lookFollow.Update (character.transform.position + horizontal * lookVector.z - vertical * lookVector.y, 
		                                  Time.deltaTime);
		transform.rotation = Quaternion.FromToRotation (transform.forward, look - transform.position)* transform.rotation;
	}
}
