using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LockedPositionHandler : PhonoBlocksController
{
		HashSet<int> lockedPositions;
		int numCorrectLockedLettersThatUserChanged = 0;
		int[] lockedPositionStates;
		const int HAS_NEVER_BEEN_FILLED_WITH_CORRECT_LETTER = 0;
		const int IS_FILLED_WITH_WRONG_LETTER = 1;
		const int IS_FILLED_WITH_CORRECT_LETTER = 2;

		public void ResetForNewProblem ()
		{
				lockedPositions = new HashSet<int> ();
				
			
		}



		//REQUIRES the 2 strings have equal length.
		//(i.e., the "shorter" has had blanks appended to it.
		public void RememberPositionsThatShouldNotBeChanged (string initialLetters, string targetLetters)
		{
				lockedPositionStates = new int[initialLetters.Length];

				//Debug.Log (initialLetters + " " + targetLetters);
				int longer_length = Math.Min (targetLetters.Length, initialLetters.Length);
				//a position is locked iff the char at that position is the same between initial and target letters
				for (int i=0; i<longer_length; i++) {
						//Debug.Log ((initialLetters [i] == targetLetters [i]) + " " + initialLetters [i] + " " + targetLetters [i]);
						if (initialLetters [i] == targetLetters [i]) {
								lockedPositions.Add (i); //!! possibly should be i+1 because the user does not add items to the position 0
						
						}

				}
				
		}

		public bool IsLocked (int position)
		{
		        
				return lockedPositions.Contains (position);
		}
                                                                                                             

		//need to remember the changes/unchanges separately from locked positions (actually, we can basically just use this as-- 
		public void HandleChangeToLockedPosition (int position, char change, string targetState, char[] usersMostRecentChanges, ArduinoLetterController arduinoLetterController)
		{
				if (ACorrectLockedLetterWasChanged (position, targetState, usersMostRecentChanges, change)) 
                {
						lockedPositionStates [position] = IS_FILLED_WITH_WRONG_LETTER;
						numCorrectLockedLettersThatUserChanged++;
						//if (numCorrectLockedLettersThatUserChanged == 1)
								//arduinoLetterController.LockAllLetters ();

				} else if (LockedLetterIsNowCorrect (change, targetState [position])) {
						if (lockedPositionStates [position] == IS_FILLED_WITH_WRONG_LETTER) {
								numCorrectLockedLettersThatUserChanged--;
								if (numCorrectLockedLettersThatUserChanged == 0)
										arduinoLetterController.UnLockAllLetters ();
						
						}
						arduinoLetterController.UnLockASingleLetter (position);
						lockedPositionStates [position] = IS_FILLED_WITH_CORRECT_LETTER;
				}
		}

		bool PreviousStateAndNewLetterAreNotIdentical (int positionOfChange, string desiredStateString, char change)
		{
				return change != desiredStateString [positionOfChange];


		}

		public bool AllLockedPositionsAreInCorrectState ()
		{
				return false;

		}

		bool LockedLetterIsNowCorrect (char change, char desiredLetter)
		{
				return LettersAreSameIgnoreCase (change, desiredLetter);
			
		}

		bool ACorrectLockedLetterWasChanged (int position, string targetLetters, char[] prevStateOfTangibleLetters, char change)
		{
				return LettersAreSameIgnoreCase (targetLetters [position], prevStateOfTangibleLetters [position]);

		}
		
		bool LettersAreSameIgnoreCase (char a, char b)
		{
			   
				int ai = (int)a;
				int bi = (int)b;
				return ai == bi || ai + 32 == bi || ai - 32 == bi;


		}




}
