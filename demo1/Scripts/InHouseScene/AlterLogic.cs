using UnityEngine;
using System.Collections;

namespace InHouseScene
{
	public class AlterLogic : MonoBehaviour
	{
		#region static instance
		private static AlterLogic s_Instance;
		public static AlterLogic Instance	{ get{ return s_Instance; } }
		#endregion

		#region public member for unity
		//
		public int m_IDAlertWelcome = -1;
		//
		public float m_AutoShowWelcomeDelta;
		//
		public int m_IDAlertDrag = -1;

		public int m_IDAlertSelectMenu = -1;

		public int m_IDAlertOtherRoom = -1;
		#endregion

		#region private member
		//
		private bool m_bInited = false;
		//
		private UIAlert m_AlertWelcome = null;
		//
		private UIAlert m_AlertDrag = null;
		//
		private UIAlert m_AlertSelectMenu = null;

		private UIAlert m_AlterOtherRoom = null;

		private bool m_bHasAlertShow;

		private bool m_bHasWelcomeShowed = false;

		#endregion

		#region public properties

		public bool HasWelcomeShowed {get {return m_bHasWelcomeShowed;}}

		#endregion

		#region private function for unity

		private void Awake()
		{
			s_Instance = this;
		}

		// Use this for initialization
		private void Start ()
		{
		
			this.StartCoroutine(DelayInit());

			//this.StartCoroutine(AutoShowWelcome());

			this.Invoke("AutoShowWelcome",m_AutoShowWelcomeDelta);
		}
		
		// Update is called once per frame
		private void Update ()
		{
		
		}

		#endregion

		#region private function

		private void AutoShowWelcome()
		{

			if( !m_bInited )
				return;

			m_AlertWelcome.Show(1.0f);

			m_bHasAlertShow = true;
		}

		private IEnumerator DelayInit()
		{
			yield return null;

//			OverGameObject instace = OverGameObject.Instace;
//			
//			if(instace != null)
//			{
//				instace.OnOverGameObject += OnOverGameObject;
//			}

			m_AlertWelcome = UIAlertManager.Instance.GetAlert( m_IDAlertWelcome );

			if( m_AlertWelcome == null )
			{
				Debug.LogError( "There's not a UIAlert -- IDAlertWelcome:" + m_IDAlertWelcome, this );
				yield break;
			}

			m_AlertWelcome.OnClose = this.OnCloseAlertWelcome;

			m_AlertDrag = UIAlertManager.Instance.GetAlert( m_IDAlertDrag );
			
			if( m_AlertDrag == null )
			{
				Debug.LogError( "There's not a UIAlert -- IDAlertDrag:" + m_IDAlertDrag, this );
				yield break;
			}

			m_AlertSelectMenu = UIAlertManager.Instance.GetAlert( m_IDAlertSelectMenu );
			
			if( m_AlertSelectMenu == null )
			{
				Debug.LogError( "There's not a UIAlert -- IDAlertSelectMenu:" + m_IDAlertSelectMenu, this );
				yield break;
			}

			m_AlterOtherRoom = UIAlertManager.Instance.GetAlert( m_IDAlertOtherRoom );
			
			if( m_AlterOtherRoom == null )
			{
				Debug.LogError( "There's not a UIAlert -- m_IDAlertOtherRoom:" + m_IDAlertOtherRoom, this );
				yield break;
			}

			m_bInited = true;
		}


		private void OnCloseAlertWelcome()
		{
			m_bHasAlertShow = false;
			m_bHasWelcomeShowed = true;
		}

		#endregion

		public void ShowAlertSelectMenu()
		{

			if( !m_bInited )
				return;
			
			if( m_AlertWelcome.GetVisible() )
				return;

			if( m_AlertDrag.GetVisible() )
				m_AlertDrag.Hide();

			m_AlertSelectMenu.Show();
		}

		public void ShowAlertDrag()
		{
			if( !m_bInited )
				return;
			
			Main instance = Main.Instance;
			
			if( instance.IsMenuShowing)
				return;
			
			if( m_AlertWelcome.GetVisible() )
				return;
			
			m_AlertDrag.Show();
		}

		public void OnHoverOverOtherRoom()
		{

			if(!m_bInited)
				return;
			
			Main instance = Main.Instance;
			
			if( instance.IsMenuShowing)
				return;

			if( m_AlertWelcome.GetVisible() )
				return;

			if(m_AlertDrag.GetVisible())
			{
				m_AlertDrag.Hide();
			}

			m_AlterOtherRoom.Show();
		}
		
		public void OnHoverOutOtherRoom()
		{
			m_AlterOtherRoom.Hide();
		}
	}
}
