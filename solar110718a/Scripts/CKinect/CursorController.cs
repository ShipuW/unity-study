using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CKinect
{
	/// <summary>
	/// Kinect光标控制器
	/// </summary>
	public class CursorController : MonoBehaviour
	{
		#region static instance
		public static CursorController Instance { get { return s_Instance; } }
		
		private static CursorController s_Instance = null;
		#endregion
		
		#region public member variables
		//
		public bool m_DebugLog = true;
		// NGUI图集
		public UIAtlas m_Atlas;
		// 正在推(按)状态序列帧前缀名
		public string m_CursorPushingStateNamePrefix;
		// 正在推(按)状态序列帧开始与结束编号
		public int m_CursorPushingStateIndexStart, m_CursorPushingStateIndexEnd;
		// 完成推(按)状态序列帧前缀名
		public string m_CursorPushedStateNamePrefix;
		// 完成推(按)状态序列帧开始与结束编号
		public int m_CursorPushedStateIndexStart, m_CursorPushedStateIndexEnd;
		// 拖动状态序列帧前缀名
		public string m_CursorDragStateNamePrefix;
		// 拖动状态序列帧开始与结束编号
		public int m_CursorDragStateIndexStart, m_CursorDragStateIndexEnd;
		// 光标动画帧率
		public int m_CursorAnimFramePerSecond = 30;
		// 光标大小
		public int m_CursorWidth, m_CursorHeight;
		// NGUI光标对象
		public UISprite m_SpriteHandLeft = null;
		public UISprite m_SpriteHandRight = null;
		// 连续帧阀值，用于当某个状态可信度低的时候需要考察后续多少续的状态
		public int m_ConsecutiveThreshold = 15;
		// 是否启用推按动作
		public bool m_PushActive = true;
		// 推进(push)距离阀值
		public float m_PushThreshold = 0.2f;
		
		public bool m_LoadingActive = true;
		
		// debug
		public bool m_Debug = false;
		public Color m_ColorCursorOpen = Color.white;
		public Color m_ColorCursorClosed = Color.green;
		public Color m_ColorCursorLasso = Color.blue;
		public Color m_ColorCursorNotTracked = Color.red;
		public Color m_ColorCursorUnknown = Color.yellow;
		
		// 正在推(按)状态序列帧前缀名
		public string m_CursorLoadingStateNamePrefix;
		// 正在推(按)状态序列帧开始与结束编号
		public int m_CursorLoadingStateIndexStart, m_CursorLoadingStateIndexEnd;
		public UISprite m_CursorLoadingSprite;
		
		//显示loading动画的阈值
		public float m_ShowLoadingAnimThreshold = 1.0f;
		
		//cursor晃动阀值
		public int m_LoadingThreshold = 10;
		
		public delegate void OnLoadingStartDelegate();
		
		public OnLoadingStartDelegate OnLoadingStart;
		
		public delegate void OnLoadingDelegate(float progress);
		
		public OnLoadingDelegate OnLoading;
		
		public delegate void OnLoadingFinishingDelegate();
		
		public OnLoadingFinishingDelegate OnLoadingFinish;
		
		// 当正在推按时调用
		public delegate void OnPushingDelegate(float fPushLength);
		
		public OnPushingDelegate OnHandLeftPushing, OnHandRightPushing;
		// 当开始推按动画时调用
		public delegate void OnPushedStartDelegate();
		
		public OnPushedStartDelegate OnHandLeftPushedStart, OnHandRightPushedStart;
		// 当完成推按动画时调用
		public delegate void OnPushedFinishedDelegate();
		
		public OnPushedFinishedDelegate OnHandLeftPushedFinished, OnHandRightPushedFinished;
		// 当开始拖动时调用
		public delegate void OnDragStartDelegate(Vector3 vCursorPos);
		
		public OnDragStartDelegate OnHandLeftDragStart, OnHandRightDragStart;
		// 当结束拖动时调用
		public delegate void OnDragFinishedDelegate(Vector3 vCursorPos);
		
		public OnDragFinishedDelegate OnHandLeftDragFinished, OnHandRightDragFinished;
		#endregion
		
		#region private member variables
		// 用于判断是否初始化成功
		private bool m_bInit = false;
		// 用于保存左右手图标的最后的位置
		private Vector3 m_vHandLeftCursorLastPos = Utils.InvaildVec3;
		private Vector3 m_vHandRightCursorLastPos = Utils.InvaildVec3;
		// 用于保存左右手做推挤动作时的开始位置
		private Vector3 m_vHandLeftPushStartPos = Utils.InvaildVec3;
		private Vector3 m_vHandRightPushStartPos = Utils.InvaildVec3;
		// 用于保存左右手的原始数据的位置
		private Vector3 m_vHandLeftRawPos = Utils.InvaildVec3;
		private Vector3 m_vHandRightRawPos = Utils.InvaildVec3;
		private Vector3 m_vHandLeftRawLastPos = Utils.InvaildVec3;
		private Vector3 m_vHandRightRawLastPos = Utils.InvaildVec3;
		// 推按动作的推按长度（是个比例值，值范围0-1）
		private float m_fPushing = 0;
		// 用于保存左右手图标的最后的位置
		//private Vector3 m_vHandLeftLastPos = Vector3.zero;
		//private Vector3 m_vHandRightLastPos = Vector3.zero;
		//private bool m_bHasSetLastPos = false;
		// 用于保存左右手状态
		private Windows.Kinect.HandState m_HandLeftState = Windows.Kinect.HandState.NotTracked;
		private Windows.Kinect.HandState m_HandRightState = Windows.Kinect.HandState.NotTracked;
		// 手部状态循环队列限制长度
		private const int LIMIT_LEN = 30; // for a second
		// 队列先进先出，用于保存最近的记录
		private List<HandStateData> m_HandLeftStates = new List<HandStateData>(LIMIT_LEN);
		private List<HandStateData> m_HandRightStates = new List<HandStateData>(LIMIT_LEN);
		
		
		private int m_iCurrentLoadingFrameIndex;
		private bool m_IsPlayingLoadingAmin;
		
		// 光标动画一帧时间
		private float CursorLoadingAnimFrameTime { set; get; }
		
		// 用于保存上一次光标动画更新时间
		private float CursorLoadingAnimLastTime { set; get; }
		
		private float m_OnOverUITime;
		private GameObject m_OverGameObject;
		private Vector3 m_CursorPosWhenOver = Vector3.zero;
		private Vector3 m_LoadingStartPos = Vector3.zero;
		
		#endregion
		
		#region private enum/class/struct
		// 光标动画状态
		private enum CursorAnimType
		{
			Pushing,
			Pushed,
			Drag,
			Lasso,
			None
		}
		
		// 手部状态数据集合
		private struct HandStateData
		{
			public Windows.Kinect.HandState state;
			public Windows.Kinect.TrackingConfidence confidence;
			
			public HandStateData(Windows.Kinect.HandState state, Windows.Kinect.TrackingConfidence confidence)
			{
				this.state = state;
				this.confidence = confidence;
			}
		}
		#endregion
		
		#region public properties
		// NGUI光标对象
		public UISprite SpriteHandLeft
		{
			get{ return m_SpriteHandLeft; }
			set
			{
				m_SpriteHandLeft = value;
				m_SpriteHandLeft.flip = UIBasicSprite.Flip.Horizontally;
			}
		}
		
		public UISprite SpriteHandRight { get { return m_SpriteHandRight; } set { m_SpriteHandRight = value; } }
		
		public UISprite SpriteCursorLoading { get { return m_CursorLoadingSprite; } set { m_CursorLoadingSprite = value;} }
		
		// 推按阀值
		public float PushThreshold { get { return m_PushThreshold; } set { m_PushThreshold = value; } }
		// 是否启用推按动作状态
		public bool IsPushActive { get { return m_PushActive; } set { m_PushActive = value; } }
		// 左右手状态
		public Windows.Kinect.HandState HandLeftState { get { return m_HandLeftState; } private set { m_HandLeftState = value; } }
		
		public Windows.Kinect.HandState HandRightState { get { return m_HandRightState; } private set { m_HandRightState = value; } }
		// 是否有任意一只手处于握拳状态
		public bool HasHandClosed { get { return HandLeftState == Windows.Kinect.HandState.Closed || HandRightState == Windows.Kinect.HandState.Closed; } }
		// 连续帧阀值，用于当某个状态可信度低的时候需要考察后续多少续的状态
		public int ConsecutiveThreshold { get { return m_ConsecutiveThreshold; } set { m_ConsecutiveThreshold = value; } }
		
		// 是否启用推按动作状态
		public bool IsLoadingActive { get { return m_LoadingActive; } set { m_LoadingActive = value; } }
		
		// 是否启用光标功能
		public bool IsEnableCursor { get; private set; }
		#endregion
		
		#region private properties
		// 默认光标名称
		private string CursorDefaultName { set; get; }
		// 拖动光标开始名称
		private string CursorDragStartName { set; get; }
		// 拖动光标结束名称
		private string CursorDragEndName { set; get; }
		
		// 是否有光标动画正在播放
		private bool HasCursorAnimPlaying { set; get; }
		// 光标动画一帧时间
		private float CursorAnimFrameTime { set; get; }
		// 用于保存上一次光标动画更新时间
		private float CursorAnimLastTime { set; get; }
		// 光标动画的目标状态
		private CursorAnimType CursorAnimTypeDest { set; get; }
		// 光标动画的源状态
		private CursorAnimType CursorAnimTypeSource { set; get; }
		// 光标动画的帧计数
		private int CursorAnimFrameIndex { get; set; }
		#endregion
		
		#region public member methods
		/// <summary>
		/// 设置左手图标位置
		/// </summary>
		/// <param name="pos">新位置</param>
		public void SetHandLeftCursorPos(Vector3 pos)
		{
			// 是否初始化
			if( !m_bInit )
				return;
			
			// 当做推按动作时，停止更新位置
			if( this.IsPushActive && m_fPushing >= 0 )
				return;
			
			m_SpriteHandLeft.transform.localPosition = pos;
		}
		
		/// <summary>
		/// 设置右手图标位置
		/// </summary>
		/// <param name="pos">新位置</param>
		public void SetHandRightCursorPos(Vector3 pos)
		{
			// 是否初始化
			if( !m_bInit )
				return;
			
			// 当做推按动作时，停止更新位置
			if( this.IsPushActive && m_fPushing >= 0 )
				return;
			
			m_SpriteHandRight.transform.localPosition = pos;
		}
		
		/// <summary>
		/// 设置左手图标位置
		/// </summary>
		/// <param name="pos">新位置</param>
		public void SetHandLeftRawPos(Vector3 pos)
		{
			// 是否初始化
			if( !m_bInit )
				return;
			
			m_vHandLeftRawPos = pos;
		}
		
		/// <summary>
		/// 获取平滑后的左手原始位置
		/// </summary>
		/// <returns>The hand left raw position.</returns>
		public Vector3 GetHandLeftRawPos()
		{
			//
			if( !m_bInit )
			{
				return Utils.InvaildVec3;
			}
			
			return m_vHandLeftRawPos;
		}
		
		/// <summary>
		/// 设置右手图标位置
		/// </summary>
		/// <param name="pos">新位置</param>
		public void SetHandRightRawPos(Vector3 pos)
		{
			// 是否初始化
			if( !m_bInit )
				return;
			
			m_vHandRightRawPos = pos;
		}
		
		/// <summary>
		/// 获取平滑后的右手原始位置
		/// </summary>
		/// <returns>The hand left raw position.</returns>
		public Vector3 GetHandRightRawPos()
		{
			//
			if( !m_bInit )
			{
				return Utils.InvaildVec3;
			}
			
			return m_vHandRightRawPos;
		}
		
		/// <summary>
		/// 隐藏左手光标
		/// </summary>
		public void HideHandLeft()
		{
			SetHandLeftVisible(false);
		}
		
		/// <summary>
		/// 隐藏右手光标
		/// </summary>
		public void HideHandRight()
		{
			SetHandRightVisible(false);
		}
		
		/// <summary>
		/// 隐藏左右手光标
		/// </summary>
		public void HideCursors()
		{
			SetHandLeftVisible(false);
			SetHandRightVisible(false);
		}
		
		/// <summary>
		/// 启用光标功能
		/// </summary>
		public void EnableCursor()
		{
			this.IsEnableCursor = true;
			
			// reset
			ResetCursorHandLeftState();
			ResetCursorHandRightState();
		}
		
		/// <summary>
		/// 复位左手光标状态
		/// </summary>
		public void ResetCursorHandLeftState()
		{
			SpriteHandLeft.spriteName = this.CursorDefaultName;
			
			this.HasCursorAnimPlaying = false;
			
			this.CursorAnimTypeSource = CursorAnimType.None;
			this.CursorAnimTypeDest = CursorAnimType.None;
		}
		
		/// <summary>
		/// 复位右手光标状态
		/// </summary>
		public void ResetCursorHandRightState()
		{
			SpriteHandRight.spriteName = this.CursorDefaultName;
			
			this.HasCursorAnimPlaying = false;
			
			this.CursorAnimTypeSource = CursorAnimType.None;
			this.CursorAnimTypeDest = CursorAnimType.None;
		}
		
		/// <summary>
		/// 禁用光标功能
		/// </summary>
		public void DisableCursor()
		{
			this.IsEnableCursor = false;
			
			//
			HideCursors();
		}
		
		/// <summary>
		/// 显示左手光标
		/// </summary>
		public void ShowHandLeft()
		{
			SetHandLeftVisible(true);
		}
		
		/// <summary>
		/// 显示右手光标
		/// </summary>
		public void ShowHandRight()
		{
			SetHandRightVisible(true);
		}
		
		/// <summary>
		/// 设置左手光标的显示状态
		/// </summary>
		/// <param name="b">显示状态值</param>
		public void SetHandLeftVisible(bool b)
		{
			// 是否初始化
			if( !m_bInit )
				return;
			
			if( b && this.IsEnableCursor )
			{
				m_SpriteHandLeft.gameObject.SetActive(b);
				
				ResetCursorHandLeftState();
			}
			else if( !b )
			{
				m_SpriteHandLeft.gameObject.SetActive(b);
			}
			
			// 当隐藏光标时，使旧位置无效化
			if( !b )
			{
				m_vHandLeftCursorLastPos = Utils.InvaildVec3;
				
				m_vHandLeftRawLastPos = Utils.InvaildVec3;
				
				m_vHandLeftPushStartPos = Utils.InvaildVec3;
				
				m_fPushing = -1f;
				//
				this.HasCursorAnimPlaying = false;
				//
				this.CursorAnimTypeDest = CursorAnimType.None;
				this.CursorAnimTypeSource = CursorAnimType.None;
			}
		}
		
		/// <summary>
		/// 设置右手光标的显示状态
		/// </summary>
		/// <param name="b">显示状态值</param>
		public void SetHandRightVisible(bool b)
		{
			// 是否初始化
			if( !m_bInit )
				return;
			
			if( b && this.IsEnableCursor )
			{
				m_SpriteHandRight.gameObject.SetActive(b);
				
				ResetCursorHandRightState();
			}
			else if( !b )
			{
				m_SpriteHandRight.gameObject.SetActive(b);
			}
			
			// 当隐藏光标时，使旧位置无效化
			if( !b )
			{
				m_vHandRightCursorLastPos = Utils.InvaildVec3;
				
				m_vHandRightRawLastPos = Utils.InvaildVec3;
				
				m_vHandRightPushStartPos = Utils.InvaildVec3;
				
				m_fPushing = -1f;
				//
				this.HasCursorAnimPlaying = false;
				//
				this.CursorAnimTypeDest = CursorAnimType.None;
				this.CursorAnimTypeSource = CursorAnimType.None;
			}
		}
		
		/// <summary>
		/// 得到左手光标的显示状态
		/// </summary>
		/// <returns><c>true</c>, if hand left visible was gotten, <c>false</c> otherwise.</returns>
		public bool GetHandLeftVisible()
		{
			// 是否初始化
			if( !m_bInit )
				return false;
			
			return m_SpriteHandLeft.gameObject.activeSelf;
		}
		
		/// <summary>
		/// 得到右手光标的显示状态
		/// </summary>
		/// <returns><c>true</c>, if hand right visible was gotten, <c>false</c> otherwise.</returns>
		public bool GetHandRightVisible()
		{
			// 是否初始化
			if( !m_bInit )
				return false;
			
			return m_SpriteHandRight.gameObject.activeSelf;
		}
		
		/// <summary>
		/// 获得是否有手部光标处于显示状态
		/// </summary>
		/// <returns><c>true</c> if this instance has hand visible; otherwise, <c>false</c>.</returns>
		public bool GetHandVisible()
		{
			// 是否初始化
			if( !m_bInit )
				return false;
			
			return m_SpriteHandLeft.gameObject.activeSelf || m_SpriteHandRight.gameObject.activeSelf;
		}
		
		/// <summary>
		/// Gets the current sprite cursor.
		/// </summary>
		/// <returns>The current sprite cursor.</returns>
		public UISprite GetCurrentSpriteCursor()
		{
			// 是否初始化
			if( !m_bInit )
				return null;
			
			if( m_SpriteHandLeft.gameObject.activeSelf )
				return m_SpriteHandLeft;
			
			if( m_SpriteHandRight.gameObject.activeSelf )
				return m_SpriteHandRight;
			
			return null;
		}
		
		/// <summary>
		/// 获得当前光标的位置（不关心左手与右手）
		/// </summary>
		/// <returns>当前光标的位置</returns>
		public Vector3 GetCurrentCursorPos()
		{
			// 是否初始化
			if( !m_bInit )
				return Vector3.zero;
			
			if( m_SpriteHandLeft.gameObject.activeSelf )
				return m_SpriteHandLeft.transform.localPosition;
			
			if( m_SpriteHandRight.gameObject.activeSelf )
				return m_SpriteHandRight.transform.localPosition;
			
			return Vector3.zero;
		}
		
		/// <summary>
		/// 获得当前光标的上一帧位置（不关心左手与右手）
		/// </summary>
		/// <returns>当前光标的上一帧位置</returns>
		public Vector3 GetLastCursorPos()
		{
			// 是否初始化
			if( !m_bInit )
				return Vector3.zero;
			
			if( m_SpriteHandLeft.gameObject.activeSelf )
				return m_vHandLeftCursorLastPos;
			
			if( m_SpriteHandRight.gameObject.activeSelf )
				return m_vHandRightCursorLastPos;
			
			return Vector3.zero;
		}
		
		/// <summary>
		/// 获得当前光标平滑后的原始位置（不关心左手与右手）
		/// </summary>
		/// <returns>当前光标平滑后的原始位置</returns>
		public Vector3 GetCurrentCursorRawPos()
		{
			// 是否初始化
			if( !m_bInit )
				return Utils.InvaildVec3;
			
			if( m_SpriteHandLeft.gameObject.activeSelf )
				return m_vHandLeftRawPos;
			
			if( m_SpriteHandRight.gameObject.activeSelf )
				return m_vHandRightRawPos;
			
			return Utils.InvaildVec3;
		}
		
		/// <summary>
		/// 测试手是否处于放下状态
		/// </summary>
		/// <returns><c>true</c>, if put down was handed, <c>false</c> otherwise.</returns>
		/// <param name="vElbow">手肘位置</param>
		/// <param name="vHand">手位置</param>
		public static bool HandPutDown(Vector3 vElbow, Vector3 vHand)
		{
			// 
			Vector3 vec = vHand - vElbow;
			vec.Normalize();
			
			float dot = Vector3.Dot(vec, Vector3.down);
			
			// 测试夹角值
			if( dot > 0.8f ) // 0.8约37度
				return true;
			else
				return false;
		}
		
		/// <summary>
		/// 设置左手追踪状态
		/// </summary>
		/// <param name="state">新的状态</param>
		public void SetHandLeftState(Windows.Kinect.HandState state, Windows.Kinect.TrackingConfidence confidence)
		{
			//
			SetHandState(state, confidence, m_SpriteHandLeft, ref m_HandLeftState, ref m_HandLeftStates);
			
			// Debug
			if( !m_Debug )
				return;
			
			//
			switch( m_HandLeftState )
			{
			case Windows.Kinect.HandState.Closed:
				m_SpriteHandLeft.color = m_ColorCursorClosed;
				break;
			case Windows.Kinect.HandState.Lasso:
				m_SpriteHandLeft.color = m_ColorCursorLasso;
				break;
			case Windows.Kinect.HandState.NotTracked:
				m_SpriteHandLeft.color = m_ColorCursorNotTracked;
				break;
			case Windows.Kinect.HandState.Open:
				m_SpriteHandLeft.color = m_ColorCursorOpen;
				break;
			case Windows.Kinect.HandState.Unknown:
				m_SpriteHandLeft.color = m_ColorCursorUnknown;
				break;
			}
		}
		
		/// <summary>
		/// 设置右手追踪状态
		/// </summary>
		/// <param name="state">新的状态</param>
		public void SetHandRightState(Windows.Kinect.HandState state, Windows.Kinect.TrackingConfidence confidence)
		{
			//
			SetHandState(state, confidence, m_SpriteHandRight, ref m_HandRightState, ref m_HandRightStates);
			
			// Debug
			if( !m_Debug )
				return;
			
			//
			switch( m_HandRightState )
			{
			case Windows.Kinect.HandState.Closed:
				m_SpriteHandRight.color = m_ColorCursorClosed;
				break;
			case Windows.Kinect.HandState.Lasso:
				m_SpriteHandRight.color = m_ColorCursorLasso;
				break;
			case Windows.Kinect.HandState.NotTracked:
				m_SpriteHandRight.color = m_ColorCursorNotTracked;
				break;
			case Windows.Kinect.HandState.Open:
				m_SpriteHandRight.color = m_ColorCursorOpen;
				break;
			case Windows.Kinect.HandState.Unknown:
				m_SpriteHandRight.color = m_ColorCursorUnknown;
				break;
			}
		}
		#endregion
		
		#region private member functions for Unity
		//
		private void Awake()
		{
			s_Instance = this;
		}
		
		// Use this for initialization
		private void Start()
		{
			// check
			if( m_Atlas == null )
			{
				Debug.LogError( "The Atlas has not assigned.", this );
				return;
			}
			
			// check
			if( m_SpriteHandLeft == null )
			{
				if( KinectServices.SpriteHandLeft != null )
				{
					m_SpriteHandLeft = KinectServices.SpriteHandLeft;
				}
				else
				{
					Debug.LogError( "The SpriteHandLeft has not assigned.", this );
					return;
				}
			}
			
			// check
			if( m_SpriteHandRight == null )
			{
				if( KinectServices.SpriteHandRight != null )
				{
					m_SpriteHandRight = KinectServices.SpriteHandRight;
				}
				else
				{
					Debug.LogError( "The SpriteHandRight has not assigned.", this );
					return;
				}
			}
			
			// check
			if( m_CursorLoadingSprite == null )
			{
				if( KinectServices.SpriteCursorLoading != null )
				{
					m_CursorLoadingSprite = KinectServices.SpriteCursorLoading;
				}
				else
				{
					Debug.LogError( "The SpriteLoading has not assigned.", this );
					return;
				}
			}
			
			// check
			if( string.IsNullOrEmpty( m_CursorLoadingStateNamePrefix ) )
			{
				Debug.LogError( "The 'CursorLoadingStateNamePrefix' has not assigln!", this );
				return;
			}
			
			
			// 翻转右手图片
			m_SpriteHandLeft.flip = UIBasicSprite.Flip.Horizontally;
			
			// 设置光标名称
			this.CursorDragStartName = m_CursorDragStateNamePrefix + m_CursorDragStateIndexStart.ToString("D2");
			this.CursorDragEndName = m_CursorDragStateNamePrefix + m_CursorDragStateIndexEnd.ToString("D2");
			
			this.CursorDefaultName = this.CursorDragStartName;
			
			// 设置默认显示内容
			m_SpriteHandLeft.atlas = m_Atlas;
			m_SpriteHandRight.atlas = m_Atlas;
			
			m_SpriteHandLeft.spriteName = this.CursorDefaultName;
			m_SpriteHandRight.spriteName = this.CursorDefaultName;
			
			m_SpriteHandLeft.width = m_CursorWidth;
			m_SpriteHandRight.width = m_CursorWidth;
			
			m_SpriteHandLeft.height = m_CursorHeight;
			m_SpriteHandRight.height = m_CursorHeight;
			
			// 默认不显示图标
			m_SpriteHandLeft.gameObject.SetActive(false);
			m_SpriteHandRight.gameObject.SetActive(false);
			
			// 设置光标动画单帧时间
			this.CursorAnimFrameTime = 1.0f / m_CursorAnimFramePerSecond;
			
			// 光标动画的默认类型
			this.CursorAnimTypeDest = CursorAnimType.None;
			this.CursorAnimTypeSource = CursorAnimType.None;
			
			//
			this.IsEnableCursor = true;
			
			//
			m_bInit = true;
		}
		
		// Update is called once per frame
		private void Update()
		{
			// 是否初始化
			if( !m_bInit )
				return;
			
			if( IsLoadingActive )
				CheckLoadingAnim();
			// 没有光标动画时，才改变成其它光标状态
			//      if( !this.HasCursorAnimPlaying )
			//      {
			//          //
			//          if( this.HandLeftState == Windows.Kinect.HandState.Closed )
			//          {
			//              m_SpriteHandLeft.spriteName = m_CursorDragStateNamePrefix + m_CursorDragStateIndexEnd.ToString("D2");
			//          }
			//          else
			//          {
			//              m_SpriteHandLeft.spriteName = m_CursorDragStateNamePrefix + m_CursorDragStateIndexStart.ToString("D2");
			//          }
			//          
			//          //
			//          if( this.HandRightState == Windows.Kinect.HandState.Closed )
			//          {
			//              m_SpriteHandRight.spriteName = m_CursorDragStateNamePrefix + m_CursorDragStateIndexEnd.ToString("D2");
			//          }
			//          else
			//          {
			//              m_SpriteHandRight.spriteName = m_CursorDragStateNamePrefix + m_CursorDragStateIndexStart.ToString("D2");
			//          }
			//      }
		}
		
		//
		private void LateUpdate()
		{
			// 是否初始化
			if( !m_bInit )
				return;
			
			// 检查是否启用推按动作
			if( this.IsPushActive )
			{
				// 测试左手推按动作，在左手张开状态下才能做推按动作；或是已经开始推按动作，只是在推按动作过程中手部有弯曲，导致变成其它状态，但仍然追踪它
				if( this.HandLeftState == Windows.Kinect.HandState.Open || m_fPushing > 0f )
					CheckHandPush(m_vHandLeftRawPos, m_vHandLeftRawLastPos, ref m_vHandLeftPushStartPos, this.OnHandLeftPushing, this.OnHandLeftPushedStart);
				
				// 测试右手推按动作，在右手张开状态下才能做推按动作；或是已经开始推按动作，只是在推按动作过程中手部有弯曲，导致变成其它状态，但仍然追踪它
				if( this.HandRightState == Windows.Kinect.HandState.Open || m_fPushing > 0f )
					CheckHandPush(m_vHandRightRawPos, m_vHandRightRawLastPos, ref m_vHandRightPushStartPos, this.OnHandRightPushing, this.OnHandRightPushedStart);
			} 
			
			// 更新位置 -- 左手
			if( this.GetHandLeftVisible() )
			{
				// update hand cursor pos
				m_vHandLeftCursorLastPos = m_SpriteHandLeft.transform.localPosition;
				// update hand raw pos
				m_vHandLeftRawLastPos = m_vHandLeftRawPos;
				
				// 处理光标动画
				ProcessCursorAnims(this.HandLeftState, ref m_SpriteHandLeft, this.OnHandLeftPushedFinished, this.OnHandLeftDragStart, this.OnHandLeftDragFinished);
			}
			
			// 更新位置 -- 右手
			if( this.GetHandRightVisible() )
			{
				// update hand cursor pos
				m_vHandRightCursorLastPos = m_SpriteHandRight.transform.localPosition;
				// update hand raw pos
				m_vHandRightRawLastPos = m_vHandRightRawPos;
				
				// 处理光标动画
				ProcessCursorAnims(this.HandRightState, ref m_SpriteHandRight, this.OnHandRightPushedFinished, this.OnHandRightDragStart, this.OnHandRightDragFinished);
			}
		}
		#endregion
		
		#region private member functions
		
		public void CheckLoadingAnim()
		{
			if( IsOverUI() && this.GetHandVisible() )
			{
				
				// UserManager userManager = UserManager.Instance;
				
				Vector3 currentCursorPos = UICamera.lastTouchPosition;
				
				//            if(userManager!= null && userManager.HasLockUser)
				//            {
				//                currentCursorPos = CKinect.CursorController.Instance.GetCurrentCursorPos();
				//            }
				//            else
				//            {
				//                currentCursorPos = Input.mousePosition;
				//            }
				
				//Debug.Log("cursor : "  + currentCursorPos);
				if( m_OverGameObject == null )
				{
					m_OverGameObject = UICamera.hoveredObject;
					m_OnOverUITime = Time.time;
					
					m_CursorPosWhenOver = currentCursorPos;
					
					//Debug.Log("hove gameobject name: " +m_OverGameObject.name);
					
				}
				
				if( ( m_CursorPosWhenOver - currentCursorPos ).magnitude > m_LoadingThreshold )
				{
					this.StartCoroutine( DelayResetLoadingAnim() );
					return;
				}
				LoadingButton lb = m_OverGameObject.GetComponent<LoadingButton>();
				
				float threshold = (lb == null?m_ShowLoadingAnimThreshold:lb.Priority);
				
				if( Time.time - m_OnOverUITime > threshold && UICamera.hoveredObject == m_OverGameObject )
				{
					m_IsPlayingLoadingAmin = true;
					
					if( m_LoadingStartPos == Vector3.zero )
					{
						m_LoadingStartPos = currentCursorPos;
					}
					
					ProcessCursorLoadingAnims( currentCursorPos );
				}
			}
			else
			{
				ResetLoadingAnim();
			}
		}
		
		/// <summary>
		/// 处理光标的加载动画
		/// </summary>
		/// <param name="cursorPos">加载动画的显示坐标</param>
		private void ProcessCursorLoadingAnims(Vector2 cursorPos)
		{
			if( Time.time - CursorLoadingAnimLastTime > CursorLoadingAnimFrameTime && m_IsPlayingLoadingAmin )
			{
				
				if( m_iCurrentLoadingFrameIndex <= 0 && OnLoadingStart != null )
				{
					OnLoadingStart();
				}
				
				m_CursorLoadingSprite.gameObject.SetActive(true);
				m_CursorLoadingSprite.gameObject.transform.localPosition = cursorPos;
				m_CursorLoadingSprite.spriteName = GetSpriteNameByFrameIndex(m_CursorLoadingStateNamePrefix, m_CursorLoadingStateIndexStart, m_CursorLoadingStateIndexEnd, "D2", m_CursorLoadingStateIndexStart + m_iCurrentLoadingFrameIndex);
				
				this.m_iCurrentLoadingFrameIndex ++;
				//Debug.Log("-------------------------------->" + m_iCurrentFrameIndex + "   " + m_CursorLoadingSprite.spriteName);
				CursorLoadingAnimLastTime = Time.time;
				
				if( OnLoading != null )
				{
					OnLoading( this.m_iCurrentLoadingFrameIndex / ( m_CursorLoadingStateIndexEnd - m_CursorLoadingStateIndexStart ) * 100 );
				}
				
				
				if( m_iCurrentLoadingFrameIndex >= m_CursorLoadingStateIndexEnd - m_CursorLoadingStateIndexStart )
				{
					
					//UICamera.Notify();
					
					if( OnLoadingFinish != null )
					{
						OnLoadingFinish();
					}
					
					this.StartCoroutine( DelayResetLoadingAnim() );
				}
			}
		}
		
		private IEnumerator DelayResetLoadingAnim()
		{
			// skip a frame
			yield return null;
			ResetLoadingAnim();
			
		}
		
		/// <summary>
		/// 重新设置动画
		/// </summary>
		private void ResetLoadingAnim()
		{
			this.m_iCurrentLoadingFrameIndex = 0;
			if( m_CursorLoadingSprite == null )
				return;
			m_CursorLoadingSprite.gameObject.SetActive(false);
			m_IsPlayingLoadingAmin = false;
			
			m_OverGameObject = null;
			m_OnOverUITime = 0;
			
			m_LoadingStartPos = m_CursorPosWhenOver = Vector3.zero;
		}
		
		/// <summary>
		/// 检测手部的推按状态
		/// </summary>
		/// <param name="vHandRawPos">手部原始数据位置</param>
		/// <param name="vHandRawLastPos">手部原始数据的上一帧位置</param>
		/// <param name="vHandPushStartPos">手部推按的开始位置</param>
		/// <param name="onPushing">当在手部推按时的回调函数</param>
		/// <param name="onPushFinished">当手部完成推按动作时的回调函数</param>
		private void CheckHandPush(Vector3 vHandRawPos, Vector3 vHandRawLastPos, ref Vector3 vHandPushStartPos,
		                           OnPushingDelegate onPushing, OnPushedStartDelegate onPushedStart)
		{
			// 当做光标动画时停止做推按动作处理
			if( this.HasCursorAnimPlaying )
				return;
			
			//
			if( !Utils.IsInvaildVec3( vHandRawLastPos ) )
			{
				// 计算指向新位置的矢量
				Vector3 vDir = vHandRawPos - vHandRawLastPos;
				float fLen = vDir.magnitude;
				// Normalize
				vDir.x /= fLen;
				vDir.y /= fLen;
				vDir.z /= fLen;
				float fDot = Vector3.Dot( vDir, Vector3.back );
				
				// 推进方向，向前
				if( fDot > 0.9f ) // 0.9约26度
				{
					//
					if( Utils.IsInvaildVec3( vHandPushStartPos ) )
					{
						vHandPushStartPos = vHandRawPos;
					}
					
					//
					m_fPushing = ( vHandRawPos - vHandPushStartPos ).magnitude / this.PushThreshold;
					// 当推进到一定距离表示完成了推进动作
					if( m_fPushing < 1f )
					{
						//
						if( onPushing != null )
							onPushing( m_fPushing );
						
						//#if UNITY_EDITOR
						//if( m_DebugLog )
						//  Debug.Log( "Pushing:"+m_fPushing );
						//#endif
					}
					else
					{
						//
						if( onPushedStart != null )
							onPushedStart();
						
						// set to invalid
						vHandPushStartPos = Utils.InvaildVec3;
						
						#if UNITY_EDITOR
						if( m_DebugLog )
							Debug.Log( "Pushed Start!!! m_fPushing:"+m_fPushing );
						#endif
						
						//
						// Do Pushed Animations
						
						// 设置正在有动画播放
						this.HasCursorAnimPlaying = true;
						// 更新动画时间
						this.CursorAnimLastTime = Time.time;
						// 设置动画类型
						this.CursorAnimTypeDest = CursorAnimType.Pushed;
						this.CursorAnimTypeSource = CursorAnimType.Pushing;
						// 设置动画帧计数
						this.CursorAnimFrameIndex = m_CursorPushedStateIndexStart;
					}
				}
				// 推进方向，向后
				else if( fDot < -0.9f ) // 0.9约26度
				{
					//
					m_fPushing -= Mathf.Abs( m_fPushing - ( vHandRawPos - vHandPushStartPos ).magnitude / this.PushThreshold );
					
					// 后退表示不做推按动作
					if( m_fPushing < 0f )
					{
						m_fPushing = -1f;
						
						vHandPushStartPos = Utils.InvaildVec3;
					}
				}
				// 其它方向，这时取消推按状态
				else if( fDot > -0.2f && fDot < 0.2f && fLen > 0.01f ) // 0.2约79度 ； (fLen)用于过滤抖动(Jitter)
				{               //
					m_fPushing = -1f;
					//
					vHandPushStartPos = Utils.InvaildVec3;
					//
					this.HasCursorAnimPlaying = false;
					//
					this.CursorAnimTypeDest = CursorAnimType.None;
					this.CursorAnimTypeSource = CursorAnimType.None;
				}
			}
		}
		
		/// <summary>
		/// 根据标量来获得sprite的名称
		/// </summary>
		/// <returns>新的名称</returns>
		/// <param name="namePrefix">名称前缀</param>
		/// <param name="nIndexStart">开始索引值</param>
		/// <param name="nIndexEnd">结束索引值</param>
		/// <param name="digitalFormat">数字字符格式</param>
		/// <param name="fScalar">标量</param>
		private string GetSpriteNameByScalar(string namePrefix, int nIndexStart, int nIndexEnd, string digitalFormat, float fScalar)
		{
			//
			if( string.IsNullOrEmpty( digitalFormat ) )
				return string.Empty;
			
			return namePrefix + (nIndexStart + (int)(((float)Mathf.Abs(nIndexEnd - nIndexStart)) * fScalar)).ToString(digitalFormat);
		}
		
		/// <summary>
		/// 根据帧索引来获得sprite的名称
		/// </summary>
		/// <returns>新的名称</returns>
		/// <param name="namePrefix">名称前缀</param>
		/// <param name="nIndexStart">开始索引值</param>
		/// <param name="nIndexEnd">结束索引值</param>
		/// <param name="digitalFormat">数字字符格式</param>
		/// <param name="nFrameCount">帧计数</param>
		private string GetSpriteNameByFrameIndex(string namePrefix, int nIndexStart, int nIndexEnd, string digitalFormat, int nFrameIndex)
		{
			//
			if( string.IsNullOrEmpty( digitalFormat ) )
				return string.Empty;
			
			return namePrefix + Mathf.Clamp(nFrameIndex, nIndexStart, nIndexEnd).ToString(digitalFormat);
		}
		
		/// <summary>
		/// 设置手部追踪状态
		/// </summary>
		/// <param name="state">新的手部状态</param>
		/// <param name="confidence">新的手部状态可信度</param>
		/// <param name="spriteHand">手部UISprite对象</param>
		/// <param name="handState">当前手部状态对象</param>
		/// <param name="handStates">当前手部状态列表对象</param>
		private void SetHandState(Windows.Kinect.HandState state, Windows.Kinect.TrackingConfidence confidence, UISprite spriteHand, ref Windows.Kinect.HandState handState, ref List<HandStateData> handStates)
		{
			// 如果光标除了隐藏状态，那么不记录状态
			if( !spriteHand.gameObject.activeSelf )
			{
				handState = Windows.Kinect.HandState.NotTracked;
				
				// 清除队列
				if( handStates.Count > 0 )
					handStates.Clear();
				
				return;
			}
			
			// 加入队列
			handStates.Add(new HandStateData(state, confidence));
			
			// 保持队列固定长度
			if( handStates.Count > LIMIT_LEN )
				handStates.RemoveAt( 0 );
			
			// 检查状态的可信度
			if( confidence == Windows.Kinect.TrackingConfidence.High )
			{
				handState = state;
				return;
			}
			//
			// Windows.Kinect.TrackingConfidence.Low
			//
			
			Windows.Kinect.HandState lastState = state;
			// 用于状态统计
			int nStateCount = 0, nStateUnknownCount = 0;
			
			// 检查过往数据，以推断手部状态
			for( int i = handStates.Count - 1, count = 0; count < handStates.Count && count < this.ConsecutiveThreshold; count++, i-- )
			{
				//
				HandStateData data = handStates[i];
				
				//
				if( data.confidence == Windows.Kinect.TrackingConfidence.High )
				{
					// 遇到高可信度状态，直接返回
					handState = data.state;
					return;
				}
				else if( data.state == Windows.Kinect.HandState.Unknown ) // Windows.Kinect.TrackingConfidence.Low
				{
					nStateUnknownCount++;
				}
				else if( data.state == lastState ) // Windows.Kinect.TrackingConfidence.Low
				{
					nStateCount++;
				}
			}
			
			// 当未知状态过多时，推断为未追踪状态
			if( ( handStates.Count > 0 && nStateUnknownCount == handStates.Count ) ||
			   nStateUnknownCount == this.ConsecutiveThreshold )
			{
				handState = Windows.Kinect.HandState.NotTracked;
			}
			// 当某个状态过多时，推断为该状态
			else if( ( handStates.Count > 0 && nStateCount == handStates.Count ) ||
			        nStateCount == this.ConsecutiveThreshold )
			{
				handState = state;
			}
			else
			{
				// 最后什么也不改变
			}
		}
		
		/// <summary>
		/// 处理光标动画
		/// </summary>
		/// <param name="handState">手部状态</param>
		/// <param name="spriteHand">手部光标UISprite对象</param>
		/// <param name="onPushedFinished">推按动画完成时调用</param>
		/// <param name="onDragStart">拖动动画完成时拖动功能开始调用</param>
		/// <param name="onDragFinished">拖动动画反向播放时拖动功能完成时调用</param>
		private void ProcessCursorAnims(Windows.Kinect.HandState handState, ref UISprite spriteHand, OnPushedFinishedDelegate onPushedFinished,
		                                OnDragStartDelegate onDragStart, OnDragFinishedDelegate onDragFinished)
		{
			//string str = string.Empty;
			
			// 检查是否启用推按动作，或是否为正在进行推按完成动画
			if( this.IsPushActive && ( handState != Windows.Kinect.HandState.Closed ||
			                          ( this.CursorAnimTypeSource == CursorAnimType.Pushing && this.CursorAnimTypeDest == CursorAnimType.Pushed ) ) )
			{
				//str += "Time:" + Time.time + "  push. sprite name:"+spriteHand.spriteName+"\n";
				
				if( !this.HasCursorAnimPlaying )
				{
					//str += "push no anim\n";
					
					spriteHand.spriteName = GetSpriteNameByScalar( m_CursorPushingStateNamePrefix, m_CursorPushingStateIndexStart,
					                                              m_CursorPushingStateIndexEnd, "D2", m_fPushing >= 0f ? m_fPushing : 0f );
				}
				else if( this.CursorAnimTypeDest == CursorAnimType.Pushed ) // 是否为正在进行推按完成动画
				{
					//
					if( Time.time - this.CursorAnimLastTime > this.CursorAnimFrameTime )
					{
						//str += "push update anim\n";
						
						// 更新时间
						this.CursorAnimLastTime = Time.time;
						
						// 更新动画帧
						spriteHand.spriteName = GetSpriteNameByFrameIndex(m_CursorPushedStateNamePrefix, m_CursorPushedStateIndexStart,
						                                                  m_CursorPushedStateIndexEnd, "D2", this.CursorAnimFrameIndex);
						
						// update frame count
						this.CursorAnimFrameIndex++;
						
						// 检查动画是否完成
						if( this.CursorAnimFrameIndex > m_CursorPushedStateIndexEnd )
						{
							//str += "push anim finised\n";
							
							// delegate call
							if( onPushedFinished != null )
							{
								onPushedFinished();
							}
							
							#if UNITY_EDITOR
							if( m_DebugLog )
								Debug.Log( "PushedFinished" );
							#endif
							
							//
							StartCoroutine(DelayResetPushAnim());
						}
					}
				}
			}
			
			// 握拳
			if( handState == Windows.Kinect.HandState.Closed && m_fPushing < 0f )
			{
				//str += "Time:" + Time.time + "  hand closed. sprite name:"+spriteHand.spriteName+"\n";
				
				// 握拳动画
				if( !this.HasCursorAnimPlaying && spriteHand.spriteName != this.CursorDragEndName )
				{
					//str += "hand closed no anim\n";
					
					// 当没有光标动画播放时，并且光标动画状态不为光标拖动动画最后一帧时
					// 那么重新从头播放光标拖动动画
					//
					
					//
					spriteHand.spriteName = this.CursorDragStartName;
					//
					this.HasCursorAnimPlaying = true;
					//
					this.CursorAnimTypeSource = CursorAnimType.None;
					this.CursorAnimTypeDest = CursorAnimType.Drag;
					// 更新时间
					this.CursorAnimLastTime = Time.time;
					// 使用索引值
					this.CursorAnimFrameIndex = m_CursorDragStateIndexStart;
				}
				else if( this.HasCursorAnimPlaying && this.CursorAnimTypeDest == CursorAnimType.Drag ) // 检查是否为拖动动画
				{
					//
					if( Time.time - this.CursorAnimLastTime > this.CursorAnimFrameTime )
					{
						//str += "hand closed update anim\n";
						
						// 更新时间
						this.CursorAnimLastTime = Time.time;
						
						// 更新动画帧
						spriteHand.spriteName = GetSpriteNameByFrameIndex(m_CursorDragStateNamePrefix, m_CursorDragStateIndexStart,
						                                                  m_CursorDragStateIndexEnd, "D2", this.CursorAnimFrameIndex);
						
						// update frame count
						this.CursorAnimFrameIndex++;
						
						// 检查动画是否完成
						if( this.CursorAnimFrameIndex > m_CursorDragStateIndexEnd )
						{
							//str += "hand closed anim finished\n";
							// reset
							this.HasCursorAnimPlaying = false;
							// reset
							//this.CursorAnimTypeDest = CursorAnimType.None;
							
							//
							this.CursorAnimTypeSource = CursorAnimType.Drag;
							
							// delegate call
							if( onDragStart != null )
								onDragStart( m_SpriteHandRight.transform.localPosition );
							
							#if UNITY_EDITOR
							if( m_DebugLog )
								Debug.Log( "onDragStart:"+spriteHand.spriteName );
							#endif
						}
					}
				}
				else if( this.HasCursorAnimPlaying && this.CursorAnimTypeSource == CursorAnimType.Drag &&
				        this.CursorAnimTypeDest == CursorAnimType.None ) // 当快速的张手和握拳状态间转换时
				{
					//str += "hand closed -- anim goto closed\n";
					
					// 播放握拳动画
					this.CursorAnimTypeSource = CursorAnimType.None;
					this.CursorAnimTypeDest = CursorAnimType.Drag;
					
					// 更新时间
					this.CursorAnimLastTime = Time.time;
				}
				else
				{
					//str += "hand closed -- anim playing:"+this.HasCursorAnimPlaying + "  source:"+ this.CursorAnimTypeSource+"  dest:"+ this.CursorAnimTypeDest+"\n";
				}
			}
			
			// 张开手掌 -- 从拖动状态到张手状态
			if( handState == Windows.Kinect.HandState.Open && m_fPushing < 0f )
			{
				//str += "Time:" + Time.time + "  hand open. sprite name:"+spriteHand.spriteName+"\n";
				
				// 张开手掌动画
				if( !this.HasCursorAnimPlaying && spriteHand.spriteName != this.CursorDragStartName && this.CursorAnimTypeSource == CursorAnimType.Drag )
				{
					//str += "hand open no anim\n";
					
					// 当没有光标动画播放时，并且光标动画状态不为光标拖动动画最后一帧时
					// 那么重新从拖动状态到张手状态，播放光标拖动动画
					//
					
					//
					spriteHand.spriteName = this.CursorDragEndName;
					//
					this.HasCursorAnimPlaying = true;
					//
					this.CursorAnimTypeDest = CursorAnimType.None;
					// 更新时间
					this.CursorAnimLastTime = Time.time;
					// 使用索引值
					this.CursorAnimFrameIndex = m_CursorDragStateIndexEnd;
				}
				else if( !this.HasCursorAnimPlaying && spriteHand.spriteName != this.CursorDefaultName && this.CursorAnimTypeSource != CursorAnimType.None )
				{
					//str += "hand open no anim 2\n";
					
					// 光标没有光标动画播放时，并且光标不处于默认状态，那么切换它为默认状态，不作动画
					spriteHand.spriteName = this.CursorDefaultName;
					
					this.CursorAnimTypeSource = CursorAnimType.None;
					this.CursorAnimTypeDest = CursorAnimType.None;
					
					// delegate call
					//if( onDragFinished != null )
					//  onDragFinished( m_SpriteHandRight.transform.localPosition );
					
					//Debug.Log( "onDragFinished:"+m_SpriteHandRight.spriteName );
				}
				else if( this.CursorAnimTypeSource == CursorAnimType.Drag && this.CursorAnimTypeDest == CursorAnimType.None ) // 检查是否为拖动动画
				{
					//
					if( Time.time - this.CursorAnimLastTime > this.CursorAnimFrameTime )
					{
						//str += "hand open update anim\n";
						
						// 更新时间
						this.CursorAnimLastTime = Time.time;
						
						// 更新动画帧
						spriteHand.spriteName = GetSpriteNameByFrameIndex(m_CursorDragStateNamePrefix, m_CursorDragStateIndexStart,
						                                                  m_CursorDragStateIndexEnd, "D2", this.CursorAnimFrameIndex);
						
						// update frame count
						this.CursorAnimFrameIndex--;
						
						// 检查动画是否完成
						if( this.CursorAnimFrameIndex < m_CursorDragStateIndexStart )
						{
							//str += "hand open anim finished\n";
							
							//
							this.CursorAnimFrameIndex = m_CursorDragStateIndexStart;
							
							// reset
							this.HasCursorAnimPlaying = false;
							// reset
							this.CursorAnimTypeDest = CursorAnimType.None;
							this.CursorAnimTypeSource = CursorAnimType.None;
							
							// delegate call
							if( onDragFinished != null )
								onDragFinished( m_SpriteHandRight.transform.localPosition );
							
							#if UNITY_EDITOR
							if( m_DebugLog )
								Debug.Log( "onDragFinished:"+m_SpriteHandRight.spriteName );
							#endif
						}
					}
				} 
				else
				{
					//str += "hand open -- anim playing:"+this.HasCursorAnimPlaying + "  source:"+ this.CursorAnimTypeSource+"  dest:"+ this.CursorAnimTypeDest+"\n";
				}
			}
			
			//
			//if( !string.IsNullOrEmpty(str) )
			//  Debug.Log(str);
		}
		
		//
		private IEnumerator DelayResetPushAnim()
		{
			// skip a frame
			yield return null;
			
			// reset
			this.HasCursorAnimPlaying = false;
			// reset
			this.CursorAnimTypeDest = CursorAnimType.None;
			// reset
			m_fPushing = -1f;
		}
		
		private bool IsOverUI()
		{
			return (UICamera.isOverUI || (UICamera.hoveredObject != null && UICamera.hoveredObject.layer == LayerMask.NameToLayer("3D GUI")));
		}
		#endregion
	}
}
