using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void OnHoverIn()
	{
		//UICamera.hoveredObject.name;
		Debug.Log("------------->OnHoverIn " + UICamera.hoveredObject + "   " + UICamera.isOverUI + "   " + (UICamera.currentTouch.current != UICamera.fallThrough));
	}

	public void OnHoverOut()
	{
		Debug.Log("------------->OnHoverOut ");
	}
}
