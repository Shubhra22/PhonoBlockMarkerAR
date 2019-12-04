using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class LetterSoundComponentFactoryManager : MonoBehaviour
{

		static bool checkPhonotactics = false;
		static SyllableFactory syllableFactory = new SyllableFactory ();

		public static UserWord Decode (string word, bool bySyllables=false)
		{
				return syllableFactory.Parse (word, bySyllables);
		}


		public abstract class LetterSoundComponentFactory : MonoBehaviour
		{
				protected static VowelConsonantFactory vowelConsonantFactory = new VowelConsonantFactory ();
		
				public abstract UserWord Parse (string context, bool bySyllables=false);

		}
		
		public class VowelConsonantFactory : LetterSoundComponentFactory
		{
		
				public List<LetterSoundComponent> TryMakeLetters (string letters)
				{

						List<LetterSoundComponent> result = new List<LetterSoundComponent> ();
						int idx = 0;
						foreach (char letter in letters) {
							
								if (SpeechSoundReference.IsVowel (letter)) {
										result.Add (new Vowel (letter));

								} else if (SpeechSoundReference.IsConsonant (letter)) {
										result.Add (new Consonant (letter));

								} else
										result.Add (new Blank ());
								idx++;

				
						}
						return result;
				}

				public override UserWord Parse (string input, bool bySyllables=false)
				{
						return null;
				}
		
		}
		
		public class SyllableFactory : LetterSoundComponentFactory
		{
				//v -> vowel or vowel digraph
				//c -> consonant or consonant digraph or blend
				static readonly int MAX_SYLLABLE_LENGTH = 3; //VCE or CVC
				//2 unit patterns are CV or VC
				static readonly int MIN_SYLLABLE_LENGTH = 1; //V
				static StableSyllableFactory stableSyllableFactory = new StableSyllableFactory ();
				List<LetterSoundComponent> preSyllable;

				public override UserWord Parse (string word, bool bySyllables=false)
				{

						string substringToDecode = word.TrimEnd ();

						preSyllable = new List<LetterSoundComponent> ();
						int numBlanksToRestore = word.Length - substringToDecode.Length;
						if (bySyllables) {
								preSyllable = ParseSyllables (substringToDecode);
						}
						if (!bySyllables || preSyllable.Count == 0) {
								preSyllable = ParseBlendsDigraphsAndLetters (substringToDecode);
								IdentifyVowelSoundsAndPhonotacticErrorsBySyllable (preSyllable);
						}
						//we need to check the phonotactics after we have collected ad appended

						UserWord userWord = new UserWord (preSyllable);
				
						userWord.ApplyColoursToLetterSoundComponents (checkPhonotactics); 
						userWord.RecordIndexOfLastNonBlankLetter ();


						RestoreBlanks (userWord, numBlanksToRestore);
				

						return userWord;
  				  
				}
			
				void RestoreBlanks (UserWord word, int num)
				{
						while (num>0) {
								word.Add (new Blank ());
								num--;
						}
			
			
				}

				public List<LetterSoundComponent> ParseBlendsDigraphsAndLetters (string word)
				{    
						return FindUnitsOfLength (SpeechSoundReference.MAX_BLEND_LENGTH, word);

				}

				//only used in syllable division activity where either you form the two syllables (and they both become coloured) OR
				//you form just letters
				public List<LetterSoundComponent> ParseSyllables (string word)
				{
						
						List<LetterSoundComponent> units = new List<LetterSoundComponent> ();
						LetterSoundComponent first_syllable = null;
						LetterSoundComponent second_syllable = null;
						//find first syllable
						int length_of_first_syllable = SpeechSoundReference.MAX_STABLE_SYLLABLE_LENGTH;
						while (length_of_first_syllable>=SpeechSoundReference.MIN_STABLE_SYLLABLE_LENGTH) {
								if (length_of_first_syllable < word.Length) {
										string longest_syllable = word.Substring (0, length_of_first_syllable);
									
										first_syllable = TryMakeSyllable (longest_syllable);

										if (!ReferenceEquals (first_syllable, null)) {
										
												break;
										}
								}
								length_of_first_syllable--;

						}
						//find second syllable if we found a first syllable, otherwise return the result of the general algorithm.

						if (!ReferenceEquals (first_syllable, null)) {
								int length_of_rest = word.Length - length_of_first_syllable;
						
								if (length_of_rest >= SpeechSoundReference.MIN_STABLE_SYLLABLE_LENGTH) {
										string candidate_syllable = word.Substring (length_of_first_syllable, length_of_rest);
									
										second_syllable = TryMakeSyllable (candidate_syllable);
										if (!ReferenceEquals (second_syllable, null)) {
										

												units.Add (first_syllable);
												units.Add (second_syllable);
										

										}
						
								}
						}

						return units;
			            

				}

				LetterSoundComposite TryMakeSyllable (string candidate)
				{
						bool isSyll = SpeechSoundReference.IsStableSyllable (candidate);
						if (isSyll) {
						
								return StoreLettersInComposite (new StableSyllable (candidate), candidate);

						}
						return null;
				}

				List<LetterSoundComponent> FindUnitsOfLength (int lengthOfTargetComponent, string word)
				{
					
						List<LetterSoundComponent> units = new List<LetterSoundComponent> ();
						//base case: length=0.
						if (lengthOfTargetComponent <= 0 || word.Length == 0)
								return units;
						else {
								int inWord = 0;
								while (inWord+lengthOfTargetComponent<=word.Length) {
										string candidate = word.Substring (inWord, lengthOfTargetComponent);

										LetterSoundComponent pivot = TryMakeDigraphOrBlend (candidate);
								
										if (!ReferenceEquals (pivot, null)) 
												return Combine (FindUnitsOfLength (lengthOfTargetComponent - 1, word.Substring (0, inWord)), pivot, FindUnitsOfLength (lengthOfTargetComponent, word.Substring (inWord + lengthOfTargetComponent, word.Length - (inWord + lengthOfTargetComponent))));
										else
												inWord++;
								}
								lengthOfTargetComponent--;
								return FindUnitsOfLength (lengthOfTargetComponent, word);

						}
		
				}

				List<LetterSoundComponent> Combine (List<LetterSoundComponent> pre, LetterSoundComponent pivot, List<LetterSoundComponent> post)
				{
						pre.Add (pivot);
						pre.AddRange (post);
						return pre;


				}

				LetterSoundComponent TryMakeDigraphOrBlend (string candidate)
				{
						
						if (candidate.Length > 1)
								return TryMakeTwoLetterSpeechSound (candidate);
						else
								return vowelConsonantFactory.TryMakeLetters (candidate) [0];
		
				}

				LetterSoundComposite TryMakeTwoLetterSpeechSound (string candidate)
				{
					
						int blendType = SpeechSoundReference.IsBlendAndWhichType (candidate);
						if (blendType != SpeechSoundReference.NOT_A_BLEND) 
								return StoreLettersInComposite (new Blend (candidate, blendType), candidate);
						if (SpeechSoundReference.IsConsonantDigraph (candidate))
								return StoreLettersInComposite (new ConsonantDigraph (candidate), candidate);
						if (SpeechSoundReference.IsVowelDigraph (candidate))
								return StoreLettersInComposite (new VowelDigraph (candidate), candidate);
						if (SpeechSoundReference.IsVowelR (candidate))
								return StoreLettersInComposite (new VowelR (candidate), candidate);
						
						return null;
			
			
				}

				LetterSoundComposite StoreLettersInComposite (LetterSoundComposite candidate, string asString)
				{
						foreach (LetterSoundComponent l in vowelConsonantFactory.TryMakeLetters(asString))
								candidate.AddChild (l);
						return candidate;
		
				}

				void IdentifyVowelSoundsAndPhonotacticErrorsBySyllable (List<LetterSoundComponent> list)
				{      
						int syllableLength = MAX_SYLLABLE_LENGTH;
						while (syllableLength>list.Count)
								syllableLength--;
						if (syllableLength == 0)
								return;
			            
			               
						while (syllableLength>=MIN_SYLLABLE_LENGTH) {
								int inList = list.Count - syllableLength;
								List<LetterSoundComponent> section = list.GetRange (inList, syllableLength);
								if (TryMatchSyllablePattern (section, inList))
										break;
								
								syllableLength--;
						}
						syllableLength = (syllableLength == 0 ? 1 : syllableLength);
						IdentifyVowelSoundsAndPhonotacticErrorsBySyllable (list.GetRange (0, list.Count - syllableLength));

				}
		
				bool TryMatchSyllablePattern (List<LetterSoundComponent> pattern, int idxOfFirstLetter)
				{
						if (pattern.Count == 3)
								return TryMatch3UnitPattern (pattern, idxOfFirstLetter);
						if (pattern.Count == 2)
								return TryMatch2UnitPattern (pattern, idxOfFirstLetter);
						if (pattern.Count == 1) 
								return TryMatch1UnitPattern (pattern, idxOfFirstLetter);
						return false;

				}

				bool TryMatch1UnitPattern (List<LetterSoundComponent> pattern, int idxOfFirstLetter)
				{
						LetterSoundComponent l = pattern [0];
						if (l is Blank)
								return true;
						if (l.IsVowelOrVowelDigraph) {
								return true;
						}

						
						if (checkPhonotactics && !AdjacentToVowel (idxOfFirstLetter, 1))
								l.ViolatesPhonotactics = true;
								
						return false;

				}

				bool AdjacentToVowel (int idxOfFirstLetter, int offSetBy)
				{
						int a = idxOfFirstLetter + offSetBy;
						return a < preSyllable.Count && a > -1 && preSyllable [a].IsVowelOrVowelDigraph;
				}
		
				bool TryMatch3UnitPattern (List<LetterSoundComponent> pattern, int idxOfFirstLetter)
				{
						
						//cve or cvc
						if (TryMatchVowelConsonantE (pattern, idxOfFirstLetter))
								return true;
						if (TryMatchConsonantVowelConsonant (pattern))
								return true;
						return false;

			               
				}

				//assumes that pattern has 3 elements.
				bool TryMatchVowelConsonantE (List<LetterSoundComponent> pattern, int idxOfFirstLetter)
				{
						int idxOfCandidateEInFullWord = idxOfFirstLetter + 2;
						if (AdjacentToVowel (idxOfCandidateEInFullWord, 1))
								return false;

						LetterSoundComponent candidateE = pattern [2];
					
						if (candidateE.AsString.Equals ("e")) {
								LetterSoundComponent beforeE = pattern [1];
								if (beforeE is Consonant) {
										LetterSoundComponent beforeConsonant = pattern [0];
										if (beforeConsonant.IsVowelOrVowelDigraph) {
										
												Letter e = (Letter)candidateE;
												e.Silence ();

												return true; //long is vowel default sound; dont need to change anything.

										}
								}

						}
						return false;
		
				}
			
				bool TryMatchConsonantVowelConsonant (List<LetterSoundComponent> pattern)
				{
						
						LetterSoundComponent last = pattern [2];
						if (last.IsConsonantConsonantDigraphOrBlend) {
								LetterSoundComponent beforeConsonant = pattern [1];
								if (beforeConsonant.IsVowelOrVowelDigraph) {
										LetterSoundComponent beforeVowel = pattern [0];
										if (beforeVowel.IsConsonantConsonantDigraphOrBlend) {

												if (beforeConsonant is Vowel) {
														Vowel v = (Vowel)beforeConsonant;
														if (!IsRControlled (beforeConsonant, last)) {
																v.MakeShort ();
														} else
																v.MakeRControlled ();
												}

                                         
												return true;
										}
								}
				
						}
						return false;
		
				}
		
				bool TryMatch2UnitPattern (List<LetterSoundComponent> pattern, int idxOfFirstLetter)
				{      
						if (TryMatchVowelConsonant (pattern))
								return true;
						if (TryMatchConsonantVowel (pattern, idxOfFirstLetter))
								return true;
						return false;
				}

				bool TryMatchConsonantVowel (List<LetterSoundComponent> pattern, int idxOfFirstLetter)
				{
						LetterSoundComponent beforeVowel = pattern [0];
						if (beforeVowel.IsConsonantConsonantDigraphOrBlend) {
								LetterSoundComponent last = pattern [1];
								if (last.IsVowelOrVowelDigraph) {
										//Oct 2: Min wants open syllable patterns (consonant-vowel) to read as short.
										if (last is Vowel) {
												Vowel v = (Vowel)last;
												v.MakeShort ();
										}
										return true;
								}
								if (last.LettersAre ("y")) {
										preSyllable [idxOfFirstLetter + 1] = new Vowel ("y");
										return true;
								}

						}
						return false;
			
				}

				bool TryMatchVowelConsonant (List<LetterSoundComponent> pattern)
				{
		
						LetterSoundComponent last = pattern [1];
						if (last.IsConsonantConsonantDigraphOrBlend) {
								LetterSoundComponent beforeConsonant = pattern [0];
								if (beforeConsonant.IsVowelOrVowelDigraph) {
					                    
										if (beforeConsonant is Vowel) {
												Vowel v = (Vowel)beforeConsonant;
												if (!IsRControlled (v, last)) {
														v.MakeShort ();
												} else
														v.MakeRControlled ();
										}


								
										return true;
					
								}
								
						}
						return false;
			
				}

				bool IsRControlled (LetterSoundComponent vowel, LetterSoundComponent consonant)
				{      
						
						return consonant.AsString.Equals ("r");
				}
		}
	
		public class StableSyllableFactory : SyllableFactory
		{



				//ASSUMPTION OF THIS METHOD:
				//IF the stable syllable has an "e" vowel at the end then that e vowel is silent.
				//also assumes that the letters of a stable syllable must return at the end for them to be a legitimate 
				//unit. (otherwise, the next level decoder-- syllableFactory- will turn the letters into respective vowel/consonants, blends, etc.
				public StableSyllable ParseStableSyllable (string input)
				{
						
						StableSyllable stable = null;
						int length = SpeechSoundReference.MAX_STABLE_SYLLABLE_LENGTH;
						//we need to trim blanks for stable syllables.
						

						while (length>=SpeechSoundReference.MIN_STABLE_SYLLABLE_LENGTH) {
			
								if (length <= input.Length) {
										string candidate = input.Substring (input.Length - length, length);
									
										stable = TryMakeStableSyllable (candidate);
										if (!ReferenceEquals (stable, null)) {
												
												
												break;
										}
								}
								length--;

						}
						
						return stable;
		
				}

				StableSyllable TryMakeStableSyllable (string candidate)
				{
						StableSyllable result = null;
						if (SpeechSoundReference.IsStableSyllable (candidate)) 
								result = MakeStableSyllable (candidate);
						return result;

				}

				StableSyllable MakeStableSyllable (string candidate)
				{
						StableSyllable result = null;
						result = new StableSyllable (candidate);
						foreach (LetterSoundComponent l in vowelConsonantFactory.TryMakeLetters (candidate)) {
								result.AddChild (l);

						}
						result = SilenceFinalEIfNecessary (result);
						return result;
		
		
				}
	
				StableSyllable SilenceFinalEIfNecessary (StableSyllable stableSyllable)
				{
					
						LetterSoundComponent last = stableSyllable.Children [stableSyllable.Children.Count - 1];
						if (last.AsString.Equals ("e")) {
								Letter e = (Letter)last;
								e.Silence ();
			    
						}
						return stableSyllable;
			
				}

				public UserWord Parse (string context, bool bySyllables=false)
				{
						return null;
				}
		}

		













}
