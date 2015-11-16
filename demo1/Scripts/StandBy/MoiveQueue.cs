﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoiveQueue : MonoBehaviour 
{
	public AVProQuickTimeMovie _movieA;
	public AVProQuickTimeMovie _movieB;
	public string _folder;
	public List<string> _filenames;
	
	public UITexture m_MoiveOutputTexture;

	public float m_NextMoiveDelayTime = 1.0f;
	
	private AVProQuickTimeMovie[] _movies;
	private int _moviePlayIndex;
	private int _movieLoadIndex;
	private int _index = -1;
	private bool _loadSuccess = true;
	private int _playItemIndex = -1;

	private float finishTime = -1;
	
	public AVProQuickTimeMovie PlayingMovie  { get { return _movies[_moviePlayIndex]; } }
	public AVProQuickTimeMovie LoadingMovie  { get { return _movies[_movieLoadIndex]; } }
	public int PlayingItemIndex { get { return _playItemIndex; } }
	public bool IsPaused { get { if (PlayingMovie.MovieInstance != null) return !PlayingMovie.MovieInstance.IsPlaying; return false; } }
	
	void Start()
	{
		_movieA._loop = false;
		_movieB._loop = false;
		_movies = new AVProQuickTimeMovie[2];
		_movies[0] = _movieA;
		_movies[1] = _movieB;
		_moviePlayIndex = 0;
		_movieLoadIndex = 1;
		
		//NextMovie();

		_folder = Application.streamingAssetsPath + _folder;


		//Application.streamingAssetsPath + _filenames;
	}
	
	void Update() 
	{
		if (PlayingMovie.MovieInstance != null)
		{
			//Debug.Log("frame : " +PlayingMovie.MovieInstance.Frame + "  " + (PlayingMovie.MovieInstance.Frame >= PlayingMovie.MovieInstance.FrameCount) + " IsFinishedPlaying: " +(PlayingMovie.MovieInstance.IsFinishedPlaying) );
			if (PlayingMovie.MovieInstance.Frame != 0 && PlayingMovie.MovieInstance.Frame >= PlayingMovie.MovieInstance.FrameCount || PlayingMovie.MovieInstance.IsFinishedPlaying)
			{
				if( finishTime <= 0)
				{
					finishTime = Time.time;
				}

				//Debug.Log("time :" +Time.time + "  finishTime : " +finishTime + " Time.time - finishTime" + (Time.time - finishTime));

				if(Time.time - finishTime >= m_NextMoiveDelayTime)
				{
					NextMovie();
					finishTime = 0;
				}
			}
		}
		
//		if (!_loadSuccess)
//		{
//			_loadSuccess = true;
//			NextMovie();
//		}
	}
	
	void OnGUI()
	{	
		m_MoiveOutputTexture.mainTexture = PlayingMovie.OutputTexture;
		//if (m_MoiveOutputTexture.mainTexture == null)
		//	m_MoiveOutputTexture.mainTexture = LoadingMovie.OutputTexture;		// Display the previous video until the current one has loaded the first frame
	}
	
	public void Next()
	{
		NextMovie();
	}
	
	public void Previous()
	{
		_index -= 2;
		if (_index < 0)
			_index += _filenames.Count;
		
		NextMovie();
	}
	
	public void Pause()
	{
		if (PlayingMovie != null)
		{
			PlayingMovie.Pause();
		}
	}
	
	public void Unpause()
	{
		if (PlayingMovie != null)
		{
			PlayingMovie.Play();
		}
	}
	
	private void NextMovie()
	{	
		Pause();
		
		if (_filenames.Count > 0)
		{
			_index = (Mathf.Max(0, _index+1))%_filenames.Count;
		}
		else
			_index = -1;
		
		if (_index < 0)
			return;
		
		
		LoadingMovie._folder = _folder;
		LoadingMovie._filename = _filenames[_index];
		LoadingMovie._playOnStart = true;
		_loadSuccess = LoadingMovie.LoadMovie();
		_playItemIndex = _index;
		
		_moviePlayIndex = (_moviePlayIndex + 1)%2;
		_movieLoadIndex = (_movieLoadIndex + 1)%2;		
	}

	public void Reset()
	{
		_index = -1;
	}

}