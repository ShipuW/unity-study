using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Runtime.InteropServices;

namespace CKinect
{
    public class MultiSourceManager : MonoBehaviour
    {
		#region static member
		private static MultiSourceManager s_Instance;

		public static MultiSourceManager Instance{get { return s_Instance; } }
		#endregion

        #region public properties
        /// <summary>
        /// The Kinect width of color data
        /// </summary>
        public int ColorWidth { get; private set; }
        /// <summary>
        /// The Kinect height of color data
        /// </summary>
        public int ColorHeight { get; private set; }

		/// <summary>
		/// The Kinect width of depth data
		/// </summary>
		public int DepthWidth { get; private set; }
		/// <summary>
		/// The Kinect height of depth data
		/// </summary>
		public int DepthHeight { get; private set; }

        /// <summary>
        /// The Kinect width of body index data.
        /// </summary>
        public int BodyIndexWidth { get; private set; }
        /// <summary>
        /// The Kinect height of body index data.
        /// </summary>
        public int BodyIndexHeight { get; private set; }
		/// <summary>
		/// Gets the floor clip plane of the body frame.
		/// </summary>
		/// <value>The floor clip plane.</value>
		public UnityEngine.Vector4 FloorClipPlane { get; private set; }
        #endregion 

        #region private member varibles
        /// <summary>
        /// The Kinect sensor.
        /// </summary>
        private KinectSensor m_Sensor;

        /// <summary>
        /// The Kinect coordinate mapper.
        /// </summary>
        private CoordinateMapper m_CoordinateMapper;

        /// <summary>
        /// The Kinect MultiSourceFrameReader.
        /// </summary>
        private MultiSourceFrameReader m_Reader;

        /// <summary>
        /// The Texture2D for unity.
        /// </summary>
        private Texture2D m_ColorTexture;

        /// <summary>
        /// The Kinect color data.
        /// </summary>
        private byte[] m_ColorData;

        /// <summary>
        /// Intermediate storage for the color to depth mapping
        /// </summary>
        private DepthSpacePoint[] m_ColorMappedToDepthPoints = null;

        /// <summary>
        /// The Kinect Depth data.
        /// </summary>
        private ushort[] m_DepthData = null;

        /// <summary>
        /// The Kinect depth coordinates.
        /// </summary>
        private DepthSpacePoint[] m_DepthCoordinates = null;

        /// <summary>
        /// The Kinect Body data.
        /// </summary>
        private Body[] m_BodyData = null;
                
        /// <summary>
        /// Intermediate storage for frame data converted to color
        /// </summary>
        private byte[] m_BodyIndexData = null;

        #endregion

        #region public member methods
        //
        public Texture2D GetColorTexture()
        {
            return m_ColorTexture;
        }
        
        //
        public Body[] GetBodyData()
        {
            return m_BodyData;
        }

        //
        public DepthSpacePoint[] GetDepthCoordinates()
        {
            return m_DepthCoordinates;
        }

        //
        public byte[] GetBodyIndexData()
        {   
            return m_BodyIndexData;
        }

        public CoordinateMapper GetCoordinateMapper()
        {
            return m_CoordinateMapper;
        }

        public ushort[] GetDepthData(){
        
            return m_DepthData;
        }

        #endregion

        #region private member functions for Unity

		private void  Awake()
		{
			s_Instance = this;

			//
			FloorClipPlane = Utils.InvaildVec4;
		}

        //
        private void Start()
        {
            m_Sensor = KinectSensor.GetDefault();
            
            if (m_Sensor != null)
            {
                //
                m_CoordinateMapper = m_Sensor.CoordinateMapper;

                //
                m_Reader = m_Sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Body | FrameSourceTypes.BodyIndex);
                
                var colorFrameDesc = m_Sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
                ColorWidth = colorFrameDesc.Width;
                ColorHeight = colorFrameDesc.Height;
                
                m_ColorTexture = new Texture2D(colorFrameDesc.Width, colorFrameDesc.Height, TextureFormat.RGBA32, false);
                m_ColorData = new byte[colorFrameDesc.BytesPerPixel * colorFrameDesc.LengthInPixels];

                m_ColorMappedToDepthPoints = new DepthSpacePoint[ColorWidth * ColorHeight];

                //
                var depthFrameDesc = m_Sensor.DepthFrameSource.FrameDescription;
				DepthWidth = depthFrameDesc.Width;
				DepthHeight = depthFrameDesc.Height;

                m_DepthData = new ushort[depthFrameDesc.Width * depthFrameDesc.Height];

                m_DepthCoordinates = new DepthSpacePoint[colorFrameDesc.Width * colorFrameDesc.Height];
                
                //
                m_BodyData = new Body[m_Sensor.BodyFrameSource.BodyCount];

                //
                var bodyIndexFrameDesc = m_Sensor.BodyIndexFrameSource.FrameDescription;
                BodyIndexWidth = bodyIndexFrameDesc.Width;
                BodyIndexHeight = bodyIndexFrameDesc.Height;
                m_BodyIndexData = new byte[ bodyIndexFrameDesc.Width * bodyIndexFrameDesc.Height ];
                
                //
                if (!m_Sensor.IsOpen)
                {
                    m_Sensor.Open();
                }
            }
        }

