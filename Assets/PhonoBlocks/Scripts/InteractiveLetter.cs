using UnityEngine;
using System.Collections;
using System;
using MarkerWith3DLetter;
using UnityEngine.UI;

//container for the letter of this cell.
//stores data the image, the letter and the color
//but isn't responsible for using this data to do anything.
public class InteractiveLetter : PhonoBlocksController
{
    //changed file!
    String letter;
    Color defaultColour;

    public int position = -1;
    public Color DefaultColour
    {
        get
        {

            return defaultColour;
        }
    }

    bool isLocked = false;

    public bool IsLocked
    {
        get
        {
            return isLocked;
        }
    }

    bool isSelected = false;

    public bool Selected
    {
        get
        {
            return isSelected;
        } 
    }

    public delegate void SelectAction(bool wasSelected, GameObject o);

    public static event SelectAction LetterSelectedDeSelected;
    public delegate void PressAction(GameObject o);

    public static event PressAction LetterPressed;

    Color lockedColor = Color.clear;
    Color flashOff = Color.black;
    Image selectHighlight;
    Color selectColor = Color.clear;

    public void SetSelectColour(Color newColor)
    {
        selectColor = newColor;


    }

    public Color SelectColour
    {
        get
        {
            return selectColor;
        }


    }

    BoxCollider trigger;
    LetterSoundComponent lc;
    int flashCounter = 0;
    int timesToFlash = 5;
    float secondsDelayBetweenFlashes = .2f;
    const int NOT_AN_ARDUINO_CONTROLLED_LETTER = -1;
    int idxAsArduinoControlledLetter = NOT_AN_ARDUINO_CONTROLLED_LETTER; //i.e., if it's a word history controlled letter. you have to "opt in" to be an arduino controlled letter.

    public int IdxAsArduinoControlledLetter
    {
        set
        {
            idxAsArduinoControlledLetter = value;


        }
        get
        {
            return idxAsArduinoControlledLetter;
        }
    }

    public LetterSoundComponent LetterSoundComponentIsPartOf
    {
        get
        {
            return lc;
        }

        set
        {
            lc = value;
        }


    }

    public BoxCollider Trigger
    {
        get
        {
            return trigger;
        }
        set
        {

            trigger = value;
        }


    }

    public Image SelectHighlight
    {
        get
        {
            return selectHighlight;
        }
        set
        {
            selectHighlight = value;
            selectHighlight.enabled = false;

        }


    }

    public void Lock()
    {
        if (lockedColor == Color.clear)
        {
            
            lockedColor = SessionsDirector.colourCodingScheme.GetColorsForOff();
        }
            
        UpdateDisplayColour(lockedColor);
        isLocked = true;
    }

    public void UnLock()
    {
        UpdateDisplayColour(defaultColour);
        isLocked = false;
    }

    public String Letter()
    {
        return letter;
    }

    public Texture2D Image()
    {
        return (Texture2D)gameObject.GetComponent<Image>().sprite.texture;
    }

    public UnityEngine.Color CurrentColor()
    {
        return gameObject.GetComponent<Image>().color;        
    }

    public bool IsBlank()
    {
        return letter[0] == ' ';
    }

    public IEnumerator Flash()
    {

        int mod_To_end_on = (timesToFlash % 2 == 0 ? 1 : 0);

        while (flashCounter < timesToFlash)
        {

            if (flashCounter % 2 == mod_To_end_on)
            {
                UpdateDisplayColour(defaultColour);
                //CharacterList.Instance.UpdateColor(position,defaultColour);

            }
            else
            {
                UpdateDisplayColour(flashOff);
                //CharacterList.Instance.UpdateColor(position,flashOff);

            }
            flashCounter++;

            yield return new WaitForSeconds(secondsDelayBetweenFlashes);
        }

        flashCounter = 0;
        //UpdateDisplayColour(Color.gray);

    }

