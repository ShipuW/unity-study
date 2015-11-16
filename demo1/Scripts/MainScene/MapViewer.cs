using UnityEngine;
using System.Collections;

namespace MainScene
{
    public class MapViewer : MonoBehaviour
    {       
        #region public member variables for Untiy
		//
		public int m_IDAlertWelcome = -1;
        
		//
		public DragResize m_DragResize;

		//
		public BoxCollider m_ButtonZoomIn;
		public BoxCollider m_ButtonZoomOut;

		//
		public float m_ZoomSpeed = 0.05f;
        #endregion
        
        #region private member variables
		//
		private Menu m_Menu = null;
        //
        private bool m_bTweening = false;
        
		//
		private UIAlert m_AlertWelcome = null;

        //
        private bool m_bInit = false;

        #endregion
        
        #region public methods
        #endregion
        
        #region private functions
        
		//
		private void OnMenuOpen( Menu menu )
		{
			//
			if( !m_bInit )
				return;
			//
			m_AlertWelcome.Show( 1.0f );
			
			//
			m_bTweening = true;
		}
		
		//
		private void OnMenuOpened( Menu menu )
		{
			//
			if( !m_bInit )
				return;
			
			//
			CKinect.CursorController.Instance.EnableCursor();
			
			//
			CKinect.CursorController.Instance.IsLoadingActive = false;
			
			//
			m_bTweening = false;
		}

		//
		//
		private void OnCloseAlertWelcome()
		{
			if( !m_bInit )
				return;

			//
			CKinect.CursorController.Instance.IsLoadingActive = true;
		}

		//
		private void OnButtonZoomIn( GameObject go )
		{
			if( !m_bInit )
				return;

			m_DragResize.ZoomInTarget( m_ZoomSpeed );
		}

		//
		private void OnButtonZoomOut( GameObject go )
		{
			if( !m_bInit )
				return;

			m_DragResize.ZoomOutTarget( m_ZoomSpeed );
		}

        #endregion
        
        #region private Unity functions
        // Use this for initialization
        private void Start()
        {
			//
			if( m_DragResize == null )
			{
				Debug.LogError( "The DragResize has not assigned.", this );
				return;
			}

			//
			if( m_ButtonZoomIn == null )
			{
				Debug.LogError( "The ButtonZoomIn has not assigned.", this );
				return;
			}

			//
			if( m_ButtonZoomOut == null )
			{
				Debug.LogError( "The ButtonZoomOut has not assigned.", this );
				return;
			}

			//
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
			UIEventListener listener = UIEventListener.Get( m_ButtonZoomIn.gameObject );
			listener.onClick = this.OnButtonZoomIn;

			listener = UIEventListener.Get( m_ButtonZoomOut.gameObject );
			listener.onClick = this.OnButtonZoomOut;
            
			//
			StartCoroutine( DelayInit() );            
        }

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

			
			m_bInit = true;
		}

		//
		private void OnDestroy()
		{
			// 是否初始化
			if( !m_bInit )
				return;
			
			m_Menu.OnOpen -= this.OnMenuOpen;
			m_Menu.OnOpened -= this.OnMenuOpened;
		}

        // Update is called once per frame
        private void Update()
        {
            //
            if (!m_bInit)
                return;

            //
            if (m_bTweening)
                return;


        }

        #endregion


    }

} // namespace MainScene