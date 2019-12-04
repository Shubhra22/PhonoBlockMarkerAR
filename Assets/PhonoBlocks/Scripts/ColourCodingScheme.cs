using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public abstract class ColourCodingScheme : MonoBehaviour
{

    //requirement for all activities:
    //establish colours for all generally relevant LetterSoundComponents.
    //we can do so by adjusting the singleton (LetterSoundComponent Color Map)

    protected Color green;
    protected Color blue;
    protected Color red;
    protected Color pink;
    protected Color yellow;
    protected Color gray;
    public string label;
    protected int alternate = 0;


    public bool flashRControl;
    public bool flashVowelTeam;
    public bool flashConsonantDiagrah;
    public bool flashBlend;

    public List<char> firstLetterList;
    public List<char> secondLetterList;

    public ColourCodingScheme()
    {
        green = Color.green;
        green.r = (float)95 / (float)255;
        green.b = (float)127 / (float)255;
        green.g = (float)180 / (float)255;
        blue = Color.blue;
        blue.r = (float)105 / (float)255;
        blue.g = (float)210 / (float)255;
        blue.b = (float)231 / (float)255;
        red = Color.red;
        red.g = (float)58 / (float)255;
        red.b = (float)68 / (float)255;
        pink = Color.red;
        pink.r = (float)247 / (float)255;
        pink.g = (float)98 / (float)255;
        pink.b = (float)162 / (float)255;
        yellow = Color.yellow;
        yellow.r = (float)249 / (float)255;
        yellow.g = (float)249 / (float)255;
        yellow.b = (float)98 / (float)255;
        gray = Color.gray;
        gray.r = (float)(gray.r * 1.2);
        gray.g = gray.r;
        gray.b = gray.r;
    }

    public virtual Color GetColorsForOff()
    {
        return gray;

    }

    public virtual Color GetColorsForWholeWord()
    {
        return pink;

    }

    public virtual Color GetColorsForLongVowel(char vowel)
    {
        return Color.white;

    }

    public virtual Color GetColorsForHardConsonant(char consonant,int indexInWord = 0)
    {
        //print("Came here");
        if(indexInWord == 0)
            return blue;
        return Color.white;
    }

    public virtual Color GetColourForSilent(char letter)
    {
        return Color.white;
    }

    public virtual Color GetColorsForShortVowel(Color currentVowelColor)
    {
        return Color.white;
    }

    public virtual Color GetColorsForInitialBlends(int letterInBlendPos = 0)
    {
        return Color.white;
    }

    public virtual Color GetColorsForMiddleBlends(int letterInBlendPos = 0)
    {
        return Color.white;
    }

    public virtual Color GetColorsForFinalBlends(int letterInBlendPos = 0)
    {
        return Color.white;
    }

    public virtual Color GetColoursForSyllables(int indexInWord = 0)
    {
        return Color.white;
    }

    public virtual Color ModifyColorForSoftConsonant(Color color)
    {
        return Color.white;
    }

    public virtual Color GetColorsForConsonantDigraphs()
    {
        return Color.white;
    }

    public virtual Color GetColorsForVowelDigraphs()
    {
        return Color.white;
    }

    public virtual Color GetColorsForRControlledVowel()
    {

        return Color.white;
    }

    protected virtual Color AlternatingColours(int alternate)
    {
        //we basically distinguish between first and not first, at least for consonants and syllables.
        if (alternate == 0)
            return GetAlternateA();
        else
            return GetAlternateB();


    }

    protected virtual Color GetAlternateA()
    {
        return blue;
    }

    protected virtual Color GetAlternateB()
    {
        return green;
    }


}


//Colour coding schemes for Min's Study:
//also consonant digraphs
class ConsonantBlends : NoColour
{
    
    public ConsonantBlends() : base()
    {
        
        label = "Blends";

        var firstLetter = from elemets in SessionsDirector.instance.ConsonantBlend select elemets[0];
        firstLetterList = firstLetter.ToList();

        var blendLetter = from elemets in SessionsDirector.instance.ConsonantBlend select elemets[1];
        secondLetterList = blendLetter.ToList();
    }

    public override Color GetColorsForHardConsonant(char consonant, int indexInWord = 0)
    {
        //print("Came to COLORER Where " + consonant +" is " + firstLetterList.Contains(consonant) );

        foreach (var item in firstLetterList)
        {
//            print("*********" + item);
        }
        if((indexInWord == 0 && firstLetterList.Contains(consonant)) || (indexInWord == 1 && secondLetterList.Contains(consonant)))
            return blue;
        
        return Color.white;
    }

    public override Color GetColorsForConsonantDigraphs()
    {
        return green;
    }

    public override Color GetColorsForInitialBlends(int letterInBlendPos = 0)
    {
        return GetColorsForConsonantDigraphs ();
        //return AlternatingColours(letterInBlendPos);
    }

    public override Color GetColorsForMiddleBlends(int letterInBlendPos = 0)
    {
        return GetColorsForConsonantDigraphs();
        //return AlternatingColours (letterInBlendPos);
    }

    public override Color GetColorsForFinalBlends(int letterInBlendPos = 0)
    {
        return GetColorsForConsonantDigraphs();
        //return AlternatingColours (letterInBlendPos);
    }

}



class OpenClosedVowel : NoColour
{

    public OpenClosedVowel() : base()
    {
        label = "openClosedVowel";
    }


    public override Color GetColorsForShortVowel(Color currentVowelColor)
    {
        return yellow;
    }

    public override Color GetColorsForLongVowel(char vowel)
    {
        return yellow;
    }

    /* Min wants each consonant in the word to have a different colour. Most of the words she uses are CVC, so the rule
     * that we alternate blue and green should work*/


