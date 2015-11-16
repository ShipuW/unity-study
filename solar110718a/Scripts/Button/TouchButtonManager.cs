using UnityEngine;
using System.Collections;
/// <summary>
/// 管理所有的Buttons
/// </summary>
public class TouchButtonManager : MonoBehaviour {
	#region static instance
	private static TouchButtonManager s_Instance;
	public static TouchButtonManager Instance {get{return s_Instance;}}
	#endregion
	
	public TouchButton[] m_TouchButtons = null;
	
	private bool m_bInit = false;


	public void Show(){
		if (!m_bInit)
			return;
		foreach(TouchButton btn in m_TouchButtons)
			btn.Show();
	}

	public void Hide(){
		if (!m_bInit)
			return;
		foreach(TouchButton btn in m_TouchButtons)
			btn.Hide();
	}
	private void Awake(){
		s_Instance = this;

	}
	// Use this for initialization
	private void Start () {
		m_TouchButtons = this.GetComponentsInChildren<TouchButton>(true);
		Debug.Log(m_TouchButtons.Length);
		if( m_TouchButtons == null || m_TouchButtons.Length <= 0){
			Debug.LogError("There's not any Component:'TouchButton'.",this);
			return;
		}
		m_bInit = true;
	}
}
