using UnityEngine;
using System.Collections;

namespace MainScene
{
	public class OutDoor : MonoBehaviour
	{

		#region static instance
		private static OutDoor s_Instance;
		public static OutDoor Instance	{ get{ return s_Instance; } }
		#endregion
		
		#region public member variables for Untiy
		//
		public OrbitCamera m_OrbitCamera = null;
		//
		public TweenAlpha m_ButtonClose = null;
		public TweenAlpha m_ButtonLighting = null;
		//
		public int m_IDAlertWelcome = -1;
		//
		public int m_IDAlertHintDrag = -1;
		public float m_AlertHintDragInterval = 5;
		//
		public int m_IDAlertHint = -1;
		public float m_AlertHintInterval = 5;
		#endregion
		
		#region private member variables
		//
		private Menu m_Menu = null;
		//
		private bool m_bTweening = false;
		//
		private UIAlert m_AlertWelcome = null;
		private bool m_bHasWelcome = false;
		//
		private UIAlert m_AlertHintDrag = null;
		private float m_fAlertHintDragLastTime = 0;
		private bool m_bHasHintDrag = false;
		//
		private UIAlert m_AlertHint = null;
		private float m_fAlertHintLastTime = 0f;

		//
		private bool m_bOnIndicatorHoverOver = false;
		//
		private bool m_bInit = false;
		#endregion
		
		#region public methods
		//
		public void HideHintDrag()
		{
			//
			if( !m_bInit )
				return;

			//
			if( m_bHasHintDrag )
				return;

			//
			if( !m_AlertWelcome.GetVisible() )
				m_bHasHintDrag = true;

			//
			if( m_AlertHintDrag.GetVisible() )
				m_AlertHintDrag.Hide( 0.3f );

			//
			m_fAlertHintLastTime = Time.time;
		}

		//
		public void ShowButtons()
		{
			//
			if( !m_bInit )
				return;

			//
			m_ButtonClose.PlayForward();
			m_ButtonClose.ResetToBeginning();

			//
			m_ButtonLighting.PlayForward();
			m_ButtonLighting.ResetToBeginning();
		}

		//
		public void HideButtons()
		{
			//
			if( !m_bInit )
				return;

			m_ButtonClose.PlayReverse();
			m_ButtonLighting.PlayReverse();
		}
		#endregion
		
		#region private functions
		//
		private void OnMenuOpen( Menu menu )
		{
			//
			if( !m_bInit )
				return;
			
			//
			m_bTweening = true;

			//
			m_bOnIndicatorHoverOver = false;

			//
			m_bHasWelcome = false;

			//
			m_bHasHintDrag = false;

			//
			HideButtons();
		}
		
		//
		private void OnMenuOpened( Menu menu )
		{
			//
			if( !m_bInit )
				return;

			//
			//m_AlertWelcome.Show( 1.0f );
			            
            //
            CKinect.CursorController.Instance.EnableCursor();
            
            //
            //CKinect.CursorController.Instance.IsLoadingActive = false;
            
            //
            m_bTweening = false;

			//
			ShowButtons();

			//
			m_bHasWelcome = true;
			m_AlertHintDrag.Show( 1.0f );
			m_fAlertHintLastTime = Time.time;
			CKinect.CursorController.Instance.IsLoadingActive = true;
			m_OrbitCamera.Enable();
			m_fAlertHintDragLastTime = Time.time;
		}

		//
		private void OnCloseAlertWelcome()
		{
			if( !m_bInit )
				return;

			//
			m_bHasWelcome = true;

			//
			m_fAlertHintLastTime = Time.time;
			
			//
			CKinect.CursorController.Instance.IsLoadingActive = true;

			//m_OrbitCamera.Reset( m_OrbitCameraDistance, m_Camera.transform.eulerAngles );
			m_OrbitCamera.Enable();

			//
			//Debug.Log( "IsLoadingActive = true;" );

			//
			m_fAlertHintDragLastTime = Time.time;
		}

		//
		private void OnCloseAlertHintDrag()
		{
			if( !m_bInit )
				return;
			
			//
			m_fAlertHintDragLastTime = Time.time;
		}

		//
		private void OnCloseAlertHint()
		{
			if( !m_bInit )
				return;
			
			//
			m_fAlertHintLastTime = Time.time;
		}

		//
		private void OnIndicatorHoverOver()
		{
			//
			if( !m_bInit )
				return;

			if( m_AlertHint.GetVisible() )
				m_AlertHint.Hide( 0.3f );

			//
			m_fAlertHintLastTime = Time.time;

			m_bOnIndicatorHoverOver = true;
		}

