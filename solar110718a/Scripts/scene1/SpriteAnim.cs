using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpriteAnim : MonoBehaviour
{
	#region public member for unity
	
	// 序列帧前缀名
	public string m_CursorLoadingStateNamePrefix;
	// 序列帧开始与结束编号
	public int m_CursorLoadingStateIndexStart, m_CursorLoadingStateIndexEnd;
	
	public string m_IndexFormat = "D2";
	
	// 光标动画帧率
	public int m_CursorAnimFramePerSecond = 30;
	public bool m_IsPingPong = true;
	public bool m_IsLoop = true;
	public bool m_IsSnap = true;
	
	//
	public delegate void VoidDelegate();
	public VoidDelegate OnFinished = null;
	#endregion
	
	private bool mActive = true;
	private List<string> mSpriteNames = new List<string>();
	private float mDelta = 0f;
	private int mIndex = 0;
	
	private UISprite mSprite;
	// Use this for initialization
	void Start () 
	{
		
		mSprite = this.GetComponent<UISprite>();
		if(mSprite == null)
		{
			Debug.LogError("Can't find UISprite in gameobject " + this.gameObject.name);
			return;
		}
		
		RebuildSpriteList();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (mActive && mSpriteNames.Count > 1 && Application.isPlaying && m_CursorAnimFramePerSecond > 0)
		{
			mDelta += RealTime.deltaTime;
			float rate = 1f / m_CursorAnimFramePerSecond;
			
			if (rate < mDelta)
			{
				mDelta = (rate > 0f) ? mDelta - rate : 0f;
				
				if (++mIndex >= mSpriteNames.Count)
				{
					mIndex = 0;
					mActive = m_IsLoop||m_IsPingPong;
					
					//
					if( !mActive && OnFinished != null )
						OnFinished();
				}
				
				if (mActive)
				{
					mSprite.spriteName = mSpriteNames[mIndex];
					if (m_IsSnap) mSprite.MakePixelPerfect();
				}
			}
		}
	}
	
	/// <summary>
	/// Reset the animation to the beginning.
	/// </summary>
	
	public void Play () { mActive = true; }
	
	/// <summary>
	/// Pause the animation.
	/// </summary>
	
	public void Pause () { mActive = false; }
	
	//
	public void ResetToBeginning ()
	{
		mActive = true;
		mIndex = 0;
		
		if (mSprite != null && mSpriteNames.Count > 0)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
			if (m_IsSnap) mSprite.MakePixelPerfect();
		}
	}
	
	public void RebuildSpriteList ()
	{
		if (mSprite == null) mSprite = GetComponent<UISprite>();
		mSpriteNames.Clear();
		
		if (mSprite != null && mSprite.atlas != null)
		{
			
			for(int i = m_CursorLoadingStateIndexStart;i<= m_CursorLoadingStateIndexEnd;i++)
			{
				mSpriteNames.Add(m_CursorLoadingStateNamePrefix + i.ToString(m_IndexFormat));
			}
			
			if(m_IsPingPong)
			{
				
				for(int i = m_CursorLoadingStateIndexEnd;i>= m_CursorLoadingStateIndexStart;i--)
				{
					mSpriteNames.Add(m_CursorLoadingStateNamePrefix + i.ToString(m_IndexFormat));
				}
			}
		}
	}
	
}
