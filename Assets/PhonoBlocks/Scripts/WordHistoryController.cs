using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class WordHistoryController : PhonoBlocksController
{
    int wordLength;
    public GameObject wordHistoryPanelBackground;
    LetterImageTable letterImageTable;

    public int WordLength
    {
        get { return wordLength; }
    }

    public GameObject wordHistoryGrid;
    LetterGridController lettersOfWordInHistory;
    public List<Word> words; //words in the word history.
    Word psuedoWord; //a dummy value to return in case there is some kind of error.

    public Word PsuedoWord
    {
        get
        {
            if (psuedoWord == null)
            {
                psuedoWord = new Word("whoops");
            }

            return psuedoWord;
        }
    }

    public int showImageTime = 60 * 8;

    public void Initialize(int wordLength)
    {
        this.wordLength = wordLength;
        lettersOfWordInHistory = wordHistoryGrid.gameObject.GetComponent<LetterGridController>();

        //wordHistoryGrid.GetComponent<GridLayout>(). = wordLength;
        letterImageTable = GameObject.Find("DataTables").GetComponent<LetterImageTable>();
        InteractiveLetter.LetterPressed += PlayWordOfPressedLetter;
    }

    public void AddCurrentWordToHistory(List<InteractiveLetter> currentWord, bool playSoundAndShowImage = false)
    {
        AddLettersOfNewWordToHistory(currentWord, playSoundAndShowImage);
                
    }

    void AddLettersOfNewWordToHistory(List<InteractiveLetter> newWord, bool playSoundAndShowImage = false)
    {
        StringBuilder currentWordAsString = new StringBuilder();
        int position = words.Count * wordLength;
        
        int len = newWord.Count;
        Color[] colors = new Color[len];

        int i = 0;
        
        foreach (InteractiveLetter l in newWord)
        {
            colors[i] = SessionsDirector.instance.IsSyllableDivisionMode ? l.SelectColour : l.DefaultColour;
            i++;

            currentWordAsString.Append(l.Letter());
        }

        string word = currentWordAsString.ToString().Trim().ToLower();
        
        
        Word nw = CreateNewWordAndAddToList(word);
        
        if (playSoundAndShowImage)
        {
            AudioSourceController.PushClip(nw.Sound);
            userInputRouter.RequestDisplayImage(nw.AsString, true);
        }
        
        lettersOfWordInHistory.AddWordToHistory(word,colors,nw.Sound);
        
        //wordHistoryGrid.GetComponent<UIGrid> ().Reposition ();
        //return word;
    }

    public void ClearWordHistory()
    {
        words.Clear();
        lettersOfWordInHistory.SetAllLettersToBlank();
    }

    Word CreateNewWordAndAddToList(string newWordAsString)
    {
        Word newWord = new Word(newWordAsString);

        newWord.Sound = AudioSourceController.GetWordFromResources(newWordAsString);
        words.Add(newWord);
        return newWord;
    }

    public void PlayWordOfPressedLetter(GameObject pressedLetterCell)
    {
        InteractiveLetter l = pressedLetterCell.GetComponent<InteractiveLetter>();
        if (l.IsBlank())
            return;
        Word wordThatLettersBelongTo =
            RetrieveWordGivenLetterAndIndex(l, IndexOfWordThatLetterBelongsTo(pressedLetterCell));
        AudioSourceController.PushClip(wordThatLettersBelongTo.Sound);
    }

    int IndexOfWordThatLetterBelongsTo(GameObject pressedLetterCell)
    {
        return (Int32.Parse(pressedLetterCell.name)) / wordLength;
    }

    Word RetrieveWordGivenLetterAndIndex(InteractiveLetter pressedLetter, int idx)
    {
        if (idx > -1 && idx < words.Count)
            return words[idx];
        return PsuedoWord;
    }
}