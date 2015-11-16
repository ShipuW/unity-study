using UnityEngine;
using System.Collections;


public class TouchButton : MonoBehaviour
{
	#region public member variables for Untiy
	//
	public int m_ID = -1;
	//
	public bool m_IsFixUV = true;
	// 
	public TweenAlpha m_ToolTip;
	//
	public delegate void VoidDelegate( TouchButton btn );
	//
	public VoidDelegate OnClicked = null;

	#endregion

	#region private member variables
	//
	private CustomSpring m_Spring = null;
	//
	private TweenPosition m_TweenPosition = null;
	//
	private bool m_bInit = false;
	#endregion

	#region public properties
	//
	public int ID { get{return m_ID;} }
	#endregion

	#region public member methods
	//
	public void Show()
	{
		//
		if( !m_bInit )
			return;

		if( !this.gameObject.activeSelf )
			this.gameObject.SetActive( true );

		//
		m_Spring.IsTweening = true;
		//
		m_TweenPosition.PlayForward();
		m_TweenPosition.SetOnFinished( ()=>{ m_Spring.IsTweening = false; } );
	}

	//
	public void Hide()
	{
		//
		if( !m_bInit )
			return;

		//
		m_Spring.IsTweening = true;
		//
		m_TweenPosition.PlayReverse();
		m_TweenPosition.SetOnFinished( ()=>{ m_Spring.IsTweening = false; if( this.gameObject.activeSelf ) this.gameObject.SetActive( false ); } );
	}

	//
	public void ShowToolTip()
	{
		if( m_ToolTip == null )
			return;

		//m_ToolTip.ResetToBeginning();
		//m_ToolTip.from = 0;
		//m_ToolTip.to = 1;
		m_ToolTip.PlayForward();
	}

	//
	public void HideToolTip()
	{
		if( m_ToolTip == null )
			return;

		//m_ToolTip.ResetToBeginning();
		//m_ToolTip.from = 1;
		//m_ToolTip.to = 0;
		m_ToolTip.PlayReverse();		
	}
	#endregion

	#region private member functions
	// for NGUI
	private void OnHover( bool isOver )
	{
		if( isOver )
			ShowToolTip();
		else
			HideToolTip();
	}
	#endregion

	#region private Unity functions
	// Use this for initialization
	private void Start()
	{
		//
		m_Spring = this.GetComponent<CustomSpring>();

		if( m_Spring == null )
		{
			Debug.LogError( "There's not a Component:'CustomSpring'.", this );
			return;
		}

		//
		m_TweenPosition  = this.GetComponent<TweenPosition>();

		if( m_TweenPosition == null )
		{
			Debug.LogError( "There's not a Component:'TweenPosition'.", this );
			return;
		}

		//
		if( m_ToolTip == null )
			Debug.LogWarning( "The ToolTip has not assigned.", this );

		// 调整BOX的UV到正确的位置上
		if( m_IsFixUV )
		{
			//
			MeshFilter meshFilter = this.GetComponent<MeshFilter>();
			Mesh mesh = meshFilter.mesh;
			Vector2[] uvs = mesh.uv;
			int[] triangles = mesh.triangles;

			int triNum = triangles.Length / 3;
			//
			for( int i = 0; i < triNum; i++ )
			{
				if( i == 4 )
				{
					//Debug.Log( "index:"+triangles[i*3]+"  uv0:"+uvs[triangles[i*3]] ); // index:10 uv0:(0.0, 0.0)
					//Debug.Log( "index:"+triangles[i*3+1]+"  uv1:"+uvs[triangles[i*3+1]] ); // index:7  uv1:(1.0, 1.0)
					//Debug.Log( "index:"+triangles[i*3+2]+"  uv2:"+uvs[triangles[i*3+2]] ); // index:11  uv2:(1.0, 0.0)
					
					//Debug.Log( "index:"+triangles[i*3+3]+"  uv3:"+uvs[triangles[i*3+3]] ); // index:10  uv3:(0.0, 0.0)
					//Debug.Log( "index:"+triangles[i*3+4]+"  uv4:"+uvs[triangles[i*3+4]] ); // index:6  uv4:(0.0, 1.0)
					//Debug.Log( "index:"+triangles[i*3+5]+"  uv5:"+uvs[triangles[i*3+5]] ); // index:7  uv5:(1.0, 1.0)

					uvs[triangles[i*3]] = new Vector2( 1f, 1f );
					uvs[triangles[i*3+1]] = new Vector2( 0f, 0f );
					uvs[triangles[i*3+2]] = new Vector2( 0f, 1f );

					uvs[triangles[i*3+3]] = new Vector2( 1f, 1f );
					uvs[triangles[i*3+4]] = new Vector2( 1f, 0f );
					uvs[triangles[i*3+5]] = new Vector2( 0f, 0f );

					break;
				}
			}

			mesh.uv = uvs;

			//meshFilter.mesh = mesh;
		}

		//
		m_bInit = true;
	}
	#endregion
}