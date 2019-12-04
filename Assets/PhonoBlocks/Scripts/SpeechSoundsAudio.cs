using UnityEngine;
using System.Collections;

public class SpeechSoundsAudio : MonoBehaviour {

	public AudioClip[] letterNames; //imdex by ascii code for letter (scaled down; same as the letter image map)



	//consonant LE
	public AudioClip ble;


	public static SpeechSoundsAudio instance;


	void Awake(){
	
	 instance=this;
	}

	public AudioClip LetterName(char letter){
		int idx = (int)letter;
		//lower case letter
		if (idx > 96&&idx<123)
						idx -= 97;
				else if(idx>64&&idx<91) //upper case letter
						idx -= 65;
		else idx=0; //not a letter.
		return letterNames[idx];

	}


}
