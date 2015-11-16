using UnityEngine;
using System.Collections;

public class ChangeMouseCursor : MonoBehaviour {
	public Texture mouseTexture;
	// Use this for initialization
	void Start () {
		//Screen.showCursor = false;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI () {
		Vector3 mousePos = Input.mousePosition;
		GUI.DrawTexture(new Rect(mousePos.x-20,Screen.height-mousePos.y-40,(mouseTexture.width/2.5f),(mouseTexture.height/2.5f)),mouseTexture);
	}
}
