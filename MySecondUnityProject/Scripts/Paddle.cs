using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Kinect=Windows.Kinect;

public class Paddle : MonoBehaviour {

    public float moveSpeed = 20;
	public float x = 0.0f;
	public float moveInput_x = 0.0f;
	public float offset = 4f;
	//控制板条移动范围.
	float max = 19.0f;
	float min = 1.5f;

	public Material BoneMaterial;
	public GameObject BodySourceManager;
	
	private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
	private BodySourceManager _BodyManager;


	void Update () {
		moveInput_x = 0.0f;
		//通过键盘操作游戏.
		//获取水平方向,得到移动距离.
		moveInput_x = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
		//float moveInput_y = Input.GetAxis ("Vertical") * Time.deltaTime * moveSpeed;
		transform.position += new Vector3(moveInput_x, 0, 0);
		if (transform.position.x <= min || transform.position.x >= max)
		{
		  float xPosition = Mathf.Clamp(transform.position.x, min, max); //板条移动的x坐标范围在-max和max之间.
		transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
		}

		if (BodySourceManager == null)
		{
			return;
		}
		//得到类BodySourceManager.
		_BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
		if (_BodyManager == null)
		{
			return;
		}
		//得到实时的骨骼数据.
		Kinect.Body[] data = _BodyManager.GetData();
		if (data == null)
		{
			return;
		}
		
		List<ulong> trackedIds = new List<ulong>();
		foreach(var body in data)
		{
			if (body == null)
			{
				continue;
			}
			
			if(body.IsTracked)
			{
				trackedIds.Add (body.TrackingId);
				//print (body.TrackingId);
			}
		}
		
		List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
		
		// First delete untracked bodies
		foreach(ulong trackingId in knownIds)
		{
			if(!trackedIds.Contains(trackingId))
			{
				Destroy(_Bodies[trackingId]);
				_Bodies.Remove(trackingId);
			}
		}
		
		foreach(var body in data)
		{
			if (body == null)
			{
				continue;
			}
			
			if(body.IsTracked)
			{

				if(!_Bodies.ContainsKey(body.TrackingId))
				{
					_Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
				}

				Kinect.Joint sourceJoint2 = body.Joints[Kinect.JointType.HandRight];

				Transform jointObj2 = _Bodies[body.TrackingId].transform;

				jointObj2.localPosition = GetVector3FromJoint(sourceJoint2);
	
				//print(jointObj2.localPosition.x);
				//print(x);
				if(jointObj2.localPosition.x-x-offset>jointObj2.localPosition.x){
					moveInput_x = -0.5f * Time.deltaTime * moveSpeed;
					
					transform.position += new Vector3(moveInput_x, 0, 0);
				}
				if(jointObj2.localPosition.x-x+offset<jointObj2.localPosition.x){
					moveInput_x = 0.5f * Time.deltaTime * moveSpeed;

					transform.position += new Vector3(moveInput_x, 0, 0);

				}
				if(jointObj2.localPosition.x-x==jointObj2.localPosition.x){
					moveInput_x = 0;
			
					transform.position += new Vector3(moveInput_x, 0, 0);
				}


				//控制板条移动范围.
				//float max = 19.0f;
				//float min = 1.5f;
				if (transform.position.x <= min || transform.position.x >= max)
				{
				  float xPosition = Mathf.Clamp(transform.position.x, min, max); //板条移动的x坐标范围在-max和max之间.
				transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
				}

				x = jointObj2.localPosition.x;
				moveInput_x = 0.0f;
			}
			moveInput_x = 0.0f;
		}
	}


	/// <summary>
	/// 增加小球碰撞后的水平速度,否则小球左右反弹的效果不理想。当珠球退出碰撞时调用该方法
	/// </summary>
	/// <param name="collisionInfo">Collision info.碰撞事件</param>
	void OnCollisionExit(Collision collisionInfo ) {
		Rigidbody rigid = collisionInfo.rigidbody;//得到我们碰撞的刚体.
		float xDistance = rigid.position.x - transform.position.x;//碰撞的珠球与板条的水平距离，落到板条中间时，水平速度保持不变.
		rigid.velocity = new Vector3(rigid.velocity.x + xDistance, rigid.velocity.y, rigid.velocity.z);//刚体碰撞后的速度.
	}

	private GameObject CreateBodyObject(ulong id)
	{
		    GameObject body = new GameObject("body:"+id);
		
		    Kinect.JointType jt = Kinect.JointType.HandRight;
		
			GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			
			LineRenderer lr = jointObj.AddComponent<LineRenderer>();
			lr.SetVertexCount(2);
			lr.material = BoneMaterial;
			lr.SetWidth(0.05f, 0.05f);
			
			jointObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			jointObj.name = jt.ToString();
			jointObj.transform.parent = body.transform;

		
		return body;
	}
	




	
	private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
	{
		
		return new Vector3(joint.Position.X * 20, joint.Position.Y * 20, joint.Position.Z * 20);
	}


}
