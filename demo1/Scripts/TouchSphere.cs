using UnityEngine;
using System.Collections;

public class TouchSphere : MonoBehaviour
{
	public Vector3 m_Velocity = Vector3.zero;

	public Vector3 Velocity { get{ return m_Velocity; } private set{ m_Velocity = value; } }

	//
	private Vector3 m_vLastPos = Vector3.zero;

	// Use this for initialization
	private void Start ()
	{
		m_vLastPos = this.transform.position;
	}
	
	// Update is called once per frame
	private void Update ()
	{
		this.Velocity = ( this.transform.position - m_vLastPos ) / Time.deltaTime;
		m_vLastPos = this.transform.position;
	}
}
