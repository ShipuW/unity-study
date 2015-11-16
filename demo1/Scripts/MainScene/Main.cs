using UnityEngine;
using System.Collections;

namespace MainScene
{
	/// <summary>
	/// 主场景逻辑处理
	/// </summary>
	public class Main : MonoBehaviour
	{

        #region static instance
        private static Main s_Instance;
        public static Main Instance  { get{ return s_Instance; } }
        #endregion

		#region public member variables for Unity
		//
		public bool m_HideCursor = true;
		//
		public float m_RawFilter = 0;
		//
		public GameObject m_UserVisualPanel;
		public GameObject m_TouchButtonParent;
		public TouchButton[] m_TouchButtons;
                
        public GameObject m_CloseButton = null;

		//
		public int m_IDOutDoor = 3;
		public float m_DelayHideOnOutDoorMoveIn = 1;
		public Camera m_Camera = null;
		public Transform m_CameraDestination = null;
		public OrbitCamera m_OrbitCamera = null;
		public float m_CameraTweenDuration = 0.5f;
		public float m_CameraTweenBackDuration = 0.5f;
		public Transform[] m_CameraMovePath;
		public Transform[] m_CameraLookPath;
		public Transform m_CameraLookTarget;
		public float m_CameraPercentage;

		// NGUI光标对象
		public UISprite m_SpriteHandLeft = null;
		public UISprite m_SpriteHandRight = null;
		public UISprite m_SpriteCursorLoading = null;

		//
		public UISprite m_SceneMask = null;
		#endregion

		#region private member variables
		//
		private bool m_bInit = false;
		//
		private Transform m_CameraOriginLocation = null;
		//
		private float m_OrbitCameraDistance = 0;
		//
		private Vector3 m_vLastCursorPos = Utils.InvaildVec3;
		private Vector3 m_vLastHandRawPos = Utils.InvaildVec3;
		#endregion

        public bool HasInteractive {get;set;}

		#region private functions for Unity

        private void Awake()
        {
            //
            s_Instance = this;
        }

		// Use this for initialization
		private void Start ()
		{
			//
			if( Application.isEditor )
			{
				m_HideCursor = false;
			}

			//
			if( m_TouchButtonParent == null )
			{
				Debug.LogError( "The TouchButtonParent has not assigned.", this );
				return;
			}
			else
			{
				m_TouchButtons = m_TouchButtonParent.GetComponentsInChildren<TouchButton>( true );

				if( m_TouchButtons != null && m_TouchButtons.Length > 0 )
				{
					foreach( var btn in m_TouchButtons )
					{
						btn.OnClicked = this.OnClicked;
					}
				}
				else
				{
					Debug.LogWarning( "The TouchButtons is Null or Length is Zero.", this );
				}

				//
				CustomSpring[] springs = m_TouchButtonParent.GetComponentsInChildren<CustomSpring>( true );

				if( springs != null && springs.Length > 0 )
				{
					foreach( var spring in springs )
					{
						spring.OnHover = this.OnTouchButtonHover;
					}
				}
				else
				{
					Debug.LogWarning( "The CustomSprings is Null or Length is Zero.", this );
				}
			}

			//
            if( m_CloseButton == null )
            {
                Debug.LogError("The 'CloseButton' has not assigned!");
                return;
            }
                        
            //
			if (m_SpriteHandLeft == null)
			{
				Debug.LogError("The 'SpriteHandLeft' has not assigned!");
				return;
			}
			
			if (m_SpriteHandRight == null)
			{
				Debug.LogError("The 'SpriteHandRight' has not assigned!");
				return;
			}
			
			if ( m_SpriteCursorLoading == null )
			{
				Debug.LogError("The 'SpriteCursorLoading' has not assigned!");
				return;
			}
			//
			//
			if( m_Camera == null )
				m_Camera = Camera.main;
			
			//
			if( m_Camera == null )
			{
				Debug.LogError( "The Camera has not assigned.", this );
				return;
			}
			
			//
			if( m_CameraDestination == null )
			{
				Debug.LogError( "The CameraDestination has not assigned.", this );
				return;
			}

			//
			if( m_OrbitCamera == null )
				m_OrbitCamera = m_Camera.GetComponent<OrbitCamera>();

			//
			if( m_OrbitCamera == null )
			{
				Debug.LogError( "The OrbitCamera has not assigned.", this );
				return;
			}

			//
			if( m_CameraMovePath == null || m_CameraMovePath.Length <= 0 )
			{
				Debug.LogError( "The CameraMovePath has not assigned or Length is Zero.", this );
				return;
			}

			//
			if( m_CameraLookPath == null || m_CameraLookPath.Length <= 0 )
			{
				Debug.LogError( "The CameraLookPath has not assigned or Length is Zero.", this );
				return;
			}

			//
			if( m_CameraLookTarget == null )
			{
				Debug.LogError( "The CameraLookTarget has not assigned.", this );
				return;
			}

			//
			if( m_SceneMask == null )
			{
				Debug.LogError( "The SceneMask has not assigned.", this );
				return;
			}

			//
			if(CKinect.KinectServices.Instance == null)
			{
				CKinect.KinectServices.Create(m_SpriteHandLeft,m_SpriteHandRight,m_SpriteCursorLoading);
			}
			else if(CKinect.CursorController.Instance != null)
			{
				CKinect.CursorController.Instance.SpriteHandLeft = m_SpriteHandLeft;
				CKinect.CursorController.Instance.SpriteHandRight = m_SpriteHandRight;
				CKinect.CursorController.Instance.SpriteCursorLoading = m_SpriteCursorLoading;

			}

			if( m_UserVisualPanel == null )
				m_UserVisualPanel = UserManager.Instance.UserVisaulPanel;
			//
			if( m_UserVisualPanel == null )
			{
				Debug.LogError( "The UserVisualPanel has not assigned.", this );
				return;
			}


			UserManager.Instance.OnLockUser = this.OnLockUser;
			//
			m_OrbitCamera.enabled = false;

			//
			m_OrbitCameraDistance = (m_CameraDestination.position - m_OrbitCamera.LookAt.position).magnitude;
						
			//
			m_CameraOriginLocation = new GameObject().transform;
			
			m_CameraOriginLocation.position = m_Camera.transform.position;
			m_CameraOriginLocation.rotation = m_Camera.transform.rotation;

			//
            HasInteractive = true;
                        
            //
            m_CloseButton.SetActive( false );

			//
			if( m_HideCursor )
				Screen.showCursor = !m_HideCursor;

			//
			StartCoroutine( DelayInit() );
			
		}
		
