﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodyTexture : MonoBehaviour 
{

	private static Color[] BodyColor =
	{
		Color.blue,
		Color.cyan,
		Color.green,
		Color.red,
		Color.yellow,
		Color.magenta,
	};

	//
	private bool m_bInit = false;
	
	//
	private ComputeBuffer m_DepthBuffer;
	private ComputeBuffer m_BodyIndexBuffer;
	private ComputeBuffer m_BodyColorBuffer;
	
	private Windows.Kinect.DepthSpacePoint[] m_DepthPoints;
	private byte[] m_BodyIndexPoints;
	//private Windows.Kinect.DepthSpacePoint[] dss;

	private UITexture m_Texture;

	private Dictionary<ulong, Color> _Bodies = new Dictionary<ulong, Color>();

	// Use this for initialization
	private void Start()
	{
		//
		ReleaseBuffers();
		m_Texture = this.GetComponent<UITexture>();

		m_Texture.onRender += this.OnRender;

		//
		//this.transform.localScale = new Vector3( m_MultiSourceManger.DepthWidth * 0.01f, m_MultiSourceManger.DepthHeight * 0.01f, 1.0f );
		
		StartCoroutine( DelayInit() );
		
	}

	private void OnRender(Material mat)
	{
		//
		if( !m_bInit )
			return;

		if( m_DepthPoints == null || m_BodyIndexPoints == null)
		{
			return;
		}

		// TODO: fix perf on this call.
		m_DepthBuffer.SetData(m_DepthPoints);
		//m_DepthBuffer.SetData(dss);		
		mat.SetBuffer( "depthCoordinates", m_DepthBuffer );
		// ComputeBuffers do not accept bytes, so we need to convert to float.
		float[] buffer = new float[m_BodyIndexPoints.Length];
		for (int i = 0; i < m_BodyIndexPoints.Length; i++)
		{
			buffer[i] = (float)m_BodyIndexPoints[i];
		}

		m_BodyIndexBuffer.SetData(buffer);
		mat.SetBuffer ( "bodyIndexBuffer", m_BodyIndexBuffer );
		for(int i = 0;i<6;i++)
		{
			mat.SetColor("bodyIndexColor" + i,BodyColor[i]);
		}

		buffer = null;

		UserManager m_UserManager = UserManager.Instance;
		if (!m_UserManager.Inited || !m_UserManager.HasLockUser)
		{
			mat.SetInt("lockBodyIndex",255);
			return;
		}
		
		Windows.Kinect.Body lockBody = m_UserManager.LockBody;
		
		if (lockBody != null)
		{
			mat.SetInt("lockBodyIndex",m_UserManager.LockBodyIndex);

			Debug.Log("lockBodyIndex : " + m_UserManager.LockBodyIndex);
		} 
		else
		{
			mat.SetInt("lockBodyIndex",255);
		}

	}

	//
	private IEnumerator DelayInit()
	{
		//
		yield return null;
		
		//
		InitiateBuffers();
		
		//
		m_bInit = true;
	}
	
	// Update is called once per frame
	private void Update()
	{

		if(!m_bInit)
			return;
		
	}

	private void LateUpdate()
	{




	}

	private void OnRenderObject()
	{
	}  

	private void OnDestroy()
	{
		m_Texture.onRender -= this.OnRender;
	}

	//
	private void InitiateBuffers()
	{
		//
		Debug.Log("InitiateBuffers");

		Material m = this.GetComponent<UITexture>().material;
		//Material m = m_Texture.drawCall.dynamicMaterial;

		//
		Texture2D renderTexture = CKinect.MultiSourceManager.Instance.GetColorTexture();
		if( renderTexture != null )
		{
			m.SetTexture("_MainTex", renderTexture);
		}
		
		m_DepthPoints = CKinect.MultiSourceManager.Instance.GetDepthCoordinates();
		if( m_DepthPoints != null )
		{
			m_DepthBuffer = new ComputeBuffer( m_DepthPoints.Length, sizeof(float) * 2 );

			m.SetBuffer( "depthCoordinates", m_DepthBuffer );
		}
		
		m_BodyIndexPoints = CKinect.MultiSourceManager.Instance.GetBodyIndexData();
		if( m_BodyIndexPoints != null)
		{
			m_BodyIndexBuffer = new ComputeBuffer( m_BodyIndexPoints.Length, sizeof(float) );
			m.SetBuffer ( "bodyIndexBuffer", m_BodyIndexBuffer );
		}
	
		for(int i = 0;i<6;i++)
		{
			m.SetColor("bodyIndexColor" + i,BodyColor[i]);
		}

	}
	
	//
	private void ReleaseBuffers() 
	{
		//
		Debug.Log("ReleaseBuffers");
		
		//
		if( m_DepthBuffer != null)
		{
			m_DepthBuffer.Release();
			m_DepthBuffer = null;
		}
		
		if( m_BodyIndexBuffer != null )
		{
			m_BodyIndexBuffer.Release();
			m_BodyIndexBuffer = null;
		}

		if(m_BodyColorBuffer != null )
		{
			m_BodyColorBuffer.Release();
			m_BodyColorBuffer= null;
		}
		
		m_DepthPoints = null;
		m_BodyIndexPoints = null;
	}
	
	//
	private void OnEnable()
	{
		//
		if( !m_bInit )
			return;
		
		//
		InitiateBuffers();
	}
	
	//
	private void OnDisable() 
	{
		ReleaseBuffers();
	}
}
