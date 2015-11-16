using UnityEngine;
using System.Collections;

namespace CKinect
{
    public class CursorLoading : MonoBehaviour
    {

    #region static Instance


        public static CursorLoading Instance { get { return s_Instance; } }
    
        private static CursorLoading s_Instance = null;

    #endregion

    #region public member for unity

        // 正在推(按)状态序列帧前缀名
        public string m_CursorLoadingStateNamePrefix;
        // 正在推(按)状态序列帧开始与结束编号
        public int m_CursorLoadingStateIndexStart, m_CursorLoadingStateIndexEnd;
        public UISprite m_CursorLoadingSprite;

        //显示loading动画的阈值
        public float m_ShowLoadingAnimThreshold = 1.0f;

        //cursor晃动阀值
        public int m_LoadingThreshold = 10;

        // 光标动画帧率
        public int m_CursorAnimFramePerSecond = 30;

        public delegate void OnLoadingStartDelegate();

        public OnLoadingStartDelegate OnLoadingStart;

        public delegate void OnLoadingDelegate(float progress);

        public OnLoadingDelegate OnLoading;

        public delegate void OnLoadingFinishingDelegate();

        public OnLoadingFinishingDelegate OnLoadingFinish;
    #endregion

    #region private member

        private bool m_bInit = false;
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

    #region public properties
        //
        public float ShowAnimThreshold{get {return m_ShowLoadingAnimThreshold;}}
    #endregion 

    #region private function for unity

        private void Awake()
        {
            s_Instance = this;
        }

        // Use this for initialization
        private void Start()
        {
    
            if (string.IsNullOrEmpty(m_CursorLoadingStateNamePrefix))
            {

                Debug.LogError("The 'CursorLoadingStateNamePrefix' has not assigln!");
                return;

            }

            if (m_CursorLoadingSprite == null)
            {

                Debug.LogError("The 'CursorLoadingSprite' has not assigle!");
                return;

            }

            CursorLoadingAnimFrameTime = 1.0f / m_CursorAnimFramePerSecond;

            m_bInit = true;

        }
    
        // Update is called once per frame
        private void Update()
        {
    
            if (UICamera.isOverUI)
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

                if (m_OverGameObject == null)
                {
                    m_OverGameObject = UICamera.hoveredObject;
                    m_OnOverUITime = Time.time;

                    m_CursorPosWhenOver = currentCursorPos;

                    //Debug.Log("hove gameobject name: " +m_OverGameObject.name);

                }

                if ((m_CursorPosWhenOver - currentCursorPos).magnitude > m_LoadingThreshold)
                {
                    ResetAnim();
                    return;
                }
                LoadingButton lb = m_OverGameObject.GetComponent<LoadingButton>();

                float threshold = (lb == null?m_ShowLoadingAnimThreshold:lb.Priority);

                if (Time.time - m_OnOverUITime > threshold && UICamera.hoveredObject == m_OverGameObject)
                {
                    m_IsPlayingLoadingAmin = true;

                    if (m_LoadingStartPos == Vector3.zero)
                    {
                        m_LoadingStartPos = currentCursorPos;
                    }

                    ProcessCursorLoadingAnims(m_LoadingStartPos);
                }
            } else
            {
                ResetAnim();
            }

        }



    #endregion

    #region private function

        /// <summary>
        /// 处理光标的加载动画
        /// </summary>
        /// <param name="cursorPos">加载动画的显示坐标</param>
        private void ProcessCursorLoadingAnims(Vector2 cursorPos)
        {
            if (Time.time - CursorLoadingAnimLastTime > CursorLoadingAnimFrameTime && m_IsPlayingLoadingAmin)
            {

                if (m_iCurrentLoadingFrameIndex <= 0 && OnLoadingStart != null)
                {
                    OnLoadingStart();
                }

                m_CursorLoadingSprite.gameObject.SetActive(true);
                m_CursorLoadingSprite.gameObject.transform.localPosition = cursorPos;
                m_CursorLoadingSprite.spriteName = GetSpriteNameByFrameIndex(m_CursorLoadingStateNamePrefix, m_CursorLoadingStateIndexStart, m_CursorLoadingStateIndexEnd, "D2", m_CursorLoadingStateIndexStart + m_iCurrentLoadingFrameIndex);

                this.m_iCurrentLoadingFrameIndex ++;
                //Debug.Log("-------------------------------->" + m_iCurrentFrameIndex + "   " + m_CursorLoadingSprite.spriteName);
                CursorLoadingAnimLastTime = Time.time;

                if (OnLoading != null)
                {
                    OnLoading(this.m_iCurrentLoadingFrameIndex / (m_CursorLoadingStateIndexEnd - m_CursorLoadingStateIndexStart) * 100);
                }


                if (m_iCurrentLoadingFrameIndex >= m_CursorLoadingStateIndexEnd - m_CursorLoadingStateIndexStart)
                {

                    //UICamera.Notify();

                    if (OnLoadingFinish != null)
                    {
                        OnLoadingFinish();
                    }

                    this.StartCoroutine(DelayResetAnim());
                }
            }
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
            if (string.IsNullOrEmpty(digitalFormat))
                return string.Empty;
        
            return namePrefix + Mathf.Clamp(nFrameIndex, nIndexStart, nIndexEnd).ToString(digitalFormat);
        }

        private IEnumerator DelayResetAnim()
        {
            // skip a frame
            yield return null;
            ResetAnim();

        }

        /// <summary>
        /// 重新设置动画
        /// </summary>
        private void ResetAnim()
        {
            this.m_iCurrentLoadingFrameIndex = 0;
            m_CursorLoadingSprite.gameObject.SetActive(false);
            m_IsPlayingLoadingAmin = false;
        
            m_OverGameObject = null;
            m_OnOverUITime = 0;

            m_LoadingStartPos = m_CursorPosWhenOver = Vector3.zero;
        }
     #endregion
    }
}