		//
		private IEnumerator DelayInit()
		{
			//
			yield return null;

			Menu[] menus = MenuManager.Instance.Menus;

			//
			if( menus != null )
			{
				//
				for( int i=0; i < menus.Length; i++ )
				{
					menus[i].OnClose += OnMenuClose;
				}
			}

			//
			CKinect.CursorController.Instance.DisableCursor();

			yield return new WaitForSeconds(0.08f);

			//
			TweenAlpha.Begin( m_SceneMask.gameObject, 0.5f, 0f ).SetOnFinished( ()=>{  m_SceneMask.gameObject.SetActive(false); } );

			//
			TouchButtonManager.Instance.Show();

			//
			m_bInit = true;
		}

		//
		private void OnDestroy()
		{
			// 是否初始化
			if( !m_bInit )
				return;

			//
			Menu[] menus = MenuManager.Instance.Menus;
			
			//
			if( menus != null )
			{
				//
				for( int i=0; i < menus.Length; i++ )
				{
					menus[i].OnClose -= OnMenuClose;
				}
			}
		}

		// Update is called once per frame
		private void Update ()
		{
			if( !m_bInit )
				return;

			//
			if( !m_OrbitCamera.enabled )
				return;

			//
			if( Input.GetMouseButton(0) )
			{
				//
				if( OutDoor.Instance != null )
					OutDoor.Instance.HideHintDrag();
			}

			//
			CKinect.CursorController cursor = CKinect.CursorController.Instance;
			//
			if( !cursor.GetHandVisible() || !cursor.HasHandClosed )
            {
                m_vLastHandRawPos = Utils.InvaildVec3;
                m_vLastCursorPos = Utils.InvaildVec3;
				return;
            }

			if( Utils.IsInvaildVec3( m_vLastHandRawPos ) )
				m_vLastHandRawPos = cursor.GetCurrentCursorRawPos();

            Vector3 dir = cursor.GetCurrentCursorRawPos() - m_vLastHandRawPos;
            float fDist = dir.z;
			float fLen = dir.magnitude;
            dir.Normalize();

			float dirX = Mathf.Abs(dir.x);
			float dirY = Mathf.Abs(dir.y);
			float dirZ = Mathf.Abs(dir.z);

			//
            m_vLastHandRawPos = cursor.GetCurrentCursorRawPos();

			if( fLen > m_RawFilter && dirZ > dirX && dirZ > dirY )
            {
                //
    			if( fDist > 0 )
    			{
    				m_OrbitCamera.Distance -= m_OrbitCamera.ZoomSpeed;
    				m_OrbitCamera.Distance = Mathf.Clamp( m_OrbitCamera.Distance, m_OrbitCamera.MinZoom, m_OrbitCamera.MaxZoom );

					//
					if( OutDoor.Instance != null )
						OutDoor.Instance.HideHintDrag();
    			}
    			else if( fDist < 0 )
    			{
    				m_OrbitCamera.Distance += m_OrbitCamera.ZoomSpeed;
    				m_OrbitCamera.Distance = Mathf.Clamp( m_OrbitCamera.Distance, m_OrbitCamera.MinZoom, m_OrbitCamera.MaxZoom );

					//
					if( OutDoor.Instance != null )
						OutDoor.Instance.HideHintDrag();
    			}
            }
            else
            {
    			//
    			if( Utils.IsInvaildVec3( m_vLastCursorPos ) )
    				m_vLastCursorPos = cursor.GetCurrentCursorPos();

    			Vector3 vec = cursor.GetCurrentCursorPos() - m_vLastCursorPos;

                m_vLastCursorPos = cursor.GetCurrentCursorPos();
    			//
                m_OrbitCamera.Rot( vec );

				//
				if( OutDoor.Instance != null )
					OutDoor.Instance.HideHintDrag();
            }
		}

