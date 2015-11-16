using UnityEngine;
using System.Collections;

namespace MainScene
{
	public class MovePathPoint : MonoBehaviour
	{
		private void OnDrawGizmos()
		{
			Gizmos.color=Color.magenta;
			Gizmos.DrawWireSphere(transform.position,.25f);	
		}
	}
}