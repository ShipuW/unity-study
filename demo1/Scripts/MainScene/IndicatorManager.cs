using UnityEngine;
using System.Collections;

namespace MainScene
{
	public class IndicatorManager : MonoBehaviour
	{
		#region static instance
		private static IndicatorManager s_Instance;
		public static IndicatorManager Instance	{ get{ return s_Instance; } }
		#endregion
		
		#region public member variables for Untiy
		//
		public Indicator[] m_Indicators = null;
		//
		public Material m_Diffuse = null;
		public Material m_TransparentDiffuse = null;
		//
		public TweenAlpha[] m_TweenAlphas = null;
		//
		public EventDelegate.Callback OnHoverOver, OnHoverOut; 
		#endregion
		
		#region private member variables
		//
		private bool m_bInit = false;
		#endregion
		
		#region public methods
		//
		public void Show()
		{
			//
			if( !m_bInit )
				return;

			//
			this.gameObject.SetActive( true );

			//
			for( int i = 0; i < m_TweenAlphas.Length; i++ )
			{
				//
				MeshRenderer renderer = m_TweenAlphas[i].GetComponent<MeshRenderer>();

				//
				if( renderer != null )
				{
					renderer.material = m_TransparentDiffuse;
				}

				//
				UIEventTrigger trigger = m_TweenAlphas[i].GetComponent<UIEventTrigger>();

				//
				if( trigger != null )
				{
					if( this.OnHoverOver != null )
						EventDelegate.Add( trigger.onHoverOver, this.OnHoverOver );

					if( this.OnHoverOut != null )
						EventDelegate.Add( trigger.onHoverOut, this.OnHoverOut );
				}

				m_TweenAlphas[i].ResetToBeginning();
				m_TweenAlphas[i].PlayForward();
				m_TweenAlphas[i].SetOnFinished( this.OnFadeFinished );
			}

			//
			foreach( var obj in m_Indicators )
			{
				if( obj == null )
					continue;
				
				obj.Reset();
			}
		}

		//
		public void Hide()
		{
			//
			if( !m_bInit )
				return;

			//
			foreach( var obj in m_Indicators )
			{
				if( obj == null )
					continue;
				
				obj.Hide();
			}
			
			//
			this.gameObject.SetActive( false );
		}
		#endregion
		
		#region private functions
		//
		private void OnFadeFinished()
		{
			MeshRenderer renderer = TweenAlpha.current.gameObject.GetComponent<MeshRenderer>();

			if( renderer == null )
				return;

			Material mat = renderer.material;

			if( mat.color.a >= 1.0f )
			{
				renderer.material = m_Diffuse;
			}
		}
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
			m_Indicators = this.GetComponentsInChildren<Indicator>();

			//
			if( m_Indicators == null || m_Indicators.Length <= 0 )
			{
				Debug.LogError( "There's not any Component:'Indicator'.", this );
				return;
			}

			//
			m_TweenAlphas = this.GetComponentsInChildren<TweenAlpha>();

			//
			if( m_TweenAlphas == null || m_TweenAlphas.Length <= 0 )
			{
				Debug.LogError( "There's not any Component:'TweenAlphas'.", this );
				return;
			}

			//
			if( m_Diffuse == null )
			{
				Debug.LogError( "The Diffuse has not assigned.", this );
				return;
			}

			//
			if( m_TransparentDiffuse == null )
			{
				Debug.LogError( "The TransparentDiffuse has not assigned.", this );
				return;
			}

			StartCoroutine( DelayInit() );
		}

		//
		private IEnumerator DelayInit()
		{
			yield return null;

			//
			this.gameObject.SetActive( false );

			m_bInit = true;
		}
		#endregion
	} // class IndicatorManager

} // namespace MainScene