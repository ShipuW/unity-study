using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace scene1{

	public class Main : MonoBehaviour {
		
		
		#region static instance
		public static Main Instance { get { return s_Instance; } }
		
		private static Main s_Instance = null;
		#endregion
		
		#region public member for unity

		public GameObject m_TouchButtonParent;
		public TouchButton[] m_TouchButtons;
		public GameObject m_CloseButton = null;
		#endregion
	
		
		#region private member
		
		private bool m_bInited = false;

		#endregion

		
		#region private function for unity
		
		private void Awake()
		{
			s_Instance = this;

		}
		
		// Use this for initialization
		private void Start()
		{
			Screen.showCursor = true;
			if (m_TouchButtonParent == null) {
				Debug.LogError("Ths TouchButtonParent has not assigned.",this);
				return;
			}
			m_TouchButtons = m_TouchButtonParent.GetComponentsInChildren<TouchButton> ();
			if (m_TouchButtons != null && m_TouchButtons.Length > 0) {
				foreach (var btn in m_TouchButtons) {
					btn.OnClicked = this.OnClicked;
				}
			} else {
				Debug.LogWarning("The TouchButtons is NULL or Length is Zero.",this);
			}
			if (!m_CloseButton) {
				Debug.LogError("The 'CloseButton' has not assigned!",this);
				return;
			}

			m_CloseButton.SetActive (false);

			StartCoroutine (DelayInit());
		}

		private IEnumerator DelayInit(){
			yield return null;

			Menu[] menus = MenuManager.Instance.Menus;

			if (menus != null) {

				for(int i=0;i<menus.Length;i++){
					menus[i].OnClose +=OnMenuClose;
				}
			}

			TouchButtonManager.Instance.Show ();

			m_bInited = true;


		}
		
		// Update is called once per frame
		void Update()
		{

		}
		


		private void OnClicked(TouchButton btn){
			if (!m_bInited)
				return;
			Debug.Log ("Button ID:"+btn.ID);
			MenuManager.Instance.ShowMenu (btn.ID);
			TouchButtonManager.Instance.Hide ();
		}
		#endregion
		private void OnMenuClose(Menu menu){
			if (!m_bInited)
				return;
			TouchButtonManager.Instance.Show ();
		}


	}

}