    public IEnumerator MagiceFlash()
    {

        int mod_To_end_on = (timesToFlash % 2 == 0 ? 1 : 0);

        while (flashCounter < timesToFlash)
        {

            if (flashCounter % 2 == mod_To_end_on)
            {
                UpdateDisplayColour(defaultColour);
                //CharacterList.Instance.UpdateColor(position,defaultColour);


            }
            else
            {
                UpdateDisplayColour(flashOff);
                //CharacterList.Instance.UpdateColor(position,flashOff);
            }
            flashCounter++;

            yield return new WaitForSeconds(secondsDelayBetweenFlashes);
        }

        flashCounter = 0;
        UpdateDisplayColour(flashOff);
       // CharacterList.Instance.UpdateColor(position,flashOff);
        //UpdateDisplayColour(Color.gray);
        //userInputRouter.arduinoLetterInterface.ShutOffAt(IdxAsArduinoControlledLetter);

    }

    public IEnumerator DimFlash()
    {

        int mod_To_end_on = (timesToFlash % 2 == 0 ? 1 : 0);
        flashCounter = 4;

        while (flashCounter < timesToFlash)
        {

            if (flashCounter % 2 == mod_To_end_on)
            {
                UpdateDisplayColour(defaultColour);
                //CharacterList.Instance.UpdateColor(position,defaultColour);
                yield return new WaitForSeconds(secondsDelayBetweenFlashes * 3);

            }
            else
            {
                UpdateDisplayColour(flashOff);
                //CharacterList.Instance.UpdateColor(position,flashOff);


            }
            flashCounter++;

            yield return new WaitForSeconds(secondsDelayBetweenFlashes);
        }

        flashCounter = 0;
        UpdateDisplayColour(flashOff);
        //CharacterList.Instance.UpdateColor(position,flashOff);

    }


    public IEnumerator WrongLetterFlash()
    {

        int mod_To_end_on = (timesToFlash % 2 == 0 ? 1 : 0);
        flashCounter = 4;

        while (flashCounter < timesToFlash)
        {

            if (flashCounter % 2 == mod_To_end_on)
            {
                UpdateDisplayColour(Color.white);
                //CharacterList.Instance.UpdateColor(position,Color.white);

                yield return new WaitForSeconds(secondsDelayBetweenFlashes * 3);

            }
            else
            {
                UpdateDisplayColour(flashOff);
                //CharacterList.Instance.UpdateColor(position,flashOff);

            }
            flashCounter++;

            yield return new WaitForSeconds(secondsDelayBetweenFlashes);
        }

        flashCounter = 0;
        UpdateDisplayColour(flashOff);
        //CharacterList.Instance.UpdateColor(position,flashOff);
    }




    public void UpdateDisplayColour(UnityEngine.Color c_)
    {
        //if (c_ == Color.clear)
            //c_ = Color.white;
        //Debug.Log( transform.name +"Changing the color to "+ c_);
        
       
        GetComponent<Image>().color = c_;
        
        //int val = Int32.Parse(transform.name);
        if(position!=-1)
            CharacterList.Instance.UpdateColor(position,c_);

        //change colour of counterpart tangible letter
        ChangeColourOfTangibleCounterpartIfThereIsOne(c_);
        

    }

    public void RevertToDefaultColour()
    {
        
        UpdateDisplayColour(defaultColour);
        CharacterList.Instance.UpdateColor(position,defaultColour);
    }

    bool IsLockedColour(UnityEngine.Color c_)
    {

        return c_.r == lockedColor.r && c_.g == lockedColor.g && c_.b == lockedColor.b;
    }

    public void ChangeColourOfTangibleCounterpartIfThereIsOne(UnityEngine.Color c_)
    {
        if (lockedColor == Color.clear)
        {
            //print("LOCKED LETTER");
            lockedColor = SessionsDirector.colourCodingScheme.GetColorsForOff();
        }

        //on the screen, blank letters are just clear.
        //but we issue the black (0,0,0) colour to the arduino.
        if (letter[0] == ' ' || IsLockedColour(c_) )
        {
            
            //if (userInputRouter != null)
                //userInputRouter.arduinoLetterInterface.ShutOffAt(IdxAsArduinoControlledLetter);
            
            c_ = Color.black;
        }


        if (userInputRouter != null)
            if (IdxAsArduinoControlledLetter != NOT_AN_ARDUINO_CONTROLLED_LETTER && userInputRouter.IsArduinoMode())
                userInputRouter.arduinoLetterInterface.ColorNthTangibleLetter(IdxAsArduinoControlledLetter, c_);



    }

