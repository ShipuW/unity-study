using UnityEngine;
using System.Collections;
using Windows.Kinect;

namespace InHouseScene
{
    public class MoveableObject : MonoBehaviour
    {
        #region public member for unity
        public float m_TranlateSpeed;

		//
		public Vector2 m_RotateSpeed = new Vector2( 0.1f, 0.1f );
		//
		public float m_ZoomSpeed = 0.1f;

		public Vector3 m_MaxPostion;

		public Vector3 m_MinPostion;
        #endregion

        #region public property

        public float TranlateSpeed { get { return m_TranlateSpeed; } set { m_TranlateSpeed = value; } }
		public Vector2 RotateSpeed {get {return m_RotateSpeed;} set {m_RotateSpeed = value;}}
		public float ZoomSpeed {get {return m_ZoomSpeed;} set {m_ZoomSpeed = value;}}
		public Vector3 MaxPostion {get {return m_MaxPostion;} set {m_MaxPostion = value;}}
		public Vector3 MinPostion {get {return m_MinPostion;} set {m_MinPostion = value;}}
        #endregion

        #region private member
        private bool bInited = false;
        private Vector3 preRightHandPos = Utils.InvaildVec3;
        private float preHandDepth = 0f;
        private bool bCanMove = true;

		private Vector3 tempLocalPostion;

        #endregion

        #region private const member
        private const int leftHandIndex = (int)JointType.HandLeft;
        private const int rightHandIndex = (int)JointType.HandRight;
        private const int leftElbowIndex = (int)JointType.ElbowLeft;
        private const int rightElbowIndex = (int)JointType.ElbowRight;

		private Vector3 m_vLastCursorPos = Utils.InvaildVec3;
		private Vector3 m_vLastHandRawPos = Utils.InvaildVec3;
		
		//
		private Vector3 m_vLastMousePos = Vector3.zero;

		private Rect m_Bounds;

		private Vector3 mVectorLeft,mVectorForward,mVectorUp;

        #endregion

        public bool CanMove { get { return bCanMove; } set { bCanMove = value; } }

        #region private function for unity

        // Use this for initialization
        void Start()
        {
            bInited = true;

			m_Bounds = new Rect();
			m_Bounds.xMin = m_MinPostion.x;
			m_Bounds.xMax = m_MaxPostion.x;
			m_Bounds.yMin = m_MinPostion.y;
			m_Bounds.yMax = m_MaxPostion.y;

			tempLocalPostion = new Vector3(this.transform.localPosition.x,this.transform.localPosition.y,this.transform.localPosition.z);

			initVectorLeftForwardAndUp(ref mVectorLeft,ref mVectorForward,ref mVectorUp);
        }
    
        // Update is called once per frame
        void Update()
        {
        }

        #endregion

        #region public function

        public void MoveLeft()
        {
			//this.transform.Translate(mVectorLeft *  m_TranlateSpeed * Time.deltaTime, Space.World);
			//updataPostionInBound(this.gameObject);

			Left(Vector3.left * m_TranlateSpeed );
        }

        public void MoveRight()
        {
			//this.transform.Translate(mVectorLeft * -m_TranlateSpeed* Time.deltaTime, Space.World);
			//updataPostionInBound(this.gameObject);
			Right(Vector3.left * m_TranlateSpeed );
        }

        public void MoveUp()
        {
			//this.transform.Translate(mVectorUp * m_TranlateSpeed* Time.deltaTime, Space.World);
			//updataPostionInBound(this.gameObject);
			Up(Vector3.up);
        }

        public void MoveDown()
        {
			//this.transform.Translate(mVectorUp * -m_TranlateSpeed* Time.deltaTime,Space.World);
			//updataPostionInBound(this.gameObject);
			Down(Vector3.up);
        }

        public void MoveBack()
        {
			//this.transform.Translate(mVectorForward * m_TranlateSpeed* Time.deltaTime, Space.World);
			//updataPostionInBound(this.gameObject);
			Back(Vector3.back * m_TranlateSpeed );
        }

        public void MoveForward()
        {
			//this.transform.Translate(mVectorForward * -m_TranlateSpeed* Time.deltaTime, Space.World);
			//updataPostionInBound(this.gameObject);
			Forward(Vector3.back * m_TranlateSpeed );
        }

		public void MoveGameObject()
		{
			MoveByKinect(this.gameObject);
			
			MoveByMouse(this.gameObject);
		}
		
		private void MoveByKinect(GameObject go)
		{
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
			
			if( fLen > 0.005 && dirZ > dirX && dirZ > dirY )
			{

				if( fDist < 0 )
				{
					Back(Vector3.back);
				}
				else if( fDist >0 )
				{
					Forward(Vector3.back);
				}
			}
			else
			{
				//
				if( Utils.IsInvaildVec3( m_vLastCursorPos ) )
					m_vLastCursorPos = cursor.GetCurrentCursorPos();
				
				Vector3 vec = cursor.GetCurrentCursorPos() - m_vLastCursorPos;
				
				m_vLastCursorPos = cursor.GetCurrentCursorPos();

				Left(vec * 0.1f);
			}
		}
		
