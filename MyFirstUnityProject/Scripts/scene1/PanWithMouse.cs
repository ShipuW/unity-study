using UnityEngine;
using System.Collections;

namespace scene1
{
	public class PanWithMouse : MonoBehaviour
	{

		public Vector2 degrees = new Vector2 (3f, 1f);
		public float range = 1f;
		Transform mTrans;
		Quaternion mStart;
		Vector2 mRot = Vector2.zero;

		//无赋值时,默认为false.
		private bool bInited;

		// Use this for initialization
		void Start ()
		{
			this.StartCoroutine (this.DelayInit ());
		}
	
		// Update is called once per frame
		void Update ()
		{
			if (!bInited)
				return;
			float delta = RealTime.deltaTime;
			Vector3 pos = UICamera.lastTouchPosition;
			float halfWidth = Screen.width * 0.5f;
			float halfHeight = Screen.height * 0.5f;
		
			if (range < 0.1f)
				range = 0.1f;
			float x = Mathf.Clamp ((pos.x - halfWidth) / halfWidth / range, -1f, 1f);
			float y = Mathf.Clamp ((pos.y - halfHeight) / halfHeight / range, -1f, 1f);
			mRot = Vector2.Lerp (mRot, new Vector2 (x, y), delta * 5f);
		
			mTrans.localRotation = mStart * Quaternion.Euler (-mRot.y * degrees.y, mRot.x * degrees.x, 0f);
		}

		private IEnumerator DelayInit ()
		{
			yield return null;
			mTrans = this.transform;
			mStart = mTrans.localRotation;
			bInited = true;
		}

		public void Reset ()
		{
			mTrans = this.transform;
			mStart = mTrans.localRotation;
		}
	}
}