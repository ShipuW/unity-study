using UnityEngine;
using System.Collections;
using Windows.Kinect;

namespace OutHouse
{
    public class Main : MonoBehaviour
    {

        private static Main _instance = null;

    #region public member for unity

        public GameObject m_House;
        public float m_ZoomSpeed;
        public float m_RotateSpeed;
        public Camera m_MainCamera;
        public string m_ParentName;

        public string m_InHouseSceneName;

        // NGUI光标对象
        public UISprite m_SpriteHandLeft = null;
        public UISprite m_SpriteHandRight = null;

    #endregion 

    #region private member
        //private bool swipeLeft = false;
        //private bool swipeRight = false;
        //private bool inIdle = true;
        //private bool canSwipe = false;
        // private bool canZoom = false;
        private Quaternion targetRotation;

        //    private float handDir = 0;

        private Vector3 preRightHandPos = Utils.InvaildVec3;
        private float preHandDepth = 0f;
        private bool bDragInHouse = false;
    #endregion   

    #region private const member
        private const int leftHandIndex = (int)JointType.HandLeft;
        private const int rightHandIndex = (int)JointType.HandRight;
        private const int leftElbowIndex = (int)JointType.ElbowLeft;
        private const int rightElbowIndex = (int)JointType.ElbowRight;
        private const int leftShoulderIndex = (int)JointType.ShoulderLeft;
        private const int rightShoulderIndex = (int)JointType.ShoulderRight;
        private const int hipCenterIndex = (int)JointType.SpineBase;
        private const int shoulderCenterIndex = (int)JointType.SpineShoulder;
        private const int leftHipIndex = (int)JointType.HipLeft;
        private const int rightHipIndex = (int)JointType.HipRight;

    #endregion  

    #region public function for unity
        public void Awake()
        {

            if (_instance == null)
            {
                _instance = this;
            }

        }

        // Use this for initialization
        void Start()
        {

            if (m_House == null)
            {
                Debug.LogError("house not initanced!");
                return;
            }

            if (string.IsNullOrEmpty(m_ParentName))
            {
                Debug.LogError("The 'ParentName' has not assigned.");
                return;
            }

            if (string.IsNullOrEmpty(m_InHouseSceneName))
            {
                Debug.LogError("The 'InHouseSceneName' has not assigned.");
                return;
            }

            //
//            if (KinectServices.Instance == null)
//            {
//                KinectServices.Create(m_SpriteHandLeft, m_SpriteHandRight);
//            } else if (KinectCursorController.Instance != null)
//            {
//                KinectCursorController.Instance.SpriteHandLeft = m_SpriteHandLeft;
//                KinectCursorController.Instance.SpriteHandRight = m_SpriteHandRight;
//            }

            this.Invoke("AddDragListenner", 2);

        }

        void Update()
        {
            CKinect.CursorController controller = CKinect.CursorController.Instance;
            Vector3 screenPixelPos = Vector3.zero;
            Vector3 screenNormalPos = Vector3.zero;
            screenNormalPos = controller.GetCurrentCursorPos();
            screenPixelPos.x = (int)(screenNormalPos.x);
            screenPixelPos.y = (int)(screenNormalPos.y);
        
            CKinect.CursorController.Instance.IsPushActive = UICamera.Raycast(screenPixelPos);

       
        }

        public void OnDestroy()
        {
            DragManager dragManager = DragManager.Instace;

            if (dragManager != null && dragManager.IsInit())
            {

                dragManager.onDragGameObject -= this.onDragGameObject;
                dragManager.OnDropGameObject -= this.OnDropGameObject;
                dragManager.onReleaseGameobject -= this.onReleaseGameobject;
            }
        }

    #endregion

    #region click callback

        public void OnInHouseClick()
        {
            //Application.LoadLevel(m_ParentName);

            AsyncOperation ao = Application.LoadLevelAsync(m_InHouseSceneName);

            if(ao.isDone)
            {
                ao.allowSceneActivation = true;
            }

        }

        public void OnCloseClick()
        {
            Application.LoadLevel(m_ParentName);

        }

    #endregion 

    #region private function

        private void AddDragListenner()
        {

            DragManager dragManager = DragManager.Instace;
        
            if (dragManager != null && dragManager.IsInit())
            {
                Debug.Log("------------------------->AddDragListenner");
                dragManager.onDragGameObject += this.onDragGameObject;
                dragManager.OnDropGameObject += this.OnDropGameObject;
                dragManager.onReleaseGameobject += this.onReleaseGameobject;
            }
        }

        private void zoomHouse(float zoom)
        {
            //if (this.canZoom)
            // {
            Debug.Log("------------------------->" + zoom);
            Camera.main.transform.localPosition += new Vector3(0, 0, zoom);
            // }
        }

