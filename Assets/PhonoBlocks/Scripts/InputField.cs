using UnityEngine;
using System.Collections;

public class InputField : MonoBehaviour {
	public string stringToEdit="HELLO";
	void OnGUI ()
	{

		stringToEdit = GUI.TextField (new Rect (gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, 200, 20), stringToEdit, 25);

		
	}


}
