
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace InHouseScene
{
    public class Main : MonoBehaviour
    {

        #region static instance
        public static Main Instance { get { return s_Instance; } }

        private static Main s_Instance = null;
        #endregion

        #region public member for unity
		//
		public bool m_HideCursor = true;
        //
        //public GameObject m_EditableGameObjectParent;
        //
        public List<EditableObject> m_EditableGameObject = new List<EditableObject>();
        //
        public UIWidget m_ButtonPanel;
        //
        public UILabel m_TipsLable;

        // NGUI光标对象
        public UISprite m_SpriteHandLeft = null;
        public UISprite m_SpriteHandRight = null;
		public UISprite m_SpriteCursorLoading = null;

        public string m_ParentSceneName;
        public string m_OutHouseSceneName;
        public GameObject m_MenuGameObject;
        public Color m_FlickerColor = Color.red;

        public int m_FlickerCount = 5;

        public Camera m_MainCamera;

        public GameObject m_TouchButtonParent;
        public TouchButton[] m_TouchButtons;

		public UI.UICloseButton m_BtnCloseScene;

		public UI.UIRadioButton m_MaterialButton;
		public UI.UIRadioButton m_ModelButton;
		public UI.UIRadioButton m_MoveButton;

		public UISprite m_BtnMoveBackground,m_BtnMaterailBackground,m_BtnModelBackground;

		public Animation m_MenuAnimation;

        public float m_CameraTweenDuration = 0.5f;

		//
		public UIMore m_WidgetMore;

        public List<GameObject> m_CanChangeModelGameObject;
        public List<GameObject> m_CanChangeMetarialGameObject;
        public List<GameObject> m_CanMoveGameObject;
        public List<GameObject> m_ModelPanel;
        public List<GameObject> m_MetairalPanel;
        public GameObject m_MoveArrow;

        #endregion

        #region private const member

        private string cInMode = "修改模型模式";
        private string cInMaterial = "修改材质模式";
        private string cInMove = "移动模型模式";
        private string cNone = "请选择操作物体";

        #endregion

        #region private member

        private bool bInited = false;
        //保存当前的抓取游戏对象
        private GameObject mCurrenDragObj;
        //当前是否在移动GameObject模式
        private bool bInMove;
        //当前是否在修改材质模式
        private bool bInChangeMaterial;
        //当前是否在修改模型模式
        private bool bInChangeMode;

        private bool m_bInFlicker = false;

        private Color m_MetarialDefaultColor;

        private GameObject m_CurrentFlickerGameObject;

        private bool m_bIsMenuShowing = false;
        //
        private Transform m_CameraOriginLocation = null;

        private List<GameObject> m_HistoryRoomQueue = new List<GameObject>();

		private Vector3 m_MoveButtonInitPos,m_MaterialButtonInitPos,m_ModelButtonInitPos;
		//
		private bool m_bInTranlateCamera = false;

        //
        private bool m_bWidgetOpened = false;
        #endregion

        #region public properties

        public bool IsMenuShowing{get {return m_bIsMenuShowing;}}

        #endregion

        #region private function for unity

        private void Awake()
        {
            s_Instance = this;
        }

        // Use this for initialization
        void Start()
        {
			//
			if( Application.isEditor )
			{
				m_HideCursor = false;
			}

//            if (m_EditableGameObjectParent == null)
//            {
//                Debug.LogError("The 'EditableGameObjectParent' has not assigned!");
//                return;
//            }

            if (m_TipsLable == null)
            {
                Debug.LogError("The 'TipsLable' has not assigned!");
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

            if (string.IsNullOrEmpty(m_ParentSceneName))
            {
                Debug.LogError("The 'ParentSceneName' has not assigned.");
                return;
            }
            
            if (string.IsNullOrEmpty(m_OutHouseSceneName))
            {
                Debug.LogError("The 'OutHouseSceneName' has not assigned.");
                return;
            }

            if (m_MenuGameObject == null)
            {
                Debug.LogError("The 'MenuGameObject' has not assigned!");
                return;
            }

            if (m_MainCamera == null)
            {
                Debug.LogError("The 'MainCamera' has not assigned!");
                return;
            }

			if (m_BtnCloseScene == null)
			{
				Debug.LogError("The 'BtnCloseScene' has not assigned!");
				return;
			}

            if( m_TouchButtonParent == null )
            {
                Debug.LogError( "The TouchButtonParent has not assigned.", this );
                return;
            }
            else
            {
                m_TouchButtons = m_TouchButtonParent.GetComponentsInChildren<TouchButton>( true );
                
                if( m_TouchButtons != null && m_TouchButtons.Length > 0 )
                {
                    foreach( var btn in m_TouchButtons )
                    {
                        btn.OnClicked = this.OnTouchClicked;

						//UIEventTrigger trigger = btn.gameObject.GetComponent<UIEventTrigger>();
						//EventDelegate.Add( trigger.onClick, this.OnDragTouchBtn );
                    }
                }
                else
                {
                    Debug.LogWarning( "The TouchButtons is Null or Length is Zero.", this );
                }
            }

            if (m_MaterialButton == null)
            {
                Debug.LogError("The 'MaterialButton' has not assigned!");
                return;
            }

            if (m_MoveButton == null)
            {
                Debug.LogError("The 'MoveButton' has not assigned!");
                return;
            }

            if (m_ModelButton == null)
            {
                Debug.LogError("The 'ModelButton' has not assigned!");
                return;
            }

			if( m_WidgetMore == null )
			{
				Debug.LogError("The 'WidgetMore' has not assigned!");
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

			m_MoveButtonInitPos = m_MoveButton.transform.localPosition;
			m_ModelButtonInitPos = m_ModelButton.transform.localPosition;
			m_MaterialButtonInitPos = m_MaterialButton.transform.localPosition;

			m_MoveButton.EventReceiver = this.gameObject;
			m_MoveButton.FunctionName = "OnMoveClick";

			m_MaterialButton.EventReceiver = this.gameObject;
			m_MaterialButton.FunctionName = "OnChangeMaterialClick";

			m_ModelButton.EventReceiver = this.gameObject;
			m_ModelButton.FunctionName = "OnChangeModeClick";

            ChangeTips(cNone);

			m_BtnCloseScene.OnClicked = this.OnBackScene;

            InitModelGameObjects();
            InitMoveGameObjects();
            InitMetarialGameObjects();

            MoveButton[] mbs = m_MoveArrow.GetComponentsInChildren<MoveButton>();
            
            foreach(MoveButton mb in mbs)
            {
                UIEventListener.Get(mb.gameObject).onClick = this.OnMoveArrowClick;  
            }

            m_MenuGameObject.SetActive(false);
            m_bIsMenuShowing = false;
            EnablPanWithMouse(true);
            m_MoveArrow.SetActive(false);

			CKinect.CursorController.Instance.IsPushActive = false;
			
			//
			m_CameraOriginLocation = new GameObject().transform;
			
			m_CameraOriginLocation.position = m_MainCamera.transform.position;
			m_CameraOriginLocation.rotation = m_MainCamera.transform.rotation;
			
			m_HistoryRoomQueue.Add(m_CameraOriginLocation.gameObject);

			//
			if( m_HideCursor )
				Screen.showCursor = !m_HideCursor;

            this.StartCoroutine(DeltaAddListeners());

        }
    
        // Update is called once per frame
        void Update()
        {

			if(!bInited)
				return;

			if(UserManager.Instance.HasLockUser && !CKinect.CursorController.Instance.IsEnableCursor)
			{
				CKinect.CursorController.Instance.EnableCursor();
			}

			if(bInMove && mCurrenDragObj!=null)
			{
				MoveableObject mo = mCurrenDragObj.GetComponent<MoveableObject>();

				if(mo != null)
				{
					mo.MoveGameObject();
				}

				//MoveManager.Instance.MoveGameObject(mCurrenDragObj);
			}

        }

		private void OnDestroy() 
		{

			OverGameObject instace = OverGameObject.Instace;
			
			if(instace != null)
			{
				instace.OnOverGameObject -= OnOverGameObject;
			}

//			DragManager dragManager = DragManager.Instace;
//			
//			if (dragManager != null && dragManager.IsInit())
//			{
//				
//				dragManager.onDragGameObject -= this.onDragGameObject;
//				dragManager.OnDropGameObject -= this.OnDropGameObject;
//				dragManager.onReleaseGameobject -= this.onReleaseGameobject;
//			} 
		}
        #endregion

        #region private function

		private void TranslateCameraWhenDragGameObject(int dragGameObjectID)
		{
			
			LookAtManager instance = LookAtManager.Instance;
			
			CameraDestination[] cameraDestination = instance.CameraDestinations;
			
			CameraDestination destination = null;
			
			foreach(CameraDestination c in cameraDestination)
			{
				if(c.ID == dragGameObjectID)
				{
					destination = c;
					break;
				}
			}
			
			if(destination != null)
			{
				ProcessTranslateCamera(destination.gameObject.transform.localPosition,destination.gameObject.transform.eulerAngles,"OnTranslateCameraStart","OnTranslateCameraCompleted");
			}
		}

		private void OnTranslateCameraStart()
		{
			m_bInTranlateCamera = true;
		}

		private void OnTranslateCameraCompleted()
		{

			m_bInTranlateCamera = false;
		}


        private void OnOutRoomMoveInCompleted()
        {
            ResetPanWithMouse();
            EnablPanWithMouse(true);
			m_bInTranlateCamera = false;
        }
        //
        private void OnOutRoomStart()
        {
            EnablPanWithMouse(false);

			m_bInTranlateCamera = true;
        }

        //
		private void ProcessTranslateCamera(Vector3 postion,Vector3 eulerAngles,string startMethod,string completedMethod)
        {
			iTween.MoveTo( m_MainCamera.gameObject, iTween.Hash( "position",postion, "time", m_CameraTweenDuration,"onstart",startMethod,"onstarttarget",this.gameObject, "oncomplete", completedMethod, "oncompletetarget", this.gameObject));
            iTween.RotateTo( m_MainCamera.gameObject,eulerAngles, m_CameraTweenDuration );
        }

        private void EnablPanWithMouse(bool enabled)
        {

            m_MainCamera.GetComponent<PanWithMouse>().enabled = enabled;

        }

        private void ResetPanWithMouse()
        {
            PanWithMouse pm= m_MainCamera.GetComponent<PanWithMouse>();
            pm.Reset();
        }

        /// <summary>
        /// Inits the model game objects.
        /// </summary>
        private void InitModelGameObjects()
        {

            ModelManager modelManager =  ModelManager.Instance;

            this.m_CanChangeModelGameObject = modelManager.Models;

            this.m_ModelPanel = modelManager.ModelPanel;

			foreach(GameObject go in m_ModelPanel)
			{
				ModelPanel mp =go.GetComponent<ModelPanel>();
				
				if(mp != null && mp.OnChangeModelFinish == null)
				{
					mp.OnChangeModelFinish = this.OnChangeModelFinish;
				}

			}

        }
        /// <summary>
        /// Inits the metarial game objects.
        /// </summary>
        private void InitMetarialGameObjects()
        {
            
            MetarialManager metarialManager =  MetarialManager.Instance;
            
            this.m_CanChangeMetarialGameObject = metarialManager.Metarials;
            
            this.m_MetairalPanel = metarialManager.MetarialPanel;
            
        }

        private void InitMoveGameObjects()
        {
            
            MoveManager moveManager =  MoveManager.Instance;
            m_CanMoveGameObject = moveManager.MoveGameObjects;
            this.m_MoveArrow = moveManager.MoveArrow;
            //this.m_Can = metarialManager.Metarials;
            
            //this.m_MetairalPanel = metarialManager.MetarialPanel;
            
        }

        /// <summary>
        /// 修改提示框的显示内容
        /// </summary>
        /// <param name="tips">提示内容</param>
        private void ChangeTips(string tips)
        {
            this.m_TipsLable.text = tips;
        }

        /// <summary>
        /// 延迟注册光标落在物体上的回调
        /// </summary>
        /// <returns>The add over listener.</returns>
        public IEnumerator DeltaAddListeners()
        {
            yield return null;

			DragManager dragManager = DragManager.Instace;
			
			if (dragManager != null && dragManager.IsInit())
			{
			    
			    dragManager.onDragGameObject += this.onDragGameObject;
//			    dragManager.OnDropGameObject += this.OnDropGameObject;
//			    dragManager.onReleaseGameobject += this.onReleaseGameobject;
			} 
			else
			{
			    Debug.LogError("can't find DragManager!");
			    yield break;
			}

//			OverGameObject instace = OverGameObject.Instace;
//
//			if(instace != null)
//			{
//				instace.OnOverGameObject += OnOverGameObject;
//			}

			foreach(GameObject go in m_CanChangeModelGameObject)
			{
				AttchCallbackToEditableObject(go);
			}

			foreach( GameObject go in m_CanMoveGameObject)
			{
				AttchCallbackToEditableObject(go);
			}

			foreach( GameObject go in m_CanChangeMetarialGameObject)
			{
				AttchCallbackToEditableObject(go);
			}


			EasterEgg eggInstance = EasterEgg.Instance;

			if( eggInstance != null )
			{
				List<GameObject> eggs = eggInstance.EasterEggGameObject;

				foreach(GameObject go in eggs)
				{
					AttchCallbackToEditableObject(go);
				}
			}

            m_WidgetMore.OnOpen = () =>{HoldCloseSceneButton(); m_bWidgetOpened = true;};
            
            m_WidgetMore.OnCloseFinish = () =>{ShowCloseSceneButton(); m_bWidgetOpened = false;};

            bInited = true;
        }

		private void AttchCallbackToEditableObject(GameObject go)
		{
			EditableObject eo = go.GetComponent<EditableObject>();
			
//			if(eo != null && eo.OnClick == null)
//			{
//				eo.OnClick = this.onDragGameObject;
//			}
			
			if(eo!= null && eo.OnHoveOver == null )
			{
				eo.OnHoveOver = this.OnOverGameObject;
			}
		}

        /// <summary>
        /// 光标在物体上时的回调
        /// </summary>
        /// <param name="go">物体</param>
        /// <param name="ScreenPos">光标的坐标</param>
        private void OnOverGameObject(GameObject go,Vector3 ScreenPos)
        {

			if( !bInited )
				return;

			if( m_bInTranlateCamera )
				return;

			if( !AlterLogic.Instance.HasWelcomeShowed )
				return;

            if(m_bWidgetOpened)
                return;

			//bool isEgg = false;
           //Debug.Log("OnOverGameObject--------->" + go.name);
			EasterEgg egg = EasterEgg.Instance;
			if( egg != null && egg.IsEgg(go))
			{
				//Debug.Log("egg : " + go.name);
				egg.Run(go);
				return;
			}

            DragObject dragObject = go.GetComponent<DragObject>();

            if( dragObject != null )
            {

               GameObject parent = dragObject.Parent;
				//Debug.Log("OnOverGameObject--------->" + 1);
                if(parent!= null)
                {
					if(m_bInFlicker)
					{
						breakFlicker();
					}

					//Debug.Log("OnOverGameObject--------->" + 2);
                    if(!m_bIsMenuShowing)
                    {
						//Debug.Log("OnOverGameObject--------->" + 3);
						StartCoroutine("FlickerMetarialColor",go);
						AlterLogic.Instance.ShowAlertDrag();
                    }
                }
            }
        }

        /// <summary>
        /// 物体材质颜色闪烁
        /// </summary>
        /// <returns>The metarial color.</returns>
        /// <param name="go">Go.</param>
        private IEnumerator FlickerMetarialColor(GameObject go)
        {

            m_CurrentFlickerGameObject = go;
            
            m_bInFlicker = true;
            
            m_MetarialDefaultColor = go.renderer.material.color;

//            yield return null;

//            int i = 0;

//			while(i<m_FlickerCount)
//            {
//
//                go.renderer.material.color = Color.red;
//
//                yield return new WaitForSeconds(0.5f);
//
//                go.renderer.material.color  = m_MetarialDefaultColor;
//
//                yield return new WaitForSeconds(0.5f);
//
//                i++;
//            }

			go.renderer.material.color = Color.red;
			
			yield return new WaitForSeconds(m_FlickerCount);

			go.renderer.material.color  = m_MetarialDefaultColor;

            m_CurrentFlickerGameObject = null;
          
            m_bInFlicker = false;
        }

		/// <summary>
		/// 停止闪烁
		/// </summary>
		private void breakFlicker()
		{
			this.StopCoroutine("FlickerMetarialColor");

			m_CurrentFlickerGameObject.renderer.material.color  = m_MetarialDefaultColor;

			m_CurrentFlickerGameObject = null;
			
			m_bInFlicker = false;
		}

        /// <summary>
        /// 抓取GameObject回调
        /// </summary>
        /// <param name="go">抓取的gomeObject</param>
        /// <param name="screenPos">屏幕坐标</param>
        private void onDragGameObject(GameObject go,Vector3 screenPos)
        {
			
			if( !bInited )
				return;
            //Debug.Log("onDragGameObject CALLBACK-------------------------->" + mCurrenDragObj);
            if (mCurrenDragObj == null)
            {
                DragObject dOjb = go.GetComponent<DragObject>();
                if (dOjb != null)
                {
					mCurrenDragObj = dOjb.Parent;
					m_MenuGameObject.SetActive(true);
					EnablPanWithMouse(false);
					m_bIsMenuShowing = true;

					bool modelable = mCurrenDragObj.GetComponent<ChangeableMode>() == null?false:true;

					bool moveable = mCurrenDragObj.GetComponent<MoveableObject>() == null?false:true;

					bool materialable = mCurrenDragObj.GetComponent<ChangableMetatial>() == null?false:true;

					if(!(modelable || moveable || materialable))
					{
						return;
					}

					//Vector3 tempPos = Vector3.zero;

//					if( !moveable )
//					{
//						Vector3 v = m_MoveButton.transform.localPosition;
//						tempPos.Set( v.x,v.y,v.z);
//
//						 m_MoveButton.transform.localPosition = m_MaterialButton.transform.localPosition;
//
//						 m_MaterialButton.transform.localPosition = tempPos;
//	                        
//					}
//	                    
//					if( !materialable )
//					{
//					    Vector3 v = m_ModelButton.transform.localPosition;
//					    tempPos.Set(v.x,v.y,v.z);
//					    
//					    m_ModelButton.transform.localPosition = m_MaterialButton.transform.localPosition;
//					    
//					    m_MaterialButton.transform.localPosition = tempPos;
//					}


					m_ModelButton.gameObject.SetActive(modelable);
					
					m_MaterialButton.gameObject.SetActive(materialable);
					
					m_MoveButton.gameObject.SetActive(moveable);

					//如果只能进行一种操作 直接打开相应的操作
					int btnContent = -1;
					ResetRadioBtnState();

					btnContent += (modelable? 1:0);
					btnContent += (materialable? 2:0);
					btnContent += (moveable? 5:0);

					if( btnContent == 0 )
					{
						NGUITools.Execute<UI.UIRadioButton>(m_ModelButton.gameObject,"OnClick");
					}
					else if( btnContent == 1 )
					{
						NGUITools.Execute<UI.UIRadioButton>(m_MaterialButton.gameObject,"OnClick");
					}
					else if( btnContent == 4 )
					{
						NGUITools.Execute<UI.UIRadioButton>(m_MoveButton.gameObject,"OnClick");
					}

					EditableObject eo = mCurrenDragObj.GetComponent<EditableObject>();

					if(eo != null)
					{
						TranslateCameraWhenDragGameObject(eo.ID);
					}

					HoldMore();

					ActiveAnimation.Play(m_MenuAnimation,"Menu_In",AnimationOrTween.Direction.Forward);

					AlterLogic.Instance.ShowAlertSelectMenu();
				}
				else
				{

//					TouchButton tb = go.GetComponent<TouchButton>();
//					if( tb != null && tb.OnClicked != null )
//					{
//						tb.OnClicked( tb );
//					}

					NGUITools.Execute<UIEventTrigger>(go,"OnClick");

				}
            }
        }

        private void HoleMenuPanel()
        {
            m_ButtonPanel.gameObject.SetActive(false);
        }

		private void OnChangeModelFinish(GameObject go)
		{

			mCurrenDragObj = go;

		}

		private void ResetRadioBtnState()
		{
			m_MaterialButton.Reset();
			m_ModelButton.Reset();
			m_MoveButton.Reset();
		}

		private void HoldMore()
		{
			if(!bInited)
				return;

			m_WidgetMore.HideMoreButton();
		}

		private void ShowMore()
		{
			if(!bInited)
				return;
			
			m_WidgetMore.ShowMoreButton();
		}

        private void HoldCloseSceneButton()
        {
            if(!bInited)
                return;
            
            m_BtnCloseScene.Hide();
        }
        
        private void ShowCloseSceneButton()
        {
            if(!bInited)
                return;
            
            m_BtnCloseScene.Show();
        }
        #endregion
    
        #region public function for Button click 

        /// <summary>
        /// Button Move 点击事件处理
        /// </summary>
		public void OnMoveClick(bool state)
        {
			
			if( !bInited )
				return;

			m_BtnMoveBackground.gameObject.SetActive(state);

			if( !state )
				return;

            this.bInMove = true;
            this.bInChangeMode = false;
            this.bInChangeMaterial = false;
            ChangeTips(cInMove);
            if (mCurrenDragObj != null)
            {
                //隐藏模型选择
                foreach (GameObject g in m_ModelPanel)
                {
                    g.SetActive(false);
                }

                //隐藏材质选择
                foreach (GameObject g in m_MetairalPanel)
                {
                    g.SetActive(false);
                }

                //隐藏移动箭头
                m_MoveArrow.SetActive(true);
            }
            
            //this.Invoke ("HoleMenuPanel", 1f);
        }
        
        /// <summary>
        /// Button changge material 点击事件处理
        /// </summary>
        public void OnChangeMaterialClick( bool state )
        {
			if( !bInited )
				return;

			m_BtnMaterailBackground.gameObject.SetActive(state);

			if( !state )
				return;

            this.bInMove = false;
            this.bInChangeMode = false;
            this.bInChangeMaterial = true;
            ChangeTips(cInMaterial);
//          this.Invoke ("HoleMenuPanel", 1f);

            //Debug.Log("OnChangeMaterialClick " + (mCurrenDragObj != null));

            if (mCurrenDragObj != null)
            {
                int index = 0;
                foreach (GameObject go in m_CanChangeMetarialGameObject)
                {
                    //Debug.Log("OnChangeMaterialClick : " + go.name);
                    if (go == mCurrenDragObj)
                    {
						//MoveManager.Instance.EndMove();

						MoveableObject mo = mCurrenDragObj.GetComponent<MoveableObject>();
						if(mo != null)
						{
							mo.EndMove();
						}

                        //隐藏移动箭头
                        m_MoveArrow.SetActive(false);

                        //隐藏模型选择
                        foreach (GameObject g in m_ModelPanel)
                        {
                            g.SetActive(false);
                        }

                        //显示材质选择
                        m_MetairalPanel [index].SetActive(true);
                    }
                    
                    index ++;
                }
            }

        }

        /// <summary>
        /// Button change mode 点击事件处理
        /// </summary>
        public void OnChangeModeClick( bool state )
        {
			
			if( !bInited )
				return;

			m_BtnModelBackground.gameObject.SetActive(state);

			if( !state )
				return;

            this.bInMove = false;
            this.bInChangeMode = true;
            this.bInChangeMaterial = false;
            ChangeTips(cInMode);
//          this.Invoke ("HoleMenuPanel", 1f);

            //Debug.Log("OnChangeModeClick");

            if (mCurrenDragObj != null)
            {
                int index = 0;
                foreach (GameObject go in m_CanChangeModelGameObject)
                {
                   // Debug.Log("OnChangeModeClick : " + go.name);

                    if (go == mCurrenDragObj)
                    {
						MoveableObject mo = mCurrenDragObj.GetComponent<MoveableObject>();
						if(mo != null)
						{
							mo.EndMove();
						}

                        //隐藏移动箭头
                        m_MoveArrow.SetActive(false);

                        //隐藏材质选择
                        foreach (GameObject g in m_MetairalPanel)
                        {
                            g.SetActive(false);
                        }

                        //显示模型选择
                        m_ModelPanel [index].SetActive(true);
                    }

                    index ++;
                }
            }
        }

        public void OnBackScene()
        {
			
			if( !bInited )
				return;

//            LookAtManager instance = LookAtManager.Instance;
//            
//            if(instance != null &&  instance.HistoryQueue.Count >0)
//            {
//                GameObject go = instance.HistoryQueue.Dequeue();
//                
//				ProcessTranslateCamera(go.transform.localPosition,go.transform.eulerAngles,"OnOutRoomStart","OnOutRoomMoveInCompleted");
//
//                m_HistoryRoomQueue.RemoveAt(m_HistoryRoomQueue.Count-1);
//
//                return;
//            }

			LoadingScene.Main.LoadingScene(m_ParentSceneName);

           // Application.LoadLevel();

        }

        public void OnCloseMenu()
        {
			
			if( !bInited )
				return;

            foreach (GameObject g in m_MetairalPanel)
            {
                g.SetActive(false);
            }

            foreach (GameObject g in m_ModelPanel)
            {
                g.SetActive(false);
            }

			m_MoveButton.transform.localPosition = m_MoveButtonInitPos;
			m_ModelButton.transform.localPosition = m_ModelButtonInitPos;
			m_MaterialButton.transform.localPosition = m_MaterialButtonInitPos;
            m_MoveArrow.SetActive(false);

			bInMove = false;
			bInChangeMode = false;
			bInChangeMaterial = false;
           	
        }

        public void OnMoveArrowClick(GameObject go)
        {
			
			if( !bInited )
				return;

            if(bInMove && mCurrenDragObj != null)
            {

                MoveableObject mo = mCurrenDragObj.GetComponent<MoveableObject>();
                if(mo != null)
                {
                    MoveButton mb = go.GetComponent<MoveButton>();

                    switch(mb.Dir)
                    {

                        case MoveButton.MoveDir.LEFT:
                            mo.MoveLeft();
                            break;
                        case MoveButton.MoveDir.RIGHT:
                            mo.MoveRight();
                            break;
                        case MoveButton.MoveDir.UP:
                            mo.MoveUp();
                            break;
                        case MoveButton.MoveDir.DOWN:
                            mo.MoveDown();
                            break;
                        case MoveButton.MoveDir.BACK:
                            mo.MoveBack();
                            break;
                        case MoveButton.MoveDir.FORWARD:
                            mo.MoveForward();
                            break;
                    }
                }
            }

        }

        //
        private void OnTouchClicked( TouchButton btn )
        {
			
			if( !bInited )
				return;

            if(m_bIsMenuShowing)
                return;

            //
            //Debug.Log("Button ID:"+btn.ID);

            LookAtManager instance = LookAtManager.Instance;

            LookAt[] lookAts = instance.LookAts;

            CameraDestination[] cameraDestination = instance.CameraDestinations;

            LookAt lookAt = null;
            CameraDestination destination = null;

            foreach(LookAt l in lookAts)
            {
                if(l.ID == btn.ID)
                {
                    lookAt = l;
                    break;
                    //
                    //m_OrbitCameraDistance = (m_CameraDestination.position - m_OrbitCamera.LookAt.position).magnitude;
                }
            }

            foreach(CameraDestination c in cameraDestination)
            {
                if(c.ID == btn.ID)
                {
                    destination = c;
                    break;
                    //
                    //m_OrbitCameraDistance = (m_CameraDestination.position - m_OrbitCamera.LookAt.position).magnitude;
                }
            }

            if(lookAt != null && destination != null)
            {

               // m_OrbitCameraDistance = (m_CameraDestination.position - m_OrbitCamera.LookAt.position).magnitude;

				ProcessTranslateCamera(destination.gameObject.transform.localPosition,destination.gameObject.transform.eulerAngles,"OnOutRoomStart","OnOutRoomMoveInCompleted");

                //instance.HistoryQueue.Enqueue(m_HistoryRoomQueue[m_HistoryRoomQueue.Count-1]);

                //m_HistoryRoomQueue.Add(destination.gameObject);
            }

        }

		public void OnMenuOutFinish()
		{
			m_MenuGameObject.SetActive(false);
			
			EnablPanWithMouse(true);
			
			m_bIsMenuShowing = false;
			
			mCurrenDragObj = null;

			ProcessTranslateCamera(m_CameraOriginLocation.localPosition,m_CameraOriginLocation.eulerAngles,"OnTranslateCameraStart","OnTranslateCameraCompleted");
			ShowMore();
		}

        #endregion
       
    }

}
