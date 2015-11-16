using UnityEngine;
using System.Collections;

namespace MainScene
{
	public class Menu6Item : MonoBehaviour
	{
		public string m_ImageFilename = null;

		public string ImageFilename { get{ return m_ImageFilename; } /*set{ if(!string.IsNullOrEmpty(value)) m_ImageFilename = value; }*/ }

		// Use this for initialization
		private void Start ()
		{
			//
			if( string.IsNullOrEmpty( m_ImageFilename ) )
			{
				Debug.LogError( "The 'ImageFilename' has not assigned.", this );
			}
		}
	}

}