using UnityEngine;
using System.Collections;

public class CustomSpring : MonoBehaviour
{
	#region public member variables for Unity
	//
	//public bool m_KeyTest = true;
	//
	public float m_Rotate = 0f;
	//
	public float m_TouchVelocity = 1f;
	//
	public Vector3 m_MoveDir = Vector3.zero;
	//
	public float m_ForceFeedBack = 1f;
	//
	public float m_TouchDistance = 1f;
	//
	public delegate void VoidDelegate( TouchButton btn, bool b );
	public VoidDelegate OnHover = null;
	#endregion

	#region private member variables
	//
	private Vector3 m_vPos;
	//
	private Vector3 m_vLocalPos = Vector3.zero;
	//
	private bool m_bRotating = false;
	//
	//private bool m_bTweening = false;
	//
	private TouchSphere m_TouchSphere = null;
	//
	private TouchButton m_TouchButton = null;
	//
	private bool m_bInit = false;
	#endregion

	#region public properties
	//
	public bool IsTweening { get;set; }
	#endregion

	#region private functions for Unity
	// Use this for initialization
	private void Start ()
	{
		//
		if( this.rigidbody == null )
		{
			Debug.LogError( "This GameObject has not a Component: 'Rigidbody'.", this );
			return;
		}

		m_TouchButton = this.GetComponent<TouchButton>();
		if( m_TouchButton == null )
		{
			Debug.LogError( "This GameObject has not a Component: 'TouchButton'.", this );
			return;
		}

		//
		m_MoveDir.Normalize();

		if( m_MoveDir.x != 1 && m_MoveDir.y != 1 && m_MoveDir.z != 1 &&
		   m_MoveDir.x != -1 && m_MoveDir.y != -1 && m_MoveDir.z != -1 )
		{
			Debug.LogWarning( "The MoveDir is invalid. Set it to default.", this );
			m_MoveDir = Vector3.right;
		}

		//
		m_vPos = this.transform.position;

		//
		m_vLocalPos = this.transform.localPosition;

		//
		m_bInit = true;
	}
	
	// Update is called once per frame
	private void Update ()
	{
		//
		if( !m_bInit )
			return;
	
	}

	//
	private void OnCollisionEnter(Collision collision) 
	{
		//
		if( !m_bInit )
			return;

		//
		if( m_bRotating || this.IsTweening )
			return;

		//
		TouchSphere sphere = collision.gameObject.GetComponent<TouchSphere>();
		if( sphere == null )
			return;

		m_TouchSphere = sphere;

		if( sphere.Velocity.magnitude > m_TouchVelocity )
		{
			DoFeedBack();
		}
		else
		{
			this.transform.position += m_MoveDir * sphere.Velocity.magnitude * Time.deltaTime;
		}
	}

	//
	private void OnCollisionStay(Collision collision)
	{
		//
		if( !m_bInit )
			return;
		
		//
		if( m_bRotating || this.IsTweening )
			return;
		
		//
		TouchSphere sphere = collision.gameObject.GetComponent<TouchSphere>();
		if( sphere == null )
			return;

		//
		if( m_TouchSphere != sphere )
			return;

		//
		float fDist = (this.transform.localPosition - m_vLocalPos).magnitude;

		//
		if( sphere.Velocity.magnitude > m_TouchVelocity || fDist > m_TouchDistance )
		{
			DoFeedBack();
		}
		else
		{
			this.transform.position += m_MoveDir * sphere.Velocity.magnitude * Time.deltaTime;
		}

		//
		if( this.OnHover != null )
			this.OnHover( m_TouchButton, true );
	}

	//
	private void OnCollisionExit(Collision collision)
	{
		//
		if( !m_bInit )
			return;
		
		//
		if( m_bRotating || this.IsTweening )
			return;
		
		//
		TouchSphere sphere = collision.gameObject.GetComponent<TouchSphere>();
		if( sphere == null )
			return;

		//
		if( m_TouchSphere != sphere )
			return;

		m_TouchSphere = null;

		iTween.MoveTo( this.gameObject, iTween.Hash( "position", m_vLocalPos, "islocal", true ) );

		//
		if( this.OnHover != null )
			this.OnHover( m_TouchButton, false );
	}

	//
	private void OnMouseDown()
	{
		//
		DoFeedBack();
	}

	// for NGUI
	private void OnClick()
	{
		//
		DoFeedBack();
	}
	#endregion

	#region private functions
	//
	private void DoFeedBack()
	{
		Vector3 vPos = m_vLocalPos + m_MoveDir * m_ForceFeedBack;
		iTween.RotateBy( this.gameObject, iTween.Hash( "y", m_Rotate, "easeType", "easeInOutBack", "oncomplete", "OnRotateCompleted") );
		iTween.MoveTo( this.gameObject, iTween.Hash( "position", vPos, "time", 0.4f, "oncomplete", "OnMoveCompleted", "islocal", true ) );
		m_bRotating = true;
	}

	//
	private void OnRotateCompleted()
	{
		//
		m_bRotating = false;

		//
		m_TouchSphere = null;

		//
		if( m_TouchButton.OnClicked != null )
			m_TouchButton.OnClicked( m_TouchButton );
	}

	//
	private void OnMoveCompleted()
	{
		iTween.MoveTo( this.gameObject, iTween.Hash( "position", m_vLocalPos, "time", 0.6f, "islocal", true ) );
	}

	#endregion
}
