using UnityEngine;
using System.Collections;

public class Return : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void  Update_load3 () {
		Application.LoadLevelAsync ("1");
	}
}