    public void UpdateLetter(String letter_, Texture2D img_, UnityEngine.Color c_)
    {
        UpdateLetter(letter_, img_);
        UpdateDefaultColour(c_);
        
        //Debug.Log( transform.name +"Changing the color to "+ c_);

    }


    //update the letter images; then after they make any change it will just update them all again
    public void UpdateLetterImage(Texture2D img_)
    {
        
        gameObject.GetComponent<Image>().sprite = Texture2DtoSprite(img_);


    }

    public void UpdateLetter(String letter_, Texture2D img_)
    {

        letter = letter_;

        //de-select this cell if it was selected
        if (isSelected)
            DeSelect();

        gameObject.GetComponent<Image>().sprite = Texture2DtoSprite(img_);


    }

    public bool HasLetterOrSoundChanged(LetterSoundComponent lc)
    {
        //print(this.lc.GetType() + "<T*__*T>" + lc.GetType() + "__|||__" + this.lc.AsString + "<V*__*V>" + lc.AsString);
        //Debug.Log (this.lc.GetType ()ReferenceEquals (this.lc, null) + " " + lc.AsString);
        if (ReferenceEquals(this.lc, null) )
        {
            return false; //we do not flash the first time.

        }



        if (ReferenceEquals(this.lc, null) && lc.IsVowelOrVowelDigraph)
        {
            return true; //we do not flash the first time.

        }


        //true if old lc's class differed from new lc's class.
        //(e.g., previously it was the vowel a, now it is the vowel digraph ae.
        if (!this.lc.GetType().Equals(lc.GetType()))

        {
            print(lc.GetType() + " " + this.lc.GetType());
            return true;

        }

        //else, the classes are the same. (this is why we can check the int and not the strign representation of sound type)
        //refactor- make sound type an enum
        //same class, bt different letters
        if (!this.lc.AsString.Equals(lc.AsString))

            return true;

        //same class, same letters, but the sound type has changed (e.g., long a to short a; silent e to not silent e)
        if (this.lc.SoundType != lc.SoundType)
            return true;


        return false;

    }
    public void UpdateDefaultColour(UnityEngine.Color c_)
    {
        //if (defaultColour == Color.clear)
            //defaultColour = Color.white;
        //Debug.Log( transform.name +"Changing the color to "+ c_);
        defaultColour = c_;
        if (!IsLocked)
            UpdateDisplayColour(defaultColour);

    }

    public void SwitchImageTo(Texture2D img)
    {

        if(GetComponent<Image>()!=null)
            GetComponent<Image>().sprite = Texture2DtoSprite(img);


    }

    void Update()
    {

        if (!IsBlank())
        { //don't select blank letters

            if (MouseIsOverSelectable())
            {

                if (SwipeDetector.swipeDirection == SwipeDetector.Swipe.RIGHT)
                {

                    Select();
                }
                if (SwipeDetector.swipeDirection == SwipeDetector.Swipe.LEFT)
                {
                    DeSelect();
                }

            }

        }
    }

    bool MouseIsOverSelectable()
    {
        Vector3 mouse = SwipeDetector.GetTransformedMouseCoordinates();

        return (Vector3.Distance(mouse, gameObject.transform.position) < .3);
    }

    public void Select(bool notifyObservers = true)
    {
        if (!isSelected && !IsBlank())
        {

            isSelected = true;
            if (selectColor == Color.clear)
                selectHighlight.enabled = true;
            else
                UpdateDisplayColour(selectColor);
            if (notifyObservers && LetterSelectedDeSelected != null)
                LetterSelectedDeSelected(true, gameObject);
        }

    }

    public void DeSelect(bool notifyObservers = true)
    {
        if (isSelected)
        {
            isSelected = false;
            if (selectColor == Color.clear)
            {
                if (selectHighlight)
                    selectHighlight.enabled = false;
            }
            else
            {
                UpdateDisplayColour(defaultColour);
            }
                
            if (notifyObservers && LetterSelectedDeSelected != null)
                LetterSelectedDeSelected(false, gameObject);
        }
    }

    public void OnPress(bool pressed)
    {
        if (pressed && LetterPressed != null)
            LetterPressed(gameObject);


    }
    
    Sprite Texture2DtoSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
    }

}
