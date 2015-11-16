using System;
using UnityEngine;

/// <summary>
/// 物理交互区域（Physical Interaction Zone）
/// </summary>
public class PHIZ
{
	public float width,height;

	public Vector3 anchor, bias;

	/// <summary>
	/// 构造函数
	/// </summary>
	public PHIZ()
	{
		this.anchor = Vector3.zero;
		this.bias = Vector3.zero;
		this.width = 0;
		this.height = 0;
	}

	/// <summary>
	/// 构造函数 <see cref="PHIZ"/> class.
	/// </summary>
	/// <param name="anchor">定位点</param>
	/// <param name="bias">偏移量</param>
	/// <param name="width">宽度</param>
	/// <param name="height">高度</param>
	public PHIZ( Vector3 anchor, Vector3 bias, float width, float height )
	{
		Init( anchor, bias, width, height );
	}

	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="anchor">定位点</param>
	/// <param name="bias">偏移量</param>
	/// <param name="width">宽度</param>
	/// <param name="height">高度</param>
	public void Init( Vector3 anchor, Vector3 bias, float width, float height )
	{
		this.anchor = anchor;
		this.bias = bias;
		this.width = width;
		this.height = height;
	}

	/// <summary>
	/// 更新定位点
	/// </summary>
	/// <param name="anchor">定位点</param>
	public void UpdateAnchor( Vector3 anchor )
	{
		this.anchor = anchor;
	}

	/// <summary>
	/// 测试某点是否在PHIZ的范围内，只在二维平面内测试
	/// </summary>
	/// <param name="p">测试点</param>
	public bool Contains( Vector3 p )
	{
		Vector3 vCenter = this.anchor + this.bias;

		Vector3 vDiff = vCenter - p;

		if( Mathf.Abs( vDiff.x ) > this.width * 0.5f || Mathf.Abs( vDiff.y ) > this.height * 0.5f )
			return false;
		else 
			return true;
	}

	/// <summary>
	/// 映射到屏幕坐标，以左下角为圆点
	/// </summary>
	/// <returns>返回映射成功的点</returns>
	/// <param name="p">需要映射的点</param>
	/// <param name="fScreenWidth">屏幕宽度</param>
	/// <param name="fScreenHeight">屏幕高度</param>
    /// <param name="bRemapWhenOutside">是否限制在屏幕坐标范围内</param>
    public Vector3 MapToScreen( Vector3 p, float fScreenWidth, float fScreenHeight, bool bRestrict = false )
	{
		Vector3 vRet = Vector3.zero;

		Vector3 vCenter = this.anchor + this.bias;
		float fHalfWidth = this.width * 0.5f;
		float fHalfHeight = this.height * 0.5f;

		//
		vRet.x = ( ( p.x - ( vCenter.x - fHalfWidth ) ) / this.width ) * fScreenWidth;
		vRet.y = ( ( p.y - ( vCenter.y - fHalfHeight ) ) / this.height ) * fScreenHeight;
        
        // 是否限制在屏幕坐标范围内
        if( bRestrict )
        {
            vRet.x = Mathf.Clamp( vRet.x, 0f, fScreenWidth );
            vRet.y = Mathf.Clamp( vRet.y, 0f, fScreenHeight );
        }
        
		return vRet;
	}

    /// <summary>
    /// 创建左右手PHIZ
    /// </summary>
    /// <param name="body">Body.</param>
    /// <param name="left">Left.</param>
    /// <param name="right">Right.</param>
    public static void Create( Windows.Kinect.Body body, ref PHIZ left, ref PHIZ right )
    {
        //
        if( body == null || left == null || right == null )
        {
            Debug.Log( "Can not create PHIZ." );
            return;
        }

        //
        Windows.Kinect.Joint jointShoulderLeft = body.Joints[Windows.Kinect.JointType.ShoulderLeft];
        Windows.Kinect.Joint jointShoulderRight = body.Joints[Windows.Kinect.JointType.ShoulderRight];
        
        Windows.Kinect.Joint jointElbowLeft = body.Joints[Windows.Kinect.JointType.ElbowLeft];
        Windows.Kinect.Joint jointElbowRight = body.Joints[Windows.Kinect.JointType.ElbowRight];
        
        Windows.Kinect.Joint jointHead = body.Joints[Windows.Kinect.JointType.Head];
        
        Windows.Kinect.Joint jointSpineMid = body.Joints[Windows.Kinect.JointType.SpineMid];

        //
        Vector3 vJointShoulderLeftPos = new Vector3( jointShoulderLeft.Position.X, jointShoulderLeft.Position.Y, jointShoulderLeft.Position.Z );
        Vector3 vJointShoulderRightPos = new Vector3( jointShoulderRight.Position.X, jointShoulderRight.Position.Y, jointShoulderRight.Position.Z );
        
        Vector3 vJointElbowLeftPos = new Vector3( jointElbowLeft.Position.X, jointElbowLeft.Position.Y, jointElbowLeft.Position.Z );
        Vector3 vJointElbowRightPos = new Vector3( jointElbowRight.Position.X, jointElbowRight.Position.Y, jointElbowRight.Position.Z );
        
        Vector3 vJointHeadPos = new Vector3( jointHead.Position.X, jointHead.Position.Y, jointHead.Position.Z );
        
        Vector3 vJointSpineMidPos = new Vector3( jointSpineMid.Position.X, jointSpineMid.Position.Y, jointSpineMid.Position.Z );
        
        //Vector3 vJointSpineShoudler = GetVector3FromJoint(jointSpineShoulder);
        
        //
        float fUpperArmLeftLen = (vJointShoulderLeftPos - vJointElbowLeftPos).magnitude;
        float fUpperArmRightLen = (vJointShoulderRightPos - vJointElbowRightPos).magnitude;
        float fShouldersLen = (vJointShoulderLeftPos - vJointShoulderRightPos).magnitude;
        //float fHeadToElbowLeftLen = (vJointHeadPos - vJointElbowLeftPos).magnitude;
        //float fHeadToElbowRightLen = (vJointHeadPos - vJointElbowRightPos).magnitude;
        float fHeadToSpineMidLen = (vJointHeadPos - vJointSpineMidPos).magnitude;
        
        
        //Debug.Log( "fUpperArmLeftLen:"+fUpperArmLeftLen+"  fUpperArmRightLen:"+fUpperArmRightLen+"  fShouldersLen:"+fShouldersLen+
        //          "  fHeadToElbowLeftLen"+fHeadToElbowLeftLen+"  fHeadToElbowRightLen:"+fHeadToElbowRightLen +"  fHeadToSpineMidLen:"+fHeadToSpineMidLen);
        
        float fAverageArmLen = ( fUpperArmLeftLen + fUpperArmRightLen ) * 0.5f;
        float fHalfArmLen = fAverageArmLen * 0.5f;

        left.bias = (new Vector3(-1f,-1f,0f)).normalized * fHalfArmLen + new Vector3(0,0,-(fAverageArmLen + 0.5f));
        right.bias = (new Vector3(1f,-1f,0f)).normalized * fHalfArmLen + new Vector3(0,0,-(fAverageArmLen + 0.5f));
        
        left.width = fShouldersLen*1.5f;
        left.height = fHeadToSpineMidLen;
        
        right.width = fShouldersLen*1.5f;
        right.height = fHeadToSpineMidLen;
    }

