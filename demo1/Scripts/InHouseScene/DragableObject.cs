using UnityEngine;
using System.Collections;

namespace InHouseScene
{
    public class DragableObject : MonoBehaviour
    {
    #region public Member for unity
    
        //是否可以移动
        public bool m_CanMove = true;
    
        //是否可以更换材质
        public bool m_CanChangeMaterial = true;
    
        //是否可以更换模型
        public bool m_CanChangeMode = true;
    
        //可选择的材质
        public Texture2D[] m_ChangeMaterials;
    
        //可选择的模型
        public GameObject[] m_ChangeModes;
        public UIPanel m_ModePanel;
        public UIPanel m_MetarialPanel;
    #endregion

    #region private member

        private bool bInDragd;
        private bool bInited = false;
    
    #endregion

    #region private function for unity
        // Use this for initialization
        void Start()
        {

            if (m_CanChangeMode)
            {
            
                if (m_ChangeModes == null || m_ChangeModes.Length <= 0)
                {
                    Debug.LogError("The 'ChangeModes' has not assigned!");
                    return;
                }
            
                UIDragScrollView[] scrollViews = m_ModePanel.transform.GetComponentsInChildren<UIDragScrollView>();

                int index = 0;

                foreach (UIDragScrollView s in scrollViews)
                {
                    UIEventListener.Get(s.gameObject).onClick = onModeItemClick;
                
                    UIEventListener.Get(s.gameObject).onHover = OnModeItemHove;

                    ScrollViewItemListener l = s.gameObject.AddComponent<ScrollViewItemListener>();
                    l.Index = index ++;
                }
            
                m_ModePanel.gameObject.SetActive(false);
            
            }
        
            if (m_CanChangeMaterial)
            {
                if (m_ChangeMaterials == null || m_ChangeMaterials.Length <= 0)
                {
                    Debug.LogError("The 'ChangeMaterials' has not assigned!");
                    return;
                }
                if (m_MetarialPanel == null)
                {
                    Debug.LogError("The 'ModeMetarialPanel' has not assigned!");
                    return;
                }
                int index = 0;
                UIDragScrollView[] scrollViews = m_MetarialPanel.transform.GetComponentsInChildren<UIDragScrollView>();
            
                foreach (UIDragScrollView s in scrollViews)
                {
                    UIEventListener.Get(s.gameObject).onClick = onMetrialItemClick;
                
                    UIEventListener.Get(s.gameObject).onHover = OnMetrialItemHove;

                    ScrollViewItemListener l = s.gameObject.AddComponent<ScrollViewItemListener>();
                    l.Index = index ++;
                }
            
                m_MetarialPanel.gameObject.SetActive(false);
            
            }
        
            this.bInited = true;
        }
    
        // Update is called once per frame
        void Update()
        {
    
        }

    #endregion

    #region public function
    
        /// <summary>
        /// 移动物体的回调
        /// </summary>
        public void OnMove()
        {
        
            if (!bInited)
            {
                return;
            }
        }
    
        /// <summary>
        /// 更换材质的回调
        /// </summary>
        public void OnChangeMaterial(GameObject dragObj)
        {
            if (!bInited)
            {
                return;
            }
        
            if (dragObj == this.gameObject && m_MetarialPanel != null)
            {
                m_MetarialPanel.gameObject.SetActive(true);
            }
        
        }
    
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
            if (dragObj == this.gameObject && m_ModePanel != null)
            {
                m_ModePanel.gameObject.SetActive(true);
            }
        }
    #endregion
    
    #region private function for Metrial 
        private void onMetrialItemClick(GameObject gameObject)
        {
        
            //Debug.Log("item index = " + );
            //          if (draggedObject != null)
            //              draggedObject.renderer.material.color = gameObject.transform.FindChild("UISprite").GetComponent<UISprite>().color;
            //          
            //          if (draggedObject != null)
            //          {
            //              draggedObject = null;
            //          }
            //          
            //          if (m_MaterialPanel.active)
            //              m_MaterialPanel.gameObject.SetActive(false);
        
            Debug.Log("metarial select------------------>");
        }
    
        private void OnMetrialItemHove(GameObject go, bool state)
        {
            if (state)
            {
                go.transform.FindChild("UISlicedSprite").GetComponent<UISprite>().color = Color.gray;
            } else
            {
                go.transform.FindChild("UISlicedSprite").GetComponent<UISprite>().color = Color.white;
            }
        }
    #endregion
    
    #region private function for Mode 
        private void onModeItemClick(GameObject gameObject)
        {
        
            //Debug.Log("item index = " + );
            //          if (draggedObject != null)
            //              draggedObject.renderer.material.color = gameObject.transform.FindChild("UISprite").GetComponent<UISprite>().color;
            //          
            //          if (draggedObject != null)
            //          {
            //              draggedObject = null;
            //          }
            //          
            //          if (m_MaterialPanel.active)
            //              m_MaterialPanel.gameObject.SetActive(false);
        
            int index = gameObject.GetComponent<ScrollViewItemListener>().Index;

            MeshCollider[] colliders = this.gameObject.GetComponentsInChildren <MeshCollider>();

            foreach (MeshCollider c in colliders)
            {
                c.enabled = false;
            }
        
//      this.m_ChangeModes [index].SetActive (true);
//
//      MeshCollider[] acolliders =  this.m_ChangeModes [index].GetComponentsInChildren <MeshCollider>();
//      
//      foreach(MeshCollider c in acolliders)
//      {
//          c.enabled = true;
//      }
            StartCoroutine(HoldGameObject(index));

            if (m_ModePanel.active)
                m_ModePanel.gameObject.SetActive(false);
        
        }
    
        private void OnModeItemHove(GameObject go, bool state)
        {
            if (state)
            {
                go.transform.FindChild("UISlicedSprite").GetComponent<UISprite>().color = Color.gray;
            } else
            {
                go.transform.FindChild("UISlicedSprite").GetComponent<UISprite>().color = Color.white;
            }
        }
    
    #endregion

        IEnumerator HoldGameObject(int index)
        {  
            yield return null;
            this.gameObject.SetActive(false);
            yield return null;
            this.m_ChangeModes [index].SetActive(true);

        }

    }
}
