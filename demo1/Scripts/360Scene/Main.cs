using UnityEngine;
using System.Collections;

namespace watch360
{
	public class Main : MonoBehaviour 
	{
		#region public member for unity
		//
		public bool m_HideCursor = true;

        public UI.UICloseButton m_BtnCloseScene;

		public string m_LevelParent;
		//
		public float m_RawFilter = 0;

		public OrbitCamera m_OrbitCamera = null;

		public UIToggle m_AutoToggle;

		public UIMore m_WidgetMore;

		// NGUI光标对象
		public UISprite m_SpriteHandLeft = null;
		public UISprite m_SpriteHandRight = null;
		public UISprite m_SpriteCursorLoading = null;

		public float m_AutoRotSpeed = 25f;

		public bool m_AutoRotOnStart = false;

		//
		public int m_IDAlertWelcome = -1;

		//
		public int m_IDAlterDrag = -1;

		public int m_IDAlterAutoPlay = -1;

		public float m_ShowAlertTime = 60;

		public bool m_StopAutoRotWhenMoreOpen;
		#endregion

		#region private member
		private bool m_bInited;

		private Vector3 m_vLastCursorPos = Utils.InvaildVec3;
		private Vector3 m_vLastHandRawPos = Utils.InvaildVec3;

		private bool m_bAutoPlay = false;

		//
		private UIAlert m_AlertWelcome = null;

		private UIAlert m_AlterDrag = null;

		//
		private UIAlert m_AlertAutoPlay = null;

		private float m_AlertDragShowLastTime = float.MaxValue;

		private float m_AlertAutoPlayShowLastTime = float.MaxValue;

		private int m_iShowAlertCount = 0;

		private bool m_bMoreOpened = false;

		#endregion

		#region private function for unity
		// Use this for initialization
		void Start () 
		{
			//
			if( Application.isEditor )
			{
				m_HideCursor = false;
			}
		
			if( m_BtnCloseScene == null )
			{
				Debug.LogError("The 'BtnCloseScene' has not assigned!",this);
				return;
			}

			if( string.IsNullOrEmpty(m_LevelParent) )
			{
				Debug.LogError("The 'LevelParent' must not null or empty",this);
				return;
			}

			//
			if (m_SpriteHandLeft == null)
			{
				Debug.LogError("The 'SpriteHandLeft' has not assigned!",this);
				return;
			}
			
			if (m_SpriteHandRight == null)
			{
				Debug.LogError("The 'SpriteHandRight' has not assigned!",this);
				return;
			}
			
			if ( m_SpriteCursorLoading == null )
			{
				Debug.LogError("The 'SpriteCursorLoading' has not assigned!",this);
				return;
			}

			//
			if( m_OrbitCamera == null )
				m_OrbitCamera = Camera.main.GetComponent<OrbitCamera>();
			
			//
			if( m_OrbitCamera == null )
			{
				Debug.LogError( "The OrbitCamera has not assigned.", this );
				return;
			}

			if( m_WidgetMore == null )
			{
				Debug.LogError( "The 'WidgetMore' has not assigned.", this );
				return;
			}

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

			//UIEventListener.Get(m_BtnCloseScene.gameObject).onClick = this.OnBtnCloseSceneClick;

			EventDelegate.Add(m_AutoToggle.onChange,this.AutoPlayChange);

			//
			if( m_HideCursor )
				Screen.showCursor = !m_HideCursor;

			StartCoroutine( DelayInit() );

		}
		
