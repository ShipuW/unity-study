using UnityEngine;
using System.Collections;

public class GuiMouse : MonoBehaviour {

    public Texture bkimg;
    public Texture mouse_pic;
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        GUI.depth = 0;

        GUI.DrawTexture(new Rect(Screen.width - 27, 5, 22, 22), bkimg);
        GUI.DrawTexture(new Rect(Screen.width / 2 - 89 + 163, Screen.height / 2 - 51 + 3, 12, 12), bkimg);
        GUI.DrawTexture(new Rect(Screen.width / 2 - 89 + 48, Screen.height / 2 - 51 + 78, 87, 14), bkimg);
        
        if (new Rect(Screen.width - 27, 5, 22, 22).Contains(Event.current.mousePosition) ||
         new Rect(Screen.width / 2 - 89 + 163, Screen.height / 2 - 51 + 3, 12, 12).Contains(Event.current.mousePosition) ||
            new Rect (Screen.width / 2 - 89+48, Screen.height / 2 - 51+78, 87,14).Contains(Event.current.mousePosition) )
         //new Rect(SelectModel.guipos.x + 48, SelectModel.guipos.y + 78, 87, 14).Contains(Event.current.mousePosition))
        {
            Screen.showCursor = false;
            Vector2 mouse_pos = Event.current.mousePosition;
            GUI.DrawTexture(new Rect(mouse_pos.x - 4, mouse_pos.y, 17, 22), mouse_pic);
        }
        else
        {
            Screen.showCursor = true;
        }

    }
}
