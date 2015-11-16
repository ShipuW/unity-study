using UnityEngine;
using System.Collections;

namespace MainScene
{
	public class TouchButtonManager : MonoBehaviour 
	{
		#region static instance
		private static TouchButtonManager s_Instance;
		public static TouchButtonManager Instance	{ get{ return s_Instance; } }
		#endregion
		
		#region public member variables for Untiy
		//
		public TouchButton[] m_TouchButtons = null;
		#endregion
		
		#region private member variables
		//
		private bool m_bInit = false;
		#endregion
		
		#region public methods
		//
		public void Show()
		{
			if( !m_bInit )
				return;

			foreach( TouchButton btn in m_TouchButtons )
				btn.Show();
		}

		//
		public void Hide()
		{
			if( !m_bInit )
				return;
			
			foreach( TouchButton btn in m_TouchButtons )
				btn.Hide();
		}

		//
		public void HideToolTips()
		{
			if( !m_bInit )
				return;
			
			foreach( TouchButton btn in m_TouchButtons )
				btn.HideToolTip();
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
		private void Start()
		{
			//
			m_TouchButtons = this.GetComponentsInChildren<TouchButton>();

			//
			if( m_TouchButtons == null || m_TouchButtons.Length <= 0 )
			{
				Debug.LogError( "There's not any Component:'TouchButton'.", this );
				return;
			}

			//
			m_bInit = true;
		}
		#endregion
	} // class TouchButtonManager
} // namespace MainScene