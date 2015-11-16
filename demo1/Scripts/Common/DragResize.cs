using UnityEngine;
using System.Collections;

public class DragResize : MonoBehaviour 
{
	public enum DragEffect
	{
		None,
		Momentum,
		MomentumAndSpring,
	}
	
	/// <summary>
	/// Target object that will be dragged.
	/// </summary>	
	public Transform target;
	
	/// <summary>
	/// Panel that will be used for constraining the target.
	/// </summary>
	public UIPanel panelRegion;
	
	/// <summary>
	/// Scale value applied to the drag delta. Set X or Y to 0 to disallow dragging in that direction.
	/// </summary>
	public Vector3 dragMovement { get { return scale; } set { scale = value; } }
	
	/// <summary>
	/// Rectangle to be used as the draggable object's bounds. If none specified, all widgets' bounds get added up.
	/// </summary>
	public UIRect contentRect = null;
	
	/// <summary>
	/// Effect to apply when dragging.
	/// </summary>
	public DragEffect dragEffect = DragEffect.MomentumAndSpring;
	
	/// <summary>
	/// How much momentum gets applied when the press is released after dragging.
	/// </summary>
	public float momentumAmount = 35f;

	//
	public float m_RawFilter = 0.005f;
	//缩放速度
	public int m_ScaleSpeed = 10;
	
	// Obsolete property. Use 'dragMovement' instead.
	protected Vector3 scale = new Vector3(1f, 1f, 0f);
	//最小的缩放值
	public float m_MinScale = 1f;
	//最大的缩放值
	public float m_MaxScale = 1.5f;
	#region private member
	//光标最后的位置（kinect光标）
	private Vector3 m_vLastCursorPos = Utils.InvaildVec3;
	//手最后的位置（在camera空间中没有处理过的数据）
	private Vector3 m_vLastHandRawPos = Utils.InvaildVec3;
	//鼠标的最后的位置
	private Vector3 m_vLastMousePos = Vector3.zero;

	private Plane mPlane;
	private Vector3 mMomentum = Vector3.zero;
	private Vector3 mScroll = Vector3.zero;
	private Bounds mBounds;

	private bool m_bInited;

	#endregion

	#region public properties
	/// <summary>
	/// 获取/设置操控的Target
	/// </summary>
	/// <value>The target.</value>
	public Transform Target { get { return target; } set{ target = value; }}

	/// <summary>
	/// 获取/设置操作对象的weight
	/// </summary>
	/// <value>The content rect.</value>
	public UIRect ContentRect {get {return contentRect; } set{ contentRect = value; }}

	/// <summary>
	/// 获取/设置Panel剪裁取
	/// </summary>
	/// <value>The panel region.</value>
	public UIPanel PanelRegion { get{return panelRegion; } set { panelRegion = value; }}

	#endregion

	#region private for unity

	// Use this for initialization
	void Start ()
	{
	
//		if(dragEffect == DragEffect.MomentumAndSpring)
//		{
//			this.gameObject.AddComponent<SpringPosition>();
//		}


		if( target == null )
		{
			Debug.LogError("The 'target' has not assiged!",this);
			return;
		}

		if( contentRect == null )
		{
			Debug.LogError("The 'content Rect' has not assiged!",this);
			return;
		}

		if( panelRegion == null )
		{
			Debug.LogError("The 'panel Region' has not assiged!",this);
			return;
		}
	
		m_bInited = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//
		if( Target == null || PanelRegion == null || ContentRect == null )
			return;
		
		//鼠标左键按下
		if( Input.GetMouseButton(0) )
		{

			Vector3 vec = Input.mousePosition - m_vLastMousePos;
			
			if( vec.sqrMagnitude > 1 )
			{
				//移动
				Move(vec);
			}
		}
		
		//
		if( Input.GetAxis("Mouse ScrollWheel") < 0 )
		{

			float speed = NGUIMath.SpringLerp(m_ScaleSpeed,RealTime.deltaTime);
			//放大
			ZoomInTarget(speed);


		}
		else if( Input.GetAxis("Mouse ScrollWheel") > 0 )
		{
			//缩小
			float speed = NGUIMath.SpringLerp(m_ScaleSpeed,RealTime.deltaTime);

			ZoomOutTarget(speed);

			if ( panelRegion.ConstrainTargetToBounds(target, ref mBounds, dragEffect == DragEffect.None) )
			{
				// Adjust the momentum
				if (dragEffect != DragEffect.None)
					mMomentum = Vector3.Lerp(mMomentum, mMomentum + target.localPosition * (0.01f * momentumAmount), 0.67f);
			}
		}

		m_vLastMousePos = Input.mousePosition;


		CKinect.CursorController cursor = CKinect.CursorController.Instance;
		//
		if( !cursor.GetHandVisible() || !cursor.HasHandClosed )
		{
			m_vLastHandRawPos = Utils.InvaildVec3;
			m_vLastCursorPos = Utils.InvaildVec3;

			return;
		}
		
		if( Utils.IsInvaildVec3( m_vLastHandRawPos ) )
			m_vLastHandRawPos = cursor.GetCurrentCursorRawPos();
		
		Vector3 dir = cursor.GetCurrentCursorRawPos() - m_vLastHandRawPos;
		float fDist = dir.z;
		float fLen = dir.magnitude;
		dir.Normalize();
		
		float dirX = Mathf.Abs(dir.x);
		float dirY = Mathf.Abs(dir.y);
		float dirZ = Mathf.Abs(dir.z);
		
		//
		m_vLastHandRawPos = cursor.GetCurrentCursorRawPos();
		
		if( fLen > m_RawFilter && dirZ > dirX && dirZ > dirY )
		{
			float speed = NGUIMath.SpringLerp(m_ScaleSpeed,RealTime.deltaTime);

			//float accpet = m_TextureSize.y / m_TextureSize.x;
			//
			if( fDist > 0 )
			{
				ZoomInTarget(speed);
			}
			else if( fDist < 0 )
			{
				ZoomOutTarget(speed);
			}
		}
		else
		{
			//
			if( Utils.IsInvaildVec3( m_vLastCursorPos ) )
				m_vLastCursorPos = cursor.GetCurrentCursorPos();
			
			Vector3 vec = cursor.GetCurrentCursorPos() - m_vLastCursorPos;
			
			m_vLastCursorPos = cursor.GetCurrentCursorPos();

			Move(vec);
		}

	}

