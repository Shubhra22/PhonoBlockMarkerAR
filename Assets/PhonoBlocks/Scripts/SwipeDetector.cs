using UnityEngine;
using System.Collections;
using System;

public class SwipeDetector : MonoBehaviour
{





		static float screenWidth = 767f;
		static float screenHeight = 340f;
		static Vector3 sMP;
		public static EventHandler MousePressed;
        
	
	
	
		// Update is called once per frame
		public static Vector2 GetTransformedMouseCoordinates ()
		{
				sMP = Input.mousePosition;
				sMP.x -= screenWidth;
				sMP.y -= screenHeight;
				sMP.x /= (screenWidth / 2f);
				sMP.y /= screenHeight;
				return sMP;
				//Vector3 pos = gameObject.transform.position;
				//gameObject.transform.position = sMP;
		
		}

		
		public enum Swipe
		{
				LEFT,
				RIGHT,
				NONE


		}
		public static Swipe swipeDirection = Swipe.NONE;
		public static SwipeDetector instance;
		bool mouseIsDownAndMovingAround = false;
		Vector2 startPos;
		float swipeStartTime;

		void Awake ()
		{
				instance = this;
		}

		void Update ()
		{
				if (mouseIsDownAndMovingAround) {


						float swipeTime = Time.time - swipeStartTime; //Time the touch stayed at the screen till now.
						float swipeDist = Mathf.Abs (Input.mousePosition.x - startPos.x); //Swipedistance

						if (Mathf.Sign (Input.mousePosition.x - startPos.x) == 1f) { //Swipe-direction, either 1 or -1.
					
								swipeDirection = Swipe.RIGHT;
									
					
						} else {
								swipeDirection = Swipe.LEFT;
									
								
						}
					 

				}



		}

		void OnPress (bool pressed) //Coroutine, wich gets Started in "Start()" and runs over the whole game to check for swipes
		{       
				//Loop. Otherwise we wouldnt check continoulsy ;-)
				//foreach (Touch touch in Input.touches) { //For every touch in the Input.touches - array...
				if (pressed) {
						EventHandler handler = MousePressed;
						if (handler != null) {
								handler (this, EventArgs.Empty);
						}
				}
				if (pressed && !mouseIsDownAndMovingAround) {	
						mouseIsDownAndMovingAround = true;
						startPos = Input.mousePosition;  //Position where the touch started
						swipeStartTime = Time.time; //The time it started
				}
				if (!pressed) {
						mouseIsDownAndMovingAround = false;
						swipeDirection = Swipe.NONE;
				}
	
		}




		

}