		private void MoveByMouse(GameObject go)
		{
			
			//
			if( Input.GetMouseButton(0) && m_vLastMousePos != Vector3.zero)
			{
				Vector3 vec = Input.mousePosition - m_vLastMousePos;
				
				if( vec.sqrMagnitude > 1 )
				{
					Left(vec *0.1f);
				}
			}
			
			//
			if( Input.GetAxis("Mouse ScrollWheel") < 0 )
			{
				//Debug.Log("Mouse ScrollWheel < 0");

				Forward(Vector3.back);

			}
			else if( Input.GetAxis("Mouse ScrollWheel") > 0 )
			{
				//Debug.Log("Mouse ScrollWheel > 0");
				Back(Vector3.back);
			}
			
			//
			m_vLastMousePos = Input.mousePosition;
		}
		
		
		public void EndMove()
		{
			m_vLastMousePos = Vector3.zero;
			
			m_vLastCursorPos = Utils.InvaildVec3;
			m_vLastHandRawPos = Utils.InvaildVec3;
		}
        #endregion

		#region private function
		private void updataPostionInBound( GameObject go)
		{
			if(m_MaxPostion.y != m_MinPostion.y)
			{
				if(go.transform.localPosition.y > m_MaxPostion.y)
				{
					go.transform.localPosition = new Vector3(go.transform.localPosition.x,m_MaxPostion.y,go.transform.localPosition.z);
				}
				
				if( go.transform.localPosition.y < m_MinPostion.y)
				{
					go.transform.localPosition = new Vector3(go.transform.localPosition.x,m_MinPostion.y,go.transform.localPosition.z);
				}
			}
			else
			{
				go.transform.localPosition = new Vector3(go.transform.localPosition.x,this.tempLocalPostion.y,go.transform.localPosition.z);
			}

			if(m_MaxPostion.x != m_MinPostion.x)
			{
				if(go.transform.localPosition.x > m_MaxPostion.x)
				{
					go.transform.localPosition = new Vector3(m_MaxPostion.x,go.transform.localPosition.y,go.transform.localPosition.z);
				}
				
				if(go.transform.localPosition.x < m_MinPostion.x)
				{
					go.transform.localPosition = new Vector3(m_MinPostion.x,go.transform.localPosition.y,go.transform.localPosition.z);
				}
			}else
			{
				go.transform.localPosition = new Vector3(this.tempLocalPostion.x,go.transform.localPosition.y,go.transform.localPosition.z);
			}

			if(m_MaxPostion.z != m_MinPostion.z)
			{
				if(go.transform.localPosition.z > m_MaxPostion.z)
				{
					go.transform.localPosition = new Vector3(go.transform.localPosition.x,go.transform.localPosition.y,m_MaxPostion.z);
				}

				if(go.transform.localPosition.z < m_MinPostion.z)
				{
					go.transform.localPosition = new Vector3(go.transform.localPosition.x,go.transform.localPosition.y,m_MaxPostion.z);
				}
			}else
			{
				go.transform.localPosition = new Vector3(go.transform.localPosition.x,go.transform.localPosition.y,this.tempLocalPostion.z);
			}

		}

		private bool checkPostionInBoundX(GameObject go,Vector3 dir)
		{
			return ((dir.x * mVectorLeft).x < 0 && go.transform.localPosition.x > m_MinPostion.x) || ((dir.x * mVectorLeft).x > 0 && go.transform.localPosition.x < m_MaxPostion.x);
		}

		private bool checkPostionInBoundY(GameObject go,Vector3 dir)
		{
			return (dir.y < 0 && go.transform.localPosition.y > m_MinPostion.y) || (dir.y > 0 && go.transform.localPosition.y < m_MaxPostion.y);
		}

		private bool checkPostionInBoundZ(GameObject go,Vector3 dir)
		{
			return (dir.z < 0 && go.transform.localPosition.z > m_MinPostion.z) || (dir.z > 0 && go.transform.localPosition.z < m_MaxPostion.z);
		}

		private void initVectorLeftForwardAndUp(ref Vector3 left,ref Vector3 forward,ref Vector3 up)
		{
			Vector3 camForward = Camera.main.transform.forward;
			
			Vector3 gravity = Physics.gravity;
			
			//left = Vector3.Cross(gravity,camForward).normalized;

			left = Vector3.Cross(camForward,gravity).normalized;

			forward = Vector3.Cross(left,gravity).normalized;

			up = -gravity.normalized;

		}


		private void Left(Vector3 des)
		{
			Vector3 goRight = transform.right;

			Vector3 translate = mVectorLeft * des.x * m_TranlateSpeed * Time.deltaTime;
			//translate = goRight.x <0?-translate:translate;
			//Debug.Log(" goRight:  " + goRight + " translate : " +translate);
			transform.Translate( translate,Space.World);
			
			updataPostionInBound(gameObject);
		}
		
		private void Right(Vector3 des)
		{
			Left(-1 * des);
		}
		
		private void Up(Vector3 des)
		{
			this.transform.Translate(mVectorUp * m_TranlateSpeed* Time.deltaTime * des.y, Space.World);
			updataPostionInBound(this.gameObject);
		}
		
		private void Down(Vector3 des)
		{
			Up(-1 * des);
		}
		
		private void Back(Vector3 des)
		{
			Vector3 goForward = transform.forward;
			
			Vector3 translate = mVectorForward * this.m_ZoomSpeed * Time.deltaTime * des.z;
			//translate = goForward.z <0?-translate:translate;
			
			transform.Translate(translate,Space.World);
			
			updataPostionInBound(gameObject);
		}
		
		private void Forward(Vector3 des)
		{
			Back(-1 * des);
		}

		#endregion

    }
}
