using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System.Linq;

public class KeyboardLetterInput : MonoBehaviour
{

    public static KeyboardLetterInput instance;
    public List<string> letterList;
    public List<string> hiddenLetterList; //= new List<string>(){" "," "," "," "," "," "};
    public List<string> letterPosTrackingList;

    public List<TrackableBehaviour> letterBehaviourList;
    int letterRemoveIndex = 0;
    int letterIndex = 0;

    public bool doTrack = true;
    //public int a;
    private void Awake()
    {
        letterPosTrackingList = new List<string>() { "", "", "", "", "", "" };
        letterList = new List<string>();
        hiddenLetterList = new List<string>();//{ "", "", "", "", "", "" };

        letterBehaviourList = new List<TrackableBehaviour>();

        instance = this;

        //InvokeRepeating("DeleteAllIfListEmpty",0.1f,1);
    }

    public bool DoesContain(string letter)
    {
        if (letterList.Contains(letter))
            return true;
        return false;
    }

    public void Add(string letter, TrackableBehaviour letterTrackaleBehaviour)
    {
        if(letterList.Count() < 6)
        {
            //letterList.Add(letter);
            letterBehaviourList.Add(letterTrackaleBehaviour);
            letterList = (from a in letterBehaviourList orderby a.transform.position.x select a.name).ToList();

        }

    }

    public void Remove(string letter, TrackableBehaviour letterTrackaleBehaviour)
    {
        if(letterList.Count()>0)
        {
            letterList.Remove(letter);
            if(letterPosTrackingList.Contains(letter))
            {
                int letterPosTrackingIndex = letterPosTrackingList.IndexOf(letter);
                letterPosTrackingList[letterPosTrackingIndex] = "";
            }          
            letterBehaviourList.Remove(letterTrackaleBehaviour);
        }
    }

    public void ClearList(List<string> stringList)
    {
        for (int i = 0; i < stringList.Count; i++)
        {
            stringList[i] = "";
        }
    }

    public void SortList(List<string> letterList, List<string> withRestpectTo)
    {
        for (int i = 0; i < letterList.Count(); i++)
        {
            int targetIndex = withRestpectTo.IndexOf(letterList[i]);
            letterList[targetIndex] = letterList[i];
        }
    }

    public void ClearLetters()
    {

        RemoveAll(6);

    }

    // We dont need this method. Part of old logic
    void RemoveAll()
    {
        if (letterRemoveIndex >= 6)
        {
            letterRemoveIndex = 0;
            letterIndex = 0;
            CancelInvoke();
            ClearList(letterPosTrackingList);
            letterBehaviourList.Clear();
            letterList.Clear();
            doTrack = true;
            return;
        }

        ArduinoLetterController.Instance.ReceiveNewUserInputLetter(' ', letterRemoveIndex);
        //Transaction.Instance.UserEnteredNewLetter.Fire(' ', letterRemoveIndex);
        letterRemoveIndex++;
    }

    void RemoveAll(int numberOfIndex)
    {
       
        ArduinoLetterController.Instance.ReceiveNewUserInputLetter('*', letterRemoveIndex);



        ClearList(letterPosTrackingList);
        letterBehaviourList.Clear();
        letterList.Clear();
        doTrack = true;

    }


    // We dont need this method. Part of old logic
    void Refresh()
    {
        
        if(letterIndex >= letterList.Count())
        {
            CancelInvoke();
            letterIndex = 0;
            return;
        }
        ArduinoLetterController.Instance.ReceiveNewUserInputLetter(letterList[letterIndex][0], letterRemoveIndex);
        letterPosTrackingList[letterIndex] = letterList[letterIndex];
        letterIndex++;
           
    }


    // This method is responsible for putting the letters in right position. When a new letter is detected this method is called
    public void SetLetterInPos(string letter, bool didFound)
    {
        if(didFound)
        {
            if( letterPosTrackingList[letterList.IndexOf(letter)] != "")
            {
                //Debug.LogWarning("<color=green> The Refresh Was called </color>");
                InvokeRepeating("Refresh",0.1f,0.1f);
                return;
            }
            //Debug.LogWarning("<color=red> The Single Fire Was called </color> becasue count = "+ letterList.Count() );
            ArduinoLetterController.Instance.ReceiveNewUserInputLetter(letter[0], letterList.IndexOf(letter));


            letterPosTrackingList[letterList.IndexOf(letter)] = letter;
        }
        else
        {
            if(SessionsDirector.IsStudentMode)
            {
                ArduinoLetterController.Instance.PlaceWordStudentMode(letterPosTrackingList.IndexOf(letter));
                //Transaction.Instance.UserEnteredNewLetter.Fire(' ', letterList.IndexOf(letter));

                return;
            }
            //ArduinoLetterController.instance.ChangeTheImageOfASingleCell(letterPosTrackingList.IndexOf(letter), LetterImageTable.instance.GetBlankLetterImage());
            if(letterPosTrackingList.Contains(letter))
            {
                ArduinoLetterController.Instance.ChangeTheImageOfASingleCell(letterPosTrackingList.IndexOf(letter), LetterImageTable.instance.getBlankLetterImage());
                ArduinoLetterController.Instance.ReceiveNewUserInputLetter(' ', letterPosTrackingList.IndexOf(letter));
                //Transaction.Instance.UserEnteredNewLetter.Fire(' ', letterPosTrackingList.IndexOf(letter));
                Debug.LogWarning("<color=Green> The Letter was Removed </color>" + letter + "From Index" + letterPosTrackingList.IndexOf(letter));

            }

        }
    }

    void DebugPrintArray(List<string> s)
    {
        for (int i = 0; i < s.Count(); i++)
        {
            Debug.LogWarning("<color=blue> ****************** </color>" + s[i]+ " at "+i);
        }
    }

    public void DeleteAllIfListEmpty()
    {
        if(letterList.Count() == 0)
        {
            ClearLetters();
        }
    }

}
