using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace InHouseScene
{
    public class ModelManager : MonoBehaviour
    {

        #region static instance
        public static ModelManager Instance { get { return s_Instance; } }
        
        private static ModelManager s_Instance = null;
        #endregion

        #region public member for unity

        public List<GameObject> m_Models;

        public List<GameObject> m_ModelPanel;

        #endregion
    
        #region public property

        public List<GameObject> Models { get { return m_Models; } }
        public List<GameObject> ModelPanel { get { return m_ModelPanel; } }
        
        #endregion

        #region private function for unity

        private void Awake()
        {
            s_Instance = this;
        }

        void Start()
        {

            if (m_Models == null || m_Models.Count <= 0)
            {
                Debug.LogError("The 'Models' has not assigned!");
                return;
            }

            if (m_ModelPanel == null || m_ModelPanel.Count <= 0 && m_ModelPanel.Count != m_Models.Count)
            {
                Debug.LogError("The 'Models' has not assigned!");
                return;
            }

            int i = 0;
            foreach(GameObject go in m_Models)
            {
                ChangeableMode cm = go.AddComponent<ChangeableMode>();
                cm.ModelPanel = m_ModelPanel[i++];
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        #endregion
    }
}
