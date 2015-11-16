using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

namespace StandBy
{
	public class Main : MonoBehaviour 
	{

		#region public member
		//
		public bool m_HideCursor = true;

		//cursor sprite
		public UISprite m_CursorLeft;
		public UISprite m_CursorRight;
		public UISprite m_CursorLoading;

		//视频输出
		public UITexture m_MoiveOutputTexture;

		//视频播放队列
		public MoiveQueue m_MoivePlayer;

		//开始是否播放视频
		public bool m_bPlayMoiveOnStart;

		//没有人到播放视频的间隔
		public float m_PlayMoiveDelayTime;

		//人物剪影texture
		public UITexture m_BodyTexture;

		//下个场景名字
		public string m_NextSceneName;

		public int m_IDAlertWelcome;

		public int m_IDAlertHandUp;

		
		public float m_AlertWaveHandInterval = 5;
		#endregion

		#region private member

		private bool m_bInited = false;

		private float m_TrackedBodyTime;

		private bool m_bMoivePlaying = false;

		private ComputeBuffer m_DepthBuffer;
		private ComputeBuffer m_BodyIndexBuffer;
		
		private Windows.Kinect.DepthSpacePoint[] m_DepthPoints;
		private byte[] m_BodyIndexPoints;

		//
		private UIAlert m_AlertWelcome = null;
		
		//
		private UIAlert m_AlertHandUp = null;

		private float m_fAlertHandUpLastTime = 0;

		private bool m_bInLoadingScene = false;

		private bool m_bTrackedBody = false;

		#endregion

		#region private function for unity
		// Use this for initialization
		void Start () 
		{
			//
			if( Application.isEditor )
			{
				m_HideCursor = false;
			}

			if (m_CursorLeft == null)
			{
				Debug.LogError("The 'SpriteHandLeft' has not assigned!");
				return;
			}
			
			if (m_CursorRight == null)
			{
				Debug.LogError("The 'SpriteHandRight' has not assigned!");
				return;
			}
			
			if ( m_CursorLoading == null )
			{
				Debug.LogError("The 'SpriteCursorLoading' has not assigned!");
				return;
			}

			if (string.IsNullOrEmpty( m_NextSceneName ))
			{
				Debug.LogError("The 'NextSceneName' is empty or Null!");
				return;
			}
			if (m_MoiveOutputTexture == null)
			{
				Debug.LogError("The 'MoiveOutputTexture' has not assigned!");
				return;
			}
			
			if ( m_MoivePlayer == null )
			{
				Debug.LogError("The 'MoivePlayer' has not assigned!");
				return;
			}

			if ( m_BodyTexture == null )
			{
				Debug.LogError("The 'BodyTexture' has not assigned!");
				return;
			}

			CKinect.KinectServices instace = CKinect.KinectServices.Instance;

			if( instace == null )
			{
				CKinect.KinectServices.Create(m_CursorLeft,m_CursorRight,m_CursorLoading);
			}
			else
			{
				CKinect.CursorController.Instance.SpriteHandLeft = m_CursorLeft;
				CKinect.CursorController.Instance.SpriteHandRight = m_CursorRight;
				CKinect.CursorController.Instance.SpriteCursorLoading = m_CursorLoading;
			}

			//
			if( m_HideCursor )
				Screen.showCursor = !m_HideCursor;

			//
			this.StartCoroutine(DelayInit());
		}
		
		// Update is called once per frame
		void Update () 
		{
			if(!m_bInited || m_bInLoadingScene)
				return;

			CKinect.MultiSourceManager mmInstace = CKinect.MultiSourceManager.Instance;

			//
			if(mmInstace != null)
			{
				//
				Kinect.Body[] bodies = mmInstace.GetBodyData();

				if( bodies != null && bodies.Length > 0 && IsMoivePlay() )
				{
					foreach(Kinect.Body body in bodies)
					{
						if(body != null && body.IsTracked)
						{
							m_TrackedBodyTime = Time.time;

							m_bTrackedBody = true;

							PuaseMoive();
							HoldMoive();
							ActiveBodyTexture();
							break;
						}

						m_bTrackedBody = false;
					}
				}

				if(Time.time - m_TrackedBodyTime > m_PlayMoiveDelayTime && !m_bMoivePlaying)
				{

					//Debug.Log("----------------------> + hold moive!" );
					HoldBodyTexture();
					ActiveMoive();
					ResetMoive();
				}

				if(UserManager.Instance.Inited && UserManager.Instance.HasLockUser && !m_bInLoadingScene)
				{
					LoadScene();

					//Debug.Log("----------------------> + Loading Scene!" );
				}
			}

		}

