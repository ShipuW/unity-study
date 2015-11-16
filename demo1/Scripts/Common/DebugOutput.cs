using UnityEngine;
using System.Collections;

public class DebugOutput : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	//
	private void OnClick()
	{
		Debug.Log( this.name+" OnClick." );
	}

	//
	private void OnPress(bool b)
	{
		Debug.Log( this.name+" OnPress:"+b );
	}

    //
    private void OnHover (bool isOver){
       
        Debug.Log( this.name+" OnHover." + isOver);
    }
}
