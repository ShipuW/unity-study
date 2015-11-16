using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Runtime.InteropServices;
using System;

public class BodyIndexManager : MonoBehaviour
{

    private KinectSensor _Sensor;
    private BodyIndexFrameReader _Reader;

    /// <summary>
    /// Size of the RGB pixel in the bitmap
    /// </summary>
    private const int BytesPerPixel = 4;

    /// <summary>
    /// Description of the data contained in the body index frame
    /// </summary>
    private FrameDescription bodyIndexFrameDescription = null;


    private byte[] pBodyIndexBuffer;


    const int cDepthWidth = 512;
    const int cDepthHeight = 424;

    public void Awake()
    {
        pBodyIndexBuffer = new byte[cDepthWidth * cDepthHeight];
    }

    // Use this for initialization
    void Start()
    {
        _Sensor = KinectSensor.GetDefault();

        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyIndexFrameSource.OpenReader();


            bodyIndexFrameDescription = _Sensor.BodyIndexFrameSource.FrameDescription;

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }


    }

    // Update is called once per frame
    void Update()
    {

        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {

                var pBodyIndexData = GCHandle.Alloc(pBodyIndexBuffer, GCHandleType.Pinned);
                frame.CopyFrameDataToIntPtr(pBodyIndexData.AddrOfPinnedObject(), (uint)pBodyIndexBuffer.Length);
                pBodyIndexData.Free();

                frame.Dispose();
                frame = null;
            }
        }
    }

    public void OnApplicationQuit()
    {

        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }

    }


    public byte[] GetBodyIndexData() {

        return pBodyIndexBuffer;
    }

    public int GetBodyIndexBufferSize()
    {
        return bodyIndexFrameDescription.Width * bodyIndexFrameDescription.Height;
    }

}
