using System;
using UnityEngine;

/// <summary>
/// 助手类
/// </summary>
public class Utils
{
	/// <summary>
	/// 产生出一个无效的vector3.
	/// </summary>
	/// <returns>The vector3.</returns>
	public static Vector3 InvaildVec3
	{
		get{ return new Vector3( Mathf.Infinity, Mathf.Infinity, Mathf.Infinity ); }
	}

	/// <summary>
	/// 判断一个Vector3是否为无效
	/// </summary>
	/// <returns><c>true</c> if is invaild vec3 the specified v; otherwise, <c>false</c>.</returns>
	/// <param name="v">测试的Vector3对象</param>
	public static bool IsInvaildVec3( Vector3 v )
	{
		if( v.x == Mathf.Infinity || v.x == Mathf.NegativeInfinity ||
		    v.y == Mathf.Infinity || v.y == Mathf.NegativeInfinity ||
		    v.z == Mathf.Infinity || v.z == Mathf.NegativeInfinity )
			return true;
		else
			return false;
	}

	/// <summary>
	/// Gets the vector3 from joint.
	/// </summary>
	/// <returns>The vector3 from joint.</returns>
	/// <param name="joint">Joint.</param>
	public static Vector3 GetVector3FromJoint( Windows.Kinect.Joint joint )
	{
		return new Vector3( joint.Position.X, joint.Position.Y, joint.Position.Z );
	}
	
	/// <summary>
	/// Gets the vector3 from CameraSpacePoint.
	/// </summary>
	/// <returns>The vector3 from point.</returns>
	/// <param name="joint">Joint.</param>
	public static Vector3 GetVector3FromCameraSpacePoint( Windows.Kinect.CameraSpacePoint point )
	{
		return new Vector3( point.X, point.Y, point.Z );
	}

	/// <summary>
	/// 产生出一个无效的vector4.
	/// </summary>
	/// <returns>The vector4.</returns>
	public static Vector4 InvaildVec4
	{
		get{ return new Vector4( Mathf.Infinity, Mathf.Infinity, Mathf.Infinity, Mathf.Infinity ); }
	}
	
	/// <summary>
	/// 判断一个Vector4是否为无效
	/// </summary>
	/// <returns><c>true</c> if is invaild vec4 the specified v; otherwise, <c>false</c>.</returns>
	/// <param name="v">测试的Vector4对象</param>
	public static bool IsInvaildVec4( Vector4 v )
	{
		if( v.x == Mathf.Infinity || v.x == Mathf.NegativeInfinity ||
		    v.y == Mathf.Infinity || v.y == Mathf.NegativeInfinity ||
		    v.z == Mathf.Infinity || v.z == Mathf.NegativeInfinity ||
		    v.w == Mathf.Infinity || v.w == Mathf.NegativeInfinity )
			return true;
		else
			return false;
	}

	/// <summary>
	/// Gets the vector4 from Kinect.Vector4.
	/// </summary>
	/// <returns>The vector4 from Kinect.Vector4.</returns>
	/// <param name="vec">Vec</param>
	public static Vector4 GetVector4FromKinectVector4( Windows.Kinect.Vector4 vec )
	{
		return new Vector4( vec.X, vec.Y, vec.Z, vec.W );
	}
}
