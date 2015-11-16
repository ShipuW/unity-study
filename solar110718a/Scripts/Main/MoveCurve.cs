using UnityEngine;
using System.Collections;

public class MoveCurve : MonoBehaviour {
	public GameObject t1;//start point
	public GameObject t2;//end point
	
	
	
	// Update is called once per frame
	void Update () {
		//两者中心点
		Vector3 center = (t1.transform.position + t2.transform.position) * 0.5f;
		
		center -= new Vector3 (0, 1, 0);
		
		Vector3 start = t1.transform.position - center;
		Vector3 end = t2.transform.position - center;
		
		//弧形插值
		transform.position = Vector3.Slerp (start, end, Time.time);
		transform.position += center; 
	}
}
