using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorText : MonoBehaviour
{

    // Needed for applying color to the word history bar 
    // Method is called in LetterGridController class
    // Takes a plane text as input and color each letter based on the colors array
    public static string TextToColor(string text, Color[] colors)
    {
        string output = "";
        for (int i = 0; i < text.Length; i++)
        {
            output += "<color=#" + ColorUtility.ToHtmlStringRGBA(colors[i]) + ">"+text[i]+"</color>";
        }

        return output;
    }
    
}
