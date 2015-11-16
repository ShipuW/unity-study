using UnityEngine;
using System.Collections;

namespace MainScene
{
	public class LookPathPoint : MonoBehaviour
	{
		//
		//
		private void OnDrawGizmos()
		{
			Gizmos.color=Color.cyan;
			Gizmos.DrawWireSphere(transform.position,.25f);
		}
	}
}