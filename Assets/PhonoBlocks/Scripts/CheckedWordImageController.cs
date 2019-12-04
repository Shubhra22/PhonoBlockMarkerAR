using UnityEngine;
using System.Collections;
using System;

public class CheckedWordImageController : MonoBehaviour
{
		public GameObject checkedWordImage;
		UITexture img;
		BoxCollider clickTrigger;
		long showTime = -1;
		bool disableTextureOnPress;
		public long defaultDisplayTime = 2000;
		bool caller_ends_display;

		void Start ()
		{
				img = checkedWordImage.GetComponent<UITexture> ();
				img.enabled = false;
				clickTrigger = checkedWordImage.GetComponent<BoxCollider> ();
				clickTrigger.enabled = false;
		}

		public void ShowImage (Texture2D newimg, long showTime)
		{
				this.showTime = showTime;
				SetAndEnableTexture (newimg);


		}

		public void ShowImage (Texture2D newimg, bool disableTextureOnPress, bool caller_ends_display=false)
		{
				this.caller_ends_display = caller_ends_display;
				if (newimg != null) {
						if (disableTextureOnPress) {
								this.disableTextureOnPress = disableTextureOnPress;
								SetAndEnableTexture (newimg);
								clickTrigger.enabled = true;
						} else
								ShowImage (newimg, defaultDisplayTime);
				}
		}

		void SetAndEnableTexture (Texture2D newImg)
		{
				img.mainTexture = newImg;
				img.enabled = true;
		}

		void OnPress (bool isPressed)
		{
				if (isPressed && disableTextureOnPress) {
						EndDisplay ();
				}
		}

		public void EndDisplay ()
		{

		    
				img.enabled = false;
				caller_ends_display = false;
				if (disableTextureOnPress) {
						disableTextureOnPress = false;
						clickTrigger.enabled = false;
				}
				if (showTime > 0) {
						showTime = -1;
				}

		}

		void Update ()
		{
				if (!caller_ends_display) {
						if (showTime > 0)
								showTime--;
						if (showTime == 0) {
								EndDisplay ();
						}
				}
				
		}

		public bool WordImageIsOnDisplay ()
		{
				return img.enabled;


		}


		




}
