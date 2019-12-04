using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections;

public class ArduinoLetterController : PhonoBlocksController
{

    public String EMPTY_USER_WORD;
    List<InteractiveLetter> lettersToFlash = new List<InteractiveLetter>();
    private StringBuilder currUserControlledLettersAsStringBuilder = new StringBuilder(); //maintains this along with the letter bar so that it's easy to quickly update and get the new colours.
    public List<int> wrongIndexList = new List<int>();
    public bool hint;
    public bool showingResult;

    static ArduinoLetterController instance;

    public static ArduinoLetterController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ArduinoLetterController>();
               
            }
            return instance;
        }
        
    }

    Dictionary<int, Color> currentUsedColor = new Dictionary<int, Color>();
    string tempLetters;
    public string CurrentUserControlledLettersAsString
    {
        get
        {
            return currUserControlledLettersAsStringBuilder.ToString();
        }

        set
        {
            CurrentUserControlledLettersAsString = value;
        }

    }

    private StringBuilder selectedUserControlledLettersAsStringBuilder;

    public string SelectedUserControlledLettersAsString
    {
        get
        {
            return selectedUserControlledLettersAsStringBuilder.ToString();
        }

    }

    public static int startingIndexOfUserLetters;
    public static int endingIndexOfUserLetters;
    LetterGridController letterGrid;
    ArduinoUnityInterface tangibleLetters;
    public GameObject letterGridControllerGO;






    bool firstLetterBlink = true;
    bool isThistheFirstLetter = true;
    int expectedFirstLetterPos = 0;
    int expectedSecondLetterPos = 1;

    public int StartingIndex
    {
        get
        {
            return startingIndexOfUserLetters;
        }
        set
        {
            startingIndexOfUserLetters = value;


        }
    }

    public int EndingIndex
    {
        get
        {
            return endingIndexOfUserLetters;
        }
        set
        {
            endingIndexOfUserLetters = value;

        }
    }

    private int maxUserLetters;

    public int MaxArduinoLetters
    {
        get
        {
            return maxUserLetters;
        }


        set
        {
            maxUserLetters = value;

        }

    }

    public void Initialize(int startingIndexOfArduinoLetters, int endingIndexOfArduinoLetters, ArduinoUnityInterface tangibleLetters)
    {
        StartingIndex = startingIndexOfArduinoLetters;
        EndingIndex = endingIndexOfArduinoLetters;
        maxUserLetters = EndingIndex + 1 - StartingIndex;
        for (int i = 0; i < maxUserLetters; i++)
            currUserControlledLettersAsStringBuilder.Append(" ");

        EMPTY_USER_WORD = currUserControlledLettersAsStringBuilder.ToString();
        selectedUserControlledLettersAsStringBuilder = new StringBuilder(EMPTY_USER_WORD);

        letterGrid = letterGridControllerGO.GetComponent<LetterGridController>();
        letterGrid.InitializeBlankLetterSpaces(maxUserLetters);
        Debug.Log("Letter grid is not Null" + letterGrid);
        AssignInteractiveLettersToTangibleCounterParts();
        InteractiveLetter.LetterSelectedDeSelected += LetterSelectDeselect;
    }




    //invoked by the arduino and the keyboard on screen
    public void ReceiveNewUserInputLetter(char newLetter, int atPosition)
    {
        StudentsDataHandler.instance.LogEvent("change_letter", newLetter + "", atPosition + "");


        if (atPosition < maxUserLetters && atPosition >= StartingIndex)
        {
            if (IsUpper(newLetter))
                newLetter = ToLower(newLetter);
            
            //...automatically remember the change but don't necessarily update the colours.
            ChangeTheLetterOfASingleCell(atPosition, newLetter, false);
            userInputRouter.HandleNewUserInputLetter(newLetter,
                                          atPosition, this);
        }
    }

    public void Add(int index)
    {
        if(!wrongIndexList.Contains(index))
            wrongIndexList.Add(index);
    }

	public void Remove(int index)
	{
        if (wrongIndexList.Contains(index))
            wrongIndexList.Remove(index);
	}

    public bool Contains(int value)
    {
        return wrongIndexList.Contains(value);
    }

	//unlocks the letter at position atPosition.
	//the effect of unlocking a letter is to change its colour from the locked colour to whatever the colour of that letter should be
	//given then other letters in the word
	public void UnLockASingleLetter(int atPosition)
    {
        GameObject nthArduinoControlledLetter = letterGrid.GetLetterCell(atPosition);
        nthArduinoControlledLetter.GetComponent<InteractiveLetter>().UnLock();


    }

    public void PlaceWordStudentMode(int index)
    {
        string initWord = ProblemsRepository.instance.GetNextProblem().initialWord;
        if(initWord[index]!= ' ')
        {
            ChangeTheLetterOfASingleCell(index, initWord[index], LetterImageTable.instance.GetLetterOutlineImageFromLetter(' '));
        }
        else
        {
            ChangeTheImageOfASingleCell(index, LetterImageTable.instance.getBlankLetterImage());
        }
    }

    //locks the letter at position atPosition.
    //the effect of locking a letter is to make that letter appear black
    //instead of appearing in its usual colour.
    //letters that are locked will have their default colours updated
    //but this class will not tell the letters to instantly assume their new colour
    public void LockASingleLetter(int atPosition)
    {

        GameObject nthArduinoControlledLetter = letterGrid.GetLetterCell(atPosition);
        nthArduinoControlledLetter.GetComponent<InteractiveLetter>().Lock();


    }

    public void LockAllLetters()
    {

        List<InteractiveLetter> letters = letterGrid.GetLetters(false);

        foreach (InteractiveLetter il in letters)
        {
            il.Lock();

        }

    }

    public void UnLockAllLetters()
    {
        List<InteractiveLetter> letters = letterGrid.GetLetters(false);
        foreach (InteractiveLetter il in letters)
        {
            il.UnLock();
        }

    }

    public void ChangeTheLetterOfASingleCell(int atPosition, char newLetter, bool isStudentInitialization)
    {
        SaveNewLetterInStringRepresentation(newLetter, atPosition, currUserControlledLettersAsStringBuilder);
        letterGrid.UpdateLetter(atPosition, newLetter + "", isStudentInitialization);

    }



    public void ChangeDisplayColourOfCells(Color newColour, bool onlySelected = false, int start = -1, int count = 7)
    {
        start = (start < StartingIndex ? StartingIndex : start);
        count = (count > MaxArduinoLetters ? MaxArduinoLetters : count);
        if (!onlySelected)
        {
            for (int i = start; i < count; i++)
            {
                ChangeDisplayColourOfASingleCell(i, newColour);

            }
        }
        else
        {

            for (int i = start; i < count; i++)
            {
                if (selectedUserControlledLettersAsStringBuilder[i] != ' ')
                    ChangeDisplayColourOfASingleCell(i, newColour);
            }

        }
    }

    public void ChangeDisplayColourOfASingleCell(int atPosition, Color newColour)
    {
        letterGrid.UpdateLetter(atPosition, newColour, false);
    }

    public void ChangeTheImageOfASingleCell(int atPosition, Texture2D newImage)
    {
        InteractiveLetter i = letterGrid.GetInteractiveLetter(atPosition);
        i.SwitchImageTo(newImage);


    }

    public void RevertLettersToDefaultColour(bool onlySelected = false, int start = -1, int count = 7)
    {
        start = (start < StartingIndex ? StartingIndex : start);
        count = (count > MaxArduinoLetters ? MaxArduinoLetters : count);
        if (!onlySelected)
        {
            for (int i = start; i < count; i++)
            {
                RevertASingleLetterToDefaultColour(i);
            }
        }
        else
        {
            for (int i = start; i < count; i++)
            {
                if (selectedUserControlledLettersAsStringBuilder[i] != ' ')
                    RevertASingleLetterToDefaultColour(i);

            }

        }

    }

    public void RevertASingleLetterToDefaultColour(int atPosition)
    {
        InteractiveLetter l = letterGrid.GetLetterCell(atPosition).GetComponent<InteractiveLetter>();
        l.RevertToDefaultColour();
    }
    //updates letters and images of letter cells
    public void PlaceWordInLetterGrid(string word, bool isStudentInitialization)
    {
        for (int i = 0, j = startingIndexOfUserLetters; i < word.Length; i++, j++)
        {
            ChangeTheLetterOfASingleCell(j, word.Substring(i, 1)[0], isStudentInitialization);

        }
        //UpdateLetterBarIfPositionAndLetterSpecified();

    }

    public void activateLinesBeneathLettersOfWord(string word)
    {
        letterGrid.setNumVisibleLetterLines(word.Length);
    }

    //just updates the display images of the cells
    public void DisplayWordInLetterGrid(string word)
    {

        for (int i = 0, j = startingIndexOfUserLetters; i < word.Length; i++, j++)
        {

            ChangeTheImageOfASingleCell(j, LetterImageTable.instance.GetLetterImageFromLetter(word.Substring(i, 1)[0], false,' '));

        }


    }

    public IEnumerator DisplayEachLetterSequencially(string word)
    {
        tempLetters = CurrentUserControlledLettersAsString;
        ReplaceEachLetterWithBlank();
        showingResult = true;
        for (int i = 0, j = startingIndexOfUserLetters; i < word.Length; i++, j++)
        {

            ChangeTheImageOfASingleCell(j, LetterImageTable.instance.GetLetterImageFromLetter(word.Substring(i, 1)[0], false, ' '));
            UpdateDefaultColoursAndSoundsOfLetters(true, false, -1);
            currUserControlledLettersAsStringBuilder[i] = tempLetters[i];
            yield return new WaitForSeconds(0.5f);
        }
        //PlaceWordInLetterGrid(word, false);
        UpdateDefaultColoursAndSoundsOfLetters(true, false, -1);
        showingResult = false;
        print("************"+tempLetters + "********" + currUserControlledLettersAsStringBuilder.ToString());
        //CurrentUserControlledLettersAsString = tempLetters;
    }

    public void ReplaceEachLetterWithBlank()
    {

        PlaceWordInLetterGrid(EMPTY_USER_WORD, false);
    }

    public void UpdateDefaultColoursAndSoundsOfLetters(bool flash, bool wrongLetterStudentMode, int wrongCellNumber)
    {

        UserWord newLetterSoundComponents = GetNewColoursAndSoundsFromDecoder(letterGrid);
        AssignNewColoursAndSoundsToLetters(newLetterSoundComponents, letterGrid, flash, wrongLetterStudentMode, wrongCellNumber);
        //return newLetterSoundComponents;
    }

    public List<InteractiveLetter> GetAllUserInputLetters(bool skipBlanks)
    {

        return letterGrid.GetLetters(skipBlanks);

    }

    public string GetUserControlledLettersAsString(bool onlySelected)
    {
        if (onlySelected)
            return SelectedUserControlledLettersAsString;
        return CurrentUserControlledLettersAsString;

    }

    void SaveNewLetterInStringRepresentation(char letter, int position, StringBuilder stringRepresentation)
    {

        //replace character that was at l with new character
        stringRepresentation.Remove(position, 1);
        stringRepresentation.Insert(position, letter);

    }

    public bool IsBlank(int indexInLetterGrid)
    {
        indexInLetterGrid -= startingIndexOfUserLetters; //re-scale to the indexes of the string that represents arduino letters only.

        return currUserControlledLettersAsStringBuilder.ToString()[indexInLetterGrid] == ' ';
    }

    bool IsUpper(char letter)
    {
        int asInt = (int)letter;
        return asInt > 64 && asInt < 91;


    }

    //97-122 lower case; 65-> upper case
    char ToLower(char newLetter)
    {
        return (char)((int)newLetter + 32);


    }

    public bool NoUserControlledLetters()
    {

        return currUserControlledLettersAsStringBuilder.ToString().Equals(EMPTY_USER_WORD);
    }

    UserWord GetNewColoursAndSoundsFromDecoder(LetterGridController letterGridController)
    {

        return LetterSoundComponentFactoryManager.Decode(GetUserControlledLettersAsString(false), SessionsDirector.instance.IsSyllableDivisionMode);

    }


    void AssignInteractiveLettersToTangibleCounterParts()
    {
        int indexOfLetterBarCell = startingIndexOfUserLetters;
        for (; indexOfLetterBarCell <= endingIndexOfUserLetters; indexOfLetterBarCell++)
        {
            GameObject letterCell = letterGrid.GetLetterCell(indexOfLetterBarCell);

            letterCell.GetComponent<InteractiveLetter>().IdxAsArduinoControlledLetter = ConvertScreenToArduinoIndex(indexOfLetterBarCell);//plus 1 because the indexes are shifted.
        }
    }

    int ConvertScreenToArduinoIndex(int screenIndex)
    { //arduino starts counting at 1
        return screenIndex + 1;
    }

    void AssignNewColoursAndSoundsToLetters(UserWord letterSoundComponents, LetterGridController letterGridController, bool flash, bool wrongLetterStudentMode, int wrongCellNumber)
    {

        int indexOfLetterBarCell = startingIndexOfUserLetters;

        foreach (LetterSoundComponent p in letterSoundComponents)
        {
            //ending index == total number of letters minus one.

            if (indexOfLetterBarCell <= endingIndexOfUserLetters)
            { //no longer required because I fixed the bug in the LCFactoryManager that caused the error,
                //but I'm leaving this here for redundant error protection...


                if (p is LetterSoundComposite)
                {
                    //print("Else Block");
                    LetterSoundComposite l = (LetterSoundComposite)p;
                    foreach (LetterSoundComponent lc in l.Children)
                    {
                        if(wrongIndexList.Contains(indexOfLetterBarCell) && hint)
                        {
                            UpdateInterfaceLetters(lc, letterGridController, indexOfLetterBarCell, flash, wrongLetterStudentMode, indexOfLetterBarCell);
                        }
                        else
                        {
                            UpdateInterfaceLetters(lc, letterGridController, indexOfLetterBarCell, flash, wrongLetterStudentMode, wrongCellNumber);

                        }
                        indexOfLetterBarCell++;
                    }
                }
                else
                {
                   // print("Else Block");
                    if (wrongIndexList.Contains(indexOfLetterBarCell) && hint)
                        UpdateInterfaceLetters(p, letterGridController, indexOfLetterBarCell, flash, wrongLetterStudentMode,indexOfLetterBarCell);
                    else
                        UpdateInterfaceLetters(p, letterGridController, indexOfLetterBarCell, flash, wrongLetterStudentMode, wrongCellNumber);

                    indexOfLetterBarCell++;
                }

            }
        }

        if(hint)
        {
            hint = false;
        }
    }

    bool IsVowel(char letter)
    {
        return letter == 'a' || letter == 'e' || letter == 'o' || letter == 'u' || letter == 'i';


    }

    void UpdateInterfaceLetters(LetterSoundComponent lc, LetterGridController letterGridController, int indexOfLetterBarCell, bool flash, bool wrongLetterStudentMode, int wrongCellNumber)
    {
        InteractiveLetter i;
        char letter = lc.AsString[0];

        if (SessionsDirector.instance.IsSyllableDivisionMode)
        {
            i = letterGridController.GetInteractiveLetter(indexOfLetterBarCell);
            i.UpdateDefaultColour(SessionsDirector.colourCodingScheme.GetColorsForWholeWord());
            i.SetSelectColour(lc.GetColour());

        }
        else
        {
            if (wrongIndexList.Contains(indexOfLetterBarCell) && !showingResult)
            {
//                print("Came here to find out the wrong");
                if (indexOfLetterBarCell == wrongCellNumber)
                {
                    //currentUsedColor.Add(indexOfLetterBarCell, Color.black);
                    i = letterGridController.UpdateLetter(indexOfLetterBarCell, lc.GetColour());
                    i.StartCoroutine("WrongLetterFlash");
                }
                //if(letter!=' ')
                    return;
            }
            i = letterGridController.UpdateLetter(indexOfLetterBarCell, lc.GetColour());
            //currentUsedColor.Add(indexOfLetterBarCell, lc.GetColour());
            // print(i.Letter() + " " + i.HasLetterOrSoundChanged(lc)+" "+indexOfLetterBarCell);

        }

        if (wrongLetterStudentMode)
        {
            if (indexOfLetterBarCell == wrongCellNumber)
            {
                if(letter != ' ')
                    i.StartCoroutine("WrongLetterFlash");
            }
            if (letter != ' ')
                return;
        }


        char previousLetter = ' ';
        char afterLetter = ' ';
        if(letterGridController.GetInteractiveLetter(indexOfLetterBarCell-1) !=null)
        {
            previousLetter = letterGridController.GetInteractiveLetter(indexOfLetterBarCell-1).Letter()[0];
        }

        if (letterGridController.GetInteractiveLetter(indexOfLetterBarCell + 1) != null)
        {
            afterLetter = letterGridController.GetInteractiveLetter(indexOfLetterBarCell + 1).Letter()[0];
        }

        bool flashInteractiveLetter = SessionsDirector.instance.IsMagicERule && IsVowel(lc.AsString[0]);
        flashInteractiveLetter &= flash && i.HasLetterOrSoundChanged(lc) && lc.GetColour() == i.CurrentColor();

        // Condition for Consonant Blend'
        bool flashConsonantBlend = SessionsDirector.instance.IsConsonantBlend && !IsVowel(letter);
        flashConsonantBlend &= flash && firstLetterBlink;



        // Conditions for Vowel Team
        bool flashVowelTeam = SessionsDirector.instance.IsVowelTeam && IsVowel(letter);
        flashVowelTeam &= flash;// && firstLetterBlink;

        //print(i.IsFirstLetter(lc));

        // Conditions for ConsonantDiagrph
        bool flashConsonantDiagraph = SessionsDirector.instance.IsConsonantDiagraph && !IsVowel(letter);
        flashConsonantDiagraph &= flash;// && firstLetterBlink;


        // Conditions for R controlled Vowel
        bool flashRControlledVowel = SessionsDirector.instance.IsRControlledMode && (IsVowel(letter) || (letter == 'r'));// && (previousLetter ==' ' || IsVowel(previousLetter)));
        flashRControlledVowel &= flash;

        i.LetterSoundComponentIsPartOf = lc;

        if ((hint || showingResult) && !SessionsDirector.instance.IsCVC)
        {
            return;
        }


        // Magic e Flash
        if (flashInteractiveLetter)
        {
            if (lc.AsString[0] == 'e')
            {
                i.StartCoroutine("MagiceFlash");
            }
            else
            {
                i.StartCoroutine("Flash");
            }

        }

        else if (flashConsonantBlend)
        {
            if (indexOfLetterBarCell == 0 || indexOfLetterBarCell == 1 && SessionsDirector.colourCodingScheme.flashBlend)
            {
                Debug.Log("flashConsonantDiagraph " + flashConsonantBlend);

                if (SessionsDirector.colourCodingScheme.firstLetterList.Contains(lc.AsString[0]))
                {
                    if (SessionsDirector.colourCodingScheme.flashBlend)
                    {
                        Debug.Log("Came here because flashConsnant is " + flashConsonantBlend + " and firstLetter is " + firstLetterBlink);

                        i.StartCoroutine("Flash");
                        isThistheFirstLetter = false;
                    }
                    expectedFirstLetterPos = indexOfLetterBarCell;

                    //i.StartCoroutine("Flash");
                }

                else if (SessionsDirector.colourCodingScheme.secondLetterList.Contains(lc.AsString[0]) && !isThistheFirstLetter)
                {
                    i.StartCoroutine("Flash");
                    expectedSecondLetterPos = indexOfLetterBarCell;
                    firstLetterBlink = false;
                }

            }

        }
        // RControlled Vowel Flash  (Game)
        else if (flashRControlledVowel)
        {
            Debug.Log("WHEN REMOVED CAME HERE But Flash = " + flashRControlledVowel);// ((letter == 'r' && (previousLetter == ' ' ))));

            if (IsVowel(lc.AsString[0]))
            {
                expectedFirstLetterPos = indexOfLetterBarCell;
                if (!SessionsDirector.colourCodingScheme.flashRControl)
                {
                    i.StartCoroutine("DimFlash");
                    isThistheFirstLetter = false;
                }
                else if (SessionsDirector.colourCodingScheme.flashRControl && firstLetterBlink)
                {
                    i.StartCoroutine("Flash");
                }
            }

            else if (lc.AsString[0]=='r')
            {
                expectedSecondLetterPos = indexOfLetterBarCell;
                if (!SessionsDirector.colourCodingScheme.flashRControl && (indexOfLetterBarCell!=0))
                {
                    i.StartCoroutine("DimFlash");
                }
                else if (SessionsDirector.colourCodingScheme.flashRControl && firstLetterBlink)
                {
                    i.StartCoroutine("Flash");
                    firstLetterBlink = false;
                }


            }
        }

        // vowel teal flash (eat)
        else if (flashVowelTeam)
        {
            if (IsVowel(lc.AsString[0]))
            {
                
                if (!SessionsDirector.colourCodingScheme.flashVowelTeam)
                {
                    expectedFirstLetterPos = indexOfLetterBarCell;
                    i.StartCoroutine("DimFlash");
                    isThistheFirstLetter = false;
                    Debug.Log("This is Vowel team stuff");
                }
                else if (SessionsDirector.colourCodingScheme.flashVowelTeam && firstLetterBlink)
                {
                    i.StartCoroutine("Flash");
                    if(IsVowel(previousLetter))
                    {
                        firstLetterBlink = false;
                    }
                }

            }

            else if (lc.AsString[0] == 'r')
            {
                expectedSecondLetterPos = indexOfLetterBarCell;
                if (!SessionsDirector.colourCodingScheme.flashVowelTeam && (indexOfLetterBarCell != 0))
                {
                    i.StartCoroutine("DimFlash");
                }
                else if (SessionsDirector.colourCodingScheme.flashVowelTeam && firstLetterBlink)
                {
                    i.StartCoroutine("Flash");
                    firstLetterBlink = false;
                }


            }

        }

        // Consonant Diagraph flash (thin)
        else if (flashConsonantDiagraph)
        {
            if (SessionsDirector.colourCodingScheme.firstLetterList.Contains(lc.AsString[0]))
            {
                
                expectedFirstLetterPos = indexOfLetterBarCell;
                if (!SessionsDirector.colourCodingScheme.flashConsonantDiagrah)
                {
                    i.StartCoroutine("DimFlash");
                    isThistheFirstLetter = false;
                }
                else if (SessionsDirector.colourCodingScheme.flashConsonantDiagrah && firstLetterBlink)
                {
                    i.StartCoroutine("Flash");
                }
            }

            else if (SessionsDirector.colourCodingScheme.secondLetterList.Contains(lc.AsString[0]))
            {
//                print("Wnet Hereeeeeeeeeeeeeeeeeeeee");
                expectedSecondLetterPos = indexOfLetterBarCell;
                if(!SessionsDirector.colourCodingScheme.flashConsonantDiagrah)
                {
                    i.StartCoroutine("DimFlash");
                }
                else if (SessionsDirector.colourCodingScheme.flashConsonantDiagrah && firstLetterBlink)
                {
                    i.StartCoroutine("Flash");
                    firstLetterBlink = false;
                }
               

            }
        }

        // Consonant Blend Flash (flag)


    }
    int FindIndexOfGraphemeThatCorrespondsToLastNonBlankPhonogram(UserWord userWord)
    {
        int cursor = endingIndexOfUserLetters;
        foreach (LetterSoundComponent p in userWord)
        {
            if (p is Blank)
                cursor -= p.Length;
            else
                return cursor;
        }
        return cursor;
    }

    int IsLetterBarEmpty()
    {
        if (currUserControlledLettersAsStringBuilder.ToString().Equals(EMPTY_USER_WORD))
            return 0;
        return 1;

    }

    bool selectButtonOnSelection = false;

    public void SelectDeselectAllButtonPressed()
    {
        selectButtonOnSelection = !selectButtonOnSelection;
        InteractiveLetter l;
        //select or deselect all letters
        for (int i = 0, j = StartingIndex; i < currUserControlledLettersAsStringBuilder.Length; i++, j++)
        {
            SaveNewLetterInStringRepresentation((selectButtonOnSelection ? currUserControlledLettersAsStringBuilder[i] : ' '), i, selectedUserControlledLettersAsStringBuilder);
            l = letterGrid.GetInteractiveLetter(j);


            if (selectButtonOnSelection)
                l.Select(false);
            else
                l.DeSelect(false);
        }

        //pass along to user inpur router
        userInputRouter.HandleLetterSelection(SelectedUserControlledLettersAsString.Trim()); //pretend that all letters are selected when we press the button

    }

    public void LetterSelectDeselect(bool wasSelected, GameObject selectedLetter)
    {
        char letter = selectedLetter.GetComponent<InteractiveLetter>().Letter()[0];
        if (selectedLetter.name.Length == 1)
        {
            int position = Int32.Parse(selectedLetter.name);
            if (wasSelected)
            {
                SaveNewLetterInStringRepresentation(letter, position, selectedUserControlledLettersAsStringBuilder);
            }
            else
            {

                SaveNewLetterInStringRepresentation(' ', position, selectedUserControlledLettersAsStringBuilder);
            }

            userInputRouter.HandleLetterSelection(SelectedUserControlledLettersAsString);

        }
    }






    //all of this is for testing; simulates arduino functionality.
    static int testPosition = -1;

    public void SetTestPosition(int newPosition)
    {
        testPosition = newPosition;

        UpdateLetterBarIfPositionAndLetterSpecified();
    }

    static String testLetter;

    public void SetTestLetter(String newLetter)
    {
        testLetter = newLetter;
        UpdateLetterBarIfPositionAndLetterSpecified();

    }

    public void ClearTestLetter()
    {
        testLetter = null;


    }

    public void ClearTestPosition()
    {

        testPosition = -1;

    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
                return;

            if (ParseNumericKey() || ParseLetterKey())
                UpdateLetterBarIfPositionAndLetterSpecified();

        }

        if (IsLetterBarEmpty() == 0)
        {
            firstLetterBlink = true;
            isThistheFirstLetter = true;
            SessionsDirector.colourCodingScheme.flashRControl = false;
        }
        else if(currUserControlledLettersAsStringBuilder[expectedFirstLetterPos] == ' ' || currUserControlledLettersAsStringBuilder[0] ==' '
                || currUserControlledLettersAsStringBuilder[expectedSecondLetterPos] ==' ')
        {
            firstLetterBlink = true;
        }

        else if(currUserControlledLettersAsStringBuilder[0] != ' ' && currUserControlledLettersAsStringBuilder[1] != ' ')
        {
            SessionsDirector.colourCodingScheme.flashBlend = false;
            SessionsDirector.colourCodingScheme.flashConsonantDiagrah = false;

        }

    }

    bool ParseNumericKey()
    {
        String s;

        for (int i = 0; i < maxUserLetters; i++)
        {
            s = "" + i;
            if (Input.GetKey(s))
            {
                //testPosition = i;
                SetTestPosition(i);
                return true;
            }
        }
        return false;
    }

    bool ParseLetterKey()
    {

        //deleting a character
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            //testLetter = " ";
            SetTestLetter(" ");
            return true;
        }

        foreach (char c in SpeechSoundReference.Vowels())
        {
            string s = c + "";
            if (Input.GetKeyDown(s))
            {
                SetTestLetter(s);
                return true;
            }
        }

        foreach (char c in SpeechSoundReference.Consonants())
        {
            string s = c + "";
            if (Input.GetKeyDown(s))
            {
                //testLetter = s;
                SetTestLetter(s);
                return true;
            }
        }
        return false;

    }

    void UpdateLetterBarIfPositionAndLetterSpecified()
    {
        if (testPosition != -1 && testLetter != null)
        {

            ReceiveNewUserInputLetter(testLetter[0], testPosition);
            ClearTestPosition();

        }
    }


}