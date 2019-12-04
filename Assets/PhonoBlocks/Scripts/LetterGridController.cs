using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;

public class LetterGridController : PhonoBlocksController
{
    public GameObject letterGrid;
    public TMP_FontAsset centuryGothic;
    public GameObject letterHighlightsGrid;
    public GameObject letterUnderlinesGrid;
    public int letterImageWidth;
    public int letterImageHeight;
    float selectH;
    float selectW;

    public int LetterImageHeight
    {
        get
        {
            return letterImageHeight;
        }
        set
        {
            letterImageHeight = value;
        }
    }

    public Texture2D blankLetter;
    LetterImageTable letterImageTable;

    void Start()
    {
        InitializeFieldsIfNecessary();
    }

    void InitializeFieldsIfNecessary()
    {
        if (letterGrid == null)
            letterGrid = gameObject;
        if (letterImageTable == null)
            letterImageTable = GameObject.Find("DataTables").GetComponent<LetterImageTable>();
        if (blankLetter == null)
            blankLetter = letterImageTable.getBlankLetterImage();
        if (letterImageWidth == 0 || letterImageHeight == 0) //you can specify dimensions for the image that are different from those of the grid.
            MatchLetterImageToGridCellDimensions(); //but if nothing is specified it defaults to make it the same size as the grid cells.

        if (letterHighlightsGrid)
        {
            GridLayoutGroup high = letterHighlightsGrid.GetComponent<GridLayoutGroup>();
            GridLayoutGroup letters = letterGrid.GetComponent<GridLayoutGroup>();
            Vector2 cellsize = new Vector2(letters.cellSize.x,letters.cellSize.y);
            high.cellSize = cellsize;
            letterHighlightsGrid.transform.position = letterGrid.transform.position;


            selectH = letterImageHeight;
            selectW = letterImageWidth;

        }

        if (letterHighlightsGrid)
        {
            GridLayoutGroup high = letterHighlightsGrid.GetComponent<GridLayoutGroup>();
            GridLayoutGroup letters = letterGrid.GetComponent<GridLayoutGroup>();
            
            Vector2 cellsize = new Vector2(letters.cellSize.x,letters.cellSize.y);
            high.cellSize = cellsize;
            letterHighlightsGrid.transform.position = letterGrid.transform.position;

        }

        
    }



    //added in april; sets numVisibleLetterLines' UITexture components to active, starting from the leftmost cell; sets the rest to be inactive.
    //call when you initialize a new word. argument should be the number of letters in the word.
    public void setNumVisibleLetterLines(int numVisibleLetterLines)
    {
        int i = 0;
        for (; i < letterUnderlinesGrid.transform.childCount; i++)
        {

            GameObject cell = GetCell(i, letterUnderlinesGrid);
            Image lineImage = cell.GetComponent<Image>();


            if (i < numVisibleLetterLines)
            {
                lineImage.enabled = true;

            }
            else
            {
                lineImage.enabled = false;

            }
        }

    }

    public void RemoveImageOfLetter(int position)
    {

        GameObject letterCell = GetLetterCell(position);
        letterCell.GetComponent<InteractiveLetter>().SwitchImageTo(letterImageTable.without_line_blank);


    }

    void MatchLetterImageToGridCellDimensions()
    {

        GridLayoutGroup grid = letterGrid.GetComponent<GridLayoutGroup>();
       
        letterImageWidth = (int)grid.cellSize.x;
        letterImageHeight = (int)grid.cellSize.y;

    }

    public void InitializeBlankLetterSpaces(int numCells)
    {
        InitializeFieldsIfNecessary();

        for (int i = 0; i < numCells; i++)
        {

            GameObject newLetter = CreateLetterBarCell(" ", blankLetter, i + "", Color.white);
            //the word history controller also involves a letter grid... but doesn't highlight selected letters or show the lines.
            //consider changing this so that the arduino letter controller is what creates the underlines and highlights...
            //seems weird that this class would be making decisions about that
            if (letterHighlightsGrid)
            {
                Image letterHighlight = CreateLetterHighlightCell();
                newLetter.GetComponent<InteractiveLetter>().SelectHighlight = letterHighlight;
            }
            if (letterUnderlinesGrid)
            {
                CreateLetterUnderlineCell(i);
            }
        }
       // RepositionGrids();
    }

    public void RepositionGrids()
    {
        letterGrid.GetComponent<UIGrid>().Reposition();
        if (letterHighlightsGrid)
            letterHighlightsGrid.GetComponent<UIGrid>().Reposition();
        if (letterUnderlinesGrid)
            letterUnderlinesGrid.GetComponent<UIGrid>().Reposition();

    }

    public void SetAllLettersToBlank()
    {
        SetLettersToBlank(0, transform.childCount);
    }

    public void SetLettersToBlank(int startingFrom, int number)
    {
        for (int i = startingFrom; i < startingFrom + number; i++)
        {
            UpdateLetter(i, " ", Color.white,false);

        }
    }

    public InteractiveLetter UpdateLetter(int position, String letter, Color newNonLockColour, bool isStudentInitialization)
    {

        string initWord;
        InteractiveLetter l = GetInteractiveLetter(position);
        bool attemptErasure = letter.Equals(" ");

        if(SessionsDirector.IsStudentMode)
        {
            initWord = userInputRouter.studentActivityController.currProblem.InitialWord;
            if (attemptErasure && initWord[position] != ' ')
            {
                //Debug.Log("Came Here" + initWord + " Letter at Position " + initWord[position]);
                Texture2D letterImageStd = CopyAndScaleTexture(letterImageWidth, letterImageHeight, letterImageTable.GetLetterImageFromLetter(letter, false, initWord[position]));
                l.UpdateLetter(initWord[position].ToString(), letterImageStd, newNonLockColour);
                return l;
            }
        }

        Texture2D letterImage = CopyAndScaleTexture(letterImageWidth, letterImageHeight, letterImageTable.GetLetterImageFromLetter(letter,isStudentInitialization,' '));
        l.UpdateLetter(letter, letterImage, newNonLockColour);

        return l;

    }