    public override Color GetColorsForHardConsonant(char consonant,int indexInWord = 0)
    {
        return AlternatingColours(indexInWord);
    }

    public override Color GetColorsForVowelDigraphs()
    {
        return yellow;
    }

    //public override Color GetColorsForOff()
    //{
    //    return yellow;

    //}

}

//changes to parent
//colour silent e black
//Magic e
class VowelInfluenceERule : NoColour
{

    public VowelInfluenceERule() : base()
    {
        label = "vowelInfluenceE";
    }

    public override Color GetColorsForLongVowel(char vowel)
    {
        return red;
    }

    public override Color GetColorsForShortVowel(Color currentVowelColor)
    {
        return yellow;
    }

    public override Color GetColorsForHardConsonant(char consonant,int indexInWord = 0)
    {
        return Color.white;
    }



    public override Color GetColourForSilent(char letter)
    {
        if (letter == 'e')
            return GetColorsForLongVowel('e'); //silent e colour matches long vowel it influences.
        return GetColorsForHardConsonant('*');
    }


}


//changes to parent
//colour consonant digraphs green
class ConsonantDigraphs : NoColour
{


    public override Color GetColorsForHardConsonant(char consonant,int indexInWord = 0)
    {
        if (((indexInWord == 0 || indexInWord ==2) && firstLetterList.Contains(consonant)) || ((indexInWord == 1 || indexInWord ==3) && secondLetterList.Contains(consonant)))
        {
            flashConsonantDiagrah = false;
            print("GREEEEEEEEEEEEEEEEEEEEN");
            return green;
        }
        return Color.white;
    }

    public ConsonantDigraphs() : base()
    {
        label = "consonantDigraphs";

        var firstLetter = from elemets in SessionsDirector.instance.ConsonantDigraph select elemets[0];
        firstLetterList = firstLetter.ToList();

        var blendLetter = from elemets in SessionsDirector.instance.ConsonantDigraph select elemets[1];
        secondLetterList = blendLetter.ToList();
    }

    public override Color GetColorsForConsonantDigraphs()
    {
        return green;
    }

    public override Color GetColorsForInitialBlends(int letterInBlendPos = 0)
    {
        //return GetColorsForConsonantDigraphs();
        return AlternatingColours(1);
    }

    public override Color GetColorsForMiddleBlends(int letterInBlendPos = 0)
    {
        //return GetColorsForConsonantDigraphs();
        return AlternatingColours (1);
    }

    public override Color GetColorsForFinalBlends(int letterInBlendPos = 0)
    {
        //return GetColorsForConsonantDigraphs();
        return AlternatingColours(1);
    }
}


//changes to parent
//colour r controlled vowels purple
class RControlledVowel : VowelDigraphs
{
    public RControlledVowel() : base()
    {
        label = "rControlledVowel";
    }

    public override Color GetColorsForRControlledVowel()
    {
        return red;
    }

    public override Color GetColorsForVowelDigraphs()
    {
        return gray;
    }

    public override Color GetColorsForShortVowel(Color currentVowelColor)
    {
        // red flash and off
        flashRControl = false;
        return red;
    }

    public override Color GetColorsForHardConsonant(char consonant, int indexInWord = 0)
    {
        if(consonant == 'r'&& indexInWord!=0)
        {
            flashRControl = false;
            return red;
        }
        return Color.white;
    }


}

//changes to parent
//colour vowel digraphs orange
class VowelDigraphs : NoColour
{
    public VowelDigraphs() : base()
    {
        label = "vowel Digraphs";
    }

    public override Color GetColorsForVowelDigraphs()
    {
        return red;
    }

    public override Color GetColorsForRControlledVowel()
    {
        return GetColorsForShortVowel(Color.black);
    }

    public override Color GetColorsForLongVowel(char vowel)
    {
        return GetColorsForShortVowel(Color.black);
    }

    public override Color GetColorsForShortVowel(Color currentVowelColor)
    {
        //red flash and off
        flashVowelTeam = false;
        return red;
    }
    public override Color GetColorsForHardConsonant(char consonant, int indexInWord = 0)
    {
        return Color.white;
    }



}

class SyllableDivision : NoColour
{
    public SyllableDivision() : base()
    {
        label = "syllableDivision";
    }

    public override Color GetColoursForSyllables(int indexInWord = 0)
    {
        return AlternatingColours(indexInWord);
    }

    public override Color GetColorsForLongVowel(char vowel)
    {
        return GetColorsForWholeWord();

    }

    public override Color GetColorsForHardConsonant(char consonant,int indexInWord = 0)
    {
        return GetColorsForWholeWord();
    }

    public override Color GetColourForSilent(char letter)
    {
        return GetColorsForWholeWord();
    }

    public override Color GetColorsForShortVowel(Color currentVowelColor)
    {
        return GetColorsForWholeWord();
    }

    public override Color GetColorsForInitialBlends(int letterInBlendPos = 0)
    {
        return GetColorsForWholeWord();
    }

    public override Color GetColorsForMiddleBlends(int letterInBlendPos = 0)
    {
        return GetColorsForWholeWord();
    }

    public override Color GetColorsForFinalBlends(int letterInBlendPos = 0)
    {
        return GetColorsForWholeWord();
    }

    public override Color ModifyColorForSoftConsonant(Color color)
    {
        return GetColorsForWholeWord();
    }

    public override Color GetColorsForConsonantDigraphs()
    {
        return GetColorsForWholeWord();
    }

    public override Color GetColorsForVowelDigraphs()
    {
        return GetColorsForWholeWord();
    }

    public override Color GetColorsForRControlledVowel()
    {

        return GetColorsForWholeWord();
    }
}




//different activities can "opt out" of certain distinctions, and add new ones.
public class NoColour : ColourCodingScheme
{






}

