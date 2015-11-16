using System;
using UnityEngine;

namespace CKinect
{
	public class Cursor
	{
		public Vector2 deltaPosition; // 	The position delta since last change.
		public float deltaTime;// 	Amount of time that has passed since the last recorded change in Touch values.
		public ulong personId;// The person ID
		public bool leftHand;
		public bool rightHand;
		public int cursorId;// 	The unique index for the cursor.
		public HandState handState;
		public Vector2 position;// 	The position of the touch in pixel coordinates.

		public Cursor()
		{
			//
			deltaPosition = Vector2.zero;
			//
			deltaTime = 0;
			personId = 0;
			leftHand = false;
			rightHand = false;
			cursorId = -1;
			handState = HandState.Open;
			position = Vector2.zero;
		}
	}
}