		// Update is called once per frame
		void Update () 
		{
		
			if( !m_bInited )
				return;

			if( m_bMoreOpened && m_StopAutoRotWhenMoreOpen )
				return;

			if( !m_bAutoPlay && Time.time - m_AlertDragShowLastTime > m_ShowAlertTime)
			{
				m_AlterDrag.Show(1.0f);
				m_AlertDragShowLastTime = Time.time;
			}
			else if(Time.time - m_AlertAutoPlayShowLastTime > m_ShowAlertTime/2 && Time.time - m_AlertDragShowLastTime >m_ShowAlertTime/2)
			{
				m_AlertAutoPlay.Show(1.0f);
				m_AlertAutoPlayShowLastTime = Time.time;
			}

			//
			if( !m_OrbitCamera.enabled )
				return;

			if(m_bAutoPlay)
			{
				m_OrbitCamera.Rot( new Vector3(m_AutoRotSpeed * Time.deltaTime,0,0) );
				return;
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
					//m_OrbitCamera.Distance -= m_OrbitCamera.ZoomSpeed;
					//m_OrbitCamera.Distance = Mathf.Clamp( m_OrbitCamera.Distance, m_OrbitCamera.MinZoom, m_OrbitCamera.MaxZoom );

					m_OrbitCamera.FieldOfView -= m_OrbitCamera.ZoomSpeed;
					m_OrbitCamera.FieldOfView = Mathf.Clamp( m_OrbitCamera.FieldOfView, m_OrbitCamera.MinZoom, m_OrbitCamera.MaxZoom );

				}
				else if( fDist < 0 )
				{


					m_OrbitCamera.FieldOfView += m_OrbitCamera.ZoomSpeed;
					m_OrbitCamera.FieldOfView = Mathf.Clamp( m_OrbitCamera.FieldOfView, m_OrbitCamera.MinZoom, m_OrbitCamera.MaxZoom );

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
				m_OrbitCamera.Rot( new Vector3(-vec.x,vec.y,vec.z) );

			}

		}

		#endregion

		#region private function
		//
		private IEnumerator DelayInit()
		{
			// skip a frame
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


			m_AlterDrag = UIAlertManager.Instance.GetAlert( m_IDAlterDrag );

			if( m_AlterDrag == null)
			{
				Debug.LogError( " There's not a UIAlter -- IDAlterDrag:" + m_IDAlterDrag,this);
				yield break;
			}

			m_AlertAutoPlay = UIAlertManager.Instance.GetAlert( m_IDAlterAutoPlay );
			
			if( m_AlertAutoPlay == null)
			{
				Debug.LogError( " There's not a UIAlter -- IDAlterAutoPlay:" + m_IDAlterAutoPlay,this);
				yield break;
			}

            m_BtnCloseScene.OnClicked = this.OnBtnCloseSceneClick;

			m_WidgetMore.OnOpen = OnMoreOpen;
			m_WidgetMore.OnCloseFinish = OnMoreCloseFinish;

			yield return new WaitForEndOfFrame();
			//
			m_AlertWelcome.Show( 1.0f );
			
			m_bInited = true;
		}

		private void OnCloseAlertWelcome()
		{
			m_AlertDragShowLastTime = Time.time;
			m_AlertAutoPlayShowLastTime = Time.time;

			if( m_AutoRotOnStart )
			{
				NGUITools.Execute<UIToggle>(m_AutoToggle.gameObject,"OnClick");
			}
			else
			{
				StartCoroutine( DelayShowAlertDrag(1f) );
			}
		}

		private void OnBtnCloseSceneClick()
		{
			if(!m_bInited)
				return;

			LoadingScene.Main.LoadingScene(m_LevelParent);

		}

		private void AutoPlayChange()
		{

			if(m_AutoToggle.value)
			{
				m_AutoToggle.GetComponentInChildren<UILabel>().text = "停止";
				m_bAutoPlay = true;
			}
			else
			{

				m_AutoToggle.GetComponentInChildren<UILabel>().text = "播放";
				m_bAutoPlay = false;
			}

			if( !m_bAutoPlay && !m_bMoreOpened)
			{
				StartCoroutine( DelayShowAlertDrag(1f) );
			}
		}

		private IEnumerator DelayShowAlertDrag(float seconds)
		{
			if( !m_bInited )
				yield break;

			yield return new WaitForSeconds(seconds);

			if(!m_bAutoPlay)
			{
				
				m_AlterDrag.Show( 1.0f );
				
				m_AlertDragShowLastTime = Time.time;
			}
			
		}

		private IEnumerator DelayShowAlertAutoPlay( float seconds )
		{

			if( !m_bInited )
				yield break;

			yield return new WaitForSeconds(seconds);

			if(m_bAutoPlay)
			{
				m_AlertAutoPlay.Show(1.0f);
				
				m_AlertAutoPlayShowLastTime = Time.time;
			}
		}

		private void OnMoreOpen()
		{
            HoldCloseSceneButton();
			m_bMoreOpened = true;
		}

		private void OnMoreCloseFinish()
		{
            ShowCloseSceneButton();
			m_bMoreOpened = false;
		}

        private void HoldCloseSceneButton()
        {
            if(!m_bInited)
                return;
            
            m_BtnCloseScene.Hide();
        }
        
        private void ShowCloseSceneButton()
        {
            if(!m_bInited)
                return;
            
            m_BtnCloseScene.Show();
        }
		#endregion
	}
}
