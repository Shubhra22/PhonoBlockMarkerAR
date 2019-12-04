using UnityEngine;
using System.Collections;

public class UserStarGridController : PhonoBlocksController
{

		public GameObject userStarsGridOb;
		public Texture2D userStarImg;
		public Texture2D userStarOutlineImg;
		public int starWidth;
		public int starHeight;
		int timesToFlash = 4;
		int flashCounter;
		float secondsDelayBetweenFlashes = .20f;
		UITexture toFlash;

		public void Initialize ()
		{
				if (starWidth == 0 || starHeight == 0) //you can specify dimensions for the image that are different from those of the grid.
						MatchStarImageToGridCellDimensions (); //but if nothing is specified it defaults to make it the same size as the grid cells.


				PlaceUserStarOutlinesInGrid (); 

		}

		void PlaceUserStarOutlinesInGrid ()
		{
				int numStars = ProblemsRepository.instance.PROBLEMS_PER_SESSION;
				for (int i=0; i<numStars; i++) {
						CreateStarCellInGrid ();

				}
	
				userStarsGridOb.GetComponent<UIGrid> ().Reposition ();
		}

		public void AddNewUserStar (bool flash, int at)
		{
				UITexture newCellTexture = userStarsGridOb.transform.GetChild (at).GetComponent<UITexture> ();
				newCellTexture.mainTexture = userStarImg;
				userStarsGridOb.GetComponent<UIGrid> ().Reposition ();
				if (flash) {
						toFlash = newCellTexture;
						StartCoroutine ("Flash");
				}

		}

		void MatchStarImageToGridCellDimensions ()
		{
				UIGrid grid = userStarsGridOb.GetComponent<UIGrid> ();
				starWidth = (int)grid.cellWidth;
				starHeight = (int)grid.cellHeight;
		
		}

		public UITexture CreateStarCellInGrid ()
		{      
				Texture2D tex2dCopy = CopyAndScaleTexture (starWidth, starHeight, userStarOutlineImg);
				UITexture ut = NGUITools.AddChild<UITexture> (userStarsGridOb);
				ut.material = new Material (Shader.Find ("Unlit/Transparent Colored"));
				ut.shader = Shader.Find ("Unlit/Transparent Colored");
				ut.mainTexture = tex2dCopy;
			    
				ut.MakePixelPerfect ();
				return ut;
			
		}

		Texture2D CopyAndScaleTexture (float w, float h, Texture tex2D)
		{
				Texture2D tex2dCopy = Instantiate (tex2D) as Texture2D;
				TextureScale.Bilinear (tex2dCopy, (int)w, (int)h);
				return tex2dCopy;
		}

		public IEnumerator Flash ()
		{
		
				int mod_To_end_on = (timesToFlash % 2 == 0 ? 1 : 0);
		
				while (flashCounter<timesToFlash) {
			
						if (flashCounter % 2 == mod_To_end_on) {
								toFlash.color = Color.white;
				
				
						} else {
								toFlash.color = Color.red;
				
						}
						flashCounter++;
			
						yield return new WaitForSeconds (secondsDelayBetweenFlashes);
				}
		
				flashCounter = 0;
		
		
		
		
		}
}
