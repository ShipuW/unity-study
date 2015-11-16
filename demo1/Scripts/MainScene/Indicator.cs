using UnityEngine;
using System.Collections;

namespace MainScene
{
	public class Indicator : MonoBehaviour 
	{
		#region public member variables for Untiy
		//
		public UIWidget m_Hint = null;
		#endregion
		
		#region private member variables
		//
		private TweenAlpha m_TweenAlphaHint = null;
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
			m_bInit = true;
		}
		#endregion
	}
}