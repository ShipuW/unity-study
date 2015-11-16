using UnityEngine;
using System.Collections;

namespace MainScene
{
	public class HUD : MonoBehaviour 
	{
		//
		public int m_ID = -1;

		//
		public int ID { get{return m_ID;} }

		//
		public void Show()
		{
			if( !this.gameObject.activeSelf )
				this.gameObject.SetActive( true );
		}
		
		//
		public void Hide()
		{
			if( this.gameObject.activeSelf )
				this.gameObject.SetActive( false );
		}
	}
}