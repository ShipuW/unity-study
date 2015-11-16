using UnityEngine;
using System.Collections;

public class CloseCallback : MonoBehaviour {

    public GameObject m_Dialog;
    private bool m_bInit;

	// Use this for initialization
	void Start () {
	

        if(m_Dialog == null)
        {
            Debug.LogError("The Dialog has not assigle!");
            return;
        }
        m_bInit = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnClick()
    {
        if(!m_bInit)
            return;
        m_Dialog.SetActive(false);
    }
}