	/// <summary>
	/// 创建左右手PHIZ
	/// </summary>
	/// <param name="body">Body.</param>
	/// <param name="left">Left.</param>
	/// <param name="right">Right.</param>
	public static void Create( Windows.Kinect.CameraSpacePoint[] filteredJoints, ref PHIZ left, ref PHIZ right )
	{
		//
		if( filteredJoints == null || left == null || right == null )
		{
			Debug.Log( "Can not create PHIZ." );
			return;
		}

		//
		Vector3 vJointShoulderLeftPos = Utils.GetVector3FromCameraSpacePoint( filteredJoints[(int)Windows.Kinect.JointType.ShoulderLeft] );
		Vector3 vJointShoulderRightPos = Utils.GetVector3FromCameraSpacePoint( filteredJoints[(int)Windows.Kinect.JointType.ShoulderRight] );
		
		Vector3 vJointElbowLeftPos = Utils.GetVector3FromCameraSpacePoint( filteredJoints[(int)Windows.Kinect.JointType.ElbowLeft] );
		Vector3 vJointElbowRightPos = Utils.GetVector3FromCameraSpacePoint( filteredJoints[(int)Windows.Kinect.JointType.ElbowRight] );
		
		Vector3 vJointHeadPos = Utils.GetVector3FromCameraSpacePoint( filteredJoints[(int)Windows.Kinect.JointType.Head] );
		
		Vector3 vJointSpineMidPos = Utils.GetVector3FromCameraSpacePoint( filteredJoints[(int)Windows.Kinect.JointType.SpineMid] );
		
		//
		float fUpperArmLeftLen = (vJointShoulderLeftPos - vJointElbowLeftPos).magnitude;
		float fUpperArmRightLen = (vJointShoulderRightPos - vJointElbowRightPos).magnitude;
		float fShouldersLen = (vJointShoulderLeftPos - vJointShoulderRightPos).magnitude;
		//float fHeadToElbowLeftLen = (vJointHeadPos - vJointElbowLeftPos).magnitude;
		//float fHeadToElbowRightLen = (vJointHeadPos - vJointElbowRightPos).magnitude;
		float fHeadToSpineMidLen = (vJointHeadPos - vJointSpineMidPos).magnitude;
		
		
		//Debug.Log( "fUpperArmLeftLen:"+fUpperArmLeftLen+"  fUpperArmRightLen:"+fUpperArmRightLen+"  fShouldersLen:"+fShouldersLen+
		//          "  fHeadToElbowLeftLen"+fHeadToElbowLeftLen+"  fHeadToElbowRightLen:"+fHeadToElbowRightLen +"  fHeadToSpineMidLen:"+fHeadToSpineMidLen);
		
		float fAverageArmLen = ( fUpperArmLeftLen + fUpperArmRightLen ) * 0.5f;
		float fHalfArmLen = fAverageArmLen * 0.5f;
		
		left.bias = (new Vector3(-1f,-1f,0f)).normalized * fHalfArmLen + new Vector3(0,0,-(fAverageArmLen + 0.5f));
		right.bias = (new Vector3(1f,-1f,0f)).normalized * fHalfArmLen + new Vector3(0,0,-(fAverageArmLen + 0.5f));
		
		left.width = fShouldersLen*1.5f;
		left.height = fHeadToSpineMidLen;
		
		right.width = fShouldersLen*1.5f;
		right.height = fHeadToSpineMidLen;
	}
}