        //
        private void Update()
        {
            //
            if (m_Reader == null)
            {
                Debug.LogWarning("The 'MultiSourceFrameReader' is null.");
                return;
            }

            MultiSourceFrame multiSourceFrame = m_Reader.AcquireLatestFrame();

            // If the Frame has expired by the time we process this event, return.
            if (multiSourceFrame == null)
                return;

            //
            DepthFrame depthFrame = null;
            ColorFrame colorFrame = null;
            BodyIndexFrame bodyIndexFrame = null;
            BodyFrame bodyFrame = null;

            int depthWidth = 0;
            int depthHeight = 0;

            // We use a try/finally to ensure that we clean up before we exit the function.  
            // This includes calling Dispose on any Frame objects that we may have and unlocking the bitmap back buffer.
            try
            {                
                depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame();
                colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame();
                bodyIndexFrame = multiSourceFrame.BodyIndexFrameReference.AcquireFrame();
                bodyFrame = multiSourceFrame.BodyFrameReference.AcquireFrame();

                // check
                if (colorFrame == null || depthFrame == null || bodyIndexFrame == null || bodyFrame == null)
                    return;

                // Get Depth Frame Data.
                var pDepthData = GCHandle.Alloc(m_DepthData, GCHandleType.Pinned);
                depthFrame.CopyFrameDataToIntPtr(pDepthData.AddrOfPinnedObject(), (uint)m_DepthData.Length * sizeof(ushort));

                var pDepthCoordinatesData = GCHandle.Alloc(m_DepthCoordinates, GCHandleType.Pinned);
                // map
                m_CoordinateMapper.MapColorFrameToDepthSpaceUsingIntPtr(
                    pDepthData.AddrOfPinnedObject(), 
                    (uint)m_DepthData.Length * sizeof(ushort),
                    pDepthCoordinatesData.AddrOfPinnedObject(), 
                    (uint)m_DepthCoordinates.Length);
                
                pDepthCoordinatesData.Free();
                pDepthData.Free();

                // Get Color Frame Data
                var pColorData = GCHandle.Alloc(m_ColorData, GCHandleType.Pinned);
                colorFrame.CopyConvertedFrameDataToIntPtr(pColorData.AddrOfPinnedObject(), (uint)m_ColorData.Length, ColorImageFormat.Rgba);
                pColorData.Free();

				// Get BodyIndex Frame Data.
				var pBodyIndexData = GCHandle.Alloc(m_BodyIndexData, GCHandleType.Pinned);
				bodyIndexFrame.CopyFrameDataToIntPtr(pBodyIndexData.AddrOfPinnedObject(), (uint)m_BodyIndexData.Length);
				pBodyIndexData.Free();

                //
                bodyFrame.GetAndRefreshBodyData(m_BodyData);

				//
				FloorClipPlane = Utils.GetVector4FromKinectVector4( bodyFrame.FloorClipPlane );
            } 
			finally
            {
                if (bodyFrame != null)
                {
                    bodyFrame.Dispose();
                }
                
                if (depthFrame != null)
                {
                    depthFrame.Dispose();
                }
                
                if (colorFrame != null)
                {
                    colorFrame.Dispose();
                }
                
                if (bodyIndexFrame != null)
                {
                    bodyIndexFrame.Dispose();
                }

                multiSourceFrame = null;
            }

            //
            m_ColorTexture.LoadRawTextureData(m_ColorData);
            m_ColorTexture.Apply();
        }
        
        //
        private void OnApplicationQuit()
        {
            if (m_Reader != null)
            {
                m_Reader.Dispose();
                m_Reader = null;
            }
            
            if (m_Sensor != null)
            {
                if (m_Sensor.IsOpen)
                {
                    m_Sensor.Close();
                }
                
                m_Sensor = null;
            }
        }

        #endregion
    }
} // namespace CKinect