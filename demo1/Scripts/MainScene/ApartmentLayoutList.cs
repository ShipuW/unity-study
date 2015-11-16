using UnityEngine;
using System.Collections;

namespace MainScene
{
	/// <summary>
	/// 房屋户型列表
	/// </summary>
	public class ApartmentLayoutList : MonoBehaviour 
	{
		#region static instance
		private static ApartmentLayoutList s_Instance;
		public static ApartmentLayoutList Instance	{ get{ return s_Instance; } }
		#endregion

		#region public member variables for Untiy
		//
		//public string m_WaveLeftGestureName = null;
		//public string m_WaveRightGestureName = null;
		//
		public BoxCollider m_InputPanelLeft = null;
		public BoxCollider m_InputPanelRight = null;
		//
		public UIGrid m_GridApartmentLayout = null;
		//
		public ApartmentLayoutItem m_GridDefaultItem = null;
		//
		public ApartmentLayoutItem[] m_GridItems = null;
		//
		public int m_IDAlertWelcome = -1;

		//
		public int m_IDAlertWaveHand = -1;
		public float m_AlertWaveHandInterval = 5;

		//
		public int m_IDAlertSelect = -1;
		public float m_AlertSelectInterval = 5;
		#endregion
		
		#region private member variables
		//
		private Menu m_Menu = null;
		//
		private bool m_bPrepareUI = false;
		//
		private UICenterOnChild m_UICenterOnChild = null;
		//
		private bool m_bTweening = false;
		//
		private bool m_bHasItemPrepared = false;
		private GameObject m_CurrentItem = null;
		private GameObject m_PreviousItem = null;
		private GameObject m_NextItem = null;
		private GameObject m_LastItem = null;
		//
		private UIAlert m_AlertWelcome = null;

		//
		private UIAlert m_AlertWaveHand = null;
		private float m_fAlertWaveHandLastTime = 0;
		private bool m_bHasWaveHand = false;

		//
		private UIAlert m_AlertSelect = null;
		private float m_fAlertSelectLastTime = 0;

		//
		private bool m_bInit = false;
		#endregion
		
		#region public methods
		#endregion
		
		#region private functions
		//
//		private void OnGesture( object sender, CKinect.GestureEventArgs e )
//		{
//			// 忽略可信度低于0.9的手势
//			if( e.DetectionConfidence < 0.8f )
//				return;
//			
//			//Debug.Log( "Name:" + e.GestureName + "  " +  e.DetectionConfidence);
//			
//			if( !this.gameObject.activeSelf )
//			{
//				return;
//			}
//
//			//
//			if( m_bTweening )
//				return;
//
//			//
//			if( !m_bPrepareUI )
//				return;
//			
//			//
//			if( e.GestureName.Equals( m_WaveLeftGestureName ) )
//			{
//				Debug.Log( "wave left--------------------------->" + e.DetectionConfidence );
//				
//				CenterNext();
//				
//			} 
//			else if( e.GestureName.Equals( m_WaveRightGestureName ) )
//			{
//				Debug.Log( "wave right--------------------------->" + e.DetectionConfidence );
//
//				CenterPrevious();
//			}
//		}

		//
		private void CenterNext()
		{
			Transform target = null;

			ApartmentLayoutItem item = m_UICenterOnChild.centeredObject.GetComponent<ApartmentLayoutItem>();

			//
			for( int i = 0; i < m_GridItems.Length; i++ )
			{
				if( m_GridItems[i] == item )
				{
					if( i < m_GridItems.Length - 1 )
						target = m_GridItems[i+1].transform;

					break;
				}
			}

			// can not to NEXT
			if( target == null )
				return;

			//
			m_UICenterOnChild.CenterOn( target );
		}

		//
		private void CenterPrevious()
		{
			Transform target = null;
			
			ApartmentLayoutItem item = m_UICenterOnChild.centeredObject.GetComponent<ApartmentLayoutItem>();
			
			//
			for( int i = 0; i < m_GridItems.Length; i++ )
			{
				if( m_GridItems[i] == item )
				{
					if( i > 0 )
						target = m_GridItems[i-1].transform;

					break;
				}
			}
			
			// can not to PREVIOUS
			if( target == null )
				return;
			
			//
			m_UICenterOnChild.CenterOn( target );
		}

