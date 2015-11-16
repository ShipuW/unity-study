using UnityEngine;
using System.Collections;

namespace LoadingScene
{
	public class Main : MonoBehaviour 
	{
		//
		public bool m_HideCursor = true;

		public static string currentLoadingScene;

		public UIProgressBar m_ProgressBar;

		public UILabel m_ProgressLabel;

		// NGUI光标对象
		public UISprite m_SpriteHandLeft = null;
		public UISprite m_SpriteHandRight = null;
		public UISprite m_SpriteCursorLoading = null;

		public bool m_bInited = false;

		// Use this for initialization
		void Start ()
		{
			//currentLoadingScene = "da wei ying";
			//
			if( Application.isEditor )
			{
				m_HideCursor = false;
			}

			if( m_ProgressBar == null )
			{
				Debug.LogError("ProgressBar has not assigle!",this);
				return;
			}

			if( m_ProgressLabel == null )
			{
				Debug.LogError("Progress label has not assigle!",this);
				return;
			}

			if (m_SpriteHandLeft == null)
			{
				Debug.LogError("The 'SpriteHandLeft' has not assigned!");
				return;
			}
			
			if (m_SpriteHandRight == null)
			{
				Debug.LogError("The 'SpriteHandRight' has not assigned!");
				return;
			}
			
			if ( m_SpriteCursorLoading == null )
			{
				Debug.LogError("The 'SpriteCursorLoading' has not assigned!");
				return;
			}

			if(CKinect.KinectServices.Instance == null)
			{
				CKinect.KinectServices.Create(m_SpriteHandLeft,m_SpriteHandRight,m_SpriteCursorLoading);
				
			}
			else if(CKinect.CursorController.Instance != null)
			{
				
				//Debug.Log("befor IsNull:" + ( CKinect.KinectServices.SpriteHandLeft == null) + " ID :" + CKinect.KinectServices.SpriteHandLeft.GetInstanceID() );
				CKinect.CursorController.Instance.SpriteHandLeft = m_SpriteHandLeft;
				CKinect.CursorController.Instance.SpriteHandRight = m_SpriteHandRight;
				CKinect.CursorController.Instance.SpriteCursorLoading = m_SpriteCursorLoading;
				
				if(!UserManager.Instance.HasLockUser)
				{
					CKinect.CursorController.Instance.HideCursors();
				}
				
				//Debug.Log("after IsNull:" + ( CKinect.KinectServices.SpriteHandLeft == null) + " ID :" + CKinect.KinectServices.SpriteHandLeft.GetInstanceID() );
				//Debug.Log("CursorController sprite ID :" + CKinect.CursorController.Instance.GetCurrentSpriteCursor().GetInstanceID() );
			}
			CKinect.CursorController.Instance.IsPushActive = false;

            CKinect.CursorController.Instance.DisableCursor();

			//
			if( m_HideCursor )
				Screen.showCursor = !m_HideCursor;

			if(!string.IsNullOrEmpty(currentLoadingScene))
			{
				this.StartCoroutine(LoadScene(currentLoadingScene));
			}
		}
		
		// Update is called once per frame
		void Update () 
		{
		}

		private IEnumerator LoadScene(string loadingScene)
		{
			yield return new WaitForEndOfFrame();

			int displayProgress = 0;
			int toProgress = 0;
			AsyncOperation op = Application.LoadLevelAsync(loadingScene);
			op.allowSceneActivation = false;
			while(0.9f - op.progress <= Mathf.Epsilon ) 
			{
				toProgress = (int)op.progress * 100;
				while(displayProgress < toProgress) 
				{
					++displayProgress;
					SetLoadingPercentage(displayProgress);
					yield return new WaitForEndOfFrame();
				}
			}
			
			toProgress = 100;
			while(displayProgress < toProgress)
			{
				++displayProgress;
				SetLoadingPercentage(displayProgress);
				yield return new WaitForEndOfFrame();
			}

			op.allowSceneActivation = true;

			currentLoadingScene = null;
		}

		private void SetLoadingPercentage(int displayProgress)
		{

			m_ProgressLabel.text = displayProgress +"%";

			m_ProgressBar.value = (float)displayProgress/100f;

		}

		public static void LoadingScene(string sceneName)
		{
			if( string.IsNullOrEmpty(sceneName) )
			{
				Debug.LogError("load scene name is null or empty , Can't load !");
				return;
			}

			currentLoadingScene = sceneName;
			Application.LoadLevelAsync("LoadingScene");
		}
	}
}