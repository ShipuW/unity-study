using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class UserManager : MonoBehaviour
{

    #region static instance

    private static UserManager s_Instance;
    
    public static UserManager Instance { get { return s_Instance; } }

    #endregion

    #region public member for unity

    public CKinect.MultiSourceManager m_MultiSourceManager;
    public GameObject m_Qued;
    //public float smoothFactor = 5f;
    //public float m_ZSpeed = 5f;
    public Material m_BoneMaterial;
    public bool m_bUseFilter = true;
    public bool m_bDebug = true;

	public delegate void VoidDelegate( ulong lockBodyId );
	public VoidDelegate OnLockUser;
    #endregion

    #region private member

    private bool bInited = false;
    private bool bLocked = false;
    private ulong lockBodyId;
    private Dictionary<ulong, Kinect.Body> _Bodies = new Dictionary<ulong, Kinect.Body>();
    private int lockBodyIndex = 255;
    private float m_QuedWidth;
    private float m_quedHight;
    private CKinect.JointFilter m_JointFilter = new CKinect.JointFilter();
    private GameObject lockBodyBoneGO;
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    // The Physical Interaction Zone
    private PHIZ m_PHIZLeft = new PHIZ();
    private PHIZ m_PHIZRight = new PHIZ();
    //
    private int m_PHIZFrameCounts = 0;
    //
    private UICamera.MouseOrTouch m_Mouse = new UICamera.MouseOrTouch();
    // 用于控制NGUI光线捡取间隔时间，NGUI不需要每帧都执行光线捡取
    private float m_fNextRaycast = 0;
    // NGUI Tooltip显示时间计数
    private float m_fTooltipTime = 0f;
    // NGUI Tooltip对象
    private GameObject m_Tooltip = null;
    // NGUI Hover对象
    private GameObject m_Hover = null;
    #endregion

    #region const member
    
    //    const  JointType cHandRight = JointType.HandRight;
    const int cDepthImageWidth = 512;
    const int cDepthImageHeight = 424;
    const int cColorWidth = 1920;
    const int cColorHeight = 1080;
    #endregion

    #region public member methods

    /// <summary>
    /// Gets a value indicating whether this <see cref="UserManager"/> is inited.
    /// </summary>
    /// <value><c>true</c> if inited; otherwise, <c>false</c>.</value>
    public bool Inited
    { 
        get
        {
            return bInited;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has lock user.
    /// </summary>
    /// <value><c>true</c> if this instance has lock user; otherwise, <c>false</c>.</value>
    public bool HasLockUser
    {
        get { return bLocked;}
    }

    /// <summary>
    /// Gets the lock body identifier.
    /// </summary>
    /// <value>The lock body identifier.</value>
    public ulong LockBodyId
    {
        get
        {
            if (bLocked)
            {
                return lockBodyId;
            } else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Gets the lock body.
    /// </summary>
    /// <value>The lock body.</value>
    public Kinect.Body LockBody
    {

        get
        {
            if (bLocked && _Bodies.ContainsKey(LockBodyId))
            {

                return _Bodies [LockBodyId];
            } else
            {
                return null;
            }
        }

    }

    /// <summary>
    /// Gets the index of the lock body.
    /// </summary>
    /// <value>The index of the lock body.</value>
    public int LockBodyIndex
    {
        get
        {
            //Debug.Log("lockBodyIndex ------------> " + lockBodyIndex);
            return lockBodyIndex;
        }
    }

    /// <summary>
    /// Gets the joint filter.
    /// </summary>
    /// <value>The joint filter.</value>
    public CKinect.JointFilter JointFilter
    {
        get
        {
            if (HasLockUser)
            {

                return m_JointFilter;

            } else
            {
                return null;
            }
        }
    }
    /// <summary>
    /// Gets the width of the qued.
    /// </summary>
    /// <value>The width of the qued.</value>
    public float QuedWidth
    {

        get
        {
            return m_QuedWidth;
        }
    }
    /// <summary>
    /// Gets the qued hight.
    /// </summary>
    /// <value>The qued hight.</value>
    public float QuedHight
    {
        get { return m_quedHight;}
    }
    /// <summary>
    /// Gets the qued z.
    /// </summary>
    /// <value>The qued z.</value>
    public float QuedZ
    {

        get { return m_Qued.transform.localPosition.z;}

    }

	public GameObject UserVisaulPanel{ get {return m_Qued; } set{ m_Qued = value; } }
    #endregion

    #region private properties
    // 是否有推按动作
    private bool HasHandPushedStart { get; set; }
    
    private bool HasHandPushedFinished { get; set; }
    //
    private bool HasDragStart { get; set; }
    
    private bool HasDragFinished { get; set; }

    private bool HasCursorLoadingStart{ get; set; }

    private bool HasCursorLoadingFinished{ get; set; }
    #endregion

    #region private for unity
	private void Awake()
    {
        s_Instance = this;
    }
    // Use this for initialization
	private void Start()
    {
		//
        if( m_MultiSourceManager == null )
        {
            Debug.LogError( "MultiSourceManager has not assigned!", this );
            return;

        }
    
		//
        if( m_Qued == null )
        {
            Debug.LogError( "Qued has not assignment", this );
            return;
        }
        
        Bounds bounds = m_Qued.transform.GetComponent<MeshRenderer>().bounds;
        
        m_quedHight = bounds.size.y;
        m_QuedWidth = bounds.size.x;
        
        //Debug.Log("width -------> " + m_QuedWidth + "hight-------> " + m_quedHight + " size --------> " + bounds.size);


        //
        this.HasHandPushedStart = false;
        this.HasHandPushedFinished = false;
        this.HasDragStart = false;
        this.HasDragFinished = false;

        this.HasCursorLoadingStart = false;
        this.HasCursorLoadingFinished = false;

        // set delegate
        CKinect.CursorController.Instance.OnHandLeftPushedStart = () => {
            this.HasHandPushedStart = true; };
        CKinect.CursorController.Instance.OnHandRightPushedStart = () => {
            this.HasHandPushedStart = true; };
        
        CKinect.CursorController.Instance.OnHandLeftPushedFinished = () => {
            this.HasHandPushedFinished = true; };
        CKinect.CursorController.Instance.OnHandRightPushedFinished = () => {
            this.HasHandPushedFinished = true; };
        
        CKinect.CursorController.Instance.OnHandLeftDragStart = (x) => {
            this.HasDragStart = true; };
        CKinect.CursorController.Instance.OnHandRightDragStart = (x) => {
            this.HasDragStart = true; };
        
        CKinect.CursorController.Instance.OnHandLeftDragFinished = (x) => {
            this.HasDragFinished = true; };
        CKinect.CursorController.Instance.OnHandRightDragFinished = (x) => {
            this.HasDragFinished = true; };

        CKinect.CursorController.Instance.OnLoadingStart = () => {
            this.HasCursorLoadingStart = true;};
        CKinect.CursorController.Instance.OnLoadingFinish = () => {
            this.HasCursorLoadingFinished = true;};
        
        // set NGUI custom input
        UICamera.onCustomInput = this.onCustomInput;
        this.ResetNGUIInput();

        this.bInited = true;
    }
    
    // Update is called once per frame
    private void Update()
    {

        Kinect.Body[] data = m_MultiSourceManager.GetBodyData();
        
        
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }
            
            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                _Bodies.Remove(trackingId);

                if (lockBodyId == trackingId)
                {
                    Destroy(lockBodyBoneGO);
                    lockBodyBoneGO = null;
                    lockBodyIndex = 255;
                    bLocked = false;
                    lockBodyId = 0;
                    Debug.Log("un Locked Body ----->");
                }
            }
        }

        int i = -1;
        foreach (var body in data)
        {
            i++;
            if (body == null)
            {
                continue;
            }

            CKinect.GestureManager.Instance.SetBodyByIndex(body.TrackingId, i);

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies [body.TrackingId] = body;
                }
            }
        }

        if (bLocked)
        {
            m_JointFilter.UpdateFilter(LockBody);

            UpdataCursor(LockBody);
            if (m_bDebug)
            {
                if (lockBodyBoneGO == null)
                {
                    lockBodyBoneGO = CreateBodyObject(lockBodyId);
                } else
                {
                    RefreshBodyObject(LockBody, lockBodyBoneGO);
                }
            }

            return;
        }

        int index = -1;

        foreach (var body in data)
        {
            index++;

            if (body == null)
            {
                continue;
            }

            if (body.IsTracked && !bLocked)
            {
             
                Dictionary<Kinect.JointType, Kinect.Joint> joints = body.Joints;

                Kinect.Joint HandRight = joints [Kinect.JointType.HandRight];

                Kinect.Joint HandLeft = joints [Kinect.JointType.HandLeft];

                Kinect.Joint head = joints [Kinect.JointType.Head];

                //任意一只手高过头 锁定该用户
                if (head.TrackingState == Kinect.TrackingState.Tracked)
                {
                    if (HandRight.TrackingState == Kinect.TrackingState.Tracked)
                    {

                        if (HandRight.Position.Y > head.Position.Y)
                        {

                            lockBodyIndex = index;

                            lockBodyId = body.TrackingId;
                            
                            bLocked = true;

                            Debug.Log("Locked Body ----->" + lockBodyId);

							//
							if( this.OnLockUser != null )
								this.OnLockUser( lockBodyId );

                            return;
                        }
                    }

                    if (HandLeft.TrackingState == Kinect.TrackingState.Tracked)
                    {
                        if (HandLeft.Position.Y > head.Position.Y)
                        {
                            lockBodyIndex = index;

                            lockBodyId = body.TrackingId;
                            
                            bLocked = true;
                            Debug.Log("Locked Body ----->" + lockBodyId);

							//
							if( this.OnLockUser != null )
								this.OnLockUser( lockBodyId );

                            return;
                        }
                    }
                }
            }
        }
    


    }

    //
    private void LateUpdate()
    {
        //
        this.HasHandPushedStart = false;
        this.HasHandPushedFinished = false;
        this.HasDragStart = false;
        this.HasDragFinished = false;

        this.HasCursorLoadingStart = false;
        this.HasCursorLoadingFinished = false;
    }

    #endregion

    #region private function

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = m_BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);
            
            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        
        return body;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        
        // m_JointFilter.UpdateFilter(body);
        //  Kinect.CameraSpacePoint[] filteredJoints = m_JointFilter.GetFilteredJoints();
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints [jt];
            Kinect.Joint? targetJoint = null;
            
            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints [_BoneMap [jt]];
            }
            
            Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
            
            if (sourceJoint.TrackingState == Kinect.TrackingState.Tracked)
            {
                jointObj.localPosition = GetVector3FromJoint(sourceJoint.Position);
                
                LineRenderer lr = jointObj.GetComponent<LineRenderer>();
                
                if (targetJoint.HasValue)
                {
                    Vector3 targetJointPos = GetVector3FromJoint(targetJoint.Value.Position);
                    lr.SetPosition(0, jointObj.localPosition);
                    lr.SetPosition(1, targetJointPos);
                    lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
                } else
                {
                    lr.enabled = false;
                }
            }
        }
    }

    private  Vector3 GetVector3FromJoint(Kinect.CameraSpacePoint joint)
    {
        Kinect.CoordinateMapper coordMapper = m_MultiSourceManager.GetCoordinateMapper();
        Vector3 pos = Vector3.zero;
        if (coordMapper != null)
        {
            Kinect.ColorSpacePoint jointColorPos = coordMapper.MapCameraPointToColorSpace(joint);
            
            
            float xNorm = (float)jointColorPos.X / cColorWidth;
            float yNorm = (1.0f - (float)jointColorPos.Y / cColorHeight);
            
            // Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(xNorm, yNorm, distanceToCamera));
            
            pos = new Vector3(xNorm * m_QuedWidth - m_QuedWidth / 2, yNorm * m_quedHight - m_quedHight / 2, m_Qued.transform.localPosition.z);
            
        }
        return pos;
    }

    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;
                
            case Kinect.TrackingState.Inferred:
                return Color.red;
                
            default:
                return Color.black;
        }
    }

    /// <summary>
    /// Sets the NGUI custom input.
    /// </summary>
    private void onCustomInput()
    {
        // 是否初始化
        if (!bInited)
            return;
        
//#if !UNITY_EDITOR
		//
        if (!CKinect.CursorController.Instance.GetHandVisible())
            return;

		//
		Vector3 vCursorPos = CKinect.CursorController.Instance.GetCurrentCursorPos();
//#else
//        //
//        Vector3 vCursorPos = CKinect.CursorController.Instance.GetCurrentCursorPos();
//
//        if( CKinect.CursorController.Instance.GetHandVisible() )
//        {
//            vCursorPos = CKinect.CursorController.Instance.GetCurrentCursorPos();
//        }
//        else
//        {
//            vCursorPos = Input.mousePosition;
//        }
//#endif

        // Update the position and delta
        Vector2 lastTouchPosition = vCursorPos;
        UICamera.MouseOrTouch mouse = m_Mouse;//UICamera.GetMouse(0);
        mouse.delta = lastTouchPosition - mouse.pos;
        mouse.pos = lastTouchPosition;
        bool posChanged = mouse.delta.sqrMagnitude > 0.001f;
        //
        UICamera.lastTouchPosition = lastTouchPosition;
        
        //      // Propagate the updates to the other mouse buttons
        //      for (int i = 1; i < 3; ++i)
        //      {
        //          UICamera.GetMouse(i).pos = mouse.pos;
        //          UICamera.GetMouse(i).delta = mouse.delta;
        //      }
        
        // Is any button currently pressed?
        bool isPressed = false;
        bool justPressed = false;
        
        //      for (int i = 0; i < 3; ++i)
        //      {
        //          if (Input.GetMouseButtonDown(i))
        //          {
        //              currentScheme = ControlScheme.Mouse;
        //              justPressed = true;
        //              isPressed = true;
        //          }
        //          else if (Input.GetMouseButton(i))
        //          {
        //              currentScheme = ControlScheme.Mouse;
        //              isPressed = true;
        //          }
        //      }
        
        //
        if (this.HasHandPushedStart )
        {
            UICamera.currentScheme = UICamera.ControlScheme.Mouse;
            justPressed = true;
            isPressed = true;
        } else if (this.HasHandPushedFinished)
        {
            UICamera.currentScheme = UICamera.ControlScheme.Mouse;
            isPressed = true;
        }


        if(this.HasCursorLoadingFinished)
        {
            UICamera.currentScheme = UICamera.ControlScheme.Mouse;
            isPressed = true;
        }

        
        // No need to perform raycasts every frame
        if (isPressed || posChanged || m_fNextRaycast < RealTime.time)
        {
            m_fNextRaycast = RealTime.time + 0.02f;
            if (!UICamera.Raycast(vCursorPos))
                UICamera.hoveredObject = UICamera.fallThrough;
            if (UICamera.hoveredObject == null)
                UICamera.hoveredObject = UICamera.genericEventHandler;
            //for (int i = 0; i < 3; ++i) UICamera.GetMouse(i).current = UICamera.hoveredObject;
            mouse.current = UICamera.hoveredObject;
        }
        
        bool highlightChanged = (mouse.last != mouse.current);
        if (highlightChanged)
            UICamera.currentScheme = UICamera.ControlScheme.Mouse;
        
        if (isPressed)
        {
            // A button was pressed -- cancel the tooltip
            m_fTooltipTime = 0f;
        } else if (posChanged && (!UICamera.current.stickyTooltip || highlightChanged))
        {
            if (m_fTooltipTime != 0f)
            {
                // Delay the tooltip
                m_fTooltipTime = RealTime.time + UICamera.current.tooltipDelay;
            } else if (m_Tooltip != null)
            {
                // Hide the tooltip
                UICamera.current.ShowTooltip(false);
            }
        }
        
        // Generic mouse move notifications
        if (posChanged && UICamera.onMouseMove != null)
        {
            UICamera.currentTouch = mouse;
            UICamera.onMouseMove(UICamera.currentTouch.delta);
            UICamera.currentTouch = null;
        }
        
        // The button was released over a different object -- remove the highlight from the previous
        if ((justPressed || !isPressed) && m_Hover != null && highlightChanged)
        {
            UICamera.currentScheme = UICamera.ControlScheme.Mouse;
            UICamera.currentTouch = mouse;
            if (m_Tooltip != null)
                UICamera.current.ShowTooltip(false);
            if (UICamera.onHover != null)
                UICamera.onHover(m_Hover, false);
            UICamera.Notify(m_Hover, "OnHover", false);
            m_Hover = null;
        }
        
        // Process all 3 mouse buttons as individual touches
        //      for( int i = 0; i < 3; ++i )
        //      {
        //          bool pressed = Input.GetMouseButtonDown(i);
        //          bool unpressed = Input.GetMouseButtonUp(i);
        //          
        //          if (pressed || unpressed) UICamera.currentScheme = UICamera.ControlScheme.Mouse;
        //          
        //          currentTouch = mMouse[i];
        //          
        //          #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        //          if (i == 0 && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        //          {
        //              currentTouchID = -2;
        //              currentKey = KeyCode.Mouse1;
        //          }
        //          else
        //              #endif
        //          {
        //              currentTouchID = -1 - i;
        //              currentKey = KeyCode.Mouse0 + i;
        //          }
        //          
        //          // We don't want to update the last camera while there is a touch happening
        //          if (pressed) currentTouch.pressedCam = currentCamera;
        //          else if (currentTouch.pressed != null) currentCamera = currentTouch.pressedCam;
        //          
        //          // Process the mouse events
        //          ProcessTouch(pressed, unpressed);
        //          currentKey = KeyCode.None;
        //      }
        
        //
        bool pressed = this.HasDragStart || this.HasHandPushedStart||this.HasCursorLoadingStart;
        bool unpressed = this.HasDragFinished || this.HasHandPushedFinished||this.HasCursorLoadingFinished;
        
        if (pressed || unpressed)
            UICamera.currentScheme = UICamera.ControlScheme.Mouse;
        
        UICamera.currentTouch = mouse;
        
        #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        if (i == 0 && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            UICamera.currentTouchID = -2;
            UICamera.currentKey = KeyCode.Mouse1;
        }
        else
        #endif
        {
            UICamera.currentTouchID = -1;
            UICamera.currentKey = KeyCode.Mouse0;
        }
        
        // We don't want to update the last camera while there is a touch happening
        if (pressed)
            UICamera.currentTouch.pressedCam = UICamera.currentCamera;
        else if (UICamera.currentTouch.pressed != null)
            UICamera.currentCamera = UICamera.currentTouch.pressedCam;

        // Process the mouse events
        UICamera.current.ProcessTouch(pressed, unpressed);

        UICamera.currentKey = KeyCode.None;
        
        //Debug.Log( "pressed:"+pressed+"   unpressed:"+unpressed );
        
        // If nothing is pressed and there is an object under the touch, highlight it
        if (!isPressed && highlightChanged)
        {
            UICamera.currentScheme = UICamera.ControlScheme.Mouse;
            m_fTooltipTime = RealTime.time + UICamera.current.tooltipDelay;
            m_Hover = mouse.current;
            UICamera.currentTouch = mouse;
            if (UICamera.onHover != null)
                UICamera.onHover(m_Hover, true);
            UICamera.Notify(m_Hover, "OnHover", true);
        }
        UICamera.currentTouch = null;
        
        // Update the last value
        mouse.last = mouse.current;
        //for (int i = 1; i < 3; ++i) mMouse[i].last = mMouse[0].last;
    }

    /// <summary>
    /// Resets the NGUI input.
    /// </summary>
    public void ResetNGUIInput()
    {
        // Save the starting mouse position
        UICamera.MouseOrTouch mouse = m_Mouse;//UICamera.GetMouse(0);
        mouse.pos = CKinect.CursorController.Instance.GetCurrentCursorPos();
        
        for (int i = 1; i < 3; ++i)
        {
            UICamera.GetMouse(i).pos = mouse.pos;
            UICamera.GetMouse(i).lastPos = mouse.pos;
        }
        UICamera.lastTouchPosition = mouse.pos;
    }

    private void UpdataCursor(Kinect.Body body)
    {
        Kinect.CameraSpacePoint[] filteredJoints = m_JointFilter.GetFilteredJoints();
        // Create PHIZ 
        if( m_PHIZFrameCounts <= 0 )
        {
            PHIZ.Create(body, ref m_PHIZLeft, ref m_PHIZRight);
            m_PHIZFrameCounts = 0;
        }
        
        // PHIZ
        // 得到左右肩部关节点数据
        Kinect.CameraSpacePoint jointShoulderLeft = filteredJoints [(int)Kinect.JointType.ShoulderLeft];
        Kinect.CameraSpacePoint jointShoulderRight = filteredJoints [(int)Kinect.JointType.ShoulderRight];
        
        //
        Vector3 vShoulderLeftPos = Utils.GetVector3FromCameraSpacePoint(jointShoulderLeft);
        Vector3 vShoulderRightPos = Utils.GetVector3FromCameraSpacePoint(jointShoulderRight);
        
        //
        m_PHIZLeft.anchor = m_PHIZLeft.anchor * (float)m_PHIZFrameCounts / (float)(m_PHIZFrameCounts + 1) + vShoulderLeftPos / (float)(m_PHIZFrameCounts + 1);
        m_PHIZRight.anchor = m_PHIZRight.anchor * (float)m_PHIZFrameCounts / (float)(m_PHIZFrameCounts + 1) + vShoulderRightPos / (float)(m_PHIZFrameCounts + 1);
        
        //
        m_PHIZFrameCounts++;
        
        float fLen = (m_PHIZLeft.anchor - vShoulderLeftPos).magnitude;
        
        //
        if( fLen > 0.15f )
        {
            m_PHIZLeft.anchor = vShoulderLeftPos;
            m_PHIZFrameCounts = 1;
        }
        
        fLen = (m_PHIZRight.anchor - vShoulderRightPos).magnitude;
        //
        if( fLen > 0.15f )
        {
            m_PHIZRight.anchor = vShoulderRightPos;
            m_PHIZFrameCounts = 1;
        }
        
        //
        // 得到左右手部关节点数据
        Vector3 vJointHandLeftPos = Utils.GetVector3FromCameraSpacePoint(filteredJoints [(int)Kinect.JointType.HandLeft]);
        Vector3 vJointHandRightPos = Utils.GetVector3FromCameraSpacePoint(filteredJoints [(int)Kinect.JointType.HandRight]);
        //
        Vector3 vJointElbowLeftPos = Utils.GetVector3FromCameraSpacePoint(filteredJoints [(int)Kinect.JointType.ElbowLeft]);
        Vector3 vJointElbowRightPos = Utils.GetVector3FromCameraSpacePoint(filteredJoints [(int)Kinect.JointType.ElbowRight]);
        
        Vector2 vLeft = m_PHIZLeft.MapToScreen(vJointHandLeftPos, Screen.width, Screen.height, true);
        Vector2 vRight = m_PHIZRight.MapToScreen(vJointHandRightPos, Screen.width, Screen.height, true);
        
		//
		CKinect.CursorController cursorController = CKinect.CursorController.Instance;

        //
        if( CKinect.CursorController.HandPutDown( vJointElbowLeftPos, vJointHandLeftPos ) )
        {
			if( cursorController.GetHandLeftVisible() )
				cursorController.HideHandLeft();
        }
		else
        {
			if( !cursorController.GetHandLeftVisible() )
				cursorController.ShowHandLeft();
        }
        
		//
        if( CKinect.CursorController.HandPutDown( vJointElbowRightPos, vJointHandRightPos ) )
        {
			if( cursorController.GetHandRightVisible() )
				cursorController.HideHandRight();
        }
		else
        {
            // 当右手在操作时隐藏左手
			if( cursorController.GetHandLeftVisible() )
				cursorController.HideHandLeft();
            
			if( !cursorController.GetHandRightVisible() )
				cursorController.ShowHandRight();
        }
        
        //
		cursorController.SetHandLeftCursorPos(vLeft);
		cursorController.SetHandRightCursorPos(vRight);
        //
		cursorController.SetHandLeftRawPos(vJointHandLeftPos);
		cursorController.SetHandRightRawPos(vJointHandRightPos);
        //
        // 设置手部状态和手部可信度
		cursorController.SetHandLeftState(body.HandLeftState, body.HandLeftConfidence);
		cursorController.SetHandRightState(body.HandRightState, body.HandRightConfidence);

    }

	#endregion

	#region public function

	public void HideUserViualPanel()
	{

		if(!bInited)
			return;

		m_Qued.SetActive(false);
	}

	public void ShowUserVisualPanel()
	{

		if( !bInited )
			return;

		m_Qued.SetActive(true);

	}

	#endregion

}
