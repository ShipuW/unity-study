

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace InHouseScene
{
    public class MoveManager : MonoBehaviour
    {

        #region static instance
        public static MoveManager Instance { get { return s_Instance; } }
        
        private static MoveManager s_Instance = null;
        #endregion

        #region public member for unity
        
        public List<GameObject> m_MoveGameObjects;

		public List<Vector3> m_MaxPostion;

		public List<Vector3> m_MinPostion;

        public GameObject m_MoveArrow;
		//
		public Vector2 m_RotateSpeed = new Vector2( 0.1f, 0.1f );
		//
		public float m_ZoomSpeed = 0.1f;

		public float m_TranlateSpeed = 5;
        #endregion

        #region public property
        
        public List<GameObject> MoveGameObjects { get { return m_MoveGameObjects; } }
        public GameObject MoveArrow { get { return m_MoveArrow; } }
        
        #endregion

		private Vector3 m_vLastCursorPos = Utils.InvaildVec3;
		private Vector3 m_vLastHandRawPos = Utils.InvaildVec3;
		//
		private Vector3 m_vLastMousePos = Vector3.zero;
	

        #region private function for unity

        private void Awake()
        {
            s_Instance = this;
        }

        void Start()
        {

            if(m_MoveArrow == null)
            {
                Debug.LogError("The 'MoveArrow' has not assigned!");
                return;
            }

            if (m_MoveGameObjects == null || m_MoveGameObjects.Count <= 0)
            {
                Debug.LogError("The 'm_MoveGameObjects' has not assigned!");
                return;
            }

			if(m_MaxPostion == null || m_MaxPostion.Count != m_MoveGameObjects.Count)
			{
				Debug.LogError("The 'm_MaxPostion' has not assigned or size not equels m_MoveGameObjects size!");
				return;
			}

			if(m_MinPostion == null || m_MinPostion.Count != m_MoveGameObjects.Count)
			{
				Debug.LogError("The 'm_MinPostion' has not assigned or size not equels m_MoveGameObjects size!");
				return;
			}
            
            int i = 0;
            foreach(GameObject go in m_MoveGameObjects)
            {
                MoveableObject mo = go.AddComponent<MoveableObject>();

				mo.RotateSpeed= m_RotateSpeed;
				mo.ZoomSpeed = m_ZoomSpeed;
				mo.MaxPostion = m_MaxPostion[i];
				mo.MinPostion = m_MinPostion[i];
				mo.TranlateSpeed = m_TranlateSpeed;
				i++;
            }
        }
        
        // Update is called once per frame
        void Update()
        {
            
        }
        
        #endregion

	
    }
}
