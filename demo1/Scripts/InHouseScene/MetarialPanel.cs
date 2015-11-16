using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace InHouseScene
{
    public class MetarialPanel : MonoBehaviour {

    
        #region public member for unity
        
        public List<Texture> m_MetarialTextures;
        public GameObject m_ChangeMetarialGameObject;

        public int m_MaterialIndex;
        
        #endregion
        
        // Use this for initialization
        void Start () 
        {
            if (m_MetarialTextures == null || m_MetarialTextures.Count <= 0)
            {
                Debug.LogError("The 'MetarialTextures' has not assigned!");
                return;
            }
            
            if (m_ChangeMetarialGameObject == null)
            {
                Debug.LogError("The 'ChangeMetarial' has not assigned!");
                return;
            }
            
            UIButton[] buttons = this.GetComponentsInChildren<UIButton>();

            int i = 0;
            foreach(UIButton b in buttons)
            {
                
                ScrollViewItemListener listener = b.gameObject.AddComponent<ScrollViewItemListener> ();
                listener.Index = i;
                
				UITexture tex = b.GetComponentInChildren<UITexture>();

				if(tex != null)
				{
					tex.mainTexture = m_MetarialTextures[i % m_MetarialTextures.Count];
				}
				i++;

                UIEventListener.Get(b.gameObject).onClick = OnItemClick;
            }
        }
        
        // Update is called once per frame
        void Update () 
        {
            
        }
        
        public void OnItemClick(GameObject go)
        {
            //Debug.Log("OnItemClick");
            ScrollViewItemListener item = go.GetComponent<ScrollViewItemListener>();
            int index = item.Index;

            Material[] materials = m_ChangeMetarialGameObject.renderer.materials;

            materials[m_MaterialIndex % materials.Length].mainTexture = m_MetarialTextures[index% m_MetarialTextures.Count];
        }
        
    }
}

