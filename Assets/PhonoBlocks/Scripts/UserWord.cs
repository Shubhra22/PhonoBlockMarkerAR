using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserWord : PhonoBlocksController, IEnumerable
{

    List<LetterSoundComponent> letterSoundUnits;

    public List<LetterSoundComponent> LetterSoundUnits
    {
        get
        {
            return letterSoundUnits;
        }
    }

    int indexOfLastNonBlankLetter;

    public int IndexOfLastNonBlankLetter
    {
        get
        {
            return indexOfLastNonBlankLetter;
        }
    }

    bool violatesPhonotactics;

    public bool ViolatesPhonotactics
    {
        get
        {
            return violatesPhonotactics;
        }
    }


    //discoveered while the components were being checked
    public void PhonotacticViolation(LetterSoundComponent perpetrator, int idxOfSelf)
    {

        violatesPhonotactics = true;

    }

    public void PhonotacticsWereFixed(LetterSoundComponent fix, int idxOfSelf)
    {       //refreshes, since something changed (we still need to confirm that the others are not in error.
            //atm, this only really matters for the affixes. if an affix is placed erroneously, it violates the phonotactics of 
            //the word, but none of its components.
        violatesPhonotactics = false;
        foreach (LetterSoundComponent l in letterSoundUnits)
            violatesPhonotactics = violatesPhonotactics || l.ViolatesPhonotactics;
    }

    public void ApplyColoursToLetterSoundComponents(bool checkPhonotactics)
    {
        int idx = 0;
        foreach (LetterSoundComponent l in letterSoundUnits)
        {
            if (!l.ViolatesPhonotactics)
            { //it may already be pegged from the syllable division performed in syllaber factory class.
                l.Update(this, idx, checkPhonotactics);

            }
            else
                violatesPhonotactics = true;
            idx++;

        }
    }

    public UserWord(List<LetterSoundComponent> letterSoundUnits)
    {
        this.letterSoundUnits = letterSoundUnits;



    }

    public int Count
    {
        get
        {
            return letterSoundUnits.Count;
        }
    }

    public void Add(LetterSoundComponent l)
    {
        letterSoundUnits.Add(l);

    }

    public LetterSoundComponent Get(int idx)
    {
        return letterSoundUnits[idx];

    }

    public IEnumerator GetEnumerator()
    {
        return letterSoundUnits.GetEnumerator();


    }

    public void RecordIndexOfLastNonBlankLetter()
    {
        indexOfLastNonBlankLetter = letterSoundUnits.Count - 1;


    }









}
