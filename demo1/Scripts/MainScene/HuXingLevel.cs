using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HuXingLevel : MonoBehaviour
{

    #region public member for unity

    public List<string> m_Levels = new List<string>();

    #endregion

    #region private member

    private bool m_bInit = false;

    private bool m_bLevelLoading = false;

    #endregion

    // Use this for initialization
    void Start()
    {
    
        if(m_Levels.Count <= 0)
        {

            Debug.LogError("The levels count must >0 !");
            return ;
        }


        m_bInit = true;
    }
    
    // Update is called once per frame
    void Update()
    {

        if(!m_bInit)
            return;

        if(m_bLevelLoading)
            return;

        CKinect.CursorController controller = CKinect.CursorController.Instance;
        
        if(controller.HasHandClosed )
        {
            
            m_bLevelLoading = true;

            this.StartCoroutine(LoadLevel(0));
            
        }
    }

    private IEnumerator LoadLevel(int level)
    {

        yield return new WaitForSeconds(1f);


      AsyncOperation ao =  Application.LoadLevelAsync(m_Levels[level]);

      
        if(ao.isDone)
        {
            ao.allowSceneActivation = true;
        }

    }

}
