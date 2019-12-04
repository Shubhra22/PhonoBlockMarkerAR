using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class LetterSoundComponent : MonoBehaviour
{

    protected PhonotacticChecker.Phonotactics[] rules;
    protected bool violatesPhonotactics;
    protected int placeInWord;

    public bool flashVowel;


    public bool ViolatesPhonotactics
    {
        get
        {
            return violatesPhonotactics;
        }
        set
        {
            violatesPhonotactics = value;

            if (violatesPhonotactics)
                ApplyErrorColor();
        }
    }

    protected int soundType;

    public int SoundType
    {
        get
        {
            return soundType;
        }
    }

    public virtual string Sound()
    {
        switch (soundType)
        {
            case SILENT:
                return "silent";
            case R_CONTROLLED:
                return "r controlled";

        }
        return "none";

    }

    protected const int SILENT = -1;
    protected const int R_CONTROLLED = SILENT + 3;

    public void MakeRControlled()
    {
        soundType = R_CONTROLLED;
    }

    public void Silence()
    {
        soundType = SILENT;
    }

    public bool IsConsonantConsonantDigraphOrBlend
    {

        get
        {
            return this is ConsonantDigraph || this is Blend || this is Consonant;
        }

    }

    public bool LettersMatch(LetterSoundComponent other)
    {
        return LettersAre(other.asString);


    }

    public bool IsVowelOrVowelDigraph
    {

        get
        {
            return this is Vowel || this is VowelDigraph;
        }



    }

    public LetterSoundComponent(string asString, PhonotacticChecker.Phonotactics[] rules)
    {
        this.asString = asString;

        this.rules = rules;
    }

    string asString;

    public virtual string AsString
    {
        get
        {
            return asString;
        }


    }

    public bool LettersAre(string test)
    {
        return asString.Equals(test);


    }

    public char LetterAt(int c)
    {
        return AsString[c];
    }

    public int Length
    {
        get { return asString.Length; }

    }

    protected Color color = Color.white;

    public virtual Color GetColour()
    {

        return color;



    }

    public void ApplyColor()
    {
        if (!CheckForAndApplySpecialColor())
        {
            ApplyLetterSoundUnitColor();
            ModifyColorBySound();
        }

    }

    protected abstract void ApplyLetterSoundUnitColor();

    public bool TryApplyColor(Color suggestion)
    {
        if (CheckForAndApplySpecialColor())
            return false;
        color = suggestion;
        ModifyColorBySound();

        return true;

    }

    protected virtual bool CheckForAndApplySpecialColor()
    {
        if (violatesPhonotactics)
        {
            ApplyErrorColor();
            return true;
        }
        return false;
        //else, check if this unit is special.


    }
    //error colors get a special protocol, 
    //for composites-- unlike other colors that apply to the parent-
    //we by default will apply the error color to all children regardless of other considertions.
    public virtual void ApplyErrorColor()
    {
        //color = SessionsDirector.colourCodingScheme.GetErrorColor ();
    }

    protected virtual void ModifyColorBySound()
    {
        if (soundType == SILENT)
            color = SessionsDirector.colourCodingScheme.GetColourForSilent(asString[0]);

    }

    public void Update(UserWord context, int placeOfSelf, bool checkPhonotactics)
    {
        this.placeInWord = placeOfSelf;
        if (checkPhonotactics)
            CheckPhonotactics(context, placeOfSelf);
        ApplyColor();

    }

    public void CheckPhonotactics(UserWord context, int placeOfSelf)
    {
        foreach (PhonotacticChecker.Phonotactics rule in rules)
        {
            if (rule(context.LetterSoundUnits, this, placeOfSelf))
            {
                violatesPhonotactics = true;
                context.PhonotacticViolation(this, placeOfSelf); //alert the context (cache the fact that we're in error straight away)
                return; //once we break a single phonotactical rule, stop checking - has the form of "or"
            }
        }
            
        //if we got here, then we (currently) have not broken any rules.

        //only the affixes (which are represented by enduring objects, not objects that are renewed each time as the letters are)
        //could be in the position where we need to "change" the state from
        //violates (i.e., place "s" affix next to y)
        //to, does not violate (the user changes the y to an i).
        //in these cases, we need to notify the context (which woud have remembered that we violated them)
        //that the violation was fixed.
        //the context will respond by checking eaxch of its letters for a violation.
        if (violatesPhonotactics == true) //mostly in case this is an affix. if we fixed an error we'd made previously, then tell the context to check if all other errors are fixed (and we should switch).
            context.PhonotacticsWereFixed(this, placeOfSelf); //all letters "refresh" each time there is a change but the affix objects are enduring.
        violatesPhonotactics = false;
    }





}

