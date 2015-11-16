using UnityEngine;
using System.Collections;

namespace CKinect
{
	public class KinectServices : MonoBehaviour
	{
		//
		private static KinectServices s_Instance = null;
		public static KinectServices Instance { get{ return s_Instance; } }
		
		//
		public static UISprite SpriteHandLeft { set;get; }
		public static UISprite SpriteHandRight { set;get; }
		public static UISprite SpriteCursorLoading { set;get; }
		
		//
		private void Awake()
		{
			s_Instance = this;
		}
		
		//
		private void Start()
		{
			GameObject.DontDestroyOnLoad( this.gameObject );
		}
		
		//
		public static void Create( UISprite handLeft, UISprite handRight ,UISprite loading)
		{
			//
			GameObject go = Resources.Load<GameObject>( "Prefabs/KinectServices" );
			
			if( go == null )
			{
				Debug.LogError( "Can not load 'Kinect Services' from Resources." );
				return;
			}
			
			//
			SpriteHandLeft = handLeft;
			SpriteHandRight = handRight;
			SpriteCursorLoading = loading;
			
			//
			Instantiate( go );
			// 总是存在无需释放
			//Resources.UnloadAsset(go);
		}
	}
}
