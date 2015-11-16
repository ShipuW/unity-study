using UnityEngine;
using System.Collections;

public class cru : MonoBehaviour {
	public Texture mousetexture;
	// Use this for initialization
	void Start () {
		Screen.showCursor = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI () {
		Vector3 mousepos = Input.mousePosition;
		GUI.DrawTexture (new Rect (mousepos.x, Screen.height - mousepos.y, mousetexture.width, mousetexture.height), mousetexture);
	}
}
