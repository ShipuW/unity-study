using UnityEngine;
using System.Collections;

public class CollisionListenner : MonoBehaviour
{

    #region private function for unity
    // Use this for initialization
    private void Start()
    {
    
    }
    
    // Update is called once per frame
    private void Update()
    {
    
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter------------> " + collision.gameObject.name);
        /*
        HingeJoint hingeJoint = collision.gameObject.GetComponent<HingeJoint>();

        if(hingeJoint != null)
        {

           

            hingeJoint.useMotor = true;
            JointMotor motor = hingeJoint.motor;
            motor.force = 1000;
            motor.freeSpin = true;
            motor.targetVelocity = 0;
            hingeJoint.motor = motor;

            collision.rigidbody.AddForce(Vector3.right * 50);

        }
        */
       // collision.rigidbody.AddForce(Vector3.right * 100000,ForceMode.Impulse);
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("OnCollisionExit------------>");

       
    }

    private void OnCollisionStay(Collision collisionInfo) 
    {
       
    }

    #endregion


}
