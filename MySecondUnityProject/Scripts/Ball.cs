using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

    public float maxVelocity = 18;//珠球最大速度.
    public float minVelocity = 13;//珠球最小速度.

	void Awake () {
        rigidbody.velocity = new Vector3(0, 0, -13);//小球初始速度.
	}

	void Update () {
        //控制小球的速度在15—20之间.
        float totalVelocity = Vector3.Magnitude(rigidbody.velocity);//得到珠球的总的速度.
        if(totalVelocity > maxVelocity){
            float tooHard = totalVelocity / maxVelocity;
            rigidbody.velocity /= tooHard;
        }
        else if (totalVelocity < minVelocity)
        {
            float tooSlowRate = totalVelocity / minVelocity;
            rigidbody.velocity /= tooSlowRate;
        }
		//print(rigidbody.velocity);
        //若珠球的z坐标小于-3,游戏结束.
        if(transform.position.z <= -5){            
            BreakoutGame.SP.LostBall();
            Destroy(gameObject);//消除游戏组件.
        }
		//print (rigidbody.velocity);
	}
}
