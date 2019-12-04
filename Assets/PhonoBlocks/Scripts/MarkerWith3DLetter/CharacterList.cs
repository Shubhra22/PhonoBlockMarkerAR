using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vuforia;

namespace MarkerWith3DLetter
{ 
    
    // This Class represents each letter
    public class Character
    {
        public char nameOfChar;
        public double xPos;
        public Renderer alphabetObject;
        public bool indexChanged;

        private int index = -1;
        public int Index
        {
            get { return index; }
            set
            {
                if (index == -1)
                {
                    index = value;
                }
                else if (index != value)
                {
                    index = value;
                    indexChanged = true;
                }
            }
        }

        public Character(char name, double pos, Renderer alphabetObject)
        {
            nameOfChar = name;
            xPos = pos;
            this.alphabetObject = alphabetObject;
        }
    }
    
    public class CharacterList : MonoBehaviour
    {
        static CharacterList instance;

        // Singleton for the class
        public static CharacterList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<CharacterList>();
                }
                return instance;
            } 
        }

        public bool checkMarked;

        private List<Character> characters; // A List to contain all the character as they are being scanned
        private Color[] currentLetterColor; // List of the current colors of a letter corresponding to index i

        private List<Character> sortedList; // A list to contain the tracked letter sorted based on the position left to right

        private void Awake()
        {
            characters = new List<Character>();
            sortedList = new List<Character>();
            currentLetterColor = new Color[6];
        }

        // Adding letters in the list and sort them based on their location in real world
        public void Add(Character c)
        {
            if (characters.Count < 6)
            {
                characters.Add(c);
                sortedList = characters.OrderBy(o => o.xPos).ToList();
                for (int i = 0; i < sortedList.Count; i++)
                {
                    sortedList[i].Index = i;
                }
            }
        }

        // Update the letters color and position in the slot by passing them from the sorted list to the LetterController.   
        public void UpdateCharecters(Character letter, bool didFound)
        {
            if (didFound)
            {
                int index = GetIndex(letter);                
                ArduinoLetterController.Instance.ReceiveNewUserInputLetter(letter.nameOfChar, index);

                // if the index has got changed, redo each letters position and color from left to right
                if (didChangedIndex())
                {
                    for (int i = 0; i < sortedList.Count; i++)
                    {
                        ArduinoLetterController.Instance.ReceiveNewUserInputLetter(sortedList[i].nameOfChar, i);
                    }
                }
                
            }
            else
            {
                ArduinoLetterController.Instance.ReceiveNewUserInputLetter(' ', GetIndex(letter));
            }
        }
        
        public void Remove(Character c)
        {
            characters.Remove(c);
            //ArduinoLetterController.Instance.ReceiveNewUserInputLetter(' ', GetIndex(c));
        }

        // Update chanreter overloaded method. In case we need it in future. 
        public void UpdateCharecters()
        {
            if (sortedList.Count > 0)
            {
                sortedList.Clear();
            }
            sortedList = characters.OrderBy(o => o.xPos).ToList();
            
            
            for (int i = 0; i < sortedList.Count; i++)
            {
                ArduinoLetterController.Instance.ReceiveNewUserInputLetter(sortedList[i].nameOfChar, i);
            }

//            for (int j = sortedList.Count; j < 6; j++)
//            {
//                ArduinoLetterController.Instance.ReceiveNewUserInputLetter(' ', j);
//               
//            }
            
        }

        // Update the color of the Augmented 3d model based on the color information provided by the letter controller 
        public void UpdateColor(int index, Color c)
        {
            //if(checkMarked) return;
            currentLetterColor[index] = c; 
            
            if(sortedList.Count > index && sortedList[index]!=null)
                sortedList[index].alphabetObject.material.color = c;
        }

        // get index of the chracter in the list. 
        public int GetIndex(Character c)
        {
            return sortedList.IndexOf(c);
        }

         // check if the user has changed the position of any le
        bool didChangedIndex()
        {
            for (int i = 0; i < sortedList.Count; i++)
            {
                if (sortedList[i].indexChanged)
                {
                    return true;
                }
            }

            return false;
        }

        public void RemoveAll()
        {
            sortedList.Clear();
            characters.Clear();
            ArduinoLetterController.Instance.ReplaceEachLetterWithBlank();
        }

        // Pause or resume the tracking of the vuforia camera
        private bool clicked = false;
        public void Pause()
        {
            clicked = !clicked;
            if (clicked)
            {
                TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();
            }
            else
            {
                TrackerManager.Instance.GetTracker<ObjectTracker>().Start();
            }
        }
    }
    

}
