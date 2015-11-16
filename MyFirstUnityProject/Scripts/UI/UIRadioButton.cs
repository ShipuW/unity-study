using UnityEngine;
using System.Collections;

namespace UI
{
	[AddComponentMenu("NGUI/Interaction/RadioButton")]
	public class UIRadioButton : MonoBehaviour
	{

		static public UIRadioButton current;
		//创建委托,处理事件.
		public delegate void OnStateChange (bool state);

		/// <summary>
		/// The sprite_1 is visible when ‘isChecked’ status is 'true'.
		/// </summary>
		public UISprite sprite_1;
		public UISprite sprite_2;

		/// <summary>
		/// Animation to play on the checkmark sprite, if any.
		/// </summary>
		public Animation checkAnimation;

		/// <summary>
		/// Whether the checkbox starts checked.
		/// </summary>
		public bool startsChecked = true;

		/// <summary>
		/// If the checkbox is part of a radio button group, specify the root object to use that all checkboxes are parented to.
		/// </summary>
		public Transform radioButtonRoot;

		/// <summary>
		/// Can the radio button option be 'none'?
		/// </summary>
		public bool optionCanBeNone = false;

		/// <summary>
		/// Generic event receiver that will be notified when the state changes.
		/// </summary>
		public GameObject eventReceiver;

		/// <summary>
		/// Function that will be called on the event receiver when the state changes.
		/// </summary>
		public string functionName = "OnActivate";

		/// <summary>
		/// Delegate that will be called when the checkbox's state changes. Faster than using 'eventReceiver'.
		/// </summary>
		public OnStateChange onStateChange;

		public GameObject EventReceiver{get {return eventReceiver;} set{eventReceiver = value; }}
		public string FunctionName {set {functionName = value;} get {return functionName;}}

		[HideInInspector][SerializeField] bool option = false;
		bool mChecked = true;
		bool mStarted = false;
		Transform mTrans;

		/// <summary>
		/// Whether the checkbox is checked.
		/// </summary>
		public bool isChecked
		{
			get { return mChecked; }
			set { if (radioButtonRoot == null || value || optionCanBeNone || !mStarted) Set(value); }
		}

		void Awake(){
			mTrans = transform;
			
			if (sprite_1 != null) sprite_1.alpha = startsChecked ? 1f : 0f;
			
			if( sprite_2 != null ) sprite_2.alpha = startsChecked ? 1f : 0f;
			
			if (option)
			{
				option = false;
				if (radioButtonRoot == null) radioButtonRoot = mTrans.parent;
			}
		}
		// Use this for initialization
		void Start ()
		{
			if (eventReceiver == null) eventReceiver = gameObject;
			mChecked = !startsChecked;
			mStarted = true;
			Set(startsChecked);
		}

		/// <summary>
		/// Check or uncheck on click.
		/// </summary>
		void OnClick () { if (enabled) isChecked = !isChecked; }

		/// <summary>
		/// Fade out or fade in the checkmark and notify the target of OnChecked event.
		/// </summary>
		void Set (bool state)
		{
			if (!mStarted)
			{
				mChecked = state;
				startsChecked = state;
				if (sprite_1 != null) sprite_1.alpha = state ? 1f : 0f;
				
				if( sprite_2 != null ) sprite_2.alpha = state ? 1f : 0f;
			}
			else if (mChecked != state)
			{
				// Uncheck all other checkboxes
				if (radioButtonRoot != null && state)
				{
					UIRadioButton[] cbs = radioButtonRoot.GetComponentsInChildren<UIRadioButton>(true);
					
					for (int i = 0, imax = cbs.Length; i < imax; ++i)
					{
						UIRadioButton cb = cbs[i];
						if (cb != this && cb.radioButtonRoot == radioButtonRoot) cb.Set(false);
					}
				}
				
				// Remember the state
				mChecked = state;
				
				// Tween the color of the checkmark
				if (sprite_1 != null)
				{
					TweenAlpha.Begin(sprite_1.gameObject, 0.15f, mChecked ? 1f : 0f);
				}
				
				// Tween the color of the icon checkmark
				if(sprite_2 != null )
				{					
					TweenAlpha.Begin(sprite_2.gameObject, 0.15f, !mChecked ? 1f : 0f);
				}
				
				current = this;
				
				// Notify the delegate
				if (onStateChange != null) onStateChange(mChecked);
				
				// Send out the event notification
				if (eventReceiver != null && !string.IsNullOrEmpty(functionName))
				{
					eventReceiver.SendMessage(functionName, mChecked, SendMessageOptions.DontRequireReceiver);
				}
				current = null;
				
				// Play the checkmark animation
				if (checkAnimation != null)
				{
					ActiveAnimation.Play(checkAnimation, state ? AnimationOrTween.Direction.Forward : AnimationOrTween.Direction.Reverse);
				}
			}
		}
		public void Reset()
		{
			mChecked = !startsChecked;
			Set(startsChecked);
		}
	}
}