public abstract class LetterSoundComposite : LetterSoundComponent
{


    protected List<LetterSoundComponent> children;

    public List<LetterSoundComponent> Children
    {
        get
        {
            return children;
        }
    }

    public LetterSoundComposite(string asString, PhonotacticChecker.Phonotactics[] rules) : base(asString, rules)
    {
        children = new List<LetterSoundComponent>();
    }

    public void AddChild(LetterSoundComponent child)
    {
        if (children == null)
            children = new List<LetterSoundComponent>();
        children.Add(child);

    }

    protected override void ApplyLetterSoundUnitColor()
    {
        ApplyColorToComposite();
        foreach (LetterSoundComponent child in children)
            child.TryApplyColor(GetColour());

    }

    public override void ApplyErrorColor()
    {
        //color = SessionsDirector.colourCodingScheme.GetErrorColor ();
        foreach (LetterSoundComponent child in children)
            child.ApplyErrorColor();
    }

    protected abstract void ApplyColorToComposite();


}

public abstract class Letter : LetterSoundComponent
{
    public Letter(string asString, PhonotacticChecker.Phonotactics[] rules) : base(asString, rules)
    {
    }




}

public class Consonant : Letter
{
    const int HARD = SILENT + 1;
    const int SOFT = SILENT + 2;


    //note: as of Min's study (fall 2015) we aren't distinguishing between hard and soft consonants. also note that
    //at the moment, effects on small components of digraphs (e.g., consonant letters) are overriding colours that are assigned to
    //larger units. for example having the "s" default to soft consonant colour was overriding the special colouration that s would have when it
    //appears in a consonant digraph. so in the future you will need to change how colour pritory works if you want to incoporate this
    public override string Sound()
    {
        switch (soundType)
        {

            case HARD:
                return "hard";
            case SOFT:
                return "soft";
        }
        return base.Sound();


    }

    public void MakeSoft()
    {
        soundType = SOFT;
    }

    public void MakeHard()
    {
        soundType = HARD;
    }

    public Consonant(string asString) : base(asString, SpeechSoundReference.GetRulesForConsonant(asString[0]))
    {

    }

    public Consonant(char asString) : base(asString + "", SpeechSoundReference.GetRulesForConsonant(asString))
    {

    }

    protected override void ApplyLetterSoundUnitColor()
    {
        color = SessionsDirector.colourCodingScheme.GetColorsForHardConsonant(LetterAt(0),placeInWord);
//        print(LetterAt(0)+ " in " + placeInWord);

    }

    protected override void ModifyColorBySound()
    {
        base.ModifyColorBySound();
        if (soundType == SOFT)
            color = SessionsDirector.colourCodingScheme.ModifyColorForSoftConsonant(color);

    }
}

public class Vowel : Letter
{
    const int LONG = SILENT + 1;
    const int SHORT = SILENT + 2;

    public override string Sound()
    {
        switch (soundType)
        {

            case LONG:
                return "long";
            case SHORT:
                return "short";
        }
        return base.Sound();


    }

    public void MakeLong()
    {
        soundType = LONG;

    }

    public void MakeShort()
    {
        soundType = SHORT;

    }

    public Vowel(string asString) : base(asString, SpeechSoundReference.GetRulesForVowel(asString[0]))
    {

        MakeLong();
    }

    public Vowel(char asString) : base(asString + "", SpeechSoundReference.GetRulesForVowel(asString))
    {

        MakeLong();
    }

    protected override void ApplyLetterSoundUnitColor()
    {
        color = SessionsDirector.colourCodingScheme.GetColorsForLongVowel(LetterAt(0));
        //color = SessionManager.activeColourScheme.GetColorsForLongVowel (LetterAt (0)); //for the experiment I'm not worrying about r controlled.
        //we are only distinguishing between long and short.

    }

    protected override void ModifyColorBySound()
    {
        base.ModifyColorBySound();
        if (soundType == SHORT)
        {
            color = SessionsDirector.colourCodingScheme.GetColorsForShortVowel(color);
            //				Debug.Log (SessionManager.activeColourScheme.label + " " + color);

        }


    }
}

