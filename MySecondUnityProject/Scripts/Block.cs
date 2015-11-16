
using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	/// <summary>
	/// 触发器
	/// </summary>
	void OnCollisionEnter () {
        BreakoutGame.SP.HitBlock();
        Destroy(gameObject);//删除组件
	}
}
