using UnityEngine;
using System.Collections;

public class UIAlert : MonoBehaviour 
{	
	#region public member variables for Untiy
	//
	public int m_ID = -1;
	//
	public float m_AutoHideDelay = -1;
	//
	public UIAlertComponent[] m_ComponentsNeedToReset = null;
	
	//
	public delegate void VoidDelegate();
	public VoidDelegate OnClose;
	#endregion
	
	#region private member variables
	//
	private TweenAlpha m_TweenAlpha = null;
	//
	private float m_fTweenAlphaDuration = 1f;
	//
	private bool m_bInit = false;
	#endregion
	
	#region public properties
	public int ID { get{ return m_ID; } set{ m_ID = value; } }
	#endregion
	
	#region public methods
	//
	public void Reset()
	{
		if( !m_bInit )
			return;
		
		//
		m_TweenAlpha.delay = 0f;
		m_TweenAlpha.ResetToBeginning();
		UIWidget widget = this.GetComponent<UIWidget>();
		if( widget != null )
			widget.alpha = m_TweenAlpha.from;
		
		//
		for( int i = 0; i < m_ComponentsNeedToReset.Length; i++ )
		{
			m_ComponentsNeedToReset[i].Reset();
		}
	}
	
	//
	public void Show( float fFadeDuration = 0 )
	{
		//
		if( !m_bInit )
			return;
		
		//
		this.gameObject.SetActive( true );
		
		//
		Reset();
		
		//
		if( fFadeDuration > 0 )
		{
			//
			m_TweenAlpha.from = 0;
			m_TweenAlpha.to = 1;
			m_TweenAlpha.duration = fFadeDuration;
			
			m_TweenAlpha.SetOnFinished( this.OnFadeInComplete );
			
			m_TweenAlpha.PlayForward();
			
			return;
		}
		
		//
		m_TweenAlpha.SetOnFinished( this.Close );
		
		//
		if( m_AutoHideDelay > 0 )
		{
			m_TweenAlpha.delay = m_AutoHideDelay;
			m_TweenAlpha.PlayForward();
		}
	}
	
	//
	public void Hide( float fFadeDuration = 0 )
	{
		//
		if( fFadeDuration > 0 )
		{
			m_TweenAlpha.enabled = false;
			m_TweenAlpha.ResetToBeginning();
			m_TweenAlpha.SetStartToCurrentValue();
			m_TweenAlpha.to = 0;
			m_TweenAlpha.delay = 0f;
			m_TweenAlpha.duration = fFadeDuration;
			EventDelegate.Remove( m_TweenAlpha.onFinished, this.OnFadeInComplete );
			m_TweenAlpha.PlayForward();
			
			return;
		}
		
		//
		Close();
	}
	
	//
	public void Close()
	{
		//
		this.gameObject.SetActive( false );
		
		//
		if( this.OnClose != null )
			this.OnClose();
	}
	
	//
	public bool GetVisible()
	{
		return this.gameObject.activeSelf;
	}
	#endregion
	
	#region private functions
	//
	private void OnFadeInComplete()
	{
		m_TweenAlpha.from = 1;
		m_TweenAlpha.to = 0;
		m_TweenAlpha.duration = m_fTweenAlphaDuration;
		
		m_TweenAlpha.SetOnFinished( this.Close );
		
		m_TweenAlpha.ResetToBeginning();
		
		//
		if( m_AutoHideDelay > 0 )
		{
			m_TweenAlpha.delay = m_AutoHideDelay;
			m_TweenAlpha.PlayForward();
		}
	}
	#endregion
	
	#region private Unity functions
	// Use this for initialization
	private void Start()
	{
		//
		m_TweenAlpha = this.GetComponent<TweenAlpha>();
		
		if( m_TweenAlpha == null )
		{
			Debug.LogError( "There's not a Component:'TweenAlpha'.", this );
			return;
		}
		
		// save old
		m_fTweenAlphaDuration = m_TweenAlpha.duration;
		
		//
		m_ComponentsNeedToReset = this.GetComponentsInChildren<UIAlertComponent>(true);
		
		//
		if( m_ComponentsNeedToReset == null || m_ComponentsNeedToReset.Length <= 0 )
		{
			Debug.LogWarning( "There's not any UIAlertComponent in UIAlert ID:"+m_ID, this );
		}
		
		//
		m_bInit = true;
	}
	#endregion
}

