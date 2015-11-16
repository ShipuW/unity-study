using UnityEngine;
using System.Collections;

public class UIMore : MonoBehaviour 
{
	#region public member for unity
	public UIButton m_BtnMore;
	public UIWidget m_MoreWidget;
	public UISprite m_BtnClose;

	public BoxCollider m_InputPanelLeft = null;
	public BoxCollider m_InputPanelRight = null;

	//
	public UIGrid m_GridApartmentLayout = null;
	//
	public MainScene.ApartmentLayoutItem m_GridDefaultItem = null;
	//
	public MainScene.ApartmentLayoutItem[] m_GridItems = null;

	#endregion

	public delegate void VoidDelegate();

	public VoidDelegate OnOpen,OnClose,OnOpenFinish,OnCloseFinish;

	#region private member
	private bool m_bInited = false;
	private bool m_bTweening = false;
	private bool m_MoreWidgetOpened = false;
	//
	private UICenterOnChild m_UICenterOnChild = null;
	#endregion

	#region private function for unity
	// Use this for initialization
	void Start ()
	{

		if( m_BtnMore == null )
		{
			Debug.LogError("The 'BtnMore' has not assigle!" ,this);
			return;
		}

		if( m_BtnClose == null )
		{
			Debug.LogError("The 'BtnClose' has not assigle!" ,this);
			return;
		}

		if( m_MoreWidget == null )
		{
			Debug.LogError("The 'MoreWidget' has not assigle!" ,this);
			return;
		}

		//
		if( m_InputPanelLeft == null )
		{
			Debug.LogError( "The InputPanelLeft has not assigned.", this );
			return;
		}
		
		//
		if( m_InputPanelRight == null )
		{
			Debug.LogError( "The InputPanelRight has not assigned.", this );
			return;
		}

		//
		m_GridItems = m_GridApartmentLayout.GetComponentsInChildren<MainScene.ApartmentLayoutItem>();
		
		//
		if( m_GridItems == null || m_GridItems.Length <= 0 )
		{
			Debug.LogError( "The GridItems is Null or Empty.", this );
			return;
		}
		
		//
		if( m_GridDefaultItem == null )
			m_GridDefaultItem = m_GridItems[0];
		
		//
		if( m_GridDefaultItem == null )
		{
			Debug.LogError( "The GridDefaultItem has not assigned.", this );
			return;
		}
		
		//
		m_UICenterOnChild = m_GridApartmentLayout.GetComponent<UICenterOnChild>();
		
		//
		if( m_UICenterOnChild == null )
		{
			Debug.LogError( "The GridApartmentLayout has not assigned Component:'UICenterOnChild'.", this );
			return;
		}

		AddListenner();

		m_MoreWidget.gameObject.SetActive(false);

		m_bInited = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	#endregion

	#region private function

	private void AddListenner()
	{
		UIEventListener.Get(m_BtnMore.gameObject).onClick = this.MoreClick;
		UIEventListener.Get(m_BtnClose.gameObject).onClick = this.CloseClick;

		//
		UIEventListener listener = UIEventListener.Get( m_InputPanelLeft.gameObject );
		listener.onHover = this.OnHoverInputPanelLeft;
		listener.onClick = this.OnClickedInputPanelLeft;
		
		listener = UIEventListener.Get( m_InputPanelRight.gameObject );
		listener.onHover = this.OnHoverInputPanelRight;
		listener.onClick = this.OnClickedInputPanelRight;
	}

	private void MoreClick( GameObject go )
	{
		if(!m_bInited)
			return;

		if(m_MoreWidgetOpened)
			return;

		if( OnOpen != null )
		{
			OnOpen();
		}

        HideMoreButton();

		m_MoreWidget.gameObject.SetActive(true);
		m_bTweening = true;
		TweenPosition tp = m_MoreWidget.GetComponent<TweenPosition>();
		tp.from = new Vector3(1920f,0,0);
		tp.to = Vector3.zero;
		tp.SetOnFinished( this.TweenOpenFinish);
		tp.PlayForward();
	}

	private void CloseClick(GameObject go)
	{
		if(!m_bInited)
			return;

		if( OnClose != null )
		{
			OnClose();
		}

       

		//m_MoreWidget.gameObject.SetActive(true);
		m_bTweening = true;
		TweenPosition tp = m_MoreWidget.GetComponent<TweenPosition>();
		//tp.to = new Vector3(1920f,0,0);
		//tp.from = Vector3.zero;
		tp.SetOnFinished (this.TweenCloseFinish);
		tp.PlayReverse();
	}

	private void TweenOpenFinish()
	{
		//m_MoreWidget.gameObject.SetActive(false);
		m_bTweening = false;
		m_MoreWidgetOpened = true;


		if( OnOpenFinish != null )
		{
			OnOpenFinish();
		}
	}

	private void TweenCloseFinish()
	{
		m_MoreWidget.gameObject.SetActive(false);
		m_bTweening = false;
		m_MoreWidgetOpened = false;
        ShowMoreButton();
		if( OnCloseFinish != null )
		{
			OnCloseFinish();
		}
	}

	//
	private void OnHoverInputPanelLeft( GameObject go, bool state )
	{
		//
		if( !m_bInited ) 
			return;
		
		//
		if( m_bTweening )
			return;
		
		if( !state )
			return;
		
		//
		CenterPrevious();
	}
	
	//
	private void OnHoverInputPanelRight( GameObject go, bool state )
	{
		//
		if( !m_bInited ) 
			return;
		
		//
		if( m_bTweening )
			return;
		
		if( !state )
			return;
		
		//
		CenterNext();
	}
	
	//
	private void OnClickedInputPanelLeft( GameObject go )
	{
		//
		if( !m_bInited ) 
			return;
		
		//
		if( m_bTweening )
			return;
		
		//
		CenterPrevious();
	}
	
	//
	private void OnClickedInputPanelRight( GameObject go )
	{//
		if( !m_bInited ) 
			return;
		
		//
		if( m_bTweening )
			return;
		
		//
		CenterNext();
	}

	private void CenterNext()
	{
		Transform target = null;
		
		MainScene.ApartmentLayoutItem item = m_UICenterOnChild.centeredObject.GetComponent<MainScene.ApartmentLayoutItem>();
		
		//
		for( int i = 0; i < m_GridItems.Length; i++ )
		{
			if( m_GridItems[i] == item )
			{
				if( i < m_GridItems.Length - 1 )
					target = m_GridItems[i+1].transform;
				
				break;
			}
		}
		
		// can not to NEXT
		if( target == null )
			return;
		
		//
		m_UICenterOnChild.CenterOn( target );
	}
	
	//
	private void CenterPrevious()
	{
		Transform target = null;
		
		MainScene.ApartmentLayoutItem item = m_UICenterOnChild.centeredObject.GetComponent<MainScene.ApartmentLayoutItem>();
		
		//
		for( int i = 0; i < m_GridItems.Length; i++ )
		{
			if( m_GridItems[i] == item )
			{
				if( i > 0 )
					target = m_GridItems[i-1].transform;
				
				break;
			}
		}
		
		// can not to PREVIOUS
		if( target == null )
			return;
		
		//
		m_UICenterOnChild.CenterOn( target );
	}

	#endregion

	#region public function

	public void HideMoreButton()
	{
		TweenPosition.Begin(m_BtnMore.gameObject,1,new Vector3(300,0,0));
	}

	public void ShowMoreButton()
	{
		TweenPosition.Begin(m_BtnMore.gameObject,1,new Vector3(0,0,0));
	}

	#endregion
}
