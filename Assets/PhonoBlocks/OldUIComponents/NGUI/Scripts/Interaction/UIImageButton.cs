//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright Â© 2011-2013 Tasharen Entertainment
//----------------------------------------------
using UnityEngine;

/// <summary>
/// Sample script showing how easy it is to implement a standard button that swaps sprites.
/// </summary>

[AddComponentMenu("NGUI/UI/Image Button")]
public class UIImageButton : MonoBehaviour
{
		public UISprite target;
		public string normalSprite;
		public string hoverSprite;
		public string pressedSprite;
		public string disabledSprite;
		public string downSprite;
		public string upSprite;
	    
		void Start ()
		{
				upSprite = normalSprite;
				downSprite = disabledSprite;
		}

		bool down;
	
		public bool isEnabled {
				get {
						Collider col = GetComponent<Collider>();
						return col && col.enabled;
				}
				set {
						Collider col = GetComponent<Collider>();
						if (!col)
								return;

						if (col.enabled != value) {
								col.enabled = value;
								UpdateImage ();
						}
				}
		}

		void OnEnable ()
		{
				if (target == null)
						target = GetComponentInChildren<UISprite> ();
				UpdateImage ();
		}
	
		void UpdateImage ()
		{   
				if (target != null) {
						if (isEnabled) {
								target.spriteName = UICamera.IsHighlighted (gameObject) ? hoverSprite : normalSprite;
						} else {
								target.spriteName = disabledSprite;
						}
						target.MakePixelPerfect ();
				}
		}

		void OnHover (bool isOver)
		{
				if (isEnabled && target != null) {
						target.spriteName = isOver ? hoverSprite : normalSprite;
						target.MakePixelPerfect ();
				}
		}

		void OnPress (bool pressed)
		{
				if (pressed) {
						target.spriteName = pressedSprite;
						target.MakePixelPerfect ();
						down = !down;
						if (down&&downSprite!=null) {
								normalSprite = downSprite;
								pressedSprite = upSprite;
						} else {
								normalSprite = upSprite;
								pressedSprite = downSprite;
						}
				} else
						UpdateImage ();
		}
}
