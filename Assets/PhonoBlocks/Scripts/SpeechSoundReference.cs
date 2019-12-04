using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpeechSoundReference : MonoBehaviour
{

		public static readonly int MAX_BLEND_LENGTH = 3;
		public static readonly int MAX_STABLE_SYLLABLE_LENGTH = 4;
		public static readonly int MIN_STABLE_SYLLABLE_LENGTH = 2;
		public static readonly int NOT_A_BLEND = -1;
		public static readonly int MIDDLE_BLEND = 0;
		public static readonly int FINAL_BLEND = 2;
		public static readonly int INITIAL_BLEND = 3;
		static SpeechSoundLookup<char> vowels_ = new SpeechSoundLookup<char> ();
		static SpeechSoundLookup<char> consonants_ = new SpeechSoundLookup<char> ();
		static SpeechSoundLookup<string> consonant_digraphs_ = new SpeechSoundLookup<string> ();
		static SpeechSoundLookup<string> vowel_rs = new SpeechSoundLookup<string> ();
		static SpeechSoundLookup<string> vowel_digraphs_ = new SpeechSoundLookup<string> ();
		static SpeechSoundLookup<string> stable_syllables_ = new SpeechSoundLookup<string> ();
		static SpeechSoundLookup<string> special_units = new SpeechSoundLookup<string> ();
		static SpeechSoundLookup<string> initial_blends_ = new SpeechSoundLookup<string> ();
		static SpeechSoundLookup<string> middle_blends_ = new SpeechSoundLookup<string> ();
		static SpeechSoundLookup<string> final_blends_ = new SpeechSoundLookup<string> ();
		static PhonotacticChecker.Phonotactics[] blankRules = new PhonotacticChecker.Phonotactics[]{PhonotacticChecker.NoRestrictions};

		public static PhonotacticChecker.Phonotactics[] BlankRules ()
		{
				return blankRules;
		}

		public class SpeechSoundLookup<Type>
		{

				Dictionary<Type, PhonotacticChecker.Phonotactics[]> rules = new Dictionary<Type,PhonotacticChecker.Phonotactics[]> ();

				public void Add (Type letters, PhonotacticChecker.Phonotactics rule)
				{
						this.rules.Add (letters, new PhonotacticChecker.Phonotactics[]{rule});
				}

				public void Add (Type letters, PhonotacticChecker.Phonotactics a, PhonotacticChecker.Phonotactics b)
				{
						this.rules.Add (letters, new PhonotacticChecker.Phonotactics[]{a,b});		
				}

				public void Add (Type letters, PhonotacticChecker.Phonotactics a, PhonotacticChecker.Phonotactics b, PhonotacticChecker.Phonotactics c)
				{
						this.rules.Add (letters, new PhonotacticChecker.Phonotactics[]{a,b,});			
				}

				public void Add (Type letters, PhonotacticChecker.Phonotactics[] rules)
				{
						this.rules.Add (letters, rules);		
				}

				public void Add (Type letters)
				{
						this.rules.Add (letters, new PhonotacticChecker.Phonotactics[]{PhonotacticChecker.NoRestrictions});
				}

				public bool Contains (Type letters)
				{

						return rules.ContainsKey (letters);

				}

				public PhonotacticChecker.Phonotactics[] TryGetValue (Type key)
				{
						PhonotacticChecker.Phonotactics[] val = null;
						rules.TryGetValue (key, out val);
						return val;

				}

				public Dictionary<Type,PhonotacticChecker.Phonotactics[]>.KeyCollection Units ()
				{
						return rules.Keys;

				}

		}


	   

		public static PhonotacticChecker.Phonotactics[] GetRulesForConsonant (char consonant)
		{

				return consonants_.TryGetValue (consonant);

		}

		public static PhonotacticChecker.Phonotactics[] GetRulesForVowel (char vowel)
		{
				if (IsY (vowel))
						return consonants_.TryGetValue (vowel);
				return vowels_.TryGetValue (vowel);
		
		}

		public static PhonotacticChecker.Phonotactics[] GetRulesForBlend (string blend, int type)
		{
				if (type == INITIAL_BLEND)
						return initial_blends_.TryGetValue (blend);
				if (type == FINAL_BLEND)
						return final_blends_.TryGetValue (blend);
				return middle_blends_.TryGetValue (blend);
		
		}

		public static PhonotacticChecker.Phonotactics[] GetRulesForConsonantDigraph (string cDigraph)
		{
		
				return consonant_digraphs_.TryGetValue (cDigraph);
		
		}

		public static PhonotacticChecker.Phonotactics[] GetRulesForVowelDigraph (string vDigraph)
		{
		
				return vowel_digraphs_.TryGetValue (vDigraph);
		
		}

		public static PhonotacticChecker.Phonotactics[] GetRulesForStableSyllable (string stable)
		{
		
				return stable_syllables_.TryGetValue (stable);
		
		}






		//set of "can be doubled". if we find a final stray consonant and its not the end of the word, then check and see if the neighbor of that consonant
		//is the same as itself (only in one direction? or both?)
		//and then if yes, check to see if it is in the set that can be doubled (or, you could make it so we provide another phonotactics rule--
		//can be doubled? (how would we know which??)
		//ss could possibly-- cache this all in some kin dof data structure, instead of using strings.
		//but for now a query of a set would suffice.


	     
		//we need another category of special units that aren't consonants blend, (have a silent e), 
		//and arent stable syllables, but have the ability to made the vowel sound short.
		//I don't think the tutors will want dge to take on the blend colour. (it should be- 
		//take on the colors of consonants....
		static bool initialized;

		public static void Initialize ()
		{

				AddVowels ();
				AddVowelRs ();
				AddConsonants ();
				AddBlends ();
				AddConsonantDigraphs ();
				AddVowelDigraphs ();
				AddStableSyllables ();
				AddSpecialUnits ();


				initialized = true;

		}

		static void AddVowels ()
		{
				
				vowels_.Add ('a', PhonotacticChecker.NoRestrictions);
				vowels_.Add ('e', PhonotacticChecker.NoRestrictions);
				vowels_.Add ('i', PhonotacticChecker.NoRestrictions);
				vowels_.Add ('o', PhonotacticChecker.NoRestrictions);
				vowels_.Add ('u', PhonotacticChecker.NoRestrictions);
				

		}

		static void AddConsonants ()
		{
	
				consonants_.Add ('b', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('c', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('d', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('f', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('g', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('h', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('j', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('k', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('l', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('m', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('n', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('p', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('q', PhonotacticChecker.Q, PhonotacticChecker.NoRestrictions);
				consonants_.Add ('r', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('s', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('t', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('v', PhonotacticChecker.CannotBeLast, PhonotacticChecker.NoRestrictions);
				consonants_.Add ('w', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('x', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('y', PhonotacticChecker.NoRestrictions);
				consonants_.Add ('z', PhonotacticChecker.NoRestrictions);

		}

		static void AddBlends ()
		{    
				middle_blends_.Add ("st", PhonotacticChecker.NoRestrictions);
				middle_blends_.Add ("sp", PhonotacticChecker.NoRestrictions);
				middle_blends_.Add ("ll", PhonotacticChecker.NoRestrictions);

				initial_blends_.Add ("sl", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
				initial_blends_.Add ("bl", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
				initial_blends_.Add ("gl", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
				initial_blends_.Add ("cl", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
				initial_blends_.Add ("pl", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
				initial_blends_.Add ("fl", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
				initial_blends_.Add ("cr", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
				initial_blends_.Add ("tr", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
		
				initial_blends_.Add ("dr", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
		

				final_blends_.Add ("ft", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeFirst);

				final_blends_.Add ("nd", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeFirst);
				final_blends_.Add ("sk", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeFirst);
				final_blends_.Add ("mp", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeFirst);
				final_blends_.Add ("nt", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeFirst);
	
				initial_blends_.Add ("str", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
				initial_blends_.Add ("spr", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
				initial_blends_.Add ("scr", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
				initial_blends_.Add ("spl", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
				initial_blends_.Add ("squ", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
				initial_blends_.Add ("shr", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
				initial_blends_.Add ("thr", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);

	

		}

		static void AddConsonantDigraphs ()
		{
				consonant_digraphs_.Add ("th", PhonotacticChecker.NoRestrictions);
                consonant_digraphs_.Add("gh", PhonotacticChecker.NoRestrictions);
                //consonant_digraphs_.Add("ph", PhonotacticChecker.NoRestrictions);

				consonant_digraphs_.Add ("qu", PhonotacticChecker.NoRestrictions);
				consonant_digraphs_.Add ("ck", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeFirst);
				consonant_digraphs_.Add ("ch", PhonotacticChecker.NoRestrictions);
				consonant_digraphs_.Add ("ng", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeFirst);
				consonant_digraphs_.Add ("nk", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeFirst);
				consonant_digraphs_.Add ("sh", PhonotacticChecker.NoRestrictions);
				consonant_digraphs_.Add ("wh", PhonotacticChecker.NoRestrictions, PhonotacticChecker.CannotBeLast);
		}
	
		static void AddVowelDigraphs ()
		{
				vowel_digraphs_.Add ("ea");
				vowel_digraphs_.Add ("ai");
				vowel_digraphs_.Add ("ae");
				vowel_digraphs_.Add ("aa");
				vowel_digraphs_.Add ("ee");
				vowel_digraphs_.Add ("ie");
				vowel_digraphs_.Add ("oe");
				vowel_digraphs_.Add ("ue");
				vowel_digraphs_.Add ("ou");
				vowel_digraphs_.Add ("ay");
				vowel_digraphs_.Add ("oa");
		}

		static void AddVowelRs ()
		{
				vowel_rs.Add ("er");
				vowel_rs.Add ("ur");
				vowel_rs.Add ("or");
				vowel_rs.Add ("ir");
				vowel_rs.Add ("ar");
		}

		static void AddStableSyllables ()
		{       

				stable_syllables_.Add ("ov");
				stable_syllables_.Add ("er");
				stable_syllables_.Add ("wa");
				stable_syllables_.Add ("ter");
				stable_syllables_.Add ("cree");
				stable_syllables_.Add ("py");


		}


		//the rule for special units is that they query the 
		//concept to see if they should be coloured especially.
		//if not, then each component just takes whatever colour it has for being vowel, consonant, silent, etc.
		static void AddSpecialUnits ()
		{       
				special_units.Add ("dge", PhonotacticChecker.CannotBeFirst, PhonotacticChecker.ConsonantCannotPrecede);
				


		}

		public static bool IsStableSyllable (string candidate)
		{
				//if (candidate.Length > MAX_STABLE_SYLLABLE_LENGTH || candidate.Length < MIN_STABLE_SYLLABLE_LENGTH)
						//return false;
		
				if (!initialized)
						Initialize ();
				return stable_syllables_.Contains (candidate);
		
		
		
		}
	
		public static int IsBlendAndWhichType (string candidate)
		{
				if (candidate.Length > MAX_BLEND_LENGTH)
						return NOT_A_BLEND;

				if (!initialized)
						Initialize ();
				if (middle_blends_.Contains (candidate))
						return MIDDLE_BLEND;
				if (initial_blends_.Contains (candidate))
						return INITIAL_BLEND;
				if (final_blends_.Contains (candidate))
						return FINAL_BLEND;

				return NOT_A_BLEND;
		
		
		
		}

		public static bool IsConsonantDigraph (string candidate)
		{
				if (candidate.Length > 2)
						return false;
		
				if (!initialized)
						Initialize ();
				return consonant_digraphs_.Contains (candidate);

		}
		
		public static bool IsVowelR (string candidate)
		{
				if (candidate.Length > 2)
						return false;
		
				if (!initialized)
						Initialize ();
				return vowel_rs.Contains (candidate);
		
		}
	
		public static bool IsVowelDigraph (string candidate)
		{
				if (candidate.Length > 2)
						return false;
		
				if (!initialized)
						Initialize ();
				return vowel_digraphs_.Contains (candidate);

		}

		public static bool IsVowel (char candidate)
		{
				if (!initialized)
						Initialize ();
				return  vowels_.Contains (candidate);
		
		
		
		
		}

		public static bool IsY (char letter)
		{
				return letter == 'y';
		}

		public static bool IsConsonant (char candidate)
		{
				if (!initialized)
						Initialize ();
				return consonants_.Contains (candidate);

		}

		public static Dictionary<char,PhonotacticChecker.Phonotactics[]>.KeyCollection Vowels ()
		{	
				return vowels_.Units ();
		
		
		}

		public static Dictionary<char,PhonotacticChecker.Phonotactics[]>.KeyCollection Consonants ()
		{	
				return consonants_.Units ();
		
		
		}

}
