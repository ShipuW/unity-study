using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{		
	#region public member variables for Untiy
	//
	public int m_ID = -1;
	//
	//public BoxCollider m_ButtonClose = null;
	//
	public UI.UICloseButton m_CloseButton = null;
	//
	//public Vector3 m_From = Vector3.zero;
	//public Vector3 m_To = Vector3.zero;
	
	//
	//		public TweenInStyle m_TweenInStyle;
	//		public TweenOutStyle m_TweenOutStyle;
	//
	//		//
	//		public enum TweenInStyle
	//		{
	//			FromLeft,
	//			FromRight,
	//			FromTop,
	//			FromBottom,
	//			FadeIn
	//		}
	//
	//		//
	//		public enum TweenOutStyle
	//		{
	//			ToLeft,
	//			ToRight,
	//			ToTop,
	//			ToBottom,
	//			FadeOut
	//		}
	
	#endregion
	
	#region public member variables
	// 菜单打开回调
	//
	public delegate void VoidDelegate( Menu menu );
	public event VoidDelegate OnOpen, OnOpened, OnClose;
	#endregion
	
	#region private member variables
	//
	private TweenPosition m_TweenPosition = null;
	//
	private bool m_bTweenning = false;
	//
	private bool m_bInit = false;
	#endregion
	
	#region public properties
	//
	public int ID { get{ return m_ID; } set{ m_ID = value; } }
	//
	public bool hasOpened { get; private set; }
	#endregion
	
	
	#region public methods
	//
	public void TweenIn()
	{
		//
		TweenIn(0);
	}
	
	//
	public void TweenIn( float fDelay )
	{
		//
		if( !m_bInit )
			return;
		
		//
		if( m_bTweenning )
			return;
		
		//
		if( !this.gameObject.activeSelf )
			this.gameObject.SetActive( true );
		
		m_bTweenning = true;
		
		//
		m_TweenPosition.SetOnFinished( this.OnTweenInFinished );
		
		//
		m_TweenPosition.delay = fDelay;
		
		//
		m_TweenPosition.PlayForward();
		
		//
		if( OnOpen != null )
			OnOpen( this );
	}
	
	//
	public void TweenOut()
	{
		//
		TweenOut(0);
	}
	
	//
	public void TweenOut( float fDelay )
	{
		//
		if( !m_bInit )
			return;
		
		//
		if( m_bTweenning )
			return;
		
		//
		this.hasOpened = false;
		
		//
		m_bTweenning = true;
		//
		m_TweenPosition.SetOnFinished( this.OnTweenOutFinished );
		//
		m_TweenPosition.delay = fDelay;
		//
		m_TweenPosition.PlayReverse();
		
		//
		UIAlertManager.Instance.Hide();
	}
	
	#endregion
	
	#region private functions
	//
	private void OnTweenInFinished()
	{
		//
		m_bTweenning = false;
		
		//
		this.hasOpened = true;
		
		//
		if( OnOpened != null )
			OnOpened( this );
		
		m_CloseButton.OnClicked = this.OnCloseButton;
		//m_CloseButton.gameObject.SetActive( true );
	}
	
	//
	private void OnTweenOutFinished()
	{
		m_bTweenning = false;
		
		this.gameObject.SetActive( false );
		
		if( OnClose != null )
			OnClose( this );
		
		m_CloseButton.OnClicked = this.OnCloseButton;
		//m_CloseButton.gameObject.SetActive( false );
	}
	
	//
	private void OnButtonClose( GameObject go )
	{
		TweenOut();
	}
	
	//
	private void OnCloseButton()
	{
		TweenOut();
	}
	#endregion
	
	#region private Unity functions
	// Use this for initialization
	private void Start ()
	{
		//
		m_TweenPosition = this.GetComponent<TweenPosition>();
		//
		if( m_TweenPosition == null )
		{
			Debug.LogError( "There's NOT Component:'TweenPosition'.", this );
			return;
		}
		
		//
		//			if( m_ButtonClose == null )
		//			{
		//				Debug.LogError( "The ButtonClose has not assigned.", this );
		//				return;
		//			}
		//
		//            //
		//            UIEventListener listener = UIEventListener.Get( m_ButtonClose.gameObject );
		//            listener.onClick = this.OnButtonClose;
		
		if( m_CloseButton == null )
		{
			Debug.LogError( "The CloseButton has not assigned.", this );
			return;
		}
		
		//
		this.hasOpened = false;
		
		//
		m_bInit = true;
	}
	#endregion
}

