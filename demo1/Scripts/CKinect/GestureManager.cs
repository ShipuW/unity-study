using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using System.IO;

namespace CKinect
{
/// <summary>
/// Kinect手势检测器
/// </summary>
    public class GestureManager : MonoBehaviour
    {
        #region static instance
        public static GestureManager Instance { get { return s_Instance; } }

        private static GestureManager s_Instance = null;
        #endregion
    
        #region public member variables
        // VGB数据库文件名称
        public string m_VGBDatabaseFilename = "VGBDatabase.gbd";
        // 显示当前加载的VGB数据库里的手势名称列表
        public string[] m_GestureNames;

        // 手势检测事件
        public event EventHandler<GestureEventArgs> OnGesture;
        #endregion
    
        #region private member variables
        // 用于判断是否初始化成功
        private bool m_bInit = false;
        // kinect设备对象
        private KinectSensor m_Sensor = null;
        //
        private List<GestureDetector> m_GestureDetectorList = new List<GestureDetector>();
        #endregion

        #region public member methods
        /// <summary>
        /// 设置需要检测手势的身体标识符
        /// </summary>
        /// <param name="id">身体标识符</param>
        public void SetBodyByIndex(ulong trackingId, int index)
        {
            // 是否初始化
            if (!m_bInit)
                return;

            //
            if (index < m_GestureDetectorList.Count && m_GestureDetectorList [index].TrackingId != trackingId)
            {
                m_GestureDetectorList [index].SetBody(trackingId);

                Debug.Log("index:" + index + "  set body:" + trackingId);
            }

        }
        /// <summary>
        /// 根据手势的名称来添加需要检测的手势.
        /// </summary>
        /// <param name="gestureName">Gesture name.</param>
        public void AddGestureByName(string gestureName)
        {
            // 是否初始化
            if (!m_bInit)
                return;
        
            // 遍历查询手势对象
            foreach (var detector in m_GestureDetectorList)
            {
                detector.AddGestureByName(gestureName);
            }
        }
    
        /// <summary>
        /// 根据手势的名称来移除不需要检测的手势.
        /// </summary>
        /// <param name="gestureName">Gesture name.</param>
        public void RemoveGestureByName(string gestureName)
        {
            // 是否初始化
            if (!m_bInit)
                return;
        
            // 遍历查询手势对象
            foreach (var detector in m_GestureDetectorList)
            {
                detector.RemoveGestureByName(gestureName);
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
            if (string.IsNullOrEmpty(m_VGBDatabaseFilename))
            {
                Debug.LogError("The VGBDatabaseFilename is null or empty.");
                return;
            }

            // 获得kinect设备对象
            m_Sensor = KinectSensor.GetDefault();

            //
            if (m_Sensor == null)
            {
                Debug.LogError("There is not Kinect Device.");
                return;
            }

            int nBodyCount = m_Sensor.BodyFrameSource.BodyCount;

            for (int i = 0; i < nBodyCount; ++i)
            {
                GestureDetector detector = new GestureDetector(m_Sensor, m_VGBDatabaseFilename);
                detector.OnGesture += this.OnGestureInLocal;
                m_GestureDetectorList.Add(detector);
            }

            // 加载手势特征库
            var databasePath = Path.Combine(Application.streamingAssetsPath, m_VGBDatabaseFilename);
            using (VisualGestureBuilderDatabase database = VisualGestureBuilderDatabase.Create( databasePath ))
            {
                List<string> names = new List<string>();
                foreach (Gesture gesture in database.AvailableGestures)
                {
                    names.Add(gesture.Name);
                }
                m_GestureNames = names.ToArray();
            }

            //
            m_bInit = true;
        }
        //
        private void OnDestroy()
        {
            //
            foreach (var detector in m_GestureDetectorList)
            {
                detector.OnGesture -= this.OnGestureInLocal;
                detector.Dispose();
            }
            //
            m_GestureDetectorList.Clear();
        }

        //
        private void OnApplicationQuit()
        {
            //
            if (m_Sensor != null)
            {
                if (m_Sensor.IsOpen)
                {
                    m_Sensor.Close();
                }
            
                m_Sensor = null;
            }

            //
            foreach (var detector in m_GestureDetectorList)
            {
                detector.OnGesture -= this.OnGestureInLocal;
                detector.Dispose();
            }
            //
            m_GestureDetectorList.Clear();
        }
    
        // Update is called once per frame
        private void Update()
        {
            // 是否初始化
            if (!m_bInit)
                return;
    
            //
        }
        #endregion

        #region private member functions
        //
        private void OnGestureInLocal(object sender, GestureEventArgs e)
        {
            GestureDetector detector = sender as GestureDetector;

            if (detector == null)
                return;

            // Fire Event
            if (this.OnGesture != null)
            {
                GestureEventArgs _e = new GestureEventArgs(e.GestureName, e.IsBodyTrackingIdValid, e.IsGestureDetected, e.DetectionConfidence, detector);
                this.OnGesture(this, _e);
            }
        }
        #endregion

    }
}
