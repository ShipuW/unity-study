using UnityEngine;
using System.Collections;

namespace MainScene
{
    public class Gallery : MonoBehaviour
    {       
        #region public member variables for Untiy
		//
		public int m_IDAlertWelcome = -1;
        //
        public string m_Prefix = "Gallery/img_";
        public string m_FilenameFormat = "d3";
        public int m_StartIndex = 0;
        public int m_EndIndex = 0;
        public int m_CurrIndex = 0;
        public UITexture m_UITextureCurrent, m_UITextureNext, m_UITexturePrevious;
		public bool m_AutoSize = true;
        public Vector2 m_TextureSize = new Vector2(1366, 768);
        public float m_TextureTweenDuration = 1f;
        public Vector3 m_LeftPos = Vector3.zero;
        public Vector3 m_RightPos = Vector3.zero;
        //
        public float m_PageMoveSpeed = 500.0f;
        //
        public BoxCollider m_ButtonNext = null;
        public BoxCollider m_ButtonPrevious = null;
        public string m_WaveLeftGestureName;
        public string m_WaveRightGestureName;
        #endregion
        
        #region private member variables
		//
		private Menu m_Menu = null;
        //
        private bool m_bTweening = false;
        //
        private Texture2D m_TextureNext, m_TexturePrevious; // for preload
        //
        private Vector3 m_vPageMoveDir = Vector3.zero;
        
        //private float m_fAutoPageTimeCount = 0;
        
        // 用于保存对应的位置
        //private Vector3 m_vCurrentPageStartPos = Vector3.zero;
        //private Vector3 m_vNextPageStartPos = Vector3.zero;
        //private Vector3 m_vPreviousPageStartPos = Vector3.zero;
        //
        //private float m_fLastInputTime = 0;

		//
		private UIAlert m_AlertWelcome = null;

        //
        private bool m_bInit = false;

        #endregion
        
        #region public methods
        #endregion
        
        #region private functions
        //
        private void ProcessPageMoving()
        {
            //
            if (m_vPageMoveDir.x < 0) // to left
            {
                // show previous page
                m_UITextureCurrent.transform.localPosition += m_vPageMoveDir * m_PageMoveSpeed * Time.deltaTime; 
                m_UITextureNext.transform.localPosition += m_vPageMoveDir * m_PageMoveSpeed * Time.deltaTime;
                m_UITexturePrevious.transform.localPosition += m_vPageMoveDir * m_PageMoveSpeed * Time.deltaTime;
                
                //
                if (m_UITextureCurrent.transform.localPosition.x <= m_LeftPos.x)
                {
                    //
                    m_CurrIndex++;
                    //          
                    if (m_CurrIndex > m_EndIndex)
                        m_CurrIndex = m_StartIndex;
                    
                    // exchange
                    UITexture temp = m_UITexturePrevious;
                    m_UITexturePrevious = m_UITextureCurrent;
                    m_UITextureCurrent = m_UITextureNext;
                    m_UITextureNext = temp;
                    
                    //
                    m_TexturePrevious = m_UITexturePrevious.mainTexture as Texture2D;
                    m_TextureNext = m_UITextureNext.mainTexture as Texture2D;
                    
                    // release
                    if (Mathf.Abs(m_EndIndex - m_StartIndex) > 3)
                    {
                        m_UITextureNext.mainTexture = null;
                        Resources.UnloadAsset(m_TextureNext);
                        m_TextureNext = null;
                    }
                    
                    // load next
                    int next = m_CurrIndex + 1;
                    if (next > m_EndIndex)
                        next = m_StartIndex;
                    m_TextureNext = Resources.Load<Texture2D>(m_Prefix + next.ToString(m_FilenameFormat));
                    
                    m_UITextureNext.mainTexture = m_TextureNext;
                    
                    //
                    m_UITextureCurrent.transform.localPosition = Vector3.zero; 
                    m_UITextureNext.transform.localPosition = m_RightPos;
                    m_UITexturePrevious.transform.localPosition = m_LeftPos;
                }
            } else if (m_vPageMoveDir.x > 0) // to right
            {
                // show next page
                m_UITextureCurrent.transform.localPosition += m_vPageMoveDir * m_PageMoveSpeed * Time.deltaTime; 
                m_UITextureNext.transform.localPosition += m_vPageMoveDir * m_PageMoveSpeed * Time.deltaTime;
                m_UITexturePrevious.transform.localPosition += m_vPageMoveDir * m_PageMoveSpeed * Time.deltaTime;
                
                if (m_UITextureCurrent.transform.localPosition.x >= m_RightPos.x)
                {
                    //
                    m_CurrIndex--;
                    //          
                    if (m_CurrIndex < m_StartIndex)
                        m_CurrIndex = m_EndIndex;
                    
                    // exchange
                    UITexture temp = m_UITextureNext;
                    m_UITextureNext = m_UITextureCurrent;
                    m_UITextureCurrent = m_UITexturePrevious;
                    m_UITexturePrevious = temp;
                    
                    //
                    m_TexturePrevious = m_UITexturePrevious.mainTexture as Texture2D;
                    m_TextureNext = m_UITextureNext.mainTexture as Texture2D;
                    
                    // release
                    if (Mathf.Abs(m_EndIndex - m_StartIndex) > 3)
                    {
                        m_UITexturePrevious.mainTexture = null;
                        Resources.UnloadAsset(m_TexturePrevious);
                        m_TexturePrevious = null;
                    }
                    
                    // load previous
                    int previous = m_CurrIndex - 1;
                    if (previous < m_StartIndex)
                        previous = m_EndIndex;
                    m_TexturePrevious = Resources.Load<Texture2D>(m_Prefix + previous.ToString(m_FilenameFormat));
                    
                    m_UITexturePrevious.mainTexture = m_TexturePrevious;
                    
                    //
                    m_UITextureCurrent.transform.localPosition = Vector3.zero; 
                    m_UITextureNext.transform.localPosition = m_RightPos;
                    m_UITexturePrevious.transform.localPosition = m_LeftPos;
                }
            }
        }
                
