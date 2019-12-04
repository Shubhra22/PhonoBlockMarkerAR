using UnityEngine;
using System.Collections;

public class HintController : PhonoBlocksController
{
    StudentActivityController studentActivityController;
    public AudioClip sound_out_word;
    GameObject hintButton;
    int currHintIdx = -1;
    public const int NUM_HINTS = 2;

    public void Initialize(GameObject hintButton)
    {

        this.hintButton = hintButton;
        studentActivityController = gameObject.GetComponent<StudentActivityController>();
        sound_out_word = InstructionsAudio.instance.soundOutTheWord;
    }

    public void Reset()
    {
        currHintIdx = -1;

    }

    public void DeActivateHintButton()
    {

        hintButton.SetActive(false);
    }

    public void ActivateHintButton()
    {

        if (!hintButton.activeSelf)
            hintButton.SetActive(true);

    }

    public bool HintButtonActive()
    {
        return hintButton.activeSelf;

    }

    public void ProvideHint(Problem currProblem)
    {      //maybe make this a couroutine that can "iterate" thru each hint step
           //each of which is an audio file, except for the last which involves a visual change as wellS

        //userInputRouter.DisplayLettersOf(currProblem.TargetWord(false));

        print("****"+currHintIdx);
        switch (currHintIdx)
        {
            case 0:
                currProblem.PlaySoundedOutWord();
                userInputRouter.DisplayLettersOf(currProblem.TargetWord(false));

                break;

            case 1:
                currProblem.PlaySoundedOutWord ();
               // AudioSourceController.PushClip(sound_out_word);
                userInputRouter.DisplayLettersOf(currProblem.TargetWord(false));

                break;


        }
        StudentsDataHandler.instance.LogEvent("requested_hint", currHintIdx + "", "NA");

    }

    public bool UsedLastHint()
    {
        return currHintIdx == NUM_HINTS;
    }

    public bool OnLastHint()
    {

        return currHintIdx == NUM_HINTS - 1;
    }

    public void AdvanceHint()
    {

        if (currHintIdx < NUM_HINTS)
            currHintIdx++;
    }


}
