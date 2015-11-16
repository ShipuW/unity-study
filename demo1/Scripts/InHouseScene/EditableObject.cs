using UnityEngine;
using System.Collections;

namespace InHouseScene
{
    [ExecuteInEditMode]
    //[RequireComponent(typeof(BoxCollider))]
	public class EditableObject : MonoBehaviour
    {
		public int m_ID;

		#region public delegate

		public delegate void OnHoverOverDelegate(GameObject go,Vector3 screenPos);
		
		public OnHoverOverDelegate OnHoveOver;
		
		public delegate void OnClickDelegate(GameObject go,Vector3 screenPos);
		
		public OnClickDelegate OnClick;
		
		public delegate void OnHoverOverOutDelegate();
		
		public OnHoverOverOutDelegate OnHoveOverOut;

		#endregion

		#region public properties

		public int ID { get { return m_ID;}}

		#endregion

	    #region private member

		private MeshFilter[] _Meshs;
		private bool bInited = false;

	    #endregion

	    #region private function for unity
	        // Use this for initialization
	    private void Start()
		{
			//
			if (Application.isEditor)
			{
				//MeshCollider[] colliders = this.GetComponents<MeshCollider>();

				//Debug.Log("--------------------------->");

				_Meshs = this.transform.GetComponentsInChildren<MeshFilter>();

				if ( _Meshs != null && _Meshs.Length > 0 )
				{
					foreach ( MeshFilter mesh in _Meshs )
					{
						BoxCollider collider = mesh.gameObject.GetComponent<BoxCollider>();

						if ( collider == null )
						{
							collider = mesh.gameObject.AddComponent<BoxCollider>();
							DragObject p = mesh.gameObject.AddComponent<DragObject>();
							p.Parent = this.gameObject;

						}
					}
				}
			}

			this.bInited = true;

		}
	    
	    // Update is called once per frame
	    private void Update()
	    {
	    
	    }

    	#endregion

		#region private function

		#endregion
    }
}