        //
        private void NextPage()
        {
            //
            float time = (m_UITextureCurrent.transform.localPosition.x - m_LeftPos.x) / m_TextureSize.x * m_TextureTweenDuration;
            //
            TweenPosition tp = m_UITextureCurrent.GetComponent<TweenPosition>();
            tp.SetStartToCurrentValue();
            tp.to = m_LeftPos;
            tp.duration = time;
            tp.ResetToBeginning();
            tp.onFinished.Clear();
            tp.PlayForward();
            
            //
            tp = m_UITexturePrevious.GetComponent<TweenPosition>();
            tp.SetStartToCurrentValue();
            tp.to = new Vector3(m_LeftPos.x - m_TextureSize.x, m_LeftPos.y, m_LeftPos.z);
            tp.duration = time;
            tp.ResetToBeginning();
            tp.onFinished.Clear();
            tp.PlayForward();
            
            //
            tp = m_UITextureNext.GetComponent<TweenPosition>();
            tp.SetStartToCurrentValue();
            tp.to = Vector3.zero;
            tp.duration = time;
            tp.ResetToBeginning();
            tp.SetOnFinished(this.OnTweenNextPageFinished);
            tp.PlayForward();
            
            //
            m_CurrIndex++;
            
            if (m_CurrIndex > m_EndIndex)
                m_CurrIndex = m_StartIndex;
            
            m_bTweening = true;
        }
        
        //
        private void PreviousPage()
        {
            //
            float time = (m_RightPos.x - m_UITextureCurrent.transform.localPosition.x) / m_TextureSize.x * m_TextureTweenDuration;
            //
            TweenPosition tp = m_UITextureCurrent.GetComponent<TweenPosition>();
            tp.SetStartToCurrentValue();
            tp.to = m_RightPos;
            tp.duration = time;
            tp.ResetToBeginning();
            tp.onFinished.Clear();
            tp.PlayForward();
            
            //
            tp = m_UITextureNext.GetComponent<TweenPosition>();
            tp.SetStartToCurrentValue();
            tp.to = new Vector3(m_RightPos.x + m_TextureSize.x, m_RightPos.y, m_RightPos.z);
            tp.duration = time;
            tp.ResetToBeginning();
            tp.onFinished.Clear();
            tp.PlayForward();
            
            tp = m_UITexturePrevious.GetComponent<TweenPosition>();
            tp.SetStartToCurrentValue();
            tp.to = Vector3.zero;
            tp.duration = time;
            tp.ResetToBeginning();
            tp.SetOnFinished(this.OnTweenPreviousPageFinished);
            tp.PlayForward();
            
            //
            m_CurrIndex--;
            
            if (m_CurrIndex < m_StartIndex)
                m_CurrIndex = m_EndIndex;
            
            m_bTweening = true;
        }

