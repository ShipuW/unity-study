using UnityEngine;
using System.Collections;

namespace MainScene
{
	public class MenuManager : MonoBehaviour
	{
		#region static instance
		private static MenuManager s_Instance;
		public static MenuManager Instance	{ get{ return s_Instance; } }
		#endregion
		
		#region public member variables for Untiy
		//
		public GameObject m_ParentOfMenus = null;
		//
		public Menu[] m_Menus = null;
		#endregion

		#region public properties
		//
		public Menu[] Menus	{ get{ return m_Menus; } }
		#endregion
		
		#region private member variables
		//
		private bool m_bInit = false;
		#endregion
		
		#region public methods
		//
		public void ShowMenu( int id )
		{
			ShowMenu( id, 0 );
		}

		//
		public void ShowMenu( int id, float fDelay )
		{
			if( !m_bInit )
				return;
			
			//
			for( int i=0; i < m_Menus.Length; i++ )
			{
				if( m_Menus[i].ID == id )
				{
					
					m_Menus[i].TweenIn( fDelay );
					return;
				}
			}
		}
		#endregion
		
		#region private functions
		#endregion
		
		#region private Unity functions
		//
		private void Awake()
		{
			//
			s_Instance = this;
		}
		
		// Use this for initialization
		private void Start ()
		{
			//
			if( m_ParentOfMenus == null )
			{
				Debug.LogError( "The ParentOfMenus has not assigned.", this );
				return;
			}

			//
			m_Menus = m_ParentOfMenus.GetComponentsInChildren<Menu>();

			//
			if( m_Menus == null || m_Menus.Length <= 0 )
			{
				Debug.LogWarning( "There's NOT Menu.", this );
            }
            //
            StartCoroutine( DelayInit() );
			
		}

        //
        private IEnumerator DelayInit()
        {
            // wait for two frames
            yield return null;
			yield return null;

            //
            for( int i=0; i < m_Menus.Length; i++ )
            {
                m_Menus[i].gameObject.SetActive(false);
            }
            //
            m_bInit = true;
        }
		#endregion
	}
} // namespace MainScene