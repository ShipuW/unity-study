using UnityEngine;
using System.Collections;

namespace InHouseScene
{
	public class TouchButton : MonoBehaviour 
	{
		#region public member variables for Untiy
		//
		public int m_ID = -1;
		// 
		public TweenAlpha m_ToolTip;
		//
		public delegate void VoidDelegate( TouchButton btn );
		//
		public VoidDelegate OnClicked = null;
		
		#endregion
		
		#region private member variables
		//
		private UIEventTrigger m_EventTrigger;
		//
		//private TweenPosition m_TweenPosition = null;
		//
		private bool m_bInit = false;
		#endregion
		
		#region public properties
		//
		public int ID { get{return m_ID;} }
		#endregion
		
		#region public member methods
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
//			//
//			m_TweenPosition  = this.GetComponent<TweenPosition>();
//			
//			if( m_TweenPosition == null )
//			{
//				Debug.LogError( "There's not a Component:'TweenPosition'.", this );
//				return;
//			}

			m_EventTrigger = this.GetComponent<UIEventTrigger>();

			if( m_EventTrigger == null )
			{
				Debug.LogError("There's not a compoment:'UIEventTrigger'.",this);
				return;
			}
			
			//
			if( m_ToolTip == null )
				Debug.LogWarning( "The ToolTip has not assigned.", this );


			EventDelegate.Add(m_EventTrigger.onClick,this.OnClick);

			//EventDelegate.Add(m_EventTrigger.onDragOver,this.OnDragOver);


			//
			m_bInit = true;
		}
		#endregion

		private void OnClick()
		{
			if( OnClicked == null || !m_bInit )
				return;

			OnClicked(this);
		}
	}
}