		//
		private void OnDrawGizmos()
		{
			//
			//if( !m_bInit )
			//	return;

			iTween.DrawPath( m_CameraMovePath, Color.magenta );
			iTween.DrawPath( m_CameraLookPath, Color.cyan );
			Gizmos.color = Color.black;
			Gizmos.DrawLine( Camera.main.transform.position, m_CameraLookTarget.position );
		}

		#endregion

		#region private functions
		//
		private void OnLockUser( ulong lockBodyId )
		{
			//
			if( !m_bInit )
				return;

			//
			TouchButtonManager.Instance.Show();
		}

		//
		private void OnClicked( TouchButton btn )
		{
			//
			if( !m_bInit )
				return;

            if(HasInteractive == false)
                return;

			Debug.Log("Button ID:"+btn.ID);

			//
            HasInteractive = false;

			//
			if( btn.ID == m_IDOutDoor )
			{
				ProcessOutDoor();
				MenuManager.Instance.ShowMenu( btn.ID, m_CameraTweenDuration );
				//
				StartCoroutine( DelayHideOnOutDoorMoveIn( m_DelayHideOnOutDoorMoveIn ) );
			}
			else
			{
				//
				MenuManager.Instance.ShowMenu( btn.ID );
			}

			//
			TouchButtonManager.Instance.Hide();
			TouchButtonManager.Instance.HideToolTips();
		}

		//
		private void OnTouchButtonHover( TouchButton btn, bool b )
		{
			//
			if( !m_bInit )
				return;
			
			if(HasInteractive == false)
				return;

			//
			if( b )
				btn.ShowToolTip();
			else
				btn.HideToolTip();
		}

		//
		private void ProcessOutDoor()
		{
			//iTween.MoveTo( m_Camera.gameObject, iTween.Hash( "position",m_CameraDestination.position, "time", m_CameraTweenDuration,
			//                                                 "oncomplete", "OnOutDoorMoveInCompleted", "oncompletetarget", this.gameObject ) );
			//iTween.RotateTo( m_Camera.gameObject, m_CameraDestination.eulerAngles, m_CameraTweenDuration );

			iTween.ValueTo( m_Camera.gameObject, iTween.Hash( "from", 0f, "to", 1f, "time", m_CameraTweenDuration, "easetype", iTween.EaseType.easeInOutCubic,
			                                                 "onupdate","OnUpdateCameraPath", "onupdatetarget", this.gameObject,
			                                                 "oncomplete", "OnOutDoorMoveInCompleted", "oncompletetarget", this.gameObject ) );
		}

		//
		private void OnUpdateCameraPath( float f )
		{
			iTween.PutOnPath( m_Camera.gameObject, m_CameraMovePath, f );
			iTween.PutOnPath( m_CameraLookTarget, m_CameraLookPath, f );
			m_Camera.transform.LookAt( iTween.PointOnPath( m_CameraLookPath, f ) );

			m_CameraPercentage = f;
		}

		//
		private void OnMenuClose( Menu menu )
		{
			//
			if( !m_bInit )
				return;

			//
			TouchButtonManager.Instance.Show();

			//
			CKinect.CursorController.Instance.DisableCursor();

			//
			if( menu.ID == m_IDOutDoor )
			{
				iTween.MoveTo( m_Camera.gameObject, iTween.Hash( "position",m_CameraOriginLocation.position, "time", m_CameraTweenBackDuration,
				                                                "oncomplete", "OnOutDoorMoveBackCompleted", "oncompletetarget", this.gameObject ) );
				iTween.RotateTo( m_Camera.gameObject, m_CameraOriginLocation.eulerAngles, m_CameraTweenBackDuration );

				//
				m_UserVisualPanel.SetActive( true );
				m_TouchButtonParent.SetActive( true );
				//
				m_OrbitCamera.enabled = false;
				//
				IndicatorManager.Instance.Hide();
				//
				OutDoorInteractiveObjectManager.Instance.Hide();
			}
			else
			{
				//
				HasInteractive = true;
			}
		}

		//
		private void OnOutDoorMoveInCompleted()
		{
			//m_UserVisualPanel.SetActive( false );
			//m_TouchButtonParent.SetActive( false );

			m_OrbitCamera.Reset( m_OrbitCameraDistance, m_Camera.transform.eulerAngles );
			//m_OrbitCamera.enabled = true;

			//
			IndicatorManager.Instance.Show();
			//
			OutDoorInteractiveObjectManager.Instance.Show();
		}
		
		//
		private void OnOutDoorMoveBackCompleted()
		{
			//
			HasInteractive = true;

			//m_UserVisualPanel.SetActive( true );
			//m_TouchButtonParent.SetActive( true );
		}

		//
		private IEnumerator DelayHideOnOutDoorMoveIn( float fDelay )
		{
			yield return new WaitForSeconds( fDelay );

			m_UserVisualPanel.SetActive( false );
			m_TouchButtonParent.SetActive( false );
		}
		#endregion
	}
} // namespace MainScene