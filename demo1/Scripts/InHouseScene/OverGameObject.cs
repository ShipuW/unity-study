using UnityEngine;
using System.Collections;

namespace InHouseScene
{
	public class OverGameObject : MonoBehaviour
	{
		#region public static 
		private static OverGameObject s_Instace;
		
		public static OverGameObject Instace { get { return s_Instace; } }
		#endregion

	    public float m_DeltaCheckTime = 1.0f;

	    public delegate void OnOverGameObjectDelegate(GameObject gameObject,Vector3 pos);

	    public OnOverGameObjectDelegate OnOverGameObject;

		void Awake()
		{
			s_Instace = this;
		}

	    // Use this for initialization
	    void Start()
	    {
	        this.StartCoroutine(CheckOverGameObject());
	    }
	    
	    // Update is called once per frame
	    void Update()
	    {
	    
	    }


	    void OnDestroy()
	    {

	        this.StopCoroutine(CheckOverGameObject());

	    }

	    private IEnumerator CheckOverGameObject()
	    {

	        while (true)
	        {

	            yield return new WaitForSeconds(m_DeltaCheckTime);

	            Vector3 screenPixelPos = UICamera.lastTouchPosition;

	            Ray ray = Camera.main.ScreenPointToRay(screenPixelPos);
	           
	            //excluding 2DGUI 3DGUI
	            //int layerMask = (1 << 8) | (1 << 9);

	            //layerMask = ~layerMask;
	           // Debug.Log("CheckOverGoameObject" + screenPixelPos);
	            RaycastHit hit;
	            if (Physics.Raycast(ray, out hit))//Mathf.Infinity, layerMask
	            {
	               
	                if(OnOverGameObject != null && hit.collider.gameObject!= null)
	                {
	                    //Debug.DrawLine(ray.origin, hit.point);

	                   // Debug.Log("OverGameObject" + hit.collider.gameObject.name);
						if(OnOverGameObject != null)
						{
							OnOverGameObject(hit.collider.gameObject,screenPixelPos);
						}
	                }

	            }

	        }

	    }
	}
}
