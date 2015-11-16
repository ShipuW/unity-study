using UnityEngine;
using System.Collections;

public class UIAlertManager : MonoBehaviour
{
	#region static instance
	private static UIAlertManager s_Instance;
	public static UIAlertManager Instance	{ get{ return s_Instance; } }
	#endregion
	
	#region public member variables for Untiy
	//
	public UIAlert[] m_Alerts = null;
	#endregion
	
	#region private member variables
	//
	private bool m_bInit = false;
	#endregion
	
	#region public methods
	//
	public UIAlert GetAlert( int id )
	{
		if( !m_bInit )
			return null;

		if( id < 0 )
		{
			Debug.LogWarning( "The 'id' < 0 in GetAlert()." );
			return null;
		}

		//
		for( int i = 0; i < m_Alerts.Length; i++ )
		{
			if( m_Alerts[i].ID == id )
				return m_Alerts[i];
		}

		return null;
	}

	//
	public void Hide()
	{
		//
		if( !m_bInit )
			return;
				
		//
		for( int i = 0; i < m_Alerts.Length; i++ )
		{
			if( m_Alerts[i].gameObject.activeSelf )
				m_Alerts[i].Close();
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
	private void Start()
	{
		//
		m_Alerts = this.GetComponentsInChildren<UIAlert>();

		//
		if( m_Alerts == null || m_Alerts.Length <= 0 )
		{
			Debug.LogWarning( "There's not any Component:'UIAlert'.", this );
			return;
		}

		//
		m_bInit = true;

		//
		StartCoroutine( DisableAlerts() );
	}

	private IEnumerator DisableAlerts()
	{
		// skip a frame
		yield return null;

		//
		for( int i = 0; i < m_Alerts.Length; i++ )
		{
			m_Alerts[i].gameObject.SetActive( false );
		}
	}
	#endregion
}
