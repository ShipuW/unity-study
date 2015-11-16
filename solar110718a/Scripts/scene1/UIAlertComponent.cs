using UnityEngine;
using System.Collections;

//重置游戏组件动画.


public class UIAlertComponent : MonoBehaviour {

	#region public member variables for Unity
	//
	public UITweener[] m_TweenersNeedToReset = null;
	#endregion

	#region private member variables
	//
	private bool m_bInit = false;
	#endregion

	#region public methods
	//
	public void Reset()
	{
		if (!m_bInit)
			return;

		for (int i = 0; i < m_TweenersNeedToReset.Length; i++) {
			//
			TweenPosition position = m_TweenersNeedToReset[i] as TweenPosition;

			if(position != null){
				position.ResetToBeginning();

				if( position.worldSpace)
					position.transform.position = position.from;
				else
					position.transform.localPosition = position.from;
			}

			//
			TweenRotation rotation = m_TweenersNeedToReset[i] as TweenRotation;
			
			if( rotation != null )
			{
				rotation.ResetToBeginning();
				rotation.transform.localEulerAngles = rotation.from;
			}

			//
			TweenScale scale = m_TweenersNeedToReset[i] as TweenScale;
			
			if( scale != null )
			{
				scale.ResetToBeginning();
				scale.transform.localScale = scale.from;
			}

			//
			TweenTransform trans = m_TweenersNeedToReset[i] as TweenTransform;
			
			if( trans != null )
			{
				trans.ResetToBeginning();
				trans.transform.position = trans.from.position;
				trans.transform.localScale = trans.from.localScale;
				trans.transform.rotation = trans.from.rotation;
			}

			//
			TweenAlpha alpha = m_TweenersNeedToReset[i] as TweenAlpha;
			
			if( alpha != null )
			{
				alpha.ResetToBeginning();
				UIWidget widget = alpha.GetComponent<UIWidget>();
				if( widget != null )
					widget.alpha = alpha.from;
			}

			//
			TweenColor color = m_TweenersNeedToReset[i] as TweenColor;
			
			if( color != null )
			{
				color.ResetToBeginning();
				UIWidget widget = alpha.GetComponent<UIWidget>();
				if( widget != null )
					widget.color = color.from;
			}	
		}
	}


	#endregion
	// Use this for initialization
	void Start () {
		m_TweenersNeedToReset = this.GetComponents<UITweener>();
		
		//
		if( m_TweenersNeedToReset == null || m_TweenersNeedToReset.Length <= 0 )
		{
			Debug.LogWarning( "There's not any UITweeners:", this );
			return;
		}
		
		//
		m_bInit = true;
	}
}
