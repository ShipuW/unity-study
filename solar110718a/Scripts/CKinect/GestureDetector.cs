using UnityEngine;
using System.Collections;
using Microsoft.Kinect.VisualGestureBuilder;
using Windows.Kinect;
using System;
using System.Collections.Generic;
using System.IO;

namespace CKinect
{
	public class GestureDetector : IDisposable
	{
		#region public member variables
		// 手势检测事件
		public event EventHandler<GestureEventArgs> OnGesture;
		#endregion
		
		#region private member variables
		// 用于判断是否初始化成功
		private bool m_bInit = false;
		// VGB帧数据源
		private VisualGestureBuilderFrameSource m_VGBSource = null;
		// VGB帧读取器
		private VisualGestureBuilderFrameReader m_VGBReader = null;
		// VGB数据库
		private VisualGestureBuilderDatabase m_VGBDatabase = null;
		// VGB数据库手势集合缓存
		private IList<Gesture> m_VGBGestures = null;
		// kinect设备对象
		//private KinectSensor m_Sensor = null;
		
		#endregion
		
		#region public properties
		/// <summary>
		/// Gets or sets the body tracking ID associated with the current detector
		/// The tracking ID can change whenever a body comes in/out of scope
		/// </summary>
		public ulong TrackingId
		{
			get
			{
				return m_VGBSource.TrackingId;
			}
			
			set
			{
				if (m_VGBSource.TrackingId != value)
				{
					m_VGBSource.TrackingId = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether or not the detector is currently paused
		/// If the body tracking ID associated with the detector is not valid, then the detector should be paused
		/// </summary>
		public bool IsPaused
		{
			get
			{
				return m_VGBReader.IsPaused;
			}
			
			set
			{
				if (m_VGBReader.IsPaused != value)
				{
					m_VGBReader.IsPaused = value;
				}
			}
		}
		#endregion
		
		#region public member methods
		//
		public GestureDetector(KinectSensor sensor, string databaseFilename)
		{
			// check
			if (sensor == null)
			{
				Debug.LogError("There is not Kinect Device.");
				return;
			}
			
			// 创建VGB帧数据源
			m_VGBSource = VisualGestureBuilderFrameSource.Create(sensor, 0);
			m_VGBSource.TrackingIdLost += this.Source_TrackingIdLost;
			
			// 获得VGB帧读取器
			m_VGBReader = m_VGBSource.OpenReader();
			
			// check
			if (m_VGBReader != null)
			{
				m_VGBReader.IsPaused = true;
				m_VGBReader.FrameArrived += GestureFrameArrived;
				//Debug.Log("VGBFrameReader is paused");
			} else
			{
				Debug.LogError("Can not get VGBReader.");
				return;
			}
			
			// 加载手势特征库
			var databasePath = Path.Combine(Application.streamingAssetsPath, databaseFilename);
			m_VGBDatabase = VisualGestureBuilderDatabase.Create(databasePath);
			
			if (m_VGBDatabase != null)
			{
				m_VGBGestures = m_VGBDatabase.AvailableGestures;
			} else
			{
				Debug.LogError("Can not load VGBDatabase.");
				return;
			}
			
			//
			m_bInit = true;
		}
		
		/// <summary>
		/// 设置需要检测手势的身体标识符
		/// </summary>
		/// <param name="id">身体标识符</param>
		public void SetBody(ulong id)
		{
			// 是否初始化
			if (!m_bInit)
				return;
			
			if (id > 0)
			{
				m_VGBSource.TrackingId = id;
				m_VGBReader.IsPaused = false;
			} else
			{
				m_VGBSource.TrackingId = 0;
				m_VGBReader.IsPaused = true;
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
			foreach (Gesture gesture in m_VGBGestures)
			{
				if (gesture == null)
				{
					Debug.Log("gesture == null");
					continue;
				}
				
				if (gesture.Name.Equals(gestureName))
				{
					m_VGBSource.AddGesture(gesture);
					Debug.Log("gesture:'" + gesture.Name + "' has added.");
				}
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
			foreach (Gesture gesture in m_VGBSource.Gestures)
			{
				if (gesture.Name.Equals(gestureName))
				{
					m_VGBSource.RemoveGesture(gesture);
					Debug.Log("gesture:'" + gesture.Name + "' has removed.");
				}
			}
		}
		
		
		#endregion
		
		#region implement Dispose interface
		/// <summary>
		/// Disposes all unmanaged resources for the class
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		/// <summary>
		/// Disposes the VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader objects
		/// </summary>
		/// <param name="disposing">True if Dispose was called directly, false if the GC handles the disposing</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//
				m_VGBGestures = null;
				
				//
				if (m_VGBReader != null)
				{
					m_VGBReader.FrameArrived -= GestureFrameArrived;
					m_VGBReader.Dispose();
					m_VGBReader = null;
				}
				
				//
				if (m_VGBSource != null)
				{
					m_VGBSource.TrackingIdLost -= Source_TrackingIdLost;
					m_VGBSource.Dispose();
					m_VGBSource = null;
				}
				
				//
				if (m_VGBDatabase != null)
				{
					m_VGBDatabase.Dispose();
					m_VGBDatabase = null;
				}
			}
		}
		#endregion
		
		#region private member functions
		/// <summary>
		/// 处理从kinect设备发送来的手势检测结果，该结果是对应追踪已标识身体的结果。
		/// </summary>
		/// <param name="sender">发送事件的对象</param>
		/// <param name="e">事件数据集</param>
		private void GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
		{
			VisualGestureBuilderFrameReference frameReference = e.FrameReference;
			using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
			{
				if (frame != null)
				{
					// get the discrete gesture results which arrived with the latest frame
					IDictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;
					
					if (discreteResults != null)
					{
						foreach (Gesture gesture in m_VGBSource.Gestures)
						{
							if (gesture.GestureType == GestureType.Discrete)
							{
								DiscreteGestureResult result = null;
								//
								if (discreteResults.TryGetValue(gesture, out result))
								{
									// Fire Event
									if (this.OnGesture != null)
									{
										this.OnGesture(this, new GestureEventArgs(gesture.Name, true, result.Detected, result.Confidence));
									}
								}
							}
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Handles the TrackingIdLost event for the VisualGestureBuilderSource object
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
		{
			// update the GestureResultView object to show the 'Not Tracked' image in the UI
			//this.GestureResultView.UpdateGestureResult(false, false, 0.0f);
			Debug.Log("TrackingIdLost:" + this.TrackingId);
		}
		#endregion
	}
}

