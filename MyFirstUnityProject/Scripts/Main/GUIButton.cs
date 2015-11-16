using UnityEngine;  
using System.Collections;  

public class GUIButton : MonoBehaviour   
{ 
	// 按钮状态  
	protected enum StateID  
	{ 
		NORMAL=0,   // 正常  
		FOCUS,       //  高亮  
		ACTIV,       //  选中  
	} 
	protected StateID m_state = StateID. NORMAL;  
	// 按钮的贴图  
	public Texture[] m_ButtonSkin;  
	
	// 按钮的ID  
	public int m_ID = 0;  
	
	// 按钮是否处于激活状态  
	protected bool m_isOnActiv = false;  
	
	// 按钮的缩放  
	public float m_scale = 1.0f;  
	
	// 按钮的屏幕位置  
	Vector2 m_screenPosition;  
	
	// 按钮的当前贴图  
	public GUITexture m_texture;  
	
	// 初始化按钮  
	void Awake() 
	{ 
		//  获得贴图  
		m_texture = this.guiTexture;  
		
		// 获得位置  
		m_screenPosition = new Vector3(m_texture.pixelInset.x, m_texture.pixelInset.y, 0);  
		
		// 设置默认状态  
		SetState(StateID.NORMAL);  
	} 
	
	//更新按钮状态，选中按钮，返回它的ID  
	public int UpdateState(bool mouse,Vector3 mousepos) 
	{ 
		int result = -1;  
		
		if (m_texture.HitTest(mousepos)) 
		{ 
			if (mouse) 
			{ 
				SetState(StateID.ACTIV);  
				
				return m_ID;  
			} 
			else
				SetState(StateID.FOCUS);  
			
		} 
		else
		{ 
			if (m_isOnActiv) 
				SetState(StateID.ACTIV);  
			else
				SetState(StateID.NORMAL);  
		} 
		
		return result;  
	} 
	
	// 设置按钮状态  
	protected virtual void SetState(StateID state) 
	{ 
		if (m_state == state) 
			return;  
		
		m_state = state;  
		
		m_texture.texture = m_ButtonSkin[(int)m_state];  
		
		float w = m_ButtonSkin[(int)m_state].width * m_scale;  
		float h = m_ButtonSkin[(int)m_state].height * m_scale;  
		
		m_texture.pixelInset = new Rect(this.m_screenPosition.x, m_screenPosition.y, w, h);  
	} 
	
	// 设置按钮缩放  
	public virtual void SetScale(float scale) 
	{ 
		m_scale = scale;  
		
		float w = m_ButtonSkin[0].width * scale;  
		float h = m_ButtonSkin[0].height * scale;  
		
		m_screenPosition.x *= scale;  
		m_screenPosition.y *= scale;  
		
		m_texture.pixelInset = new Rect(m_screenPosition.x, m_screenPosition.y, w, h);  
	} 
	
	// 设置激活状态  
	public virtual void SetOnActiv(bool isactiv) 
	{ 
		if (isactiv) 
			SetState(StateID.ACTIV);  
		else if (m_isOnActiv) 
			SetState(StateID.NORMAL);  
		
		m_isOnActiv = isactiv;  
	} 
}