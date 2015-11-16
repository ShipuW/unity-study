using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EasterEgg : MonoBehaviour 
{
	#region static instance
	private static EasterEgg s_Instance;
	public static EasterEgg Instance  { get{ return s_Instance; } }
	#endregion

	public List<GameObject> m_EasterEggGameObject;


	public List<GameObject> EasterEggGameObject{get {return m_EasterEggGameObject;}}
	private bool bIsRunning = false;

	void Awake()
	{
		s_Instance = this;
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void Run(GameObject go)
	{
		//Debug.Log("run egg + " + IsEgg(go));
		//if(IsEgg(go) && !bIsRunning)
		//{
			DoFeedBack(go);
		//}
	}

	public bool IsEgg(GameObject go)
	{
		foreach(GameObject g in EasterEggGameObject )
		{
			if(go == g)
			{
				//Debug.Log("egg : " + go.name);
				return true;
			}
		}

		return false;
	}

	private void DoFeedBack(GameObject go)
	{
		//bIsRunning = true;

//		int f =  Random.Range(-2,2) * 20;
//
//		if(go.rigidbody == null)
//			return;
//		go.rigidbody.AddForceAtPosition((-Physics.gravity * 5 + Vector3.left * f),go.transform.localPosition * 1.1f);
		//go.rigidbody.AddForce(Vector3.left * 20);


		if(go.rigidbody == null)
			return;

		int force = Random.Range(20,30);

		go.rigidbody.AddForceAtPosition(Camera.main.transform.forward * force,go.transform.localPosition * 1.1f);

	}
}
