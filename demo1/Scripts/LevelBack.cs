using UnityEngine;
using System.Collections;

public class LevelBack : MonoBehaviour
{

	#region public member for unity
	public bool m_IsDebug = false;

	public string m_LevelName;

	public float m_DelayTime = 10f;
	#endregion

	#region private member 
	private bool bInited = false;
	//
	private float lastUpdataTime;
	//
	private Vector2 cursorLastPostion;

	private bool m_bInLoadingScene = false;
	#endregion

	#region private function for unity
	// Use this for initialization
	void Start () 
	{
		if(string.IsNullOrEmpty(m_LevelName))
		{
			Debug.LogError("The 'LevelName' is null or Empty!",this);
			return;
		}
		lastUpdataTime = Time.time;
		cursorLastPostion = UICamera.lastTouchPosition;
		bInited = true;
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(!bInited || m_IsDebug)
			return;

		UserManager instance = UserManager.Instance;
		
		if(instance != null && !instance.HasLockUser)
		{
			if(!Application.isEditor && Time.time - lastUpdataTime > m_DelayTime )
			{
				Back(); 
			}
		}
		else
		{
			lastUpdataTime = Time.time;
		}


//		Vector2 vec = UICamera.lastTouchPosition - cursorLastPostion;
//
//		if((instance != null && instance.HasLockUser) || vec.sqrMagnitude > 1)
//		{
//			lastUpdataTime = Time.time;
//			//Debug.Log("Updata last time and cursor last postion");
//		}
//
//		cursorLastPostion = UICamera.lastTouchPosition;
//
//		if(Time.time - lastUpdataTime > m_DelayTime)
//		{
//			Back();
//		}
	}
	#endregion

	#region private function
	private void Back()
	{
		if( m_bInLoadingScene )
			return;

		m_bInLoadingScene = true;

		//Application.LoadLevel(m_LevelName);
		StartCoroutine( DelayBack() );
	}

	private IEnumerator DelayBack()
	{
		yield return new WaitForEndOfFrame();

		LoadingScene.Main.LoadingScene(m_LevelName);
	}
	#endregion
}
