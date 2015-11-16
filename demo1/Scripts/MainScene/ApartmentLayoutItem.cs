using UnityEngine;
using System.Collections;

namespace MainScene
{
	public class ApartmentLayoutItem : MonoBehaviour 
	{
		//
		public string m_LevelName = null;
		//
		public Bounds m_ItemBounds;

		//
		private bool m_bInit = false;

		//
		private UIScrollView m_ScrollView;

		// Use this for initialization
		private void Start()
		{
			//
			if( m_ScrollView == null )
			{
				m_ScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);
			}

			//
			m_ItemBounds = NGUIMath.CalculateRelativeWidgetBounds( this.transform, false );

			//
			if( string.IsNullOrEmpty( m_LevelName ) )
			{
				Debug.LogError( "The LevelName is Null or Empty", this );
				return;
			}
		
			m_bInit = true;
		}

		//
		private void Update()
		{
			//
			if( !m_bInit )
				return;

			//
			if( m_ScrollView != null && m_ScrollView.panel != null )
			{
				Vector3[] corners = m_ScrollView.panel.worldCorners;
				Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;

				//
				Transform panelTrans = m_ScrollView.panel.cachedTransform;
				
				// 计算当前对象与面板中心点的距离
				Vector3 cp = panelTrans.InverseTransformPoint(this.transform.position);
				Vector3 cc = panelTrans.InverseTransformPoint(panelCenter);
				Vector3 localOffset = cp - cc;

				float fDist = localOffset.magnitude;
				// Offset shouldn't occur if blocked
				if( fDist > m_ItemBounds.extents.x && fDist <= m_ItemBounds.size.x )
				{
					Vector3 scale = new Vector3( 0.75f, 0.75f, 0.75f );
					float f = 1.0f - Mathf.Abs(fDist - m_ItemBounds.extents.x) / m_ItemBounds.extents.x;

					scale += ( Vector3.one - scale ) * f;

					this.transform.localScale = scale;
				}
				else if( fDist > m_ItemBounds.size.x )
					this.transform.localScale = new Vector3( 0.75f, 0.75f, 0.75f );
				else
					this.transform.localScale = Vector3.one;
			}

		}
		
		// Update is called once per frame
		private void OnClick()
		{
			//
			if( !m_bInit )
				return;

			//
			//Application.LoadLevel( m_LevelName );

			LoadingScene.Main.LoadingScene(m_LevelName);
		}
	}
}