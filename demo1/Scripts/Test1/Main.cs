using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Test1
{
	public class Main : MonoBehaviour 
	{
		//
		public CKinect.MultiSourceManager m_MultiSourceManager = null;

		//
		private bool m_bInit = false;

		// Use this for initialization
		private void Start () 
		{
			//
			if( m_MultiSourceManager == null )
			{
				Debug.LogError( "The MultiSourceManager has not assigned.", this );
				return;
			}

			//
			m_bInit = true;
		}
		
		// Update is called once per frame
		private void Update () 
		{
			//
			if( !m_bInit )
				return;

			//Vector4 vFloorClipPlane = m_MultiSourceManager.FloorClipPlane;

			Windows.Kinect.Body[] bodys = m_MultiSourceManager.GetBodyData();

			//
			//if( Utils.IsInvaildVec4( vFloorClipPlane ) )
			//	return;

			//
			//Debug.Log( "x:"+vFloorClipPlane.x + "  y:"+vFloorClipPlane.y+"  z:"+vFloorClipPlane.z+"  w:"+vFloorClipPlane.w );

			//
			foreach( var body in bodys )
			{
				if( body == null )
					continue;
				
				if( body.IsTracked && body.LeanTrackingState != Windows.Kinect.TrackingState.NotTracked )
				{
					Debug.Log( "body lean:"+body.Lean.X +"  "+body.Lean.Y );
				}
			}
		}

    }
}