using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
	//
	private static MenuManager s_Instance;
	public static MenuManager Instance {get {return s_Instance;}	}

	//
	public GameObject m_ParentOfMenus = null;
	//
	public Menu[] m_Menus = null;
	public Menu[] Menus{get{return m_Menus;}}
	//
	private bool m_bInit = false;
	//
	public void ShowMenu(int id){
		ShowMenu (id, 0);
	}

	public void ShowMenu(int id,float fDelay){
		if (!m_bInit)
			return;

		for (int i = 0; i<m_Menus.Length; i++) {
			if(m_Menus[i].ID == id){
				m_Menus[i].TweenIn(fDelay);
				return;
			}
		}
	}

	private void Awake(){
		s_Instance = this;
	}
	// Use this for initialization
	void Start () {
		if (m_ParentOfMenus == null) {
			Debug.LogError("This ParentOfMenus has not assigned.",this);
			return;
		}

		m_Menus = m_ParentOfMenus.GetComponentsInChildren<Menu>();

		if(m_Menus == null || m_Menus.Length <=0 ){
			Debug.LogWarning("There's no Menu.",this);
		}

		StartCoroutine (DelayInit ());
	
	}
	private IEnumerator DelayInit(){
		//等待两帧的时间再执行。
		yield return null;
		yield return null;
		//yield return new WaitForSeconds (1f);
		for(int i = 0;i<m_Menus.Length;i++){
			m_Menus[i].gameObject.SetActive(false);
		}

		m_bInit = true;

	}
}