    public Image CreateLetterHighlightCell()
    {
        GameObject obj = new GameObject();
        obj.transform.SetParent(letterHighlightsGrid.transform);
        Image selectHighlight = obj.AddComponent<Image>();
        //selectHighlight = SetShaders(selectHighlight);
        //selectHighlight.mainTexture = CopyAndScaleTexture(selectW, selectH, LetterImageTable.SelectLetterImage);
        selectHighlight.sprite = Texture2DtoSprite(LetterImageTable.SelectLetterImage);
        selectHighlight.enabled = false;

        return selectHighlight;

    }

    public void CreateLetterUnderlineCell(int index)
    {
        GameObject obj = new GameObject();
        obj.transform.SetParent(letterUnderlinesGrid.transform);
        Image underline = obj.AddComponent<Image>();
        //underline.transform.localScale = new Vector2(selectW, selectH);
       // underline = SetShaders(underline);
        underline.sprite = Texture2DtoSprite(LetterImageTable.LetterUnderlineImage);//CopyAndScaleTexture(selectW, selectH, LetterImageTable.LetterUnderlineImage);
        underline.gameObject.name = "" + index;

    }

    public InteractiveLetter UpdateLetter(int position, String letter, bool isStudentInitialization)
    {
        return UpdateLetter(position, letter, Color.white, isStudentInitialization);

    }

    public InteractiveLetter UpdateLetter(int position, Color c, bool isNewDefault = true)
    {

        
        InteractiveLetter l = GetInteractiveLetter(position);
        if (isNewDefault)
            l.UpdateDefaultColour(c);
        else
            l.UpdateDisplayColour(c);
        return l;

    }

    public InteractiveLetter UpdateLetterImage(int position, Texture2D img)
    {

        InteractiveLetter l = GetInteractiveLetter(position);
        l.UpdateLetterImage(img);
        return l;

    }

    public InteractiveLetter GetInteractiveLetter(int position)
    {

        GameObject cell = GetLetterCell(position);
        if (cell == null)
            return null;

        return cell.GetComponent<InteractiveLetter>();
    }

    public GameObject GetLetterCell(int position)
    {

        return GetCell(position, letterGrid);
    }

    public GameObject GetCell(int position, GameObject fromGrid)
    {
        if (position < fromGrid.transform.childCount && position > -1)
        {
            return fromGrid.transform.Find(position + "").gameObject; //cell to update
        }
        else
        {
           return null;
        }
    }

    public GameObject CreateLetterBarCell(String letter, Texture2D tex2D, string position, Color c)
    {
        Texture2D tex2dCopy = CopyAndScaleTexture(letterImageWidth, letterImageHeight, tex2D);
        //UITexture ut = NGUITools.AddChild<UITexture>(letterGrid);
        

        
        GameObject g = new GameObject();
        g.transform.SetParent(letterGrid.transform);
        Image ut = g.AddComponent<Image>();
        
        //ut.material = new Material(Shader.Find("Unlit/Transparent Colored"));
        //ut.shader = Shader.Find("Unlit/Transparent Colored");
        ut.sprite = Texture2DtoSprite(tex2dCopy);
        ut.color = c;
        
        BoxCollider b = g.AddComponent<BoxCollider>();
        b.isTrigger = true;
        b.size = new Vector2(.6f, .6f);


        
        InteractiveLetter l = g.AddComponent<InteractiveLetter>();
        l.Trigger = b;
        l.UpdateLetter(letter, tex2dCopy, c);

        ut.gameObject.name = position;
        l.position = Int32.Parse(position);
        
        //ut.MakePixelPerfect();

        Debug.Log("The New Letter is greated with " + letter);

        return l.gameObject;

    }

//    UITexture SetShaders(UITexture ut)
//    {
//        ut.material = new Material(Shader.Find("Unlit/Transparent Colored"));
//        ut.shader = Shader.Find("Unlit/Transparent Colored");
//
//        return ut;
//    }

    Texture2D CopyAndScaleTexture(float w, float h, Texture tex2D)
    {
        Texture2D tex2dCopy = Instantiate(tex2D) as Texture2D;
        //TextureScale.Bilinear(tex2dCopy, (int)w, (int)h);
        return tex2dCopy;
    }

    public List<InteractiveLetter> GetLetters(bool skipBlanks)
    {
        List<InteractiveLetter> list = new List<InteractiveLetter>();
        foreach (Transform child in letterGrid.transform)
        {
            InteractiveLetter letter = child.GetComponent<InteractiveLetter>();
            if (!skipBlanks || !letter.IsBlank())
            {
                list.Add(letter);
            }

        }
        return list;

    }

    Sprite Texture2DtoSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
    }

    public void AddWordToHistory(string input,Color[] colors, AudioClip sound)
    {
        GameObject button = new GameObject();
        button.transform.SetParent(letterGrid.transform);
        Button b = button.AddComponent<Button>();
        TextMeshProUGUI text = button.AddComponent<TextMeshProUGUI>();
        text.text = ColorText.TextToColor(input, colors);
        text.font = centuryGothic;

        b.onClick.AddListener(()=>OnClickWordHistory(sound));

    }

    void OnClickWordHistory(AudioClip sound)
    {
        AudioSourceController.PushClip(sound);
    }
}