		//
		private void OnMenuOpen( Menu menu )
		{
			//
			if( !m_bInit )
				return;

			//
			m_AlertWelcome.Show( 1.0f );

			//
			m_bHasWaveHand = false;

			//
			m_bTweening = true;

			//
			m_bPrepareUI = false;

			//
			m_GridApartmentLayout.gameObject.SetActive( false );
		}

		//
		private void OnMenuOpened( Menu menu )
		{
			//
			if( !m_bInit )
				return;

			//
			//m_UICenterOnChild.CenterOn( m_GridDefaultItem.transform );

			//
			CKinect.CursorController.Instance.EnableCursor();

			//
			CKinect.CursorController.Instance.IsLoadingActive = false;

			//
			m_bTweening = false;

			//
			m_bHasItemPrepared = true;
			m_LastItem = null;
		}

		//
		private void OnCenterStart( GameObject go )
		{
			if( !m_bInit )
				return;

			//
			m_bTweening = true;

			//
			if( m_bPrepareUI && m_AlertWaveHand.GetVisible() )
			{
				m_AlertWaveHand.Hide( 0.3f );
			}

			//
			if( m_bPrepareUI && m_AlertSelect.GetVisible() )
			{
				m_AlertSelect.Hide( 0.3f );
			}

			//
			m_fAlertSelectLastTime = Time.time;

			//
			m_LastItem = go;
		}

		//
		private void OnCenterFinished()
		{
			if( !m_bInit )
				return;

			//
			m_bTweening = false;

			//
			Debug.Log( "OnCenterFinished" );

			//
			if( m_bPrepareUI && !m_AlertWelcome.GetVisible() && !m_bHasWaveHand )
			{
				m_bHasWaveHand = true;
			}

			//
			m_fAlertSelectLastTime = Time.time;

			//
			if( !m_bPrepareUI )
				m_bPrepareUI = true;
		}

		//
		private void OnCloseAlertWelcome()
		{
			if( !m_bInit )
				return;

			//
			m_fAlertWaveHandLastTime = Time.time;

			//
			CKinect.CursorController.Instance.IsLoadingActive = true;

			//
			m_GridApartmentLayout.gameObject.SetActive( true );
			m_UICenterOnChild.CenterOn( m_GridDefaultItem.transform );

			//
			m_bPrepareUI = false;
		}

		//
		private void OnCloseAlertWaveHand()
		{
			if( !m_bInit )
				return;
			
			m_fAlertWaveHandLastTime = Time.time;
		}

		//
		private void OnCloseAlertSelect()
		{
			if( !m_bInit )
				return;
			
			m_fAlertSelectLastTime = Time.time;
		}

		//
		private void OnHoverInputPanelLeft( GameObject go, bool state )
		{
			//
			if( !m_bInit ) 
				return;

			//
			if( m_bTweening )
				return;

			if( !state )
				return;

			//
			CenterPrevious();
		}

		//
		private void OnHoverInputPanelRight( GameObject go, bool state )
		{
			//
			if( !m_bInit ) 
				return;
			
			//
			if( m_bTweening )
				return;
			
			if( !state )
				return;

			//
			CenterNext();
		}

		//
		private void OnClickedInputPanelLeft( GameObject go )
		{
			//
			if( !m_bInit ) 
				return;
			
			//
			if( m_bTweening )
				return;

			//
			CenterPrevious();
		}

		//
		private void OnClickedInputPanelRight( GameObject go )
		{//
			if( !m_bInit ) 
				return;
			
			//
			if( m_bTweening )
				return;

			//
			CenterNext();
		}

		#endregion
		