public class Blank : Letter
{


    public Blank() : base(" ", SpeechSoundReference.BlankRules())
    {
    }

    protected override void ApplyLetterSoundUnitColor()
    {
        color = Color.white;

    }
}

public class Blend : LetterSoundComposite
{
    int preferredPosition;
    Color[] colors = new Color[2];
    int colorIdx = 1;

    public Blend(string asString, int preferredPosition) : base(asString, SpeechSoundReference.GetRulesForBlend(asString, preferredPosition))
    {
        this.preferredPosition = preferredPosition;
    }

    protected override void ApplyColorToComposite()
    {

        if (preferredPosition == SpeechSoundReference.INITIAL_BLEND)
        {
            color = SessionsDirector.colourCodingScheme.GetColorsForInitialBlends();
            SessionsDirector.colourCodingScheme.flashBlend = true;

        }
        else if (preferredPosition == SpeechSoundReference.FINAL_BLEND)
        {
            color = SessionsDirector.colourCodingScheme.GetColorsForFinalBlends();
            SessionsDirector.colourCodingScheme.flashBlend = true;

        }
        else
        {
            SessionsDirector.colourCodingScheme.flashBlend = true;
            color = SessionsDirector.colourCodingScheme.GetColorsForMiddleBlends();

        }
        /*
                        if (preferredPosition == SpeechSoundReference.INITIAL_BLEND) {
                                colors [0] = SessionsDirector.colourCodingScheme.GetColorsForInitialBlends (0);
                                colors [1] = SessionsDirector.colourCodingScheme.GetColorsForInitialBlends (1);
                        } else if (preferredPosition == SpeechSoundReference.FINAL_BLEND) {
                                colors [0] = SessionsDirector.colourCodingScheme.GetColorsForFinalBlends (0);
                                colors [1] = SessionsDirector.colourCodingScheme.GetColorsForFinalBlends (1);
                        } else {
                                colors [0] = SessionsDirector.colourCodingScheme.GetColorsForMiddleBlends (0);
                                colors [1] = SessionsDirector.colourCodingScheme.GetColorsForMiddleBlends (1);
                        }

        */

    }
    /*
		public override Color GetColour ()
		{
				colorIdx = 1 - colorIdx;
				return colors [colorIdx];
				
		}*/




}


public class VowelR : LetterSoundComposite
{
    public VowelR(string asString) : base(asString, SpeechSoundReference.GetRulesForConsonantDigraph(asString))
    {
    }

    protected override void ApplyColorToComposite()
    {
        color = SessionsDirector.colourCodingScheme.GetColorsForRControlledVowel();
        SessionsDirector.colourCodingScheme.flashRControl = true;
        //flashVowel = true;
        print("Came here and made flashvowel to" + flashVowel);

    }



}

public class ConsonantDigraph : LetterSoundComposite
{
    public ConsonantDigraph(string asString) : base(asString, SpeechSoundReference.GetRulesForConsonantDigraph(asString))
    {
    }

    protected override void ApplyColorToComposite()
    {
        
        color = SessionsDirector.colourCodingScheme.GetColorsForConsonantDigraphs();
        SessionsDirector.colourCodingScheme.flashConsonantDiagrah = true;

    }

}

public class VowelDigraph : LetterSoundComposite
{
    public VowelDigraph(string asString) : base(asString, SpeechSoundReference.GetRulesForVowelDigraph(asString))
    {
    }

    protected override void ApplyColorToComposite()
    {
        //color = (soundType == R_CONTROLLED ? SessionManager.activeColourScheme.GetColorsForRControlledVowel (LetterAt (0)) : color = SessionManager.activeColourScheme.GetColorsForVowelDigraphs ());
        color = SessionsDirector.colourCodingScheme.GetColorsForVowelDigraphs(); //for the experiment I'm not worrying about r controlled.
                                                                                 //we are only distinguishing between long and short.
        SessionsDirector.colourCodingScheme.flashVowelTeam = true;

    }


}

public class StableSyllable : LetterSoundComposite
{
    public StableSyllable(string asString) : base(asString, SpeechSoundReference.GetRulesForStableSyllable(asString))
    {
    }

    protected override void ApplyColorToComposite()
    {

        color = SessionsDirector.colourCodingScheme.GetColoursForSyllables(placeInWord);

    }



}






