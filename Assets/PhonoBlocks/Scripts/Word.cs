using UnityEngine;
using System.Collections;

public class Word : MonoBehaviour
{

		string asString;

		public string AsString {
				get {
						return this.asString;

				}
	

		}

		Texture2D image;

		public Texture2D Image {
				get {
						return this.image;
			
				}
		
				set {
						this.image = value;
			
				}
		
		}

		AudioClip sound;

		public AudioClip Sound {
				get {
						return this.sound;
			
				}
		
				set {
						this.sound = value;
			
				}



		}

		public Word (string asString_)
		{
				asString = asString_;
				sound = (AudioClip)Resources.Load ("audio/words/" + asString, typeof(AudioClip));
		}




	                            







}
