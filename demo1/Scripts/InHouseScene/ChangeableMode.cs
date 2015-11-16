using UnityEngine;
using System.Collections;

namespace InHouseScene
{
    public class ChangeableMode : MonoBehaviour
    {
        #region public member for unity

        public GameObject m_ModelPanel;
    
        #endregion

        #region public property

        public GameObject ModelPanel{ get { return m_ModelPanel; } set { m_ModelPanel = value; } }

        #endregion

        #region private member 
        private bool bInited = false;
        #endregion

        // Use this for 
        void Start()
        {

            if (m_ModelPanel == null)
            {
                Debug.LogError("The 'm_ModePanel' has not assigned!");
                return;
            }

            bInited = true;
        }
    
        // Update is called once per frame
        void Update()
        {
    
        }
    
        #region private function

        #endregion

        #region public function

        /// <summary>
        /// 更换模型的回调
        /// </summary>
        public void OnChangeMode(GameObject dragObj)
        {
            if (!bInited)
            {
                return;
            }
            //Debug.Log("onDragGameObject ------------------- > " + (dragObj == this.gameObject && m_ModePanel !=null ));
            if (m_ModelPanel != null)
            {
                m_ModelPanel.SetActive(true);
            }
        }

    #endregion
    }
}
