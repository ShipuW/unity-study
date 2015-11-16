using UnityEngine;
using System.Collections;

namespace MainScene
{
	public class OutDoorInteractiveObject : MonoBehaviour 
	{
		#region public member variables for Untiy
		//
		public UIWidget m_Hint = null;
		//
		public bool m_NoMeshRenderers = false;
		//
		public MeshRenderer[] m_MeshRenderers = null;
		#endregion
		
		#region private member variables
		//
		private TweenAlpha m_TweenAlphaHint = null;
		//
		private TweenAlpha[] m_TweenAlphaMeshRenderers = null;
		//
		private bool m_bInit = false;
		#endregion

		#region public member methods
		//
		public void Reset()
		{
			//
			if( !m_bInit )
				return;

			//
			if( m_TweenAlphaHint.direction == AnimationOrTween.Direction.Reverse )
				m_TweenAlphaHint.Toggle();
			m_TweenAlphaHint.enabled = false;
			m_TweenAlphaHint.ResetToBeginning();
			m_Hint.alpha = 0;

			//
			if( m_NoMeshRenderers )
				return;

			//
			foreach( var obj in m_MeshRenderers )
			{
				//
				if( obj == null )
					continue;

				Color color = obj.material.color;
				color.a = 0f;
				obj.material.color = color;
			}

			//
			foreach( var obj in m_TweenAlphaMeshRenderers )
			{
				//
				if( obj == null )
					continue;

				if( obj.direction == AnimationOrTween.Direction.Reverse )
					obj.Toggle();
				obj.enabled = false;
				obj.ResetToBeginning();
			}
		}

		//
		public void Hide()
		{
			//
			if( !m_bInit )
				return;

			//
			m_TweenAlphaHint.enabled = false;
			m_Hint.alpha = 0;

			//
			if( m_NoMeshRenderers )
				return;
			//
			foreach( var obj in m_TweenAlphaMeshRenderers )
			{
				//
				if( obj == null )
					continue;

				obj.enabled = false;
			}
		}
		#endregion

		#region private Unity functions
		// Use this for initialization
		private void Start()
		{
			//
			if( m_Hint == null )
			{
				Debug.LogError( "The Hint has not assigned.", this );
				return;
			}

			//
			m_TweenAlphaHint = m_Hint.GetComponent<TweenAlpha>();
			//
			if( m_TweenAlphaHint == null )
			{
				Debug.LogError( "The Hint has not assigned a Component:'TweenAlpha'.", this );
				return;
			}

			//
			if( !m_NoMeshRenderers )
			{
				//
				if( m_MeshRenderers == null || m_MeshRenderers.Length <= 0 )
				{
					Debug.LogError( "The MeshRenderers is NULL or Length is Zero.", this );
					return;
				}

				m_TweenAlphaMeshRenderers = new TweenAlpha[m_MeshRenderers.Length];

				for( int i = 0; i < m_MeshRenderers.Length; i++ )
				{
					TweenAlpha ta = m_MeshRenderers[i].GetComponent<TweenAlpha>();

					if( ta == null )
					{
						Debug.LogError( "There's not assigned a Component:'TweenAlpha'.", m_MeshRenderers[i].gameObject );
						return;
					}

					m_TweenAlphaMeshRenderers[i] = ta;
				}
			}

			//
			m_bInit = true;
		}
		#endregion
	}
}