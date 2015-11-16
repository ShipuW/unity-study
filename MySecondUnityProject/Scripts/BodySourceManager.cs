using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class BodySourceManager : MonoBehaviour 
{
    private KinectSensor _Sensor;
    private BodyFrameReader _Reader;
    private Body[] _Data = null;//定义骨骼数据. 
    
    public Body[] GetData()
    {
        return _Data;
    }
    
	//开启kinect.
    void Start () 
    {
		//实例化KinectSensor类.
        _Sensor = KinectSensor.GetDefault();

        if (_Sensor != null)
        {
			//通过资源池,打开输出流.
            _Reader = _Sensor.BodyFrameSource.OpenReader();
            
            if (!_Sensor.IsOpen)
            {
				//开启kinect.
                _Sensor.Open();
            }
        }   
    }
    
    void Update () 
    {
        if (_Reader != null)
        {
			//得到最新的帧数据.
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                if (_Data == null)
                {
					//得到骨骼数据.
                    _Data = new Body[_Sensor.BodyFrameSource.BodyCount];
                }
                //将骨骼数据传送到帧中.
                frame.GetAndRefreshBodyData(_Data);
                
                frame.Dispose();
                frame = null;
            }
        }    
    }
    //关闭输出流、关闭kinect.
    void OnApplicationQuit()
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
}
