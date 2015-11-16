using UnityEngine;
using System.Collections;

public class TouchBox : MonoBehaviour
{

	// Use this for initialization
	private void Start ()
	{
	
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if( Input.GetKeyDown(KeyCode.A) )
		{
			this.gameObject.AddComponent<SpringJoint>();
		}

		if( Input.GetKeyDown(KeyCode.S) )
		{
			rigidbody.velocity = Vector3.zero;
			SpringJoint spring = this.gameObject.GetComponent<SpringJoint>();
			Destroy(spring);
		}
	}

	private void OnCollisionEnter(Collision collision) 
	{
		Debug.Log( collision.gameObject.name );
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log( other.gameObject.name );
	}
}
