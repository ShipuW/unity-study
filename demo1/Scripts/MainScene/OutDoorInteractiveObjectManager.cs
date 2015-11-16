using UnityEngine;
using System.Collections;

namespace MainScene
{
	public class OutDoorInteractiveObjectManager : MonoBehaviour
	{
		#region static instance
		private static OutDoorInteractiveObjectManager s_Instance;
		public static OutDoorInteractiveObjectManager Instance	{ get{ return s_Instance; } }
		#endregion
		
		#region public member variables for Untiy
		//
		public OutDoorInteractiveObject[] m_Objects = null;
		//
		public EventDelegate.Callback OnHoverOver, OnHoverOut; 
		#endregion
		
		#region private member variables
		//
		private bool m_bInit = false;
		#endregion
		
		#region public methods
		//
		public void Show()
		{
			//
			if( !m_bInit )
				return;
			
			//
			this.gameObject.SetActive( true );

			//
			foreach( var obj in m_Objects )
			{
				if( obj == null )
					continue;

				obj.Reset();
			}
		}
		
		//
		public void Hide()
		{
			//
			if( !m_bInit )
				return;

			//
			foreach( var obj in m_Objects )
			{
				if( obj == null )
					continue;
				
				obj.Hide();
			}

			//
			this.gameObject.SetActive( false );
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
			m_Objects = this.GetComponentsInChildren<OutDoorInteractiveObject>();
			
			//
			if( m_Objects == null || m_Objects.Length <= 0 )
			{
				Debug.LogError( "There's not any Component:'OutDoorInteractiveObject'.", this );
				return;
			}

			StartCoroutine( DelayInit() );
		}
		
		//
		private IEnumerator DelayInit()
		{
			yield return null;
			
			//
			this.gameObject.SetActive( false );
			
			m_bInit = true;
		}
		#endregion
	}
}