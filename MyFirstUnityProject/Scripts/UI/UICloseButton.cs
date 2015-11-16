using UnityEngine;
using System.Collections;

namespace UI
{
	public class UICloseButton : MonoBehaviour 
	{
		//
		public delegate void VoidDelegate();
		
		//
		public VoidDelegate OnClicked = null;
		
		#region public member for unity
		public float m_Rotate = 0.25f;
		
		public bool m_RotateWhenIndle = true;
		//
		public Vector3 m_MoveDir = Vector3.zero;
		//
		public float m_ForceFeedBack = 100f;
		#endregion
		
		#region private member
		//
		private Vector3 m_vPos;
		//
		private Vector3 m_vLocalPos = Vector3.zero;
		
		//private bool bClicked = false;
		
		//private bool bHover = false;
		
		//
		private SpriteAnim m_SpriteAnim = null;
		
		//
		private bool m_bTweening = false;
		
		
		private Vector3 m_InitPostion;
		#endregion
		
		#region private function for unity
		// Use this for initialization
		void Start () 
		{
			//
			if( m_MoveDir.x != 1 && m_MoveDir.y != 1 && m_MoveDir.z != 1 &&
			   m_MoveDir.x != -1 && m_MoveDir.y != -1 && m_MoveDir.z != -1 )
			{
				Debug.LogWarning( "The MoveDir is invalid. Set it to default.", this );
				m_MoveDir = Vector3.right;
			}
			
			UIEventTrigger trigger = this.transform.GetComponent<UIEventTrigger>();
			
			if(trigger == null)
			{
				trigger = this.gameObject.AddComponent<UIEventTrigger>();
			}
			
			EventDelegate.Add(trigger.onHoverOver,this.OnHoverIn);
			EventDelegate.Add(trigger.onHoverOut,this.OnHoverOut);
			
			m_SpriteAnim = this.GetComponent<SpriteAnim>();
			
			if( m_SpriteAnim == null )
				Debug.LogWarning( "This GameObject has not Component:'SpriteAnim'.", this );
			else
				m_SpriteAnim.Pause();
			
			//
			StartCoroutine( DelayInit() );
		}
		
		//
		private IEnumerator DelayInit()
		{
			//
			yield return null;
			
			//
			m_vLocalPos = transform.localPosition;
			m_InitPostion = transform.localPosition;
		}
		
		// Update is called once per frame
		void Update () 
		{
			//if(!bClicked && !bHover && m_RotateWhenIndle)
			//{
			//	this.transform.localRotation =  Quaternion.Euler(Vector3.down + this.transform.localRotation.eulerAngles);
			//}
		}
		
		
		#endregion
		
		public void OnHoverIn()
		{
			//bHover = true;
			
			//
			DoFeedBack();
		}
		
		public void OnHoverOut()
		{
			//bHover = false;
		}
		
		#region private function
		
		private void OnClick()
		{
			DoFeedBack();
		}
		
		//
		private void DoFeedBack()
		{
			//
			if( m_bTweening )
				return;
			
			//bClicked = true;
			m_bTweening = true;
			Vector3 vPos = m_vLocalPos + m_MoveDir * m_ForceFeedBack;
			//iTween.RotateBy( this.gameObject, iTween.Hash( "y", m_Rotate, "easeType", "easeInOutBack", "oncomplete", "OnRotateCompleted") );
			iTween.MoveTo( this.gameObject, iTween.Hash( "position", vPos, "time", 0.4f, "oncomplete", "OnMoveCompleted", "islocal", true ) );
		}
		
		//
		//private void OnRotateCompleted()
		//{
		//	bClicked = false;
		//
		//	if( OnClicked != null )
		//		OnClicked();
		//}
		
		//
		private void OnMoveCompleted()
		{
			iTween.MoveTo( this.gameObject, iTween.Hash( "position", m_vLocalPos, "time", 0.6f, "oncomplete", "OnMoveBackCompleted", "islocal", true ) );
			
			//
			if( m_SpriteAnim != null )
				m_SpriteAnim.ResetToBeginning();
		}
		
		private void OnMoveBackCompleted()
		{
			//bClicked = false;
			
			m_bTweening = false;
			
			if( OnClicked != null )
				OnClicked();
		}
		#endregion
		
		#region public function
		
		public void Hide()
		{
			TweenPosition.Begin(gameObject,0.5f,new Vector3(200,0,0));
		}
		
		public void Show()
		{
			TweenPosition.Begin(gameObject,0.5f,m_InitPostion);
		}
		
		#endregion
		
	}
}