        //
        private void RepositionPage()
        {
            //
            float time = Mathf.Abs(m_UITextureCurrent.transform.localPosition.x) / m_TextureSize.x * m_TextureTweenDuration;
            //
            TweenPosition tp = m_UITextureCurrent.GetComponent<TweenPosition>();
            tp.SetStartToCurrentValue();
            tp.to = Vector3.zero;
            tp.duration = time;
            tp.ResetToBeginning();
            tp.SetOnFinished(this.OnTweenRepositionFinished);
            tp.PlayForward();
            
            //
            tp = m_UITextureNext.GetComponent<TweenPosition>();
            tp.SetStartToCurrentValue();
            tp.to = m_RightPos;
            tp.duration = time;
            tp.ResetToBeginning();
            tp.onFinished.Clear();
            tp.PlayForward();
            
            tp = m_UITexturePrevious.GetComponent<TweenPosition>();
            tp.SetStartToCurrentValue();
            tp.to = m_LeftPos;
            tp.duration = time;
            tp.ResetToBeginning();
            tp.onFinished.Clear();
            tp.PlayForward();
            
            //
            m_bTweening = true;
        }
        
        //
        private void OnTweenNextPageFinished()
        {
            // exchange
            UITexture temp = m_UITexturePrevious;
            m_UITexturePrevious = m_UITextureCurrent;
            m_UITextureCurrent = m_UITextureNext;
            m_UITextureNext = temp;
            
            //
            m_TexturePrevious = m_UITexturePrevious.mainTexture as Texture2D;
            m_TextureNext = m_UITextureNext.mainTexture as Texture2D;
            
            // release
            if (Mathf.Abs(m_EndIndex - m_StartIndex) > 3)
            {
                m_UITextureNext.mainTexture = null;
                Resources.UnloadAsset(m_TextureNext);
                m_TextureNext = null;
            }
            
            // load next
            int next = m_CurrIndex + 1;
            if (next > m_EndIndex)
                next = m_StartIndex;
            m_TextureNext = Resources.Load<Texture2D>(m_Prefix + next.ToString(m_FilenameFormat));
            
            m_UITextureNext.mainTexture = m_TextureNext;
            
            //
            m_UITextureCurrent.transform.localPosition = Vector3.zero; 
            m_UITextureNext.transform.localPosition = m_RightPos;
            m_UITexturePrevious.transform.localPosition = m_LeftPos;
            
			m_UITextureNext.transform.localScale = Vector3.one;
			m_UITexturePrevious.transform.localScale = Vector3.one;

            //
            m_bTweening = false;
            
            //m_fAutoPageTimeCount = m_AutoPageTime;
        }
        
        //
        private void OnTweenPreviousPageFinished()
        {
            // exchange
            UITexture temp = m_UITextureNext;
            m_UITextureNext = m_UITextureCurrent;
            m_UITextureCurrent = m_UITexturePrevious;
            m_UITexturePrevious = temp;
            
            //
            m_TexturePrevious = m_UITexturePrevious.mainTexture as Texture2D;
            m_TextureNext = m_UITextureNext.mainTexture as Texture2D;
            
            // release
            if (Mathf.Abs(m_EndIndex - m_StartIndex) > 3)
            {
                m_UITexturePrevious.mainTexture = null;
                Resources.UnloadAsset(m_TexturePrevious);
                m_TexturePrevious = null;
            }
            
            // load previous
            int previous = m_CurrIndex - 1;
            if (previous < m_StartIndex)
                previous = m_EndIndex;
            m_TexturePrevious = Resources.Load<Texture2D>(m_Prefix + previous.ToString(m_FilenameFormat));
            
            m_UITexturePrevious.mainTexture = m_TexturePrevious;
            
            //
            m_UITextureCurrent.transform.localPosition = Vector3.zero; 
            m_UITextureNext.transform.localPosition = m_RightPos;
            m_UITexturePrevious.transform.localPosition = m_LeftPos;

			m_UITextureNext.transform.localScale = Vector3.one;
			m_UITexturePrevious.transform.localScale = Vector3.one;

            //
            m_bTweening = false;
            
            //m_fAutoPageTimeCount = m_AutoPageTime;
        }
        