        private void onDragGameObject(GameObject go, Vector3 screenPos)
        {
            if (go == m_House)
            {
                bDragInHouse = true;

                //InteractionManager interactionManager = InteractionManager.Instance;
                //
                //if (interactionManager == null && !interactionManager.IsInteractionInited ())
                //{
                //  return;
                //}
                //
                //ulong pId = interactionManager.GetUserID ();
                //preRightHandPos = interactionManager.GetCursorPosition ();
                //
                //if (interactionManager.GetJointTrackingState (pId, rightHandIndex) != TrackingState.NotTracked)
                //{
                //  preHandDepth = interactionManager.GetJointPosition (pId, rightHandIndex).z;
                //}

                UserManager userManager = UserManager.Instance;

                if (userManager == null)
                {
                    return;
                }

                CameraSpacePoint[] filteredJoints = userManager.JointFilter.GetFilteredJoints();
                preHandDepth = filteredJoints [rightHandIndex].Z;
            }

        }

        private void onReleaseGameobject(GameObject go)
        {
            if (go == m_House)
            {
                bDragInHouse = false;
            }

            preHandDepth = 0f;
        
            preRightHandPos = Utils.InvaildVec3;
        }

        private void OnDropGameObject(GameObject go, Vector3 screenPos)
        {
            //Debug.Log ("------------------------->Rotate");
            if (go == m_House && bDragInHouse)
            {
                //InteractionManager interactionManager = InteractionManager.Instance;
                //
                //if (interactionManager == null && !interactionManager.IsInteractionInited ())
                //{
                //  return;
                //}
                UserManager userManager = UserManager.Instance;
            
                if (userManager == null)
                {
                    return;
                }

                Debug.Log("------------------------->1");
                CameraSpacePoint[] filteredJoints = userManager.JointFilter.GetFilteredJoints();

                Vector3 rightHandPos = Utils.GetVector3FromCameraSpacePoint(filteredJoints [rightHandIndex]);
                Vector3 rightElbowPos = Utils.GetVector3FromCameraSpacePoint(filteredJoints [rightElbowIndex]);

                CKinect.CursorController controller = CKinect.CursorController.Instance;
            
                if (controller == null)
                {
                    return;
                }

                Debug.Log("----------2 -- > " + controller.HasHandClosed + "    ---   " + !Utils.IsInvaildVec3(preRightHandPos));
                if (controller.HasHandClosed && !Utils.IsInvaildVec3(preRightHandPos))
                {
                    Debug.Log("------------------------->3");
                    if (
                    Mathf.Abs(rightHandPos.y - rightElbowPos.y) < 0.15f &&
                        Mathf.Abs(rightHandPos.x - rightElbowPos.x) < 0.15f)
                    {

                        m_MainCamera.transform.Translate(0, 0, (rightHandPos.z - preHandDepth) * m_ZoomSpeed * Time.deltaTime);
                    }

                    m_House.transform.Rotate(Vector3.up, m_RotateSpeed * (preRightHandPos.x - controller.GetHandRightRawPos().x));

                    //m_MainCamera.transform.Translate (0, (preRightHandPos.y - controller.GetHandRightRawPos ().y) * m_ZoomSpeed * Time.deltaTime,0);
                    preHandDepth = rightHandPos.z;

                }

                preRightHandPos = controller.GetHandRightRawPos();
                Debug.Log("-------------4------------" + preRightHandPos);

                //ulong pId = interactionManager.GetUserID ();
                //Debug.Log("------------------------->Rotate" + pId);
                /*
            if (pId > 0)
            {

                if (interactionManager.GetJointTrackingState (pId, rightHandIndex) != TrackingState.NotTracked &&
                    interactionManager.GetJointTrackingState (pId, rightElbowIndex) != TrackingState.NotTracked)
                {
                    Vector3 rightHandPos = interactionManager.GetJointPosition (pId, rightHandIndex);
                    Vector3 rightElbowPos = interactionManager.GetJointPosition (pId, rightElbowIndex);
               
                    if (interactionManager.GetRightHandState (pId) == HandState.Closed && preRightHandPos != Vector3.zero)
                    {
               
                        if (
                            Mathf.Abs (rightHandPos.y - rightElbowPos.y) < 0.15f &&
                            Mathf.Abs (rightHandPos.x - rightElbowPos.x) < 0.15f)
                        {
                            m_MainCamera.transform.Translate (0, 0, (rightHandPos.z - preHandDepth) * m_ZoomSpeed);
                        }
               
                        // m_House.transform.RotateAroundLocal(Vector3.up, (preRightHandPos.x - rightHandPos.x));
                        m_House.transform.RotateAroundLocal (Vector3.up, m_RotateSpeed * (preRightHandPos.x - interactionManager.GetCursorPosition ().x));
                        // m_House.transform.RotateAroundLocal(Vector3.right, (playerJointsPos[rightHandIndex].y - preRightHandPos.y));
               
               
                        preHandDepth = rightHandPos.z;
                    }
               
                    // preRightHandPos = rightHandPos;
                }
                //else
                //{
                //    preRightHandPos = Vector3.zero;
                //}

                //m_House.transform.RotateAroundLocal(Vector3.up, m_RotateSpeed*(preRightHandPos.x - interactionManager.GetCursorPosition().x) );



                preRightHandPos = interactionManager.GetCursorPosition ();
                //Debug.Log("------------------------->Rotate");
            }
            */
            }

        }
    #endregion
    }
}

