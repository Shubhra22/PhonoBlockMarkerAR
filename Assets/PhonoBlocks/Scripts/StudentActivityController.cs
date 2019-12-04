using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class StudentActivityController : PhonoBlocksController
{

    enum State
    {
        PLACE_INITIAL_LETTERS,
        MAIN_ACTIVITY,
        REMOVE_ALL_LETTERS,

    }

    State state = State.PLACE_INITIAL_LETTERS;
    LockedPositionHandler lockedPositionHandler;
    HintController hintController;
    ArduinoLetterController arduinoLetterController;
    public Problem currProblem;

    static StudentActivityController instance;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else if(instance!=this)
        {
            Destroy(this.gameObject);
        }
    }

    public bool StringMatchesTarget(string s)
    {
        return s.Equals(currProblem.TargetWord(true));

    }



    char[] usersMostRecentChanges;
    AudioClip excellent;
    AudioClip incorrectSoundEffect;
    AudioClip notQuiteIt;
    AudioClip offerHint;
    AudioClip youDidIt;
    AudioClip correctSoundEffect;
    AudioClip removeAllLetters;
    AudioClip triumphantSoundForSessionDone;

    public string UserChangesAsString
    {
        get
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < usersMostRecentChanges.Length; i++)
                result.Append(usersMostRecentChanges[i]);

            return result.ToString();

        }

    }

    public void Reset()
    {
        currProblem = null;
        ProblemsRepository.instance.Reset();
    }

    public void Initialize(GameObject hintButton, ArduinoLetterController arduinoLetterController)
    {
        this.arduinoLetterController = arduinoLetterController;
        usersMostRecentChanges = new char[UserInputRouter.numOnscreenLetterSpaces];

        lockedPositionHandler = gameObject.GetComponent<LockedPositionHandler>();

        hintController = gameObject.GetComponent<HintController>();
        hintController.Initialize(hintButton);

        SetUpNextProblem();



        excellent = InstructionsAudio.instance.excellent;
        incorrectSoundEffect = InstructionsAudio.instance.incorrectSoundEffect;
        notQuiteIt = InstructionsAudio.instance.notQuiteIt;
        offerHint = InstructionsAudio.instance.offerHint;
        youDidIt = InstructionsAudio.instance.youDidIt;
        correctSoundEffect = InstructionsAudio.instance.correctSoundEffect;
        removeAllLetters = InstructionsAudio.instance.removeAllLetters;

        triumphantSoundForSessionDone = InstructionsAudio.instance.allDoneSession;
    }

    public void SetUpNextProblem()
    {

        //get the next specific problem from the ProblemType class
        ClearSavedUserChanges();
        hintController.Reset();

        currProblem = ProblemsRepository.instance.GetNextProblem();

        StudentsDataHandler.instance.RecordActivityTargetWord(currProblem.TargetWord(false));


        lockedPositionHandler.ResetForNewProblem();
        lockedPositionHandler.RememberPositionsThatShouldNotBeChanged(currProblem.InitialWord, currProblem.TargetWord(false).Trim());

        arduinoLetterController.ReplaceEachLetterWithBlank();
        arduinoLetterController.PlaceWordInLetterGrid(currProblem.InitialWord, true);



        arduinoLetterController.UpdateDefaultColoursAndSoundsOfLetters(false,false,-1);
        arduinoLetterController.LockAllLetters();





        userInputRouter.RequestTurnOffImage();

        hintController.DeActivateHintButton();

        PlayInstructions(); //dont bother telling to place initial letters during assessment mode


        state = State.PLACE_INITIAL_LETTERS;


        //In case the initial state is already correct (...which happens when the user needs to build the word "from scratch". This makes it so
        //we don;t need to trigger the check by adding a "blank"!
        ChangeProblemStateIfAllLockedPositionsAHaveCorrectCharacter();


        //




        //april 20; added this so that we only show as many lines as there are letters in the current word.
        arduinoLetterController.activateLinesBeneathLettersOfWord(currProblem.TargetWord(true));

    }

    public void PlayInstructions()
    {

        currProblem.PlayCurrentInstruction();


    }

    void ClearSavedUserChanges()
    {
        for (int i = 0; i < usersMostRecentChanges.Length; i++)
        {
            usersMostRecentChanges[i] = ' ';

        }
    }

    bool CurrentStateOfLettersMatches(string targetLetters)
    {

        for (int i = 0; i < usersMostRecentChanges.Length; i++)
        {
            if (i >= targetLetters.Length)
            {
                if (usersMostRecentChanges[i] != ' ')
                    return false;
            }
            else if (usersMostRecentChanges[i] != targetLetters[i])
                return false;
        }
        return true;

    }

    public void ChangeProblemStateIfAllLockedPositionsAHaveCorrectCharacter()
    {

        if (state == State.PLACE_INITIAL_LETTERS)
        {
            if (CurrentStateOfLettersMatches(currProblem.InitialWord))
                BeginMainProblemState();

        }
        if (state == State.REMOVE_ALL_LETTERS)
        {

            if (CurrentStateOfLettersMatches(currProblem.TargetWord(false)))
            {

                HandleEndOfActivity();
            }

        }

    }

    void HandleEndOfActivity()
    {
        if (ProblemsRepository.instance.AllProblemsDone())
        {
            StudentsDataHandler.instance.UpdateUserSessionAndWriteAllUpdatedDataToPlayerPrefs();
            AudioSourceController.PushClip(triumphantSoundForSessionDone);
            ProblemsRepository.instance.Reset();

        }
        else
        {
            SetUpNextProblem();
        }

    }

    void BeginMainProblemState()
    {
        arduinoLetterController.UnLockAllLetters(); //we do this to unlock the letters that are outside the range of the initial word.
                                                    //during the initial stage only the letters in the initial word will absolutely be locked.
        state = State.MAIN_ACTIVITY;

    }

    public void UserRequestsHint()
    {
        if (hintController.UsedLastHint())
        {
            currProblem.PlayAnswer();
            arduinoLetterController.hint = false;
            arduinoLetterController.showingResult= true;
            arduinoLetterController.PlaceWordInLetterGrid(currProblem.TargetWord(false),false);
            arduinoLetterController.UpdateDefaultColoursAndSoundsOfLetters(true, false, -1);
            //arduinoLetterController.UpdateDefaultColoursAndSoundsOfLetters(true, false, -1);
           // CurrentProblemCompleted(false);
        }
        else
        {
            hintController.ProvideHint(currProblem);
            Debug.Log("The User Hint came here");
        }


    }

    public void HandleNewArduinoLetter(char letter, int atPosition)
    {
        //if (LetterIsActuallyNew(letter, atPosition))
        //{
            bool positionWasLocked = lockedPositionHandler.IsLocked(atPosition);

            //we treat all positions as "locked" when the state is the end of the activity.
            if (positionWasLocked || state == State.REMOVE_ALL_LETTERS)
            {
                lockedPositionHandler.HandleChangeToLockedPosition(atPosition, letter, currProblem.TargetWord(false), usersMostRecentChanges, arduinoLetterController);
            }

            if (state == State.PLACE_INITIAL_LETTERS)
                arduinoLetterController.UnLockASingleLetter(atPosition);



            RecordUsersChange(atPosition, letter);

            ChangeProblemStateIfAllLockedPositionsAHaveCorrectCharacter();

//            print(currProblem.TargetWord(true) + " Input = " + letter + " at " + atPosition);

            // This solution will work if 
            // Check for all the letters if they r in perfect pos
            // 
            if(atPosition < currProblem.TargetWord(true).Length)
            {
                if (currProblem.TargetWord(true)[atPosition] == letter) // Right Letter was placed in right place
                {
                    if(arduinoLetterController.Contains(atPosition))
                    {
                        arduinoLetterController.Remove(atPosition);
                    }
                    arduinoLetterController.UpdateDefaultColoursAndSoundsOfLetters(state == State.PLACE_INITIAL_LETTERS, false, -1);

                } 

                else
                {
                    if(letter !=' ') // if the input letter is not empty that means if the letter is not removed 
                    {
                        arduinoLetterController.Add(atPosition);
                    }
                    arduinoLetterController.UpdateDefaultColoursAndSoundsOfLetters(state == State.PLACE_INITIAL_LETTERS, true, atPosition);

                }
            }
        //}
    }

    /// <summary>
    /// Letters the is actually new.
    /// </summary>
    /// <returns><c>true</c>, if the user actually changed the letter, <c>false</c> otherwise, i.e., if we're reading a "new value" from an arduino circuit or input error.</returns>
    /// <param name="letter">Letter.</param>
    /// <param name="atPosition">At position.</param>
    bool LetterIsActuallyNew(char letter, int atPosition)
    {
        return usersMostRecentChanges[atPosition] != letter;


    }

    bool PositionIsOutsideBoundsOfTargetWord(int wordRelativeIndex)
    {
        return wordRelativeIndex >= currProblem.TargetWord(true).Length;
    }

    public virtual void HandleSubmittedAnswer()
    {
        StudentsDataHandler.instance.LogEvent("submitted_answer", UserChangesAsString, currProblem.TargetWord(false));

        currProblem.IncrementTimesAttempted();

        if (IsSubmissionCorrect())
        {
            //TO DO!!! then if this was the first time that student submitted an answer (get the data from the current student object)
            //then play the good hint else play the less good hint
            AudioSourceController.PushClip(correctSoundEffect);
            if (currProblem.TimesAttempted > 1)
                AudioSourceController.PushClip(youDidIt);
            else
                AudioSourceController.PushClip(excellent);
            currProblem.PlayAnswer();
            CurrentProblemCompleted(true);

        }
        else
        {
            HandleIncorrectAnswer();

        }

    }

    protected void HandleIncorrectAnswer()
    {

        AudioSourceController.PushClip(incorrectSoundEffect);

        if (!hintController.HintButtonActive())
        {
            hintController.ActivateHintButton();
            AudioSourceController.PushClip(notQuiteIt);
            AudioSourceController.PushClip(offerHint);
        }

        hintController.AdvanceHint();

    }

    public void CurrentProblemCompleted(bool userSubmittedCorrectAnswer)
    {


        state = State.REMOVE_ALL_LETTERS;

        currProblem.SetTargetWordToEmpty();
        //userInputRouter.AddCurrentWordToHistory(false);
        //arduinoLetterController.LockAllLetters ();

        userInputRouter.RequestDisplayImage(currProblem.TargetWord(true), false, true);

        bool solvedOnFirstTry = currProblem.TimesAttempted == 1;
        if (solvedOnFirstTry)
        {

            userInputRouter.DisplayNewStarOnScreen(ProblemsRepository.instance.ProblemsCompleted - 1);

        }


        StudentsDataHandler.instance.RecordActivitySolved(userSubmittedCorrectAnswer, UserChangesAsString, solvedOnFirstTry);

        StudentsDataHandler.instance.SaveActivityDataAndClearForNext(currProblem.TargetWord(false), currProblem.InitialWord);



    }

    public void RecordUsersChange(int position, char change)
    {

        usersMostRecentChanges[position] = change;


    }

    protected bool IsSubmissionCorrect()
    {
        string target = currProblem.TargetWord(true);

        bool result = CurrentStateOfLettersMatches(target);


        return result;

    }

}