		//
		private void FixedUpdate()
		{
			// 是否初始化
			if( !m_bInited )
				return;
			
			//
			if( m_AlertWelcome.GetVisible() )
				return;
			
			//
			if(!m_bMoivePlaying && !m_AlertHandUp.GetVisible() && m_bTrackedBody )
			{
				//
				if( Time.time - m_fAlertHandUpLastTime > m_AlertWaveHandInterval )
				{
					m_fAlertHandUpLastTime = Time.time;
					
					m_AlertHandUp.Show( 0.5f );
				}
				
				return;
			}
		}

		#endregion

		#region private function

		private IEnumerator DelayInit()
		{
			//
			yield return null;

			//
			m_AlertWelcome = UIAlertManager.Instance.GetAlert( m_IDAlertWelcome );
			
			if( m_AlertWelcome == null )
			{
				Debug.LogError( "There's not a UIAlert -- IDAlertWelcome:" + m_IDAlertWelcome, this );
				yield break;
			}
			
			//
			m_AlertWelcome.OnClose = this.OnCloseAlertWelcome;
			
			//
			m_AlertHandUp = UIAlertManager.Instance.GetAlert( m_IDAlertHandUp );
			
			if( m_AlertHandUp == null )
			{
				Debug.LogError( "There's not a UIAlert -- IDAlertWaveHand:" + m_IDAlertHandUp, this );
				yield break;
			}
			
			m_AlertHandUp.OnClose = this.OnCloseAlertWaveHand;

			//
			CKinect.CursorController.Instance.DisableCursor();
			
			if( m_bPlayMoiveOnStart )
			{
				HoldBodyTexture();
				ActiveMoive();
				PlayMoive();
			}
			else
			{
				HoldMoive();
				ActiveBodyTexture();
			}

			m_AlertWelcome.Show(1.0f);

			//
			m_bInited = true;
		}

		private void HoldMoive()
		{
			if( m_MoiveOutputTexture.gameObject.activeSelf )
				m_MoiveOutputTexture.gameObject.SetActive(false);
		}

		private void ActiveMoive()
		{
			if( !m_MoiveOutputTexture.gameObject.activeSelf )
				m_MoiveOutputTexture.gameObject.SetActive(true);
		}

		private void HoldBodyTexture()
		{
			if( m_BodyTexture.gameObject.activeSelf )
				m_BodyTexture.gameObject.SetActive(false);
		}
		
		private void ActiveBodyTexture()
		{
			if( !m_BodyTexture.gameObject.activeSelf )
				m_BodyTexture.gameObject.SetActive(true);
		}

		private void PuaseMoive()
		{
			m_MoivePlayer.Pause();
			m_bMoivePlaying = false; 
		}

		private void PlayMoive()
		{
			m_MoivePlayer.Next();
			m_bMoivePlaying = true;
		}

		private void ResetMoive()
		{
			m_MoivePlayer.Reset();
			PlayMoive();
		}

		private bool IsMoivePlay()
		{

			AVProQuickTime movieInstance = m_MoivePlayer.PlayingMovie.MovieInstance;

			if( movieInstance != null  )
			{
				return movieInstance.IsPlaying || movieInstance.IsPaused;
			}

			return false;

		}

		private void LoadScene()
		{
			//Application.LoadLevel(m_NextSceneName);
			m_bInLoadingScene = true;
			LoadingScene.Main.LoadingScene(m_NextSceneName);

		}

		//
		private void OnCloseAlertWelcome()
		{
			if( !m_bInited )
				return;
			
			//
			m_fAlertHandUpLastTime = Time.time;
		}
		
		//
		private void OnCloseAlertWaveHand()
		{
			if( !m_bInited )
				return;
			
			m_fAlertHandUpLastTime = Time.time;
		}
		#endregion
	}
}
