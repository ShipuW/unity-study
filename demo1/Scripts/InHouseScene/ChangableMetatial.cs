

using UnityEngine;
using System.Collections;

namespace InHouseScene
{
    public class ChangableMetatial : MonoBehaviour
    {
        #region public member for unity
        public GameObject m_MetarialPanel;

        #endregion

        #region public property
        public GameObject MetarialPanel{ get { return m_MetarialPanel; } set { m_MetarialPanel = value; } }

        #endregion


        #region private member
        private bool bInited = false;
        #endregion

        #region private function for unity
        // Use this for initialization
        private void Start()
        {
            if (m_MetarialPanel == null)
            {
                Debug.LogError("The 'MetarialPanel' has not assigned!");
                return;
            }

            bInited = true;

        }
    
        // Update is called once per frame
        private void Update()
        {
    
        }
        #endregion

        /// <summary>
        /// 更换材质的回调
        /// </summary>
        public void OnChangeMaterial(GameObject dragObj)
        {
            if (!bInited)
            {
                return;
            }
        
            if (m_MetarialPanel != null)
            {
                m_MetarialPanel.SetActive(true);
            }
        
        }
    }
}