        //
        private void OnTweenRepositionFinished()
        {
            //
            m_bTweening = false;
            
            //m_fAutoPageTimeCount = m_AutoPageTime;
        }
        //
        private void OnButtonPreviousClick(GameObject go)
        {
            //
            //m_fLastInputTime = Time.time;
            
            //
            if (!m_bInit)
                return;
            
            //
            if (m_bTweening)
                return;
            
            //
            m_vPageMoveDir = Vector3.zero;
            
            // 计算是否为返回当前页面
            bool bReset = m_UITextureCurrent.transform.localPosition.x < (m_TextureSize.x * -0.5f);
            
            //
            if (bReset)
                RepositionPage();
            else
                PreviousPage();
        }
        
        //
        private void OnButtonPreviousHover(GameObject go, bool state)
        {
            //
            //m_fLastInputTime = Time.time;
            
            //
            if (!m_bInit)
                return;
            //
            if (m_bTweening)
                return;
            
            //
            if (state)
			{
                m_vPageMoveDir = Vector3.right;
			}
            else
			{
                m_vPageMoveDir = Vector3.zero;
				PreviousPage();
			}
        }
        
        //
        private void OnButtonNextClick(GameObject go)
        {
            //
            //m_fLastInputTime = Time.time;
            
            //
            if (!m_bInit)
                return;
            //
            if (m_bTweening)
                return;
            
            //
            m_vPageMoveDir = Vector3.zero;
            // 计算是否为返回当前页面
            bool bReset = m_UITextureCurrent.transform.localPosition.x > (m_TextureSize.x * 0.5f);
            
            //
            if (bReset)
                RepositionPage();
            else
                NextPage();
        }
        
        //
        private void OnButtonNextHover(GameObject go, bool state)
        {
            //
            //m_fLastInputTime = Time.time;
            
            //
            if (!m_bInit)
                return;
            //
            if (m_bTweening)
                return;
            
            //
            if( state )
			{
                m_vPageMoveDir = Vector3.left;
			}
            else
			{
                m_vPageMoveDir = Vector3.zero;
				NextPage();
			}
        }
        
