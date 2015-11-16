using UnityEngine;
using System.Collections;

namespace InHouseScene
{
	[ExecuteInEditMode]
    public class DragObject : MonoBehaviour
    {

        public GameObject m_Parent = null;

        public GameObject Parent
        { 
            get { return m_Parent; }
            set { m_Parent = value;} 
        }

		private void Start()
		{
			UIEventTrigger trigger = this.transform.GetComponent<UIEventTrigger>();
			if( trigger == null )
			{
				trigger = gameObject.AddComponent<UIEventTrigger>();
				EventDelegate.Add( trigger.onHoverOver, this.OnHoverOver );
				EventDelegate.Add(trigger.onHoverOut,this.OnHoverOut);
				EventDelegate.Add( trigger.onClick, this.OnClicked );
			}

		}

		public void OnHoverOver()
		{
			if(Parent != null  )
			{
				EditableObject eo = Parent.GetComponent<EditableObject>();

				if( eo != null&& eo.OnHoveOver != null)
				{
					eo.OnHoveOver(this.gameObject,UICamera.lastTouchPosition);
				}
			}
		}
		
		public void OnClicked()
		{
			
			//Debug.Log(" -------------> OnClicked " + this.gameObject.name);

			if(Parent != null  )
			{
				EditableObject eo = Parent.GetComponent<EditableObject>();
				
				if( eo != null&& eo.OnClick != null)
				{
					eo.OnClick(this.gameObject,UICamera.lastTouchPosition);
				}
			}
		}
		
		public void OnHoverOut()
		{

			if(Parent != null  )
			{
				EditableObject eo = Parent.GetComponent<EditableObject>();
				
				if( eo != null&& eo.OnHoveOverOut != null)
				{
					eo.OnHoveOverOut();
				}
			}
		}
    }
}