		#region private Unity functions
		// Use this for initialization
		private void Start()
		{
			//
			if( m_InputPanelLeft == null )
			{
				Debug.LogError( "The InputPanelLeft has not assigned.", this );
				return;
			}

			//
			if( m_InputPanelRight == null )
			{
				Debug.LogError( "The InputPanelRight has not assigned.", this );
				return;
			}

			//
			if( m_GridApartmentLayout == null )
			{
				Debug.LogError( "The GridApartmentLayout has not assigned.", this );
				return;
			}

			//
			m_GridItems = m_GridApartmentLayout.GetComponentsInChildren<ApartmentLayoutItem>();

			//
			if( m_GridItems == null || m_GridItems.Length <= 0 )
			{
				Debug.LogError( "The GridItems is Null or Empty.", this );
				return;
			}

			//
			if( m_GridDefaultItem == null )
				m_GridDefaultItem = m_GridItems[0];

			//
			if( m_GridDefaultItem == null )
			{
				Debug.LogError( "The GridDefaultItem has not assigned.", this );
				return;
			}

			//
			m_UICenterOnChild = m_GridApartmentLayout.GetComponent<UICenterOnChild>();

			//
			if( m_UICenterOnChild == null )
			{
				Debug.LogError( "The GridApartmentLayout has not assigned Component:'UICenterOnChild'.", this );
				return;
			}

//			//
//			if( string.IsNullOrEmpty( m_WaveLeftGestureName ) )
//			{
//				Debug.LogError( "The WaveLeftGestureName is Null or Empty", this );
//				return;
//			}
//
//			//
//			if( string.IsNullOrEmpty( m_WaveRightGestureName ) )
//			{
//				Debug.LogError( "The WaveRightGestureName is Null or Empty", this );
//				return;
//			}

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
			m_UICenterOnChild.onCenter = OnCenterStart;
			m_UICenterOnChild.onFinished = OnCenterFinished;

			//
			UIEventListener listener = UIEventListener.Get( m_InputPanelLeft.gameObject );
			listener.onHover = this.OnHoverInputPanelLeft;
			listener.onClick = this.OnClickedInputPanelLeft;

			listener = UIEventListener.Get( m_InputPanelRight.gameObject );
			listener.onHover = this.OnHoverInputPanelRight;
			listener.onClick = this.OnClickedInputPanelRight;

			//
			StartCoroutine( DelayInit() );
		}

		//
		private IEnumerator DelayInit()
		{
			// skip a frame
			yield return null;

//			//
//			CKinect.GestureManager gestureManager = CKinect.GestureManager.Instance;
//
//			//
//			if( gestureManager == null )
//			{
//				Debug.LogError( "The 'KinectGestureManager' not Instance!", this );
//				yield break;
//			}
//			
//			gestureManager.OnGesture += this.OnGesture;
//			
//			gestureManager.AddGestureByName(m_WaveLeftGestureName);
//			gestureManager.AddGestureByName(m_WaveRightGestureName);

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
			m_AlertWaveHand = UIAlertManager.Instance.GetAlert( m_IDAlertWaveHand );
			
			if( m_AlertWaveHand == null )
			{
				Debug.LogError( "There's not a UIAlert -- IDAlertWaveHand:" + m_IDAlertWaveHand, this );
				yield break;
			}

			m_AlertWaveHand.OnClose = this.OnCloseAlertWaveHand;

			//
			m_AlertSelect = UIAlertManager.Instance.GetAlert( m_IDAlertSelect );
			
			if( m_AlertSelect == null )
			{
				Debug.LogError( "There's not a UIAlert -- IDAlertSelect:" + m_IDAlertSelect, this );
				yield break;
			}

			m_AlertSelect.OnClose = this.OnCloseAlertSelect;

			//
			m_GridApartmentLayout.gameObject.SetActive( false );

			//
			m_bInit = true;
		}

		//
		private void OnDestroy()
		{
			// 是否初始化
			if( !m_bInit )
				return;

//			CKinect.GestureManager.Instance.OnGesture -= this.OnGesture;
//			CKinect.GestureManager.Instance.RemoveGestureByName(m_WaveLeftGestureName);
//			CKinect.GestureManager.Instance.RemoveGestureByName(m_WaveRightGestureName);

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
			if( m_AlertWelcome.GetVisible() )
				return;

			//
			if( !m_bHasWaveHand && !m_AlertWaveHand.GetVisible() )
			{
				//
				if( Time.time - m_fAlertWaveHandLastTime > m_AlertWaveHandInterval )
				{
					m_fAlertWaveHandLastTime = Time.time;

					m_AlertWaveHand.Show( 0.5f );
				}

				return;
			}

			//
			if( m_bHasWaveHand && !m_AlertSelect.GetVisible() )
			{
				if( Time.time - m_fAlertSelectLastTime > m_AlertSelectInterval )
				{
					m_fAlertSelectLastTime = Time.time;
					
					m_AlertSelect.Show( 0.5f );
				}
			}
		}
		#endregion
	} // class ApartmentLayout
} // namespace MainScene