	void LateUpdate ()
	{
		float delta = RealTime.deltaTime;
		mMomentum -= mScroll;
		mScroll = NGUIMath.SpringLerp(mScroll, Vector3.zero, 20f, delta);
		
		// No momentum? Exit.
		if (mMomentum.magnitude < 0.0001f) return;
		CKinect.CursorController cursor = CKinect.CursorController.Instance;
		if ( !cursor.GetHandVisible() || !cursor.HasHandClosed )
		{
			
			Move(NGUIMath.SpringDampen(ref mMomentum, 9f, delta));
			
			if (panelRegion != null)
			{
				UpdateBounds();
				
				if (panelRegion.ConstrainTargetToBounds(target, ref mBounds, dragEffect == DragEffect.None))
				{
					CancelMovement();
				}
				else CancelSpring();
			}
			
			// Dampen the momentum
			NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
			
			// Cancel all movement (and snap to pixels) at the end
			if (mMomentum.magnitude < 0.0001f) CancelMovement();
		}
		else NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
	}

	#endregion


	/// <summary>
	/// 放大Target
	/// </summary>
	/// <param name="speed">Speed. 缩放速度</param>
	public void ZoomInTarget(float speed)
	{

		Vector3 ls =  target.transform.localScale;
		if( ls.x + speed>= m_MaxScale || ls.y + speed >= m_MaxScale)
		{
			target.transform.localScale = new Vector3(m_MaxScale,m_MaxScale,ls.z);
			return;
		}
		
		target.transform.localScale += new Vector3 ( speed, speed,0);

	}

	/// <summary>
	/// 缩小Target
	/// </summary>
	/// <param name="speed">Speed. 缩小速度</param>
	public void ZoomOutTarget(float speed)
	{
		Vector3 ls =  target.transform.localScale;
		if(ls.x - speed  <= m_MinScale || ls.y - speed <= m_MinScale)
		{
			target.transform.localScale = new Vector3(m_MinScale,m_MinScale,ls.z);
			return;
		}
		
		target.transform.localScale -= new Vector3 (speed , speed ,0);
	}

	/// <summary>
	/// 计算区域大小
	/// </summary>
	private void UpdateBounds ()
	{
		if (contentRect)
		{
			Transform t = panelRegion.cachedTransform;
			Matrix4x4 toLocal = t.worldToLocalMatrix;
			Vector3[] corners = contentRect.worldCorners;
			for (int i = 0; i < 4; ++i) corners[i] = toLocal.MultiplyPoint3x4(corners[i]);
			mBounds = new Bounds(corners[0], Vector3.zero);
			for (int i = 1; i < 4; ++i) mBounds.Encapsulate(corners[i]);
		}
		else
		{
			mBounds = NGUIMath.CalculateRelativeWidgetBounds(panelRegion.cachedTransform, target);
		}
	}

	/// <summary>
	/// 拖动Target
	/// </summary>
	/// <param name="screenDelta">Screen delta. 在屏幕坐标中的偏移</param>
	private void Move (Vector3 screenDelta)
	{
		// Adjust the momentum
		if (dragEffect != DragEffect.None)
			mMomentum = Vector3.Lerp(mMomentum, mMomentum + screenDelta * (0.01f * momentumAmount), 0.67f);
		
		if (screenDelta.x != 0f || screenDelta.y != 0f)
		{
			screenDelta = target.InverseTransformDirection(screenDelta);
			screenDelta.Scale(scale);
			screenDelta = target.TransformDirection(screenDelta);
		}

		if (panelRegion != null)
		{
			Vector3 after = target.localPosition + screenDelta;
			after.x = Mathf.Round(after.x);
			after.y = Mathf.Round(after.y);
			target.localPosition = after;

		}
		else target.position += screenDelta;
	}

	/// <summary>
	/// 取消回弹
	/// </summary>
	/// <returns><c>true</c> if this instance cancel movement; otherwise, <c>false</c>.</returns>
	public void CancelMovement ()
	{
		if (target != null)
		{
			Vector3 pos = target.localPosition;
			pos.x = Mathf.RoundToInt(pos.x);
			pos.y = Mathf.RoundToInt(pos.y);
			pos.z = Mathf.RoundToInt(pos.z);
			target.localPosition = pos;
		}
		mMomentum = Vector3.zero;
		mScroll = Vector3.zero;
	}

	public void CancelSpring ()
	{
		SpringPosition sp = target.GetComponent<SpringPosition>();
		if (sp != null) sp.enabled = false;
	}
}
