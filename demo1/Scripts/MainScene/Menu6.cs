using UnityEngine;
using System.Collections;

namespace MainScene
{
    public class Menu6 : MonoBehaviour
    {
        #region public member variables for Untiy
		//
		public int m_IDAlertWelcome = -1;

		//
		public GameObject m_ParentItems;
		//
		public int m_ItemGroupID = 6000;
		//
		public UITexture m_UITextureTarget = null;
		//
		public UITexture m_UITextureMask = null;
        //
		public Texture2D m_DefaultTexture = null;
        #endregion
        
        #region private member variables
		//
		private Menu m_Menu = null;
        //
        private bool m_bTweening = false;
        
		//
		private UIAlert m_AlertWelcome = null;

		//
		private UIToggle[] m_UIToggleItems = null;

        //
        private bool m_bInit = false;

		//
		private bool m_bMainTextureZoom = false;
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

			//
			BetterList<UIToggle> list = UIToggle.list;
			
			//UIToggle toggle = UIToggle.GetActiveToggle(1);

			//
			for( int i = 0; i < list.size; ++i )
			{
				UIToggle toggle = list[i];
				if (toggle != null && toggle.group == m_ItemGroupID )
				{
					toggle.Set(false);
				}
			}

			//
			m_UITextureTarget.mainTexture = m_DefaultTexture;

			//
			if( m_bMainTextureZoom )
			{
				//
				TweenTransform tt = m_UITextureTarget.GetComponent<TweenTransform>();
				tt.PlayReverse();
				tt = m_UITextureMask.GetComponent<TweenTransform>();
				tt.PlayReverse();
				//
				m_bMainTextureZoom = false;
			}
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
		private void OnToggleChanged()
		{
			//
			if( !m_bInit )
				return;

			//
			if( !UIToggle.current.value )
				return;

			//
			Menu6Item item = UIToggle.current.GetComponent<Menu6Item>();

			//
			if( item == null )
			{
				Debug.LogWarning( "There's not Component:'Item'.", this );
				return;
			}

			//
			m_UITextureTarget.mainTexture = Resources.Load<Texture2D>( item.ImageFilename );
		}

		//
		private void OnMainTextureClick( GameObject go )
		{
			//
			if( !m_bInit )
				return;

			if( !m_bMainTextureZoom )
			{
				m_bMainTextureZoom = true;

				TweenTransform tt = m_UITextureTarget.GetComponent<TweenTransform>();
				tt.PlayForward();
				tt = m_UITextureMask.GetComponent<TweenTransform>();
				tt.PlayForward();
			}
			else
			{
				m_bMainTextureZoom = false;

				TweenTransform tt = m_UITextureTarget.GetComponent<TweenTransform>();
				tt.PlayReverse();
				tt = m_UITextureMask.GetComponent<TweenTransform>();
				tt.PlayReverse();
			}
		}
        #endregion
        
        #region private Unity functions
        // Use this for initialization
        private void Start()
        {
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
			if( m_ParentItems == null )
			{
				Debug.LogError("The 'ParentItems' has not assigned!");
				return;
			}
			
			m_UIToggleItems = m_ParentItems.GetComponentsInChildren<UIToggle>();
			
			if( m_UIToggleItems == null || m_UIToggleItems.Length <= 0 )
			{
				Debug.LogError("Can't find Components 'UIToggle' in " + m_ParentItems.name);
				return;
			}
			
			foreach( UIToggle toggle in m_UIToggleItems )
			{
				toggle.group = m_ItemGroupID;
				EventDelegate.Add( toggle.onChange, OnToggleChanged );
			}

			//
			if( m_UITextureTarget == null )
			{
				Debug.LogError("The 'UITextureTarget' has not assigned!");
				return;
			}

			//
			if( m_UITextureMask == null )
			{
				Debug.LogError("The 'UITextureMask' has not assigned!");
				return;
			}

			//
			if( m_DefaultTexture == null )
			{
				Debug.LogError("The 'DefaultTexture' has not assigned!");
				return;
			}

			//
			UIEventListener listener = UIEventListener.Get( m_UITextureTarget.gameObject );
			listener.onClick = this.OnMainTextureClick;
            
			//
			StartCoroutine( DelayInit() );            
        }

		//
		private IEnumerator DelayInit()
		{
			// skip a frame
			yield return null;
			yield return new WaitForEndOfFrame();

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