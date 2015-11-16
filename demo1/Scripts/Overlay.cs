using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class Overlay : MonoBehaviour
{
	#region public member for unity
	//
	public CKinect.MultiSourceManager m_MultiSourceManager;
	
	//
	public UserManager m_UserManager;

	//
	public bool m_HideDebugSphere = false;
	
	//
	public GameObject m_OverlayGameObjectLeft;
	
	//
	public GameObject m_OverlayGameObjectRight;
	
	//
	public Transform m_TouchButtonParent;
	
	//
	public float smoothFactor = 5f;
	
	//public float m_ZSpeed = 5f;
	//
	public bool m_bUseFilter = true;
	#endregion
	
	#region private member
	
	private bool bInited = false;
	
	//
	private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
	
	//
	private float distanceToCamera = 10f;
	
	//
	private Vector3 m_vTouchButtonParentOldPos = Utils.InvaildVec3;  
	
	#endregion
	
	#region const member
	
	//    const  JointType cHandRight = JointType.HandRight;
	const int cDepthImageWidth = 512;
	const int cDepthImageHeight = 424;
	const int cColorWidth = 1920;
	const int cColorHeight = 1080;
	#endregion
	
	#region private function for unity
	
	// Use this for initialization
	private void Start()
	{
		if (m_OverlayGameObjectRight == null)
		{
			Debug.LogError("Overlay gameobject right has not assignment");
			
			return;
		}
		
		if (m_OverlayGameObjectLeft == null)
		{
			Debug.LogError("Overlay gameobject left has not assignment");
			
			return;
		}
		
		if( m_HideDebugSphere || Application.isEditor == false )
		{
			m_OverlayGameObjectLeft.renderer.enabled = false;
			m_OverlayGameObjectRight.renderer.enabled = false;
		}

		distanceToCamera = (m_OverlayGameObjectRight.transform.position - Camera.main.transform.position).magnitude;

		//
		StartCoroutine( DelayInit() );
	}

	//
	private IEnumerator DelayInit()
	{
		//
		yield return null;

		//
		if(m_MultiSourceManager == null)
		{
			m_MultiSourceManager = CKinect.MultiSourceManager.Instance;
		}
		
		if (m_UserManager == null)
		{
			m_UserManager = UserManager.Instance;
		}

		//
		if (m_MultiSourceManager == null)
		{
			Debug.LogError("MultiSourceManager has not assignment");			
			yield break;
		}

		//
		if (m_UserManager == null)
		{
			Debug.LogError("UserManager has not assignment");
			yield break;
		}

		bInited = true;
	}
	
	// Update is called once per frame
	private void Update()
	{
		
		if (!bInited || !m_UserManager.Inited)
		{
			return;
		}
		
		if (!m_UserManager.HasLockUser)
		{
			List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
			foreach (ulong trackingId in knownIds)
			{
				Destroy(_Bodies [trackingId]);
				_Bodies.Remove(trackingId);
			}
			
			m_TouchButtonParent.position = Vector3.zero;
			m_vTouchButtonParentOldPos = Utils.InvaildVec3;
			
			m_OverlayGameObjectLeft.transform.localPosition = new Vector3(0,0,10);
			m_OverlayGameObjectRight.transform.localPosition =  new Vector3(0,0,10);
			
			
			return;
		}
		
		Kinect.Body lockBody = m_UserManager.LockBody;
		
		if (lockBody == null)
		{
			return;
		}
		
		
		if (lockBody.IsTracked)
		{
			
			
			//            CKinect.CursorController.Instance.ShowHandLeft();
			//            CKinect.CursorController.Instance.ShowHandRight();
			//
			//            CKinect.JointFilter m_JointFilter = m_UserManager.JointFilter;
			//            Kinect.CameraSpacePoint[] filteredJoints = m_JointFilter.GetFilteredJoints();
			//
			//            Kinect.CameraSpacePoint jointHandLeft = filteredJoints [(int)Kinect.JointType.HandLeft];
			//            Kinect.CameraSpacePoint jointHandRight = filteredJoints [(int)Kinect.JointType.HandRight];
			
			// CKinect.CursorController.Instance.SetHandLeftCursorPos();
			
			RefreshSphere(lockBody);
		}
		
		/*
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
                Destroy(_Bodies [trackingId]);
                _Bodies.Remove(trackingId);
            }
        }
        
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }
            
            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies [body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                if(m_bDebug)
                {
                    RefreshBodyObject(body, _Bodies [body.TrackingId]);
                }        

                RefreshSphere(body);
            }
        }
        */
	}
	
	#endregion
	
	#region private function
	
	/// <summary>
	/// Maps the space point to depth coords.
	/// </summary>
	/// <returns>The space point to depth coords.</returns>
	/// <param name="spacePos">Space position.</param>
	private Vector2 MapSpacePointToDepthCoords(Vector3 spacePos)
	{
		Vector2 vPoint = Vector2.zero;
		
		Kinect.CoordinateMapper coordMapper = m_MultiSourceManager.GetCoordinateMapper();
		
		if (coordMapper != null)
		{
			Kinect.CameraSpacePoint camPoint = new Kinect.CameraSpacePoint();
			camPoint.X = spacePos.x;
			camPoint.Y = spacePos.y;
			camPoint.Z = spacePos.z;
			
			Kinect.CameraSpacePoint[] camPoints = new Kinect.CameraSpacePoint[1];
			camPoints [0] = camPoint;
			
			Kinect.DepthSpacePoint[] depthPoints = new Kinect.DepthSpacePoint[1];
			coordMapper.MapCameraPointsToDepthSpace(camPoints, depthPoints);
			
			Kinect.DepthSpacePoint depthPoint = depthPoints [0];
			
			if (depthPoint.X >= 0 && depthPoint.X < cDepthImageWidth &&
			    depthPoint.Y >= 0 && depthPoint.Y < cDepthImageHeight)
			{
				vPoint.x = depthPoint.X;
				vPoint.y = depthPoint.Y;
			}
		}
		
		return vPoint;
	}
	
	/// <summary>
	/// Gets the depth for pixel.
	/// </summary>
	/// <returns>The depth for pixel.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	private ushort GetDepthForPixel(int x, int y)
	{
		
		ushort[] depthData = m_MultiSourceManager.GetDepthData();
		
		if (depthData != null && depthData.Length > 0)
		{
			int index = y * cDepthImageWidth + x;
			
			if (index >= 0 && index < cDepthImageWidth * cDepthImageHeight)
			{
				return depthData [index];
			}
		}
		
		return 0;
	}
	
	/// <summary>
	/// Maps the depth point to color coords.
	/// </summary>
	/// <returns>The depth point to color coords.</returns>
	/// <param name="depthPos">Depth position.</param>
	/// <param name="depthVal">Depth value.</param>
	public Vector2 MapDepthPointToColorCoords(Vector2 depthPos, ushort depthVal)
	{
		Vector2 vPoint = Vector2.zero;
		Kinect.CoordinateMapper coordMapper = m_MultiSourceManager.GetCoordinateMapper();
		
		if (coordMapper != null && depthPos != Vector2.zero)
		{
			Kinect.DepthSpacePoint depthPoint = new Kinect.DepthSpacePoint();
			depthPoint.X = depthPos.x;
			depthPoint.Y = depthPos.y;
			
			Kinect.DepthSpacePoint[] depthPoints = new Kinect.DepthSpacePoint[1];
			depthPoints [0] = depthPoint;
			
			ushort[] depthVals = new ushort[1];
			depthVals [0] = depthVal;
			
			Kinect.ColorSpacePoint[] colPoints = new Kinect.ColorSpacePoint[1];
			coordMapper.MapDepthPointsToColorSpace(depthPoints, depthVals, colPoints);
			
			Kinect.ColorSpacePoint colPoint = colPoints [0];
			vPoint.x = colPoint.X;
			vPoint.y = colPoint.Y;
		}
		
		return vPoint;
	}
	
	/// <summary>
	/// Refreshs the sphere.
	/// </summary>
	/// <param name="body">Body.</param>
	private void RefreshSphere(Kinect.Body body)
	{
		
		if (body != null && body.IsTracked)
		{
			
			// Debug.Log("------------------>RefreshSphere");
			
			Kinect.CameraSpacePoint jointHandLeft, jointHandRight, jointSpineShoulder;
			if (m_bUseFilter)
			{
				// m_JointFilter.UpdateFilter(body);
				CKinect.JointFilter m_JointFilter = m_UserManager.JointFilter;
				Kinect.CameraSpacePoint[] filteredJoints = m_JointFilter.GetFilteredJoints();
				
				jointHandLeft = filteredJoints [(int)Kinect.JointType.HandLeft];
				jointHandRight = filteredJoints [(int)Kinect.JointType.HandRight];
				
				jointSpineShoulder = filteredJoints [(int)Kinect.JointType.SpineShoulder];
			}
			else
			{
				jointHandLeft = body.Joints [Kinect.JointType.HandLeft].Position;
				jointHandRight = body.Joints [Kinect.JointType.HandRight].Position;
				
				jointSpineShoulder = body.Joints [Kinect.JointType.SpineShoulder].Position;
			}
			
			Vector3 handLeftPos = Utils.GetVector3FromCameraSpacePoint(jointHandLeft);
			Vector3 handRightPos = Utils.GetVector3FromCameraSpacePoint(jointHandRight);
			
			Vector3 spineShoulderPos = Utils.GetVector3FromCameraSpacePoint(jointSpineShoulder);
			
			// Debug.Log("------------------>RefreshSphere " + handRightPos);
			
			if (!Utils.IsInvaildVec3(handLeftPos))
			{
				
				Vector2 posDepth = MapSpacePointToDepthCoords(handLeftPos);
				
				ushort depthValue = GetDepthForPixel((int)posDepth.x, (int)posDepth.y);
				
				if (depthValue > 0)
				{
					// depth pos to color pos
					Vector2 posColor = MapDepthPointToColorCoords(posDepth, depthValue);
					
					float xNorm = (float)posColor.x / cColorWidth;
					float yNorm = (1.0f - (float)posColor.y / cColorHeight);
					
					if (m_OverlayGameObjectLeft)
					{
						// Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(xNorm, yNorm, distanceToCamera));
						
						//Debug.Log("z Depth -------->" + CheckPushDepth(body));
						
						// Vector3 vPosOverlay = new Vector3(xNorm * m_QuedWidth - m_QuedWidth / 2, yNorm * m_quedHight - m_quedHight / 2, m_Qued.transform.localPosition.z - CheckPushDepth(body) * m_ZSpeed);
						
						float quedwidth = m_UserManager.QuedWidth;
						float quedHight = m_UserManager.QuedHight;
						float quedZ = m_UserManager.QuedZ;
						
						Vector3 vPosOverlay = new Vector3(xNorm * quedwidth - quedwidth / 2, yNorm * quedHight - quedHight / 2, quedZ - CheckPushDepth(body));
						
						m_OverlayGameObjectLeft.transform.position = Vector3.Lerp(m_OverlayGameObjectLeft.transform.position, vPosOverlay, smoothFactor * Time.deltaTime);
						//  Debug.Log("------------------>RefreshSphere end");
					}
				}
			}
			
			if (!Utils.IsInvaildVec3(handRightPos))
			{
				
				Vector2 posDepth = MapSpacePointToDepthCoords(handRightPos);
				
				ushort depthValue = GetDepthForPixel((int)posDepth.x, (int)posDepth.y);
				
				if (depthValue > 0)
				{
					// depth pos to color pos
					Vector2 posColor = MapDepthPointToColorCoords(posDepth, depthValue);
					
					float xNorm = (float)posColor.x / cColorWidth;
					float yNorm = (1.0f - (float)posColor.y / cColorHeight);
					
					if (m_OverlayGameObjectRight)
					{
						// Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(xNorm, yNorm, distanceToCamera));
						
						//Debug.Log("z Depth -------->" + CheckPushDepth(body));
						
						//Vector3 vPosOverlay = new Vector3(xNorm * m_QuedWidth - m_QuedWidth / 2, yNorm * m_quedHight - m_quedHight / 2, m_Qued.transform.localPosition.z - CheckPushDepth(body) * m_ZSpeed);
						
						float quedWidth = m_UserManager.QuedWidth;
						float quedHight = m_UserManager.QuedHight;
						float quedZ = m_UserManager.QuedZ;
						
						Vector3 vPosOverlay = new Vector3(xNorm * quedWidth - quedWidth / 2, yNorm * quedHight - quedHight / 2, quedZ - CheckPushDepth(body));
						
						m_OverlayGameObjectRight.transform.position = Vector3.Lerp(m_OverlayGameObjectRight.transform.position, vPosOverlay, smoothFactor * Time.deltaTime);
						//  Debug.Log("------------------>RefreshSphere end");
					}
				}
			}
			
			//
			if (Utils.IsInvaildVec3(spineShoulderPos))
			{
				m_TouchButtonParent.position = Vector3.zero;
				m_vTouchButtonParentOldPos = Utils.InvaildVec3;
			}
			else
			{
				Vector2 posDepth = MapSpacePointToDepthCoords(spineShoulderPos);
				
				ushort depthValue = GetDepthForPixel((int)posDepth.x, (int)posDepth.y);
				
				if (depthValue > 0)
				{
					// depth pos to color pos
					Vector2 posColor = MapDepthPointToColorCoords(posDepth, depthValue);
					
					float xNorm = (float)posColor.x / cColorWidth;
					float yNorm = (1.0f - (float)posColor.y / cColorHeight);
					
					if (m_OverlayGameObjectRight)
					{
						// Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(xNorm, yNorm, distanceToCamera));
						
						//Debug.Log("z Depth -------->" + CheckPushDepth(body));
						
						Vector3 pos = m_TouchButtonParent.position;
						
						float quedWidth = m_UserManager.QuedWidth;
						float quedHight = m_UserManager.QuedHight;
						
						Vector3 vPosOverlay = new Vector3(xNorm * quedWidth - quedWidth / 2, yNorm * quedHight - quedHight / 2, pos.z);
						
						m_TouchButtonParent.position = Vector3.Lerp(m_TouchButtonParent.position, vPosOverlay, smoothFactor * Time.deltaTime);
						
						m_vTouchButtonParentOldPos = m_TouchButtonParent.position;
					}
				}
			}
			
		}
	}
	
	/// <summary>
	/// Checks the push depth.
	/// </summary>
	/// <returns>The push depth.</returns>
	/// <param name="body">Body.</param>
	private float CheckPushDepth(Kinect.Body body)
	{
		
		if (body.IsTracked)
		{
			
			Kinect.Joint handRightJoint = body.Joints [Kinect.JointType.HandRight];
			Kinect.Joint shoulderRightJoint = body.Joints [Kinect.JointType.ShoulderRight];
			
			if (handRightJoint.TrackingState == Kinect.TrackingState.Tracked && shoulderRightJoint.TrackingState == Kinect.TrackingState.Tracked)
			{
				
				Vector3 handRightPos = Utils.GetVector3FromCameraSpacePoint(handRightJoint.Position);
				Vector3 shoulderRightPos = Utils.GetVector3FromCameraSpacePoint(shoulderRightJoint.Position);
				float xDis = Mathf.Abs(handRightPos.x - shoulderRightPos.x);
				float dis = Vector3.Distance(handRightPos, shoulderRightPos);
				
				return Mathf.Sqrt(dis * dis - xDis * xDis);
				
			}
			
		}
		return 0;
	}
	
	#endregion
	
}
