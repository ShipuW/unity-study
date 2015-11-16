using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MainScene
{
	public class VideoPlayer : MonoBehaviour 
	{
		//
		public GameObject m_ParentVideoItem;
		//
		public VideoItem[] m_VideoItems;
		//
		public string m_Folder;
		//
		public List<string> m_Filenames;
		//视频输出的texture
		public UITexture m_VideoOutputTexture;

		//
		public UITexture m_VideoOutputTexMask;

		//播放的AVProQuickTimeMovie和加载的AVProQuickTimeMovie
		public AVProQuickTimeMovie m_MovieA;

		public AVProQuickTimeMovie m_MovieB;
		//在menu打开时是否自动播放
		public bool m_PlayOnStart;
		//
		public int m_IDAlertWelcome = -1;

		//
		public string m_WaveLeftGestureName = null;

		public string m_WaveRightGestureName = null;
		//连续两次挥手的时间间隔
		public float m_WaveVideoDelayTime = 2f;

		public Texture m_VideoDefaultTexture;

		#region private member
		//
		private Menu m_Menu = null;
		//
		private bool m_bInit = false;
		//
		private bool m_bTweening = false;
		//
		private UIAlert m_AlertWelcome = null;
		
		private AVProQuickTimeMovie[] _movies;
		private int _moviePlayIndex;
		private int _movieLoadIndex;
		private int _index = -1;
		private bool _loadSuccess = true;
		private int _playItemIndex = -1;
		//上次挥手的时间间隔
		private float m_WaveHandTime;

		private bool m_bInZoomIn = false;
		private bool n_bInTweenZoom = false;
		
		#endregion

		#region public properties
		public AVProQuickTimeMovie PlayingMovie  { get { return _movies[_moviePlayIndex]; } }
		public AVProQuickTimeMovie LoadingMovie  { get { return _movies[_movieLoadIndex]; } }
		public int PlayingItemIndex { get { return _playItemIndex; } }
		public bool IsPaused { get { if (PlayingMovie.MovieInstance != null) return !PlayingMovie.MovieInstance.IsPlaying; return false; } }
		#endregion

		#region private function for unity
		// Use this for initialization
		void Start ()
		{

			if(m_ParentVideoItem == null)
			{
				Debug.LogError("The 'ParentVideoItem' has not assigned!");
				return;
			}

			m_VideoItems = m_ParentVideoItem.GetComponentsInChildren<VideoItem>();

			if( m_VideoItems == null || m_VideoItems.Length<=0 )
			{
				Debug.LogError("Can't find Components 'VideoItem' in " + m_ParentVideoItem.name);
				return;
			}

			foreach(VideoItem vi in m_VideoItems)
			{
				//UIEventListener.Get(vi.gameObject).onClick = OnVideoItemClick;

				UIToggle toggle = vi.GetComponent<UIToggle>();

				EventDelegate.Add(toggle.onChange,OnToggleChanged);
			}

			if( m_VideoOutputTexture == null )
			{
				Debug.LogError( "The 'VideoOutputTexture' has not assigned!" );
				return;
			}

			if( m_VideoOutputTexMask == null )
			{
				Debug.LogError( "The 'VideoOutputTexMask' has not assigned!" );
				return;
			}

			if( string.IsNullOrEmpty( m_Folder ) )
			{
				Debug.LogError( "The Video folder is Null or Empty", this );
				return;
			}

			if( m_Filenames == null || m_Filenames.Count <= 0 )
			{
				Debug.LogError( "The Video file names is Null or count <= 0!", this );
				return;
			}

			if( m_MovieA == null )
			{
				Debug.LogError( "The 'MovieA' has not assigned!", this );
				return;
			}

			if( m_MovieB == null )
			{
				Debug.LogError( "The 'MovieB' has not assigned!", this );
				return;
			}

			//
			if( string.IsNullOrEmpty( m_WaveLeftGestureName ) )
			{
				Debug.LogError( "The WaveLeftGestureName is Null or Empty", this );
				return;
			}
			
			//
			if( string.IsNullOrEmpty( m_WaveRightGestureName ) )
			{
				Debug.LogError( "The WaveRightGestureName is Null or Empty", this );
				return;
			}

			if( m_VideoDefaultTexture == null )
			{
				Debug.LogError( " The 'VideoDefaultTexture' has not assigned! ",this);
				return;
			}

			UIEventListener.Get(m_VideoOutputTexture.gameObject).onClick= this.ZoomMoiveTex;

			//
			m_Menu = this.GetComponent<Menu>();
			
			//
			if( m_Menu == null )
			{
				Debug.LogError( "This GameObject has not assigned Component:'Menu'.", this );
				return;
			}

			m_VideoOutputTexture.mainTexture = m_VideoDefaultTexture;

			//
			m_Menu.OnOpen += this.OnMenuOpen;
			m_Menu.OnOpened += this.OnMenuOpened;
			m_Menu.OnClose += this.OnMenuClose;

			m_MovieA._loop = false;
			m_MovieB._loop = false;
			_movies = new AVProQuickTimeMovie[2];
			_movies[0] = m_MovieA;
			_movies[1] = m_MovieB;
			_moviePlayIndex = 0;
			_movieLoadIndex = 1;

			m_Folder = Application.streamingAssetsPath + m_Folder;

			StartCoroutine( DelayInit() );

		}
		
		// Update is called once per frame
		void Update ()
		{
			if( !m_bInit )
				return;

			if (PlayingMovie.MovieInstance != null)
			{
				//自动播放下一个视频
				//Debug.Log( "PlayingMovie.MovieInstance.Frame  " + PlayingMovie.MovieInstance.Frame +"  PlayingMovie.MovieInstance.FrameCount  " + PlayingMovie.MovieInstance.FrameCount);
				if (PlayingMovie.MovieInstance.Frame >= PlayingMovie.MovieInstance.FrameCount - 1 && PlayingMovie.MovieInstance.FrameCount > 0 || PlayingMovie.MovieInstance.IsFinishedPlaying)
				{
					//Debug.Log( " Next movie in update! ");
					//NextMovie();
					GameObject go = m_VideoItems[ (_index + 1) % m_VideoItems.Length ].gameObject;
					//NGUITools.Execute<UIEventListener>(go,"OnClick");
					NGUITools.Execute<UIToggle>(go,"OnClick");
				}
			}
			
	//		if (!_loadSuccess)
	//		{
	//			_loadSuccess = true;
	//			NextMovie();
	//		}

			if(!IsPaused)
			{
				Texture texture = PlayingMovie.OutputTexture;
				//if ( texture == null )
				//	texture = LoadingMovie.OutputTexture;

				if( texture != null )
				{
					m_VideoOutputTexture.mainTexture = texture;
				}
//
//				if(n_bInTweenZoom)
//				{
//
//				}

			}
		}

		//
		//
		private void OnDestroy()
		{
			// 是否初始化
			if( !m_bInit )
				return;
			
			//			CKinect.GestureManager.Instance.OnGesture -= this.OnGesture;
			//			CKinect.GestureManager.Instance.RemoveGestureByName(m_WaveLeftGestureName);
			//			CKinect.GestureManager.Instance.RemoveGestureByName(m_WaveRightGestureName);
			
			m_Menu.OnOpen -= this.OnMenuOpen;
			m_Menu.OnOpened -= this.OnMenuOpened;
			m_Menu.OnClose -= this.OnMenuClose;
		}

		private void  OnEnable ()
		{

			BetterList<UIToggle> list = UIToggle.list;
			
			//UIToggle toggle = UIToggle.GetActiveToggle(1);
			
			for (int i = 0; i < list.size; ++i)
			{
				UIToggle toggle = list[i];
				if (toggle != null && toggle.group == 1 )
				{
					toggle.Set(false);
					
					Debug.Log("toggle set false");
					
				}
			}
		}

		private void OnDisable ()
		{
			if( !m_bInit )
				return;

			//恢复原始状态
			Pause();
			m_VideoOutputTexture.mainTexture = m_VideoDefaultTexture;
			_index = -1;
			_moviePlayIndex = 0;
			_movieLoadIndex = 1;
		}

		#endregion

		#region public function

		/// <summary>
		/// UIToggle 的OnChanged回调
		/// </summary>
		public void OnToggleChanged()
		{

			//Debug.Log( " OnToggleChanged --------- > ");

			UIToggle current = UIToggle.current;

			VideoItem vi = current.transform.gameObject.GetComponentInChildren<VideoItem>();
			
			if( vi != null && current.value)
			{
				_index = vi.ID - 1;
				
				NextMovie();
			}

//			BetterList<UIToggle> list = UIToggle.list;
//
//			//UIToggle toggle = UIToggle.GetActiveToggle(1);
//
//			for (int i = 0; i < list.size; ++i)
//			{
//				UIToggle toggle = list[i];
//				if (toggle != null && toggle.group == 1 )
//				{
//					//UISprite sprite = toggle.gameObject.GetComponentInChildren<UISprite>();
//					if( toggle.value )
//					{
//						//sprite.color = Color.green;
//						toggle.Set(true);
//					}
//					else
//					{
//						//sprite.color = Color.white;
//						toggle.Set(false);
//					}
//				}
//			}
		}

		/// <summary>
		/// 点击回调
		/// </summary>
		/// <param name="go">当前点击的GameObject</param>
		public void OnVideoItemClick(GameObject go)
		{

			VideoItem vi = go.GetComponentInChildren<VideoItem>();

			if( vi != null )
			{
				_index = vi.ID - 1;

				NextMovie();
			}
		}

		/// <summary>
		/// 暂停播放
		/// </summary>
		public void Pause()
		{
			if (PlayingMovie != null)
			{
				PlayingMovie.Pause();
			}
		}

		/// <summary>
		/// 恢复播放
		/// </summary>
		public void Unpause()
		{
			if (PlayingMovie != null)
			{
				PlayingMovie.Play();
			}
		}

		public void ZoomMoiveTex(GameObject go)
		{
			if(!m_bInit || m_bTweening ||n_bInTweenZoom)
				return;

			if(_index < 0)
				return;

			n_bInTweenZoom = true;
			if(m_bInZoomIn)
			{
				//缩小
				TweenTransform tt = m_VideoOutputTexture.GetComponent<TweenTransform>();
				tt.PlayReverse();
				tt = m_VideoOutputTexMask.GetComponent<TweenTransform>();
				tt.PlayReverse();
				tt.SetOnFinished(OnZoomOutFinish);

				m_bInZoomIn = false;
			}
			else
			{
				//放大
				TweenTransform tt = m_VideoOutputTexture.GetComponent<TweenTransform>();
				tt.PlayForward();
				tt = m_VideoOutputTexMask.GetComponent<TweenTransform>();
				tt.PlayForward();
				tt.SetOnFinished(ZoomInFinish);

				m_bInZoomIn = true;
			}
		}

		private void OnZoomOutFinish()
		{
			n_bInTweenZoom = false;
		}

		private void ZoomInFinish()
		{
			n_bInTweenZoom = false;
		}
		#endregion

		#region private function
		//
		private IEnumerator DelayInit()
		{
			//
			yield return null;

//			//
//			CKinect.GestureManager gestureManager = CKinect.GestureManager.Instance;
//			
//			//
//			if( gestureManager == null )
//			{
//				Debug.LogError( "The 'KinectGestureManager' not Instance!", this );
//				yield break;
//			}
//			
//			gestureManager.OnGesture += this.OnGesture;
//			
//			gestureManager.AddGestureByName(m_WaveLeftGestureName);
//			gestureManager.AddGestureByName(m_WaveRightGestureName);

			//
			m_AlertWelcome = UIAlertManager.Instance.GetAlert( m_IDAlertWelcome );
			
			if( m_AlertWelcome == null )
			{
				Debug.LogError( "There's not a UIAlert -- IDAlertWelcome:" + m_IDAlertWelcome, this );
				yield break;
			}
			
			//
			m_AlertWelcome.OnClose = this.OnCloseAlertWelcome;

			m_bInit = true;
		}

		//
		private void OnCloseAlertWelcome()
		{
			if( !m_bInit )
				return;
			//
			CKinect.CursorController.Instance.IsLoadingActive = true;

			if(m_PlayOnStart)
			{
				//NextMovie();
				GameObject go = m_VideoItems[0].gameObject;
				NGUITools.Execute<UIEventListener>(go,"OnClick");

				NGUITools.Execute<UIToggle>(go,"OnClick");
				
				//UISprite sprite = go.GetComponentInChildren<UISprite>();
				
				//sprite.color = Color.green;
			}
		}

		public void OnMenuClose( Menu menu )
		{
			Debug.Log("OnMenuClose");
			BetterList<UIToggle> list = UIToggle.list;
			
			//UIToggle toggle = UIToggle.GetActiveToggle(1);
			
			for (int i = 0; i < list.size; ++i)
			{
				UIToggle toggle = list[i];
				if (toggle != null && toggle.group == 1 )
				{
					toggle.Set(false);

					Debug.Log("toggle set false");

				}
			}
		}

		//
		private void OnMenuOpen( Menu menu )
		{
			//
			if( !m_bInit )
				return;
			//
			m_AlertWelcome.Show( 1.0f );
			
			//
			m_bTweening = true;

			//
			if(m_bInZoomIn)
			{
				//缩小
				TweenTransform tt = m_VideoOutputTexture.GetComponent<TweenTransform>();
				tt.PlayReverse();
				tt = m_VideoOutputTexMask.GetComponent<TweenTransform>();
				tt.PlayReverse();				
				m_bInZoomIn = false;
				n_bInTweenZoom = false;
			}
		}
		
		//
		private void OnMenuOpened( Menu menu )
		{
			//
			if( !m_bInit )
				return;
			
			//
			CKinect.CursorController.Instance.EnableCursor();
			
			//
			CKinect.CursorController.Instance.IsLoadingActive = false;

			m_WaveHandTime = Time.time;

			//
			m_bTweening = false;
		}

		private void NextMovie()
		{	
			Pause();

			//Debug.Log("play index  " + _index);

			if (m_Filenames.Count > 0)
			{
				_index = (Mathf.Max(0, _index+1))%m_Filenames.Count;
			}
			else
				_index = -1;
			
			if (_index < 0)
				return;

			//Debug.Log("play index  " + _index);
			
			LoadingMovie._folder = m_Folder;
			LoadingMovie._filename = m_Filenames[_index];
			LoadingMovie._playOnStart = true;
			_loadSuccess = LoadingMovie.LoadMovie();
			_playItemIndex = _index;
			
			_moviePlayIndex = ( _moviePlayIndex + 1 )%2;
			_movieLoadIndex = ( _movieLoadIndex + 1 )%2;		
		}


		//
//		private void OnGesture( object sender, CKinect.GestureEventArgs e )
//		{
//			// 忽略可信度低于0.9的手势
//			if( e.DetectionConfidence < 0.9f )
//				return;
//			
//			if( !this.gameObject.activeSelf )
//			{
//				return;
//			}
//			
//			//
//			if( m_bTweening )
//				return;
//
//			if(Time.time - m_WaveHandTime < m_WaveVideoDelayTime)
//			{
//				return;
//			}
//
//			//
//			if( e.GestureName.Equals( m_WaveLeftGestureName ) )
//			{
//				//Debug.Log( "wave left--------------------------->" + e.DetectionConfidence );
//				GameObject go = m_VideoItems[ (_index - 1) % m_VideoItems.Length].gameObject;
//				//NGUITools.Execute<UIEventListener>(go,"OnClick");
//				NGUITools.Execute<UIToggle>(go,"OnClick");
//
//				m_WaveHandTime = Time.time;
//
//			} 
//			else if( e.GestureName.Equals( m_WaveRightGestureName ) )
//			{
//				//Debug.Log( "wave right--------------------------->" + e.DetectionConfidence );
//				GameObject go = m_VideoItems[ (_index + 1) % m_VideoItems.Length ].gameObject;
//				//NGUITools.Execute<UIEventListener>(go,"OnClick");
//				NGUITools.Execute<UIToggle>(go,"OnClick");
//
//				m_WaveHandTime = Time.time;
//			}
//		}

		#endregion
	}
}
