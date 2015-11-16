using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace InHouseScene
{
    public class ModelPanel : MonoBehaviour
    {

		#region public de
		public delegate void OnChangeModelFinishDelegate(GameObject model);
		public OnChangeModelFinishDelegate OnChangeModelFinish;
		
		#endregion

        #region public member for unity

        public List<Texture> m_ModelTextures;
        public List<GameObject> m_Models;

        #endregion

    	// Use this for initialization
    	void Start () 
        {
            if (m_Models == null || m_Models.Count <= 0)
            {
                Debug.LogError("The 'Modes' has not assigned!");
                return;
            }
            
            if (m_ModelTextures == null || m_ModelTextures.Count <= 0 && m_ModelTextures.Count != m_Models.Count)
            {
                Debug.LogError("The 'Modes' has not assigned!");
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
					tex.mainTexture = m_ModelTextures[i % m_Models.Count];
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

            foreach( GameObject g in m_Models)
            {
                g.SetActive(false);
            }

            m_Models[index % m_Models.Count].SetActive(true);

			if( OnChangeModelFinish != null )
			{
				OnChangeModelFinish( m_Models[index % m_Models.Count] );
			}
        }

    }
}