		//
		private void OnIndicatorHoverOut()
		{
			//
			if( !m_bInit )
				return;

			//
			m_fAlertHintLastTime = Time.time;

			m_bOnIndicatorHoverOver = false;
		}
		#endregion
		
		#region private Unity functions
		//
		private void Awake()
		{
			//
			s_Instance = this;
		}
		
		// Use this for initialization
		private void Start()
		{
			//
			if( m_ButtonClose == null )
			{
				Debug.LogError( "The ButtonClose has not assigned.", this );
				return;
			}

			//
			if( m_ButtonLighting == null )
			{
				Debug.LogError( "The ButtonLighting has not assigned.", this );
				return;
			}

			//
			m_Menu = this.GetComponent<Menu>();
			
			//
			if( m_Menu == null )
			{
				Debug.LogError( "This GameObject has not assigned Component:'Menu'.", this );
				return;
			}
            
            //
			m_Menu.OnOpen += this.OnMenuOpen;
			m_Menu.OnOpened += this.OnMenuOpened;

			//
			if( m_OrbitCamera == null )
				m_OrbitCamera = Camera.main.GetComponent<OrbitCamera>();
			
			//
			if( m_OrbitCamera == null )
			{
				Debug.LogError( "The OrbitCamera has not assigned.", this );
				return;
			}

			//
			StartCoroutine( DelayInit() );
		}
		
		//
		private IEnumerator DelayInit()
		{
			yield return null;

			//
			m_AlertWelcome = UIAlertManager.Instance.GetAlert( m_IDAlertWelcome );
			
			if( m_AlertWelcome == null )
			{
				Debug.LogError( "There's not a UIAlert -- IDAlertWelcome:" + m_IDAlertWelcome, this );
				yield break;
			}

			//
			m_AlertWelcome.OnClose = this.OnCloseAlertWelcome;

			//
			m_AlertHintDrag = UIAlertManager.Instance.GetAlert( m_IDAlertHintDrag );
			
			if( m_AlertHintDrag == null )
			{
				Debug.LogError( "There's not a UIAlert -- IDAlertHintDrag:" + m_IDAlertHintDrag, this );
				yield break;
			}
			
			m_AlertHintDrag.OnClose = this.OnCloseAlertHintDrag;
			
			//
			m_AlertHint = UIAlertManager.Instance.GetAlert( m_IDAlertHint );
			
			if( m_AlertHint == null )
			{
				Debug.LogError( "There's not a UIAlert -- IDAlertHint:" + m_IDAlertHint, this );
				yield break;
			}
			
			//
			m_AlertHint.OnClose = this.OnCloseAlertHint;

			//
			IndicatorManager.Instance.OnHoverOver = this.OnIndicatorHoverOver;
			IndicatorManager.Instance.OnHoverOut = this.OnIndicatorHoverOut;
			
			m_bInit = true;
		}

		private void OnDestroy()
		{
			// 是否初始化
			if( !m_bInit )
				return;
			
			m_Menu.OnOpen -= this.OnMenuOpen;
			m_Menu.OnOpened -= this.OnMenuOpened;
		}

		//
		private void FixedUpdate()
		{
			// 是否初始化
			if( !m_bInit )
				return;

			//
			if( !m_Menu.hasOpened )
				return;

			//
			if( !m_bHasWelcome )
				return;
			
			//
			//if( m_AlertWelcome.GetVisible() )
			//	return;

			//
			if( !m_bHasHintDrag && !m_AlertHintDrag.GetVisible() )
			{
				//
				if( Time.time - m_fAlertHintDragLastTime > m_AlertHintDragInterval )
				{
					m_fAlertHintDragLastTime = Time.time;
					
					m_AlertHintDrag.Show( 0.5f );
				}
				
				return;
			}
			
			//
			//if( m_bHasHintDrag && !m_AlertHint.GetVisible() )
			//{
			//	// always update when hover over
			//	if( m_bOnIndicatorHoverOver )
			//		m_fAlertHintLastTime = Time.time;
			//
			//	//
			//	if( Time.time - m_fAlertHintLastTime > m_AlertHintInterval )
			//	{
			//		m_fAlertHintLastTime = Time.time;
			//		
			//		m_AlertHint.Show( 0.5f );
			//	}
			//	
			//	return;
			//}
		}
		#endregion
	} // class OverallView
} // namespace MainScene