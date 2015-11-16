using UnityEngine;
using System.Collections;

public class OrbitCamera : MonoBehaviour
{
	#region static instance
	private static OrbitCamera s_Instance = null;
	public static OrbitCamera Instance	{ get{ return s_Instance; } }
	#endregion
	
	#region public member variables for Untiy
	// 观察目标
	public Transform m_Lookat = null;
	//
	public Vector2 m_RotateSpeed = new Vector2( 0.1f, 0.1f );
	//
	// 环绕仰角的最小值
	public float m_MinPitchAngle = -80;
	// 环绕仰角的最大值
	public float m_MaxPitchAngle = 80;
	//
	public float m_ZoomSpeed = 0.1f;
	public float m_MaxZoom = 100f;
	public float m_MinZoom = 5f;
	#endregion
	
	#region private member variables
	// 保存实始视角
	private Vector3 m_vInitRot = Vector3.zero;
	//
	private Vector3 m_vRot = Vector3.zero;
	//
	private Vector3 m_vLastMousePos = Vector3.zero;
	//
	private float m_fDist = 0;
	
	private Vector3 m_vLookat = Vector3.zero;
	//
	private bool m_bInit = false;
	#endregion
	
	#region public properties
	public Transform LookAt { get{ return m_Lookat; } set{ m_Lookat = value; } }
	public Vector3 Rotation	{ get{ return m_vRot; } set{ m_vRot = value; } }
	public Vector2 RotateSpeed{ get{ return m_RotateSpeed; } set{ m_RotateSpeed = value; } }
	public float Distance { get{ return m_fDist; } set{m_fDist = value;} }

	//
	public float MaxZoom{ set{m_MaxZoom=value;}get{return m_MaxZoom;} }
	public float MinZoom{ set{m_MinZoom=value;}get{return m_MinZoom;} }

	public float ZoomSpeed{ set{m_ZoomSpeed=value;}get{return m_ZoomSpeed;} }
	#endregion
	
	#region public methods
	public void Enable(){ this.enabled = true; m_vLastMousePos = Input.mousePosition; }
	public void Disable() { this.enabled = false; }
	//
	public void Reset()
	{
		m_vRot = m_vInitRot;
	}

	//
	public void Reset( float fDist, Vector3 vInitRot )
	{
		m_fDist = fDist;

		m_vInitRot = vInitRot;

		m_vRot = vInitRot;

		m_vLookat = m_Lookat.transform.localPosition;
		
		//
		m_vLastMousePos = Input.mousePosition;
	}

	//
	public void UpdateOrbit( Vector2 vRot )
	{
		//
		Quaternion rot = Quaternion.Euler( vRot.x, vRot.y, 0);
		Vector3 eye = m_vLookat + rot * new Vector3(0.0f, 0.0f, -m_fDist);
		
		//
		this.gameObject.transform.localRotation = rot;
		this.gameObject.transform.localPosition = eye;
	}

	//
	public void Rot( Vector2 vRot )
	{
		// 操作绕X轴旋转
		if( !Mathf.Approximately( vRot.x, 0f ) )
            m_vRot.y += m_RotateSpeed.y * vRot.x;

		// 操作绕Y轴旋转
        if( !Mathf.Approximately( vRot.y, 0f ))
            m_vRot.x -= m_RotateSpeed.x * vRot.y;

		//
		if( m_vRot.y < -360 )
			m_vRot.y -= -360;
		if( m_vRot.y > 360 )
			m_vRot.y -= 360;
		
		//
		if( m_vRot.x > m_MaxPitchAngle )
			m_vRot.x = m_MaxPitchAngle;
		
		//
		if( m_vRot.x < m_MinPitchAngle )
			m_vRot.x = m_MinPitchAngle;
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
		if( m_Lookat == null )
		{
			Debug.LogError( "The 'Lookat' has not assigned." );
			return;
		}
		
		m_fDist = ( this.gameObject.transform.localPosition - m_Lookat.transform.localPosition ).magnitude;
		
		//Debug.Log( "localPosition:" + this.gameObject.transform.localPosition + " Lookat.transform.localPosition: " + m_Lookat.transform.localPosition );
		
		m_vRot = this.gameObject.transform.rotation.eulerAngles;
		m_vInitRot = m_vRot;
		
		m_vLookat = m_Lookat.transform.localPosition;
		
		//
		m_vLastMousePos = Input.mousePosition;
		
		//
		m_bInit = true;
	}
	
	//
	private void Update()
	{
		//
		if( !m_bInit )
		{
			return;
		}
		
	}
	
	//
	private void LateUpdate()
	{
		//
		if( !m_bInit )
		{
			return;
		}
		
		//
		if( Input.GetMouseButton(0) )
		{
			Vector3 vec = Input.mousePosition - m_vLastMousePos;
			
			if( vec.sqrMagnitude > 1 )
			{
				// 鼠标X轴方向是操作的绕Y轴旋转
				m_vRot.x -= m_RotateSpeed.x * vec.y;
				// 鼠标Y轴方向是操作的绕X轴旋转
				m_vRot.y += m_RotateSpeed.y * vec.x;
				
				if( m_vRot.x > m_MaxPitchAngle )
					m_vRot.x = m_MaxPitchAngle;
				
				if( m_vRot.x < m_MinPitchAngle )
					m_vRot.x = m_MinPitchAngle;
				
				
				if( m_vRot.y < -360 )
					m_vRot.y -= -360;
				if( m_vRot.y > 360 )
					m_vRot.y -= 360;
			}
		}

		//
		if( Input.GetAxis("Mouse ScrollWheel") < 0 )
		{
			this.Distance += this.ZoomSpeed;
			
			this.Distance = Mathf.Clamp( this.Distance, MinZoom, MaxZoom );
		}
		else if( Input.GetAxis("Mouse ScrollWheel") > 0 )
		{
			this.Distance -= this.ZoomSpeed;
			
			this.Distance = Mathf.Clamp( this.Distance, MinZoom, MaxZoom );
		}
		
		//
		m_vLastMousePos = Input.mousePosition;
		
		//
		//Quaternion rot = Quaternion.Euler( m_vRot.x, m_vRot.y, 0);
		//Vector3 eye = m_vLookat + rot * new Vector3(0.0f, 0.0f, -m_fDist);
		
		//
		//this.gameObject.transform.localRotation = rot;
		//this.gameObject.transform.localPosition = eye;

		UpdateOrbit( m_vRot );
	}
	#endregion
}