﻿using UnityEngine;
using System.Collections;

public class go_to_use : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void Update_load1 () {
		Application.LoadLevelAsync ("Main");
	}
}