		//
        private void OnGesture(object sender, CKinect.GestureEventArgs e)
        {

            // 忽略可信度低于0.9的手势
            if (e.DetectionConfidence < 0.8f)
                return;
            
            //Debug.Log( "Name:" + e.GestureName + "  " +  e.DetectionConfidence);

            if (!this.gameObject.activeSelf)
            {
                return;
            }

            //
            if (e.GestureName.Equals(m_WaveLeftGestureName))
            {
                Debug.Log("wave left--------------------------->" + e.DetectionConfidence);

                OnButtonPreviousClick(null);

            } else if (e.GestureName.Equals(m_WaveRightGestureName))
            {
                Debug.Log("wave right--------------------------->" + e.DetectionConfidence);
                OnButtonNextClick(null);
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
			
			//
			m_bTweening = false;
		}

		//
		//
		private void OnCloseAlertWelcome()
		{
			if( !m_bInit )
				return;

			//
			CKinect.CursorController.Instance.IsLoadingActive = true;
		}
        #endregion
        
        #region private Unity functions
        // Use this for initialization
        private void Start()
        {
            //
            if (m_UITextureCurrent == null)
            {
                Debug.LogError("The 'UITextureCurrent' has not assigned.", this);
                return;
            }
            
            //
            if (m_UITextureNext == null)
            {
                Debug.LogError("The 'UITextureNext' has not assigned.", this);
                return;
            }
            
            //
            if (m_UITexturePrevious == null)
            {
                Debug.LogError("The 'UITexturePrevious' has not assigned.", this);
                return;
            }

            
            TweenPosition tp = null;
            
            tp = m_UITextureCurrent.GetComponent<TweenPosition>();
            
            if (tp == null)
            {
                Debug.LogError("The 'UITextureCurrent' has not component'TweenPosition'.", this);
                return;
            }
            
            tp.duration = m_TextureTweenDuration;
            
            //
            tp = m_UITextureNext.GetComponent<TweenPosition>();
            
            if (tp == null)
            {
                Debug.LogError("The 'UITextureNext' has not component'TweenPosition'.", this);
                return;
            }
            
            tp.duration = m_TextureTweenDuration;
            
            //
            tp = m_UITexturePrevious.GetComponent<TweenPosition>();
            
            if (tp == null)
            {
                Debug.LogError("The 'UITexturePrevious' has not component'TweenPosition'.", this);
                return;
            }
            
            tp.duration = m_TextureTweenDuration;

            //
            if (m_ButtonNext == null)
            {
                Debug.LogError("The ButtonNext has not assigned.", this);
                return;
            }
            
            //
            if (m_ButtonPrevious == null)
            {
                Debug.LogError("The ButtonPrevious has not assigned.", this);
                return;
            }

            //
            //m_fLastInputTime = Time.time;

            //
            int next = m_CurrIndex + 1;
            if (next > m_EndIndex)
                next = m_StartIndex;
            m_TextureNext = Resources.Load<Texture2D>(m_Prefix + next.ToString(m_FilenameFormat));
            
            int previous = m_CurrIndex - 1;
            if (previous < m_StartIndex)
                previous = m_EndIndex;
            m_TexturePrevious = Resources.Load<Texture2D>(m_Prefix + previous.ToString(m_FilenameFormat));
            
            //
            m_UITextureCurrent.mainTexture = Resources.Load<Texture2D>(m_Prefix + m_CurrIndex.ToString(m_FilenameFormat));
            m_UITextureNext.mainTexture = m_TextureNext;
            m_UITexturePrevious.mainTexture = m_TexturePrevious;
            
            //
            UIEventListener listener = UIEventListener.Get(m_ButtonNext.gameObject);
            listener.onClick = this.OnButtonNextClick;
            listener.onHover = this.OnButtonNextHover;
            
            listener = UIEventListener.Get(m_ButtonPrevious.gameObject);
            listener.onClick = this.OnButtonPreviousClick;
            listener.onHover = this.OnButtonPreviousHover;

			//
			if( m_AutoSize )
			{
				m_TextureSize.x = Screen.width;
				m_TextureSize.y = Screen.height;

				m_LeftPos.x = -Screen.width;
				m_RightPos.x = Screen.width;
			}

            //
            m_UITextureNext.transform.localPosition = m_RightPos;
            m_UITexturePrevious.transform.localPosition = m_LeftPos;
            
            m_UITextureCurrent.width = (int)m_TextureSize.x;
            m_UITextureCurrent.height = (int)m_TextureSize.y;
            
            m_UITextureNext.width = (int)m_TextureSize.x;
            m_UITextureNext.height = (int)m_TextureSize.y;
            
            m_UITexturePrevious.width = (int)m_TextureSize.x;
            m_UITexturePrevious.height = (int)m_TextureSize.y;
                        
            //
            //m_vCurrentPageStartPos = m_UITextureCurrent.transform.localPosition;
            //m_vNextPageStartPos = m_UITextureNext.transform.localPosition;
            //m_vPreviousPageStartPos = m_UITexturePrevious.transform.localPosition;

            if (string.IsNullOrEmpty(m_WaveLeftGestureName))
            {
                Debug.LogError("The WaveGestureName has not assigned.");
                return;
            }

            if (string.IsNullOrEmpty(m_WaveRightGestureName))
            {
                Debug.LogError("The WaveGestureName has not assigned.");
                return;
            }

			//
			//
			m_Menu = this.GetComponent<Menu>();
			
			//
			if( m_Menu == null )
			{
				Debug.LogError( "This GameObject has not assigned Component:'Menu'.", this );
				return;
			}
			
			//
			m_Menu.OnOpen += this.OnMenuOpen;
			m_Menu.OnOpened += this.OnMenuOpened;
            
			//
			StartCoroutine( DelayInit() );            
        }

		//
		private IEnumerator DelayInit()
		{
			// skip a frame
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

//			//
//			CKinect.GestureManager gestureManager = CKinect.GestureManager.Instance;
//			
//			if (gestureManager == null)
//			{
//				Debug.LogError("The 'KinectGestureManager' not Instance!");
//				yield break;
//			}
//			
//			gestureManager.OnGesture += this.OnGesture;
//			
//			gestureManager.AddGestureByName(m_WaveLeftGestureName);
//			gestureManager.AddGestureByName(m_WaveRightGestureName);
			
			m_bInit = true;
		}

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
		}

        // Update is called once per frame
        private void Update()
        {
            //
            if (!m_bInit)
                return;

			if( !m_Menu.hasOpened )
				return;

            //
            if (m_bTweening)
                return;

            //
            ProcessPageMoving();

        }

        #endregion


    }

} // namespace MainScene