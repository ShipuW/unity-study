using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace InHouseScene
{
    public class LookAtManager : MonoBehaviour 
    {

        #region static instance
        public static LookAtManager Instance { get { return s_Instance; } }
        
        private static LookAtManager s_Instance = null;
        #endregion

        #region public member for unity

        public GameObject m_LookAtParent;

        public LookAt[] m_LookAts;

        public GameObject m_CameraDestinationParent;

        public CameraDestination[] m_CameraDestinations;

        #endregion

        #region public properties

        public LookAt[] LookAts{get {return m_LookAts;}}

        public CameraDestination[] CameraDestinations{get{return m_CameraDestinations;}}

        public Queue<GameObject> HistoryQueue{get {return m_HistoryQueue;}}

        #endregion

        #region private member

        private Queue<GameObject> m_HistoryQueue = new Queue<GameObject>();

        #endregion

        #region private member;

        private bool m_bInited = false;

        #endregion

        #region private function for unity

        private void Awake()
        {
            s_Instance = this;
        }

    	// Use this for initialization
    	void Start () 
        {
    	
            if(m_LookAtParent == null)
            {
                Debug.LogError("The 'LookAtParent' has not assgined!");

                return;
            }

            m_LookAts = m_LookAtParent.GetComponentsInChildren<LookAt>();

            if(m_LookAts == null || m_LookAts.Length<=0)
            {
                Debug.LogError("The 'LookAts' is null or length <= 0!");
                return;
            }

            if(m_CameraDestinationParent == null)
            {
                Debug.LogError("The 'CameraDestinationParent' has not assgined!");
                
                return;
            }
            
            m_CameraDestinations = m_CameraDestinationParent.GetComponentsInChildren<CameraDestination>();
            
            if(m_CameraDestinations == null || m_CameraDestinations.Length<=0)
            {
                Debug.LogError("The 'CameraDestinations' is null or length <= 0!");
                return;
            }

            this.m_bInited = true;

    	}
    	
    	// Update is called once per frame
    	void Update () 
        {
    	
    	}
        #endregion


    }
}
