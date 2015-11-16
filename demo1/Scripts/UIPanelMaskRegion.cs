using UnityEngine;
using System.Collections;

public class UIPanelMaskRegion : MonoBehaviour 
{

	public bool m_IsFullScreen = true;

	public Vector2 m_Region;

	public Vector2 m_Postion;

	// Use this for initialization
	void Start () 
	{
		UIPanel panel = this.GetComponent<UIPanel>();

		if(panel == null)
		{
			Debug.LogError("Can't find UIpanel in gameobject " + this.gameObject.name,this);
			return;
		}

		if(m_IsFullScreen)
		{
			panel.baseClipRegion = new Vector4(0,0,Screen.width,Screen.height);
		}
		else
		{
			panel.baseClipRegion = new Vector4(m_Postion.x,m_Postion.y,m_Region.x,m_Region.y);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
