using UnityEngine;
using System.Collections;

namespace InHouseScene
{
    public class MoveButton : MonoBehaviour
    {

        public enum MoveDir
        {
            LEFT = 0,
            RIGHT,
            UP,
            DOWN,
            BACK,
            FORWARD,
        };

        public MoveDir m_MoveDir;

        public MoveDir Dir{ get { return m_MoveDir; } }

        // Use this for initialization
    	void Start () {
    	
    	}
    	
    	// Update is called once per frame
    	void Update () {
    	
    	}
    }
}