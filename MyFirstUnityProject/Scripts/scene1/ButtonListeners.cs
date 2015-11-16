using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace scene1
{

	public class ButtonListeners : MonoBehaviour 
	{
		#region public de
		public delegate void OnChangeContentsFinishDelegate(GameObject contents);
		public OnChangeContentsFinishDelegate OnChangeContentsFinish;
			
		#endregion
			
		#region public member for unity

		public List<GameObject> m_Contents;

		public GameObject m_Menu;
		public GameObject m_Close;
		#endregion




		// Use this for initialization
		void Start () {
			if (m_Contents == null || m_Contents.Count <= 0)
			{
				Debug.LogError("The 'Contents' has not assigned!");
				return;
			}
			if (m_Menu == null)
			{
				Debug.LogError("The 'Menu' has not assigned!");
				return;
			}	
			if (m_Close == null)
			{
				Debug.LogError("The 'Close' has not assigned!");
				return;
			}	

			UIButton[] buttons = this.GetComponentsInChildren<UIButton>();	

			int i = 0;

			foreach(UIButton b in buttons)
			{			
				ScrollViewItemListener listener = b.gameObject.AddComponent<ScrollViewItemListener> ();
				listener.Index = i++;

				UIEventListener.Get(b.gameObject).onClick = OnItemClick;
			}
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		public void OnItemClick(GameObject go)
		{

			//Debug.Log("OnItemClick");
			ScrollViewItemListener item = go.GetComponent<ScrollViewItemListener>();
			int index = item.Index;
				
			foreach( GameObject g in m_Contents)
			{
				g.SetActive(false);
			}
			m_Menu.SetActive (false);
			m_Close.SetActive (true);
			m_Contents[index % m_Contents.Count].SetActive(true);
				
			if( OnChangeContentsFinish != null )
			{
				OnChangeContentsFinish( m_Contents[index % m_Contents.Count] );
			}
		}
	}
}
