using UnityEngine;
using System.Collections;

public class PlanetsManager : MonoBehaviour {

	#region static instance
	private static PlanetsManager s_Instance;
	public static PlanetsManager Instance {get{return s_Instance;}}
	#endregion
	
	public Planet[] m_Planet = null;
	
	private bool m_bInit = false;
	
	
	public void Show(){
		if (!m_bInit)
			return;
		foreach(Planet pla in m_Planet)
			pla.Show();
	}
	
	public void Hide(){
		if (!m_bInit)
			return;
		foreach(Planet pla in m_Planet)
			pla.Hide();
	}
	private void Awake(){
		s_Instance = this;
		
	}
	// Use this for initialization
	private void Start () {
		m_Planet = this.GetComponentsInChildren<Planet>(true);
		Debug.Log(m_Planet.Length);
		if( m_Planet == null || m_Planet.Length <= 0){
			Debug.LogError("There's not any Component:'Planet'.",this);
			return;
		}
		m_bInit = true;
	}
}
