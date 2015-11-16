using UnityEngine;
using System.Collections;

namespace MainScene
{
	public class LookTargetPoint : MonoBehaviour
	{
		//
		//
		private void OnDrawGizmos()
		{
			Gizmos.color=Color.cyan;
			Gizmos.DrawSphere(transform.position,.25f);	
		}
	}
}