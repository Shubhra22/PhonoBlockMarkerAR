using UnityEngine;
using System.Collections;
using System;

public class LetterImageTable : MonoBehaviour
{
    public static LetterImageTable instance;
    public Texture2D lockedLetterImage;
    public Texture2D selectLetterImage;
    public Texture2D test;
    public Texture2D without_line_blank;
    public Texture2D letter_underline;
    public static Texture2D DebugImage;
    public static Texture2D LockedLetterImage;
    public static Texture2D BlankLetterImage;
    public static Texture2D LetterUnderlineImage;
    public static Texture2D SelectLetterImage;
    public Texture2D[] LETTER_IMAGES = new Texture2D[27];
    public Texture2D[] DottedLetter_IMAGES = new Texture2D[27];
    public const int BLANK_IDX = 26;
    public const int a_AS_INT = (int)'a';
    public const int z_AS_INT = (int)'z';

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

    }

    void Start()
    {
        SelectLetterImage = selectLetterImage;
        LockedLetterImage = lockedLetterImage;
        LetterUnderlineImage = letter_underline;
        DebugImage = test;
        BlankLetterImage = LETTER_IMAGES[BLANK_IDX];



    }

    //can convert the char into an ASCII int, then sub the needed amount (a=0... )
    //and return the stored image.
    //returns the blank image if the argument letter is invalid.

    public Texture2D GetLetterImageFromLetter(String letter,bool isStudentInitialization, char studentDeletedLetter )
    {


        return GetLetterImageFromLetter(letter.ToLower()[0],isStudentInitialization, studentDeletedLetter);

    }

    public Texture2D GetLetterImageFromLetter(char letter, bool isStudentInitialization, char studentDeletedLetter)
    {
        int asInt = (int)letter;
        if (IsALetter(asInt))
        {
            if (isStudentInitialization)
                return DottedLetter_IMAGES[asInt - a_AS_INT];
            
            return LETTER_IMAGES[asInt - a_AS_INT];

        }

        if (studentDeletedLetter != ' ')
        {
            return DottedLetter_IMAGES[(int)studentDeletedLetter - a_AS_INT];
        }

        if (letter == '_')
            return without_line_blank;
        
        return getBlankLetterImage();


    }
    
    public Texture2D GetLetterOutlineImageFromLetter(char letter)
    {
        return ImageFromLetter(DottedLetter_IMAGES, letter);

    }
    
    Texture2D ImageFromLetter(Texture2D[] imageSource, char letter)
    {
        int asInt = (int)letter;
        if (IsALetter(asInt))
        {
            return imageSource[asInt - a_AS_INT];
        }

        if (letter == '_')
            return without_line_blank;
        return getBlankLetterImage();

    }

    public Texture2D getBlankLetterImage()
    {
        
        return LETTER_IMAGES[BLANK_IDX];
    }

    public bool IsALetter(int charAsInt)
    {
        return a_AS_INT <= charAsInt && z_AS_INT >= charAsInt;
    }

}
