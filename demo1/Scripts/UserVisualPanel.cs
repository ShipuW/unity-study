using UnityEngine;
using System.Collections;

public class UserVisualPanel : MonoBehaviour
{
	//
	public CKinect.MultiSourceManager m_MultiSourceManger = null;
    public UserManager m_UserManager = null;
	//
	private bool m_bInit = false;

	//
	private ComputeBuffer m_DepthBuffer;
	private ComputeBuffer m_BodyIndexBuffer;
	
	private Windows.Kinect.DepthSpacePoint[] m_DepthPoints;
	private byte[] m_BodyIndexPoints;

	// Use this for initialization
	private void Start()
	{
		//
		ReleaseBuffers();

		//
		if( m_MultiSourceManger == null )
		{
			Debug.LogError("The 'MultiSourceManger' has not assigned.");
			return;
		}

        if( m_UserManager == null )
        {
            Debug.LogError("The 'UserManager' has not assigned.");
            return;
        }

		//
		//this.transform.localScale = new Vector3( m_MultiSourceManger.DepthWidth * 0.01f, m_MultiSourceManger.DepthHeight * 0.01f, 1.0f );

		StartCoroutine( DelayInit() );
		
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
		//
		if( !m_bInit )
			return;

		// TODO: fix perf on this call.
		m_DepthBuffer.SetData(m_DepthPoints);
		
		// ComputeBuffers do not accept bytes, so we need to convert to float.
		float[] buffer = new float[m_BodyIndexPoints.Length];
		for (int i = 0; i < m_BodyIndexPoints.Length; i++)
		{
			buffer[i] = (float)m_BodyIndexPoints[i];
		}
		m_BodyIndexBuffer.SetData(buffer);

        buffer = null;

        if (!m_UserManager.Inited || !m_UserManager.HasLockUser)
        {
            gameObject.renderer.material.SetInt("lockBodyIndex",255);
            return;
        }

        Windows.Kinect.Body lockBody = m_UserManager.LockBody;
        
        if (lockBody != null)
        {
            gameObject.renderer.material.SetInt("lockBodyIndex",m_UserManager.LockBodyIndex);
        } else
        {
            gameObject.renderer.material.SetInt("lockBodyIndex",255);
        }
		
	}

	//
	private void InitiateBuffers()
	{
		//
		Debug.Log("InitiateBuffers");

		//
		Texture2D renderTexture = m_MultiSourceManger.GetColorTexture();
		if( renderTexture != null )
		{
			gameObject.renderer.material.SetTexture("_MainTex", renderTexture);
		}
		
		m_DepthPoints = m_MultiSourceManger.GetDepthCoordinates();
		if( m_DepthPoints != null )
		{
			m_DepthBuffer = new ComputeBuffer( m_DepthPoints.Length, sizeof(float) * 2 );
			gameObject.renderer.material.SetBuffer( "depthCoordinates", m_DepthBuffer );
		}
		
		m_BodyIndexPoints = m_MultiSourceManger.GetBodyIndexData();
		if( m_BodyIndexPoints != null)
		{
			m_BodyIndexBuffer = new ComputeBuffer( m_BodyIndexPoints.Length, sizeof(float) );
			gameObject.renderer.material.SetBuffer ( "bodyIndexBuffer", m_BodyIndexBuffer );
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
