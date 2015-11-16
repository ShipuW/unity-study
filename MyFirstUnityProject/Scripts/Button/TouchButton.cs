﻿using UnityEngine;
using System.Collections;
/// <summary>
/// 该类对按钮进行控制
/// </summary>
public class TouchButton : MonoBehaviour {

	public int m_ID = -1;
	///委托.
	public delegate void VoidDelegate(TouchButton btn);
	public VoidDelegate OnClicked = null;

	private bool m_bInit = false;

	public int ID {get{return m_ID;}}


	public void Show(){
		if (!m_bInit)
			return;
	
		if (!this.gameObject.activeSelf)
			this.gameObject.SetActive (true);

	}
	public void Hide(){
		if (!m_bInit)
			return;
		
		if (this.gameObject.activeSelf)
			this.gameObject.SetActive (false);

	}

	private void OnClick(){
		if (this.OnClicked != null)
			this.OnClicked (this);
	}
	// Use this for initialization
	void Start () {
		m_bInit = true;
	}
}
