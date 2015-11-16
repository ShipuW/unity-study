using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace InHouseScene
{
    public class MetarialManager : MonoBehaviour
    {

        #region static instance
        public static MetarialManager Instance { get { return s_Instance; } }
        
        private static MetarialManager s_Instance = null;
        #endregion

        #region public member for unity
        
        public List<GameObject> m_Metarials;
        
        public List<GameObject> m_MetarialPanel;
        
        #endregion

        #region public property
        
        public List<GameObject> Metarials { get { return m_Metarials; } }
        public List<GameObject> MetarialPanel { get { return m_MetarialPanel; } }
        
        #endregion
        
        #region private function for unity

        private void Awake()
        {
            s_Instance = this;
        }

        void Start()
        {
            
            if (m_Metarials == null || m_Metarials.Count <= 0)
            {
                Debug.LogError("The 'Modes' has not assigned!");
                return;
            }
            
            if (m_MetarialPanel == null || m_MetarialPanel.Count <= 0 && m_MetarialPanel.Count != m_Metarials.Count)
            {
                Debug.LogError("The 'Modes' has not assigned!");
                return;
            }
            
            int i = 0;
            foreach(GameObject go in m_Metarials)
            {
                ChangableMetatial cm = go.AddComponent<ChangableMetatial>();
                cm.MetarialPanel = m_MetarialPanel[i++];
            }
        }
        
        // Update is called once per frame
        void Update()
        {
            
        }
        
        #endregion
    }